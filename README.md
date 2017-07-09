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

## Hot to debug a running container:
For example, to debug the auth_service: look at the Dockerfile.debug version. I added the sshd support. Then you can attach remotely over a ssh tunnel with your ide. Note the mapping port 2222:22 to avoid conflicts with the host's ssh server.

Use the correct version of docker-compose file to overwrite the configuration for the debug version settings, like:
```sh
$ docker-compose -c docker-compose.dev.yml -c docker-compose.debug.yml up --build -d
```
Execute the ssh server on the container
```sh
$ docker exec -it container-id "bash"  
```
Copy the ssh key to the docker container
```sh
 $ ssh-copy-id -p 2222 -i your_public_key.pub -o "UserKnownHostsFile=/dev/null" -o "StrictHostKeyChecking=no" root@192.168.104.100
 ```
```sh
$ /usr/sbin/sshd
```
If you are using vs code, use the launch.json settings to attach to the remote container and select the correct process (example configuration).
```json
{
    "version": "0.2.0",
    "configurations": [  
    {
        "name": ".NET Core Remote Attach",
        "type": "coreclr",
        "request": "attach",
        "processId": "${command:pickRemoteProcess}",
        "pipeTransport": {
            "pipeCwd": "${workspaceRoot}",
            "pipeProgram": "ssh",
            "pipeArgs": ["-p", "2222", "-i", "<your-.ssh-path>/id_rsa_clrdbg", "-T", "root@<docker-host-ip>"],
            "debuggerPath": "/root/vsdbg/vsdbg",
            "quoteArgs": true
        },
        "sourceFileMap": {
                "<your-solution-folder>": "${workspaceRoot}"
        }
    }]
}
```