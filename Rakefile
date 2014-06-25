require 'rake/funnel'

Rake::Funnel::Tasks::Timing.new
Rake::Funnel::Tasks::BinPath.new
Rake::Funnel::Integration::SyncOutput.new
Rake::Funnel::Integration::ProgressReport.new

Rake::Funnel::Tasks::MSBuild.new

task default: :compile
