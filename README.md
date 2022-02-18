# Watchr

## Running the server and the client

Running the server locally:

```sh
$ docker container \
         run \
         --rm \
         --publish 8888:80 \
         agross/watchr-web
```

Running the client and connect to the locally running server:

```sh
$ docker container \
         run \
         --rm \
         --tty \
         --env Hub__Url=http://host.docker.internal:8888/shell \
         --volume $HOME:/watch:ro \
         agross/watchr-client.console
```

By default the client watches `/watch/*zsh*.log`. This can be changed by
passing `--env Watch__Glob=/something/*else*.log`.

## Building images locally

```sh
$ bundle install
$ bundle exec rake package
```

Or push to the docker registry:

```sh
$ bundle install
$ REGISTRY_USER=you REGISTRY_PASSWORD=secret bundle exec rake 'package[push]'
```
