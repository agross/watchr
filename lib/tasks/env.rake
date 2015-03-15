namespace :env do
  Tasks::Environments.new do |t|
    t.default_env = :dev
    t.customizer = Proc.new do |store|
      version = Support::VersionInfo.parse({
        version: Support::VersionInfo.read_version_from('VERSION'),
        build_number: store.build.number,
        sha: store.build.sha
      })

      store.build.version = version.assembly_informational_version
    end
  end
end
