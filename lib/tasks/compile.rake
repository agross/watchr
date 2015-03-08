Tasks::SideBySideSpecs.new :compile do |t|
  t.references = 'NUnit.Framework'
  t.enabled = configatron.env == 'production'
end

Tasks::MSBuild.new compile: [:paket, :version] do |t|
  t.args = {
    nologo: nil,
    verbosity: :minimal,
    target: :Rebuild,
    property: {
      restore_packages: false,
      download_paket: false,
      build_in_parallel: false,
      configuration: configatron.build.configuration
    }
  }.merge(Rake::Win32.windows? ? { node_reuse: false } : {})
end

Tasks::Copy.new :compile do |t|
  t.source = FileList['source/Web/**/*']
    .exclude('**/*.cs')
    .exclude('**/*.??proj')
    .exclude('**/obj/**/*')
    .exclude('**/*.intellisense.js')
    .exclude('**/*.map')
    .exclude('**/*-vsdoc.js')
    .exclude('**/bin/*.xml')
    .exclude('**/paket.references')
  t.target = 'build/bin/Web'
end
