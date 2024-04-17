# Используем образ ASP.NET Core SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Установка рабочей директории
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY . ./
RUN dotnet restore "api.yawaflua.ru.csproj"

# Собираем проект
RUN dotnet publish -c Release -o /app/out "api.yawaflua.ru.csproj"


# Конечный образ, использующий ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
# Определяем порт, который приложение будет слушать
EXPOSE 80
# Установка переменных среды
ENV CLIENTID=123
ENV CLIENTSECRET=aAbB
ENV REDIRECTURL=http://example.org/
ENV PSQL_HOST=localhost
ENV PSQL_USER=root
ENV PSQL_PASSWORD=root
ENV PSQL_DATABASE=database
ENV OWNERID=1111111
ENV READMEFILE=https://raw.githubusercontent.com/yawaflua/yawaflua/main/README.md

# Запускаем ASP.NET Core приложение
ENTRYPOINT ["dotnet", "api.yawaflua.ru.dll"]
