FROM node:14.5 AS build-env
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run-script build

FROM node:14.5
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["node", "index.js"]
