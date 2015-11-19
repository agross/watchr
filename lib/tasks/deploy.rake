def post_sync(remote_path)
  cmd = %w(powershell.exe -Version 3.0 -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Unrestricted -InputFormat none -Command)
  cmd << Rake::Win32.normalize(File.join(remote_path, 'deploy.ps1'))
  cmd << 'Install'
  cmd.join(' ')
end

def remote_path_for(local_path)
  app_assembly = File.join(local_path, 'bin/bin/Web.dll')

  version = Support::BinaryVersionReader.read_from(app_assembly).file_version

  Integration::TeamCity::ServiceMessages.build_number(version)
  File.join(configatron.deployment.remote_path, version)
end

def msdeploy_gac_hack
  # MSDeploy 3.6 installs Microsoft.Web.Deployment.Tracing 9.0.0.0 in the GAC
  # that is incompatible with MSDeploy's 3.5 Tracing assembly (which has the
  # same version number).
  #
  # We override GAC assemblies by providing a local path.
  # https://msdn.microsoft.com/en-us/library/cskzh7h6.aspx
  ENV['DEVPATH'] = File.expand_path('tools/MSDeploy').gsub(/\//, '\\')
end


Tasks::MSDeploy.new deploy: :bin_path do |t|
  msdeploy_gac_hack

  local_path = File.expand_path('build/bin/Web')
  remote_path = remote_path_for(local_path)

  t.log_file = 'deploy/msdeploy.log'
  t.args = {
    verb: :sync,
    source: {
      dir_path: local_path
    },
    dest: {
      computer_name: configatron.deployment.connection.computer_name,
      username: configatron.deployment.connection.username,
      password: configatron.deployment.connection.password,
      dir_path: remote_path
    },
    skip: [
      { directory: remote_path + '\\\\logs$' }
    ],
    post_sync_on_success: {
      run_command: post_sync(remote_path),
      wait_interval: 60 * 1000,
      wait_attempts: 10
    },
    use_check_sum: nil
  }
end
