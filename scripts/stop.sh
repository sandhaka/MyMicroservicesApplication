#!/bin/bash

docker-compose -f ../dockerstack-system/docker-compose.yml stop
docker-compose -f ../dockerstack-application/docker-compose.yml stop