require 'rake/funnel'

Rake::Funnel::Tasks::Timing.new
Rake::Funnel::Tasks::BinPath.new
Rake::Funnel::Integration::SyncOutput.new
Rake::Funnel::Integration::ProgressReport.new
Rake::Funnel::Integration::TeamCity::ProgressReport.new

Rake::Funnel::Tasks::MSBuild.new do |t|
  t.args = {
    nologo: nil,
    node_reuse: false,
    verbosity: :minimal,
    target: :Build,
    property: {
      build_in_parallel: false,
      configuration: :Debug
    }
  }
end

Rake::Funnel::Tasks::NUnit.new :test => :bin_path do |t|
  t.search_pattern = 'build/bin/Client.dll'
  t.args = {
    nologo: nil,
    noshadow: nil,
    result: "foo.xml"
  }
end

task :test do
  cmd = [
    'chutzpah.console.exe',
    *Rake::Funnel::Support::Mapper.new(:NUnit).map({
      fail_on_error: nil,
      show_failure_report: nil,
      path: nil
    }),
    'source/Web.Tests'
  ]

  shell(cmd)
end

task default: [:compile, :test]
