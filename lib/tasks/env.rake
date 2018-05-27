# frozen_string_literal: true

require 'rake/funnel'

namespace :env do
  Rake::Funnel::Tasks::Environments.new do |t|
    t.default_env = :dev
    t.customizer = proc do |store|
      version = Rake::Funnel::Support::VersionInfo.parse(
        version: Rake::Funnel::Support::VersionInfo.read_version_from('VERSION'),
        metadata: store.build.metadata.to_h
      )

      store.build.version = version

      Rake::Funnel::Integration::TeamCity::ServiceMessages
        .build_number(version.assembly_informational_version)
    end
  end
end
