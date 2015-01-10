require 'rake/funnel'

Rake::Funnel::Tasks::Timing.new
Rake::Funnel::Tasks::BinPath.new
Rake::Funnel::Integration::SyncOutput.new
Rake::Funnel::Integration::ProgressReport.new
Rake::Funnel::Integration::TeamCity::ProgressReport.new

Rake::Funnel::Tasks::Paket.new

Rake::Funnel::Tasks::MSBuild.new do |t|
  t.args = {
    nologo: nil,
    verbosity: :minimal,
    target: :Build,
    property: {
      build_in_parallel: false,
      configuration: :Debug
    }
  }.merge(Rake::Win32.windows? ? { node_reuse: false } : {})
end

Rake::Funnel::Tasks::NUnit.new :test => [:paket, :bin_path] do |t|
  t.search_pattern = 'build/bin/WinForms/Client.dll'
  t.args = {
    nologo: nil,
    noshadow: nil,
    noresult: nil
  }
end

if Rake::Win32.windows?
  task :test do
    cmd = [
      'chutzpah.console.exe',
      *Rake::Funnel::Support::Mapper.new.map({
        fail_on_error: nil,
        show_failure_report: nil,
        path: 'source/Web.Tests'
      })
    ]

    sh(*cmd)
  end
end

task default: [:compile, :test]
