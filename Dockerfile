FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY cabapi.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# La configuración real (SQL, JWT, OpenAI) llega vía /app/.env montado por compose
ENV SERVER_URL=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "cabapi.dll"]
