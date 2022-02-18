# frozen_string_literal: true

desc 'Install npm packages'
task :npm do
  sh(*%w[npm install])
end
