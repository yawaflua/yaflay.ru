FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build


WORKDIR /src
COPY ["yaflay.ru.csproj", "."]
RUN dotnet restore "./yaflay.ru.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "yaflay.ru.csproj" -c Release -o /app/build 

FROM build AS publish
ENV CLIENTID 123
ENV CLIENTSECRET aAbB
ENV REDIRECTURL http://example.org/
ENV PSQL_HOST localhost
ENV PSQL_USER root
ENV PSQL_PASSWORD root
ENV PSQL_DATABASE database
RUN dotnet publish "yaflay.ru.csproj" -c Release -o /app/publish /p:UseAppHost=false;redirectUrl=$CLIENTID;clientId=;clientSecret=$CLIENTSECRET;Host=$PSQL_HOST;Username=$PSQL_USER;Password=$PSQL_PASSWORD;Database=$PSQL_DATABASE

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "yaflay.ru.dll", "/p:redirectUrl=$REDIRECTURL;clientId=$CLIENTID;clientSecret=$CLIENTSECRET;Host=$PSQL_HOST;Username=$PSQL_USER;Password=$PSQL_PASSWORD;Database=$PSQL_DATABASE"]