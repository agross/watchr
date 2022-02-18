# frozen_string_literal: true

desc 'Restores dotnet tools'
task :dotnet_tool do
  cmd = %w[dotnet tool restore]

  sh(*cmd)
end
