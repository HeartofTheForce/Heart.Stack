FROM node:14.5 AS build-env
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run-script build

ENTRYPOINT ["node", "./out/index.js"]
