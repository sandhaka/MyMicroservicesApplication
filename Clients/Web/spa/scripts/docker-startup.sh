#!/bin/sh -

/bin/sed -i "s|CATALOG_URL|${CATALOG_URL}|" /usr/share/nginx/html/main.bundle.js
/bin/sed -i "s|AUTH_URL|${AUTH_URL}|" /usr/share/nginx/html/main.bundle.js
/bin/sed -i "s|ORDERS_URL|${ORDERS_URL}|" /usr/share/nginx/html/main.bundle.js

nginx -g 'daemon off;'
