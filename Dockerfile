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
ENV OWNERID 1111111
ENV READMEFILE https://raw.githubusercontent.com/yawaflua/yawaflua/main/README.md
RUN dotnet publish "yaflay.ru.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "yaflay.ru.dll"]