# frozen_string_literal: true

require 'rake/funnel'

Rake::Funnel::Tasks::NUnit.new(test: %i(bin_path npm)) do |t|
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

if Rake::Win32.windows?
  task :test do
    report = 'build/spec/chutzpah.xml'
    mkdir_p(File.dirname(report))

    cmd = [
      'chutzpah.console.exe',
      *Rake::Funnel::Support::Mapper.new.map(
        fail_on_error: nil,
        path: 'source/Web.Tests',
        nunit2: report
      )
    ]

    sh(*cmd)

    Rake::Funnel::Integration::TeamCity::ServiceMessages
      .import_data(type: :nunit, path: report)
  end
end
