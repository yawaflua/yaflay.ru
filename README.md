# Hello, there!
This is my custom site, with url: [yawaflua.ru](https://yawaflua.ru/)

Used frameworks: Asp.Net, Razor, jQuery

Thanks for the help to [@Mih4n](https://github.com/Mih4n)!

If you wanna help me, create Issue or Pull request.


# How to start?
You should to use this command for download this package from ghcr:
```cli
docker pull ghcr.io/yawaflua/yaflay.ru:master
```
After that create docker-compose file, for example:
```yml
version: "3.9"

services: 
  site:
    image: ghcr.io/yawaflua/yaflay.ru:master
    environment:
        PSQL_HOST: example.com
        PSQL_USER: root
        PSQL_DATABASE: MySite
        PSQL_PASSWORD: root
        REDIRECTURL: https://example.com/authorize
        CLIENTSECRET: aAbBcCdD123123
        CLIENTID: 1111111111111111111
```
For normal work this site need to give psql data to docker environ, or appsettings.json, if you download this project from github manually
Example data for appsettings.json:
```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "clientId": "111111111111",
  "clientSecret": "aAbBcCdD",
  "redirectUrl": "https://example.com/authorize"
}
```
