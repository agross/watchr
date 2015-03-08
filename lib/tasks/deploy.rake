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
    use_check_sum: nil
  }
end
