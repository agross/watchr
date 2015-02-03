require 'rake/funnel'

include Rake::Funnel

Integration::SyncOutput.new
Integration::ProgressReport.new
Integration::TeamCity::ProgressReport.new

Tasks::Paket.new
Tasks::BinPath.new :bin_path => :paket
Tasks::QuickTemplate.new
Tasks::Timing.new

Tasks::SideBySideSpecs.new :compile do |t|
  t.references = 'NUnit.Framework'
  t.enabled = true
end

Tasks::MSBuild.new :compile do |t|
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

Tasks::Copy.new :compile do |t|
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

Tasks::NUnit.new :test => :bin_path do |t|
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
      *Support::Mapper.new.map({
        fail_on_error: nil,
        show_failure_report: nil,
        path: 'source/Web.Tests'
      })
    ]

    sh(*cmd)
  end
end

Tasks::Zip.new do |t|
  t.source = FileList['build/bin/WinForms/**/*'].exclude('**/*.xml')
  t.target = File.join('deploy', 'winforms.zip')
end

Tasks::MSDeploy.new :deploy => :bin_path do |t|
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
