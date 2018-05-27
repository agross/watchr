# frozen_string_literal: true

require 'rake/funnel'

desc 'Generate version info'
task :version

if Rake::Funnel::Integration::TeamCity.running?
  Rake::Funnel::Tasks::AssemblyVersion.new(:version) do |t|
    t.source_args = { metadata: configatron.build.metadata.to_h }
    t.target_path = proc do |language, _version_info, _source|
      File.join('source', "VersionInfo.#{language}")
    end
  end
end
