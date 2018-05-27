# frozen_string_literal: true

require 'rake/funnel'

desc 'Create packages'
task package: :compile do
  def zip(target)
    version = configatron.build.version.assembly_informational_version
    File.join('deploy', "#{configatron.project}-#{target}-#{version}.zip")
  end

  %w(Web WinForms Console).each do |target|
    namespace :zip do
      zip = Rake::Funnel::Tasks::Zip.new(target) do |t|
        rm_f zip(target)

        t.source = FileList["build/bin/#{target}/**/*"]
                   .exclude('**/*.log')
                   .exclude('**/*.xml')
        t.target = zip(target)
      end

      task target do
        Rake::Funnel::Integration::TeamCity::ServiceMessages
          .publish_artifacts("#{zip(target)} => deploy")
      end

      Rake::Task[zip.name].invoke
    end
  end
end
