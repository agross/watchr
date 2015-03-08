def post_sync
  cmd = %w(powershell.exe -Version 3.0 -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Unrestricted -InputFormat none -Command)
  cmd << File.join(configatron.deployment.remote_path, 'deploy.ps1')
  cmd << 'Install'
  cmd.join(' ')
end

Tasks::MSDeploy.new deploy: [:compile, :bin_path] do |t|
  t.log_file = 'deploy/msdeploy.log'
  t.args = {
    verb: :sync,
    source: {
      dir_path: File.expand_path('build/bin/Web')
    },
    dest: {
      computer_name: configatron.deployment.connection.computer_name,
      username: configatron.deployment.connection.username,
      password: configatron.deployment.connection.password,
      dir_path: configatron.deployment.remote_path
    },
    skip: [
      {
        object_name: :dirPath,
        skip_action: :Delete,
        absolute_path: 'logs\.*'
      }
    ],
    post_sync_on_success: {
      run_command: post_sync,
      wait_interval: 60 * 1000
    },
    use_check_sum: nil
  }
end
