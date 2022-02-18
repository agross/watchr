# frozen_string_literal: true

require 'rake/funnel'

desc 'Create packages'
task :package, [:push] do |_task, args| # rubocop:disable Metrics/BlockLength
  def image_tags(dockerfile)
    image = File.basename(File.dirname(dockerfile))
    image = File.join('agross', "watchr-#{image.downcase}")
    tag = 'latest'

    [
      "#{image}:#{tag}",
      image,
      tag
    ]
  end

  def login_to_docker_registry(user, password)
    return unless user && password

    out, status = Open3.capture2e(*%W[
                                    docker
                                    login
                                    --username #{user}
                                    --password-stdin
                                  ],
                                  stdin_data: password)

    puts out
    raise "Error logging in to registry: #{status}" unless status.success?
  end

  login_to_docker_registry(ENV['REGISTRY_USER'], ENV['REGISTRY_PASSWORD'])

  builder = 'watchr-builder'

  sh(*%W[docker buildx create --name #{builder} --bootstrap]) \
    unless system(*%W[docker buildx inspect #{builder}])

  Dir['source/**/Dockerfile'].map do |dockerfile|
    image_tag, _app, _version = image_tags(dockerfile)

    build = %W[
      docker
      buildx
      build
      --builder #{builder}
      --progress plain
      --file #{dockerfile}
      --tag #{image_tag}
    ]

    build += if args[:push]
               # Push cross-platform images.
               %w[--platform linux/amd64,linux/arm64 --push]
             else
               # Load image for the local architecture into the docker engine
               # on this host. Does not support cross-platform images.
               %w[--load]
             end

    sh(*build, '.')
  end
end
