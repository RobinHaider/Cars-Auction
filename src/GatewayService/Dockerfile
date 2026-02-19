FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY CarsAuction.sln ./
COPY src/GatewayService/GatewayService.csproj ./src/GatewayService/

RUN dotnet restore ./src/GatewayService/GatewayService.csproj

COPY . .
RUN dotnet publish ./src/GatewayService/GatewayService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "GatewayService.dll" ]