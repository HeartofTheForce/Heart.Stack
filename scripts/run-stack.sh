#!/bin/bash
docker-compose rm
docker-compose build --no-cache
docker-compose up -d
while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' http://localhost:5601/api/status)" != "200" ]]; do sleep 5; echo "Waiting for Kibana..."; done
echo "Setting Index Pattern..."
curl --request POST \
  --url http://localhost:5601/api/saved_objects/index-pattern/618f3ea0-b256-11e9-9c4d-73f7dc7fa305 \
  --header 'content-type: application/json' \
  --header 'kbn-version: 7.3.0' \
  --data '{
	"attributes": {
		"title": "filebeat-*",
		"timeFieldName": "@timestamp"
	}
}' >/dev/null
echo -e "\e[32mStack Ready\e[0m. View logs http://localhost:5601/app/kibana#/discover"
echo "Press enter to clean up..."
read -n 1 -s -r -p ""
docker-compose down --rmi 'local'