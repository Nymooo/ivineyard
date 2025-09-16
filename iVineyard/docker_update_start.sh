#!/bin/bash

set -e  # Beendet das Skript, wenn ein Fehler auftritt

echo "Logging into Docker..."
docker login || exit 1

echo "Building Docker images..."
docker compose -f ./docker-compose_local.yml build --no-cache || exit 1

echo "Pushing Docker images to registry..."
docker push nymoxy/webgui:latest || exit 1
docker push nymoxy/webgui.client:latest || exit 1
docker push nymoxy/webapi:latest || exit 1

echo "Rebuilding Docker services..."
docker compose -f ./docker-compose.yml build --no-cache || exit 1

echo "Starting Docker services..."
docker compose -f ./docker-compose.yml up || exit 1

echo "All steps completed successfully."