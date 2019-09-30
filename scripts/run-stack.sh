#!/bin/bash
docker-compose rm -s -f
docker-compose build
docker-compose up -d --scale tictactoe-client=8
while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' http://localhost:5601/api/status)" != "200" ]]; do sleep 5; echo "Waiting for Kibana..."; done
echo -e "\e[32mImporting Kibana Objects\e[0m..."
curl -X POST "localhost:5601/api/saved_objects/_import" -H "kbn-xsrf: true" --form file=@config/export.ndjson >/dev/null
echo -e "\e[32mView Dashboard\e[0m: http://localhost:5601/app/kibana#/dashboard/6335ded0-b95e-11e9-88b1-030f8b6a784d?_g=()"
echo -e "\e[32mView Logs\e[0m: http://localhost:5601/app/kibana#/discover"
echo "Press enter to clean up..."
read -n 1 -s -r -p ""
docker-compose down --rmi 'local'