FROM nginx:alpine
COPY ./nginx/webapi.conf /etc/nginx/conf.d/
