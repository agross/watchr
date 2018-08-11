# frozen_string_literal: true

require 'rake/funnel'

Rake::Funnel::Tasks::NUnit.new(test: :bin_path) do |t|
  t.files = 'build/bin/WinForms/Client.dll'

  framework = if ENV['OS'] == 'Windows_NT'
                'net-4.0'
              else
                'mono-4.0'
              end

  t.args = {
    nologo: nil,
    noshadow: nil,
    framework: framework,
    result: 'build/spec/nunit.xml'
  }

  mkdir_p('build/spec')
end

task :test do
  Rake::Funnel::Integration::TeamCity::ServiceMessages
    .import_data(type: :nunit,
                 path: 'build/spec/nunit.xml')
end

task test: :npm do
  cmd = %w(
    npm
    run
    test
  )

  sh(*cmd)
end
