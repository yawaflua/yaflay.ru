# Hello, there!
[![](https://img.shields.io/website?url=https%3A%2F%2Fyawaflua.ru)](https://yawaflua.ru)
[![Publish Docker image](https://github.com/yawaflua/yaflay.ru/actions/workflows/docker-image.yml/badge.svg)](https://github.com/yawaflua/yaflay.ru/actions/workflows/docker-image.yml)
![](https://img.shields.io/github/license/yawaflua/yaflay.ru)

This is my custom site, with url: [yawaflua.ru](https://yawaflua.ru/)

Used frameworks: Asp.Net, Razor, jQuery

Thanks for the help to [@Mih4n](https://github.com/Mih4n)!

If you wanna help me, create Issue or Pull request.

# Features
- Authorization with discord OAuth2
- Main page is downloaded from user`s github readme(like [this](https://github.com/yawaflua/yawaflua))
- Blog system with loading comments after render a page, for optimization
- Admin panel for blog`s, can make article for blog and write to db new redirect setting(Discord OAuth2)
- Set discord oauth2 and database settings from .env(docker-compose file, another docker-type function) or appsettings.json(only one from this)

# How to start?
You should to use this command for download this package from ghcr:
### For latest version
  ```cli
  docker pull ghcr.io/yawaflua/yaflay.ru:master
  ```
### For stable version
  ```cli
  docker pull ghcr.io/yawaflua/yaflay.ru:latest
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
        // You can add many ids, used "," like this:
        // OWNERID: 111111111111111, 111111111111111, 1111111111111
        OWNERID: 1111111111111111
        
        READMEFILE: https://raw.githubusercontent.com/example/example/main/README.md
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
  "redirectUrl": "https://example.com/authorize",
  "connectionString": "Host=example.com;Username=root;Password=root;Database=MySite;",
  "__comment": {
        "__comment": "You can add many ids, used \",\" like this:",
        "ownerId": "111111111111111, 1111111111, 11111111111, 1111111111111",
        "__comment_2": "please, didnt use dictionary-typed json value"
  },
  "ownerId": "1111111111",
  "readmeFile": "https://raw.githubusercontent.com/example/example/main/README.md"
}
```
# Support me
[boosty](https://yawaflua.ru/boosty) - boosty

[patreon](https://yawaflua.ru/patreon) - patreon
