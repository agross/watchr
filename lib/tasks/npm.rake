desc 'Install npm packages'
task :npm do
  sh(*%w(npm install)) # rubocop:disable Lint/UnneededSplatExpansion
end

task :npm do
  cp(Dir['node_modules/xterm/dist/*.js', 'node_modules/xterm/dist/*.js.map'],
     'source/Web/Scripts/lib/xterm/')

  cp('node_modules/xterm/dist/addons/fit/fit.js',
     'source/Web/Scripts/lib/xterm/fit.js')

  cp(Dir['node_modules/xterm/dist/*.css'],
     'source/Web/Css')
end

task :npm do
  cp('node_modules/fontfaceobserver/fontfaceobserver.js',
     'source/Web/Scripts/lib/fontfaceobserver/')
end

task :npm do
  cp('node_modules/css-element-queries/src/ResizeSensor.js',
     'source/Web/Scripts/lib/css-element-queries/')
end

task :npm do
  cp(Dir['node_modules/jquery/dist/jquery.js'],
     'source/Web/Scripts/lib/jquery/')
end

task :npm do
  cp(Dir['node_modules/signalr/jquery.signalR.min.js'],
     'source/Web/Scripts/lib/signalr/')
end
