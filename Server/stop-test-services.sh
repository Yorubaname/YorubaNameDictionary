#!/usr/bin/env bash

set -u

services=(
  test.yorubadictionary.web.service
  test.yorubanamedictionary.api.net.service
  test.yorubanamedictionary.web.net.service
  test.yorubatts.service
)

for service in "${services[@]}"; do
  echo "Stopping ${service}..."
  if sudo systemctl stop "${service}"; then
    echo "Stopped ${service}"
  else
    echo "Warning: could not stop ${service}"
  fi
done
