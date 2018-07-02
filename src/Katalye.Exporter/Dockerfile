FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Katalye.Exporter/Katalye.Exporter.csproj src/Katalye.Exporter/
RUN dotnet restore src/Katalye.Exporter/Katalye.Exporter.csproj
COPY . .
WORKDIR /src/src/Katalye.Exporter
RUN dotnet build Katalye.Exporter.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Katalye.Exporter.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Katalye.Exporter.dll"]
