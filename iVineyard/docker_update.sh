#!/bin/bash

docker login

docker compose -f ./docker-compose_local.yml build --no-cache

docker push nymoxy/webgui:latest
docker push nymoxy/webgui.client:latest
docker push nymoxy/webapi:latest