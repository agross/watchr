%w(Web WinForms Console).each do |target|
  zip = File.join('deploy', "#{configatron.project}-#{target}-#{configatron.build.version.full}.zip")

  task :package do
    rm_f zip
  end

  Tasks::Zip.new package: :compile do |t|
    t.source = FileList["build/bin/#{target}/**/*"].exclude('**/*.xml')
    t.target = zip
  end
end
