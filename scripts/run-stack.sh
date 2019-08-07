#!/bin/bash
docker-compose rm
docker-compose build --no-cache
docker-compose up -d --scale tictactoe-client=6
while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' http://localhost:5601/api/status)" != "200" ]]; do sleep 5; echo "Waiting for Kibana..."; done
echo "Setting Index Pattern..."
curl -X POST "localhost:5601/api/saved_objects/_import" -H "kbn-xsrf: true" --form file=@config/export.ndjson >/dev/null
echo -e "\e[32mStack Ready\e[0m. View logs http://localhost:5601/app/kibana#/discover"
echo "Press enter to clean up..."
read -n 1 -s -r -p ""
docker-compose down --rmi 'local'