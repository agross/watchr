Tasks::NUnit.new(test: %i(bin_path npm)) do |t|
  t.files = 'build/bin/WinForms/Client.dll'

  framework = if ENV['OS'] == 'Windows_NT'
                'net-4.0'
              else
                'mono-4.0'
              end

  t.args = {
    nologo: nil,
    noshadow: nil,
    noresult: nil,
    framework: framework
  }
end

if Rake::Win32.windows?
  task :test do
    cmd = [
      'chutzpah.console.exe',
      *Support::Mapper.new.map(
        fail_on_error: nil,
        path: 'source/Web.Tests'
      )
    ]

    sh(*cmd)
  end
end
