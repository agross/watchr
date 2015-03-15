def zip(target)
  File.join('deploy', "#{configatron.project}-#{target}-#{configatron.build.version}.zip")
end

%w(Web WinForms Console).each do |target|
  task :package do
    rm_f zip(target)
  end

  Tasks::Zip.new package: :compile do |t|
    t.source = FileList["build/bin/#{target}/**/*"].exclude('**/*.xml')
    t.target = zip(target)
  end
end

task :package do
  Integration::TeamCity::ServiceMessages.build_number(configatron.build.version) if configatron.env == 'production'
end
