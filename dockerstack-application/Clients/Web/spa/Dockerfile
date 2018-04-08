FROM nginx:1.13-alpine

ENV APP_PATH /app
ENV PATH $APP_PATH/node_modules/@angular/cli/bin/:$PATH
ARG ENV
ARG CERT_NAME
ARG CERT_PWD_NAME
ARG CERT_KEY_NAME

RUN apk add --update --no-cache nodejs && mkdir $APP_PATH && rm -rf /etc/nginx/conf.d/*
WORKDIR $APP_PATH

COPY . .

COPY nginx/default.conf /etc/nginx/conf.d/
COPY certificate/* /etc/nginx/conf.d/certificate/

RUN /bin/sed -i "s|CERT_NAME|$CERT_NAME|" /etc/nginx/conf.d/default.conf \
  && /bin/sed -i "s|CERT_KEY_NAME|$CERT_KEY_NAME|" /etc/nginx/conf.d/default.conf \
  && /bin/sed -i "s|CERT_PWD_NAME|$CERT_PWD_NAME|" /etc/nginx/conf.d/default.conf

RUN npm install \
  && ng build --aot --env=${ENV} \
  && rm -rf /usr/share/nginx/html/* \
  && mv ./dist/* /usr/share/nginx/html/ \
  && apk del nodejs libstdc++ libgcc libuv http-parser ca-certificates \
  && rm -rf ./*

CMD ["nginx", "-g", "daemon off;"]
