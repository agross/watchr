Tasks::NUnit.new :test => :bin_path do |t|
  t.files = 'build/bin/WinForms/Client.dll'
  t.args = {
    nologo: nil,
    noshadow: nil,
    noresult: nil
  }
end

if Rake::Win32.windows?
  task :test do
    cmd = [
      'chutzpah.console.exe',
      *Support::Mapper.new.map({
        fail_on_error: nil,
        path: 'source/Web.Tests'
      })
    ]

    sh(*cmd)
  end
end
