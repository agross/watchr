# frozen_string_literal: true

require 'rake/funnel'

task test: :npm do
  cmd = %w[
    npm
    run
    test
  ]

  sh(*cmd)
end
