namespace :env do
  Tasks::Environments.new do |t|
    t.default_env = :dev
    t.customizer = Proc.new do |store|
      version = Support::VersionInfo.parse({
        version: Support::VersionInfo.read_version_from('VERSION'),
        build_number: configatron.build.number,
        sha: configatron.build.sha
      })

      configatron.build.version = version.assembly_informational_version
    end
  end
end
