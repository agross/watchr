desc 'Install npm packages'
task :npm do
  sh(*%w(npm install))
end

task :npm do
  cp(Dir['node_modules/xterm/dist/*.js', 'node_modules/xterm/dist/*.js.map'],
     'source/Web/Scripts')
  cp(Dir['node_modules/xterm/dist/*.css'],
     'source/Web/Css')
end
