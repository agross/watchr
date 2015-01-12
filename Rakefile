require 'rake/funnel'

Rake::Funnel::Integration::SyncOutput.new
Rake::Funnel::Integration::ProgressReport.new
Rake::Funnel::Integration::TeamCity::ProgressReport.new

Rake::Funnel::Tasks::BinPath.new
Rake::Funnel::Tasks::Paket.new
Rake::Funnel::Tasks::QuickTemplate.new
Rake::Funnel::Tasks::Timing.new

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

Rake::Funnel::Tasks::Copy.new :compile do |t|
  t.source = FileList['source/Web/**/*']
    .exclude('**/*.cs')
    .exclude('**/*.??proj')
    .exclude('**/obj/**/*')
    .exclude('**/*.intellisense.js')
    .exclude('**/*.map')
    .exclude('**/*-vsdoc.js')
    .exclude('**/bin/*.xml')
    .exclude('**/paket.references')
  t.target = 'build/bin/Web'
end

Rake::Funnel::Tasks::NUnit.new :test => [:paket, :bin_path] do |t|
  t.files = 'build/bin/WinForms/Client.dll'
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

Rake::Funnel::Tasks::Zip.new do |t|
  t.source = FileList['build/bin/WinForms/**/*'].exclude('**/*.xml')
  t.target = File.join('deploy', 'winforms.zip')
end

Rake::Funnel::Tasks::MSDeploy.new :deploy => [:bin_path] do |t|
  t.log_file = 'deploy/msdeploy.log'
  t.args = {
    verb: :sync,
    source: {
      dir_path: File.expand_path('build/bin/Web')
    },
    dest: {
      computer_name: 'grossweber.com',
      username: ENV['DEPLOY_USER'],
      password: ENV['DEPLOY_PASSWORD'],
      dir_path: 'C:/GROSSWEBER/watch/test'
    },
    use_check_sum: nil
  }
end

task default: [:compile, :test]
