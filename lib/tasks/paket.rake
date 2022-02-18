# frozen_string_literal: true

task paket: :dotnet_tool do
  cmd = %w[dotnet paket restore]

  sh(*cmd)
end
