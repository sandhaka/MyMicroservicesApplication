FROM nginx:1.13-alpine

ENV APP_PATH /app
ENV PATH $APP_PATH/node_modules/@angular/cli/bin/:$PATH

RUN apk add --update --no-cache nodejs && mkdir $APP_PATH && rm -rf /etc/nginx/conf.d/*
WORKDIR $APP_PATH

COPY . .

COPY nginx/default.conf /etc/nginx/conf.d/
COPY certificate/* /etc/nginx/conf.d/certificate/

RUN npm install \
  && ng build --aot --prod \
  && rm -rf /usr/share/nginx/html/* \
  && mv ./dist/* /usr/share/nginx/html/ \
  && npm cache clean \
  && apk del nodejs libstdc++ libgcc libuv http-parser ca-certificates \
  && rm -rf ./*

# Edit the backend urls as you like to debugging purpose
RUN /bin/sed -i "s|CATALOG_URL|https://192.168.104.100:9001|" /usr/share/nginx/html/main* \
  && /bin/sed -i "s|AUTH_URL|https://192.168.104.100:5000|" /usr/share/nginx/html/main* \
  && /bin/sed -i "s|ORDERS_URL|https://192.168.104.100:9000|" /usr/share/nginx/html/main* \
  && /bin/sed -i "s|BASKET_URL|https://192.168.104.100:9002|" /usr/share/nginx/html/main*
  #&& /bin/sed -i "s|BASKET_URL|https://192.168.104.5:443|" /usr/share/nginx/html/main*


CMD ["nginx", "-g", "daemon off;"]
