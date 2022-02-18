# frozen_string_literal: true

require 'rake/funnel'

task compile: %i[npm] do
  sh(*%w[npm run rollup])
end

task compile: %i[paket] do
  cmd = %w[
    dotnet
    build
    --nologo
    --no-incremental
    --configuration Release
  ]

  sh(*cmd)
end
