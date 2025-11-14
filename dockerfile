FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# No Render, a porta correta é definida pela variável $PORT
ENV ASPNETCORE_URLS=http://*:$PORT

EXPOSE 10000

ENTRYPOINT ["dotnet", "BackEndAPI.dll"]
