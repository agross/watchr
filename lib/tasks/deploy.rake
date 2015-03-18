def post_sync(remote_path)
  cmd = %w(powershell.exe -Version 3.0 -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Unrestricted -InputFormat none -Command)
  cmd << File.join(remote_path, 'deploy.ps1').to_windows_path
  cmd << 'Install'
  cmd.join(' ')
end

def remote_path_for(local_path)
  app_assembly = File.join(local_path, 'bin/bin/Web.dll')

  version = Support::BinaryVersionReader.read_from(app_assembly).file_version

  Integration::TeamCity::ServiceMessages.build_number(version)
  File.join(configatron.deployment.remote_path, version)
end

Tasks::MSDeploy.new deploy: :bin_path do |t|
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
      wait_interval: 60 * 1000
    },
    use_check_sum: nil
  }
end
