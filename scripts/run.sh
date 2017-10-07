#!/bin/bash

docker-compose -f ../dockerstack-system/docker-compose.yml up --remove-orphans -d &&
docker-compose -f ../dockerstack-application/docker-compose.yml up --remove-orphans -d