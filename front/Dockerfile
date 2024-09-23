FROM node:20

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN npm run build
#RUN ls -la /app
FROM nginx:stable-alpine

COPY --from=0 /app/dist /usr/share/nginx/html

RUN ["rm", "/etc/nginx/conf.d/default.conf"]

COPY ./default.conf /etc/nginx/conf.d

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]