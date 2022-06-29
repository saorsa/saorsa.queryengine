#!/usr/bin/env bash

STACK_NAME='queryengine-demo'
docker-compose --file ./docker-compose.yml -p $STACK_NAME down