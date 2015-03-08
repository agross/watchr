Tasks::MSDeploy.new deploy: [:compile, :bin_path] do |t|
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
