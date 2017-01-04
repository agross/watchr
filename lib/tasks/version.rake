task :version

if Integration::TeamCity.running?
  Tasks::AssemblyVersion.new :version do |t|
    t.source_args = { metadata: configatron.build.metadata.to_h }
    t.target_path = proc { |language, _version_info, _source|
      File.join('source', "VersionInfo.#{language}")
    }
  end
end
