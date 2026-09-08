FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY backend/GraphForge.Api.csproj backend/
COPY GraphForge.Contracts/GraphForge.Contracts.csproj GraphForge.Contracts/
RUN dotnet restore backend/GraphForge.Api.csproj

COPY backend/ backend/
COPY GraphForge.Contracts/ GraphForge.Contracts/
RUN dotnet publish backend/GraphForge.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GraphForge.Api.dll"]
