#!/bin/bash

docker-compose -f ../dockerstack-system/docker-compose.yml build
docker-compose -f ../dockerstack-application/docker-compose.dev.yml build