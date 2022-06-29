#!/usr/bin/env bash

STACK_NAME='queryengine-dev'
docker-compose --file ./docker-compose.yml -p $STACK_NAME down