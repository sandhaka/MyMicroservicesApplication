# ASP NET Core / Docker / SPA - Microservices Example application
---
*Work in progress*
--
### Authentication:
I used Json Web Token with public/private key signature (RSA256) to keep the users authenticated [RFC doc](https://tools.ietf.org/html/rfc7519).

First, SPA retrieve from the authentication service an access token. It'll be expire in one week (7 days).
Every hour and every time the user open the web application a new token will be retrieved from the token renew endpoint.
This strategy seems acceptable for a web application. 

Initial incpit from this [discussion](https://stackoverflow.com/questions/26739167/jwt-json-web-token-automatic-prolongation-of-expiration/26834685#26834685).

### Database:
I use MySQL to keep users informations running on the 'db' container with a mapping volume on the host machine.

Liquibase to database versioning, look at the dbChangeLog folder.

### Requirements
For each aspnet core service you may need to restore packages:
```sh
$ dotnet restore
```
And build the project:
```sh
$ dotnet publish
```

You have to generate a private/public keys for JWT:
```sh
openssl genrsa -des3 -out private.pem 2048
```
```sh
openssl rsa -in private.pem -outform PEM -pubout -out public.pem
```
See this [link](https://rietta.com/blog/2012/01/27/openssl-generating-rsa-key-from-command/)

Place the private and public keys in the 'keys' folder into the services base folder.
The private key is needed only by the authorization service.

And generate the HTTPS certificate:
```sh
openssl pkcs12 -export -out certificate.pfx -inkey privateKey.key -in certificate.crt
```
See this [link](https://www.ssl.com/how-to/create-a-pfx-p12-certificate-file-using-openssl/)

Install this certificate in your dev computer.

### How to run the solution:
```sh
$ docker-compose -c docker-compose.dev.yml build
```
```sh
$ docker-compose -c docker-compose.dev.yml up
```