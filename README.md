ASP.NET Core / Docker / SPA - Microservices oriented application example
---
-- *Work in progress* --
---

### Intro 
This project contains several implementation examples of habitual patterns to build a microservices oriented application with asp.NET core, Docker and Angular.
I took much inspiration from the [.Net Microservices Architecture eBook](https://www.microsoft.com/net/download/thank-you/microservices-architecture-ebook).
Contains examples about Domain-Driven-Design, S.O.L.I.D. and CQRS patterns.

![alt text](http://turnoff.us/image/en/monolith-retirement.png)

#### Requirements:
For each aspnet core service you may need to restore packages:
```sh
$ dotnet restore
```
And build/publish the project:
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

Place the private and public keys in the 'keys' folder of the services (All the *.Application and AuthService projects).
The private key is needed only by the authorization service.

See this [link](https://rietta.com/blog/2012/01/27/openssl-generating-rsa-key-from-command/) for more info.

Generate the HTTPS certificate for Kestrel:
```sh
openssl pkcs12 -export -out certificate.pfx -inkey privateKey.key -in certificate.crt
```
And put them into the "certificate" folder of the services (All the *.Application projects and AuthService projects).

See this [link](https://www.ssl.com/how-to/create-a-pfx-p12-certificate-file-using-openssl/) for more info.

Install this certificate in your dev computer.

Setup AWS credentials into a file \aws.dev\credentials for all the *.Application projects. 

See the [AWS Docs](http://docs.aws.amazon.com/cli/latest/userguide/cli-config-files.html).

For Authorization, Orders and Catalog services you need to initialize the mySQL schema. Use the dotnet utility for each service, type the following commands from the project folders:
```sh
dotnet ef database -v update
```

To work with fluentd logging driver enable them in your docker daemon.json configuration file. Add the following field:
```json
 {
   "log-driver": "fluentd"
 }
```
Restart docker

#### How to build and run the solution:
There are some script files into the "script" folder.

To build:
```sh
$ ./build.sh
```

To run:
```sh
$ ./run.sh
```

Or you can build each stage independently:
```sh
$ docker-compose -f <dockerstack-to-build>/<docker-compose-file-name>.yml build
```

Navigate to http://your-docker-host-name-or-ip/

#### See logs with Kibana:
Logs are available on Kibana at the url: http://<your-docker-host-name-or-ip>:5601

#### Authentication:
I used Json Web Token with public/private key signature (RSA256) to keep the users authenticated [RFC doc](https://tools.ietf.org/html/rfc7519).

First, SPA retrieve from the authentication service an access token. It'll be expire in one week (7 days).
Every hour and every time the user open the web application a new token will be retrieved from the token renew endpoint.
This strategy seems acceptable for a web application. 

Initial incpit from this [discussion](https://stackoverflow.com/questions/26739167/jwt-json-web-token-automatic-prolongation-of-expiration/26834685#26834685).

#### Databases:
I using [MySQL](https://hub.docker.com/_/mysql/) to keep users informations running on the 'db' container with a mapping volume on the host machine.

dotnet-ef migrations to database versioning.

To store basket and integration event instance and handlers processing status informations, I using a [redis](https://hub.docker.com/_/redis/) server.

#### Notes about the frontend:
The frontend is a single page application built by angular-cli, I changed the ng serve command in the package.json file to accept two configuration:

.1 Run the webpack-dev-server with a local backend services (ex. if you want to debug a service locally with the frontend as client). Configure the proxy with the desired redirects for the routes, for example I want to debug the login page using the authorization service run locally:
```json
  {
    "/api/token": {
        "target": "https://localhost:443",
        "secure": false,
        "changeOrigin": true,
        "logLevel": "debug"
    }
  }
```
(proxy.config.local.json)
```sh
$ npm run start
```
.2 Or run the webpack-dev-server with the backend run on the docker host
```sh
$ npm run start-docker
```
(see proxy.config.docker.json)

Others options are the same from angular-cli [docs](https://github.com/angular/angular-cli)

### How to debug a remote container:
For example, to debug the auth_service: look at the Dockerfile.debug version. I added the sshd support. Then you can attach remotely over a ssh tunnel with your ide. Notes the port mapping '2222:22' to avoid conflicts with the host's ssh server.

Use the correct version of docker-compose file to overwrite the configurations, like:
```sh
$ docker-compose -f docker-compose.dev.yml -f docker-compose.debug.yml up --build -d
```
Execute, on the docker host the ssh server of the container
```sh
$ docker exec -it <container-id> "/usr/sbin/sshd"  
```
Copy the ssh key to the docker container, look into the 'scripts' folder. Remember that you can reach the ssh server of the container through the docker-host port mapped (2222 in this case).
```sh
$ ssh-copy-id -p 2222 -i your_public_key.pub -o "UserKnownHostsFile=/dev/null" -o "StrictHostKeyChecking=no" root@<docker-host-ip>
```
If you are using vs code, you can now use the launch.json settings to attach to the remote container and select the correct process (example configuration).
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