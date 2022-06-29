#!/usr/bin/env bash

for i in "$@"
do
case $i in
    -d)
    DETACHED_MODE=1
    shift # past argument with no value
    ;;
    *)
          # unknown option
    ;;
esac
done

STACK_NAME='queryengine-demo'
DETACHED_MODE=${DETACHED_MODE:-0}

if [ $DETACHED_MODE -eq 0 ]; then
    echo "Starting in non-detached mode..."
    docker-compose --file ./docker-compose.yml -p $STACK_NAME up
else
    echo "Starting in detached mode..."
    docker-compose --file ./docker-compose.yml -p $STACK_NAME up -d
fi