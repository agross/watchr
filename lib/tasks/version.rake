task :version

if Integration::TeamCity.running?
  Tasks::AssemblyVersion.new :version do |t|
    t.source_args = {
      build_number: configatron.build.number,
      sha: configatron.build.sha
    }
    t.target_path = proc { |language, _version_info, _source| File.join('source', "VersionInfo.#{language}") }
  end
end
