FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src

COPY src/Katalye.Api/Katalye.Api.csproj /src/src/Katalye.Api/
COPY src/Katalye.Components/Katalye.Components.csproj /src/src/Katalye.Components/
COPY src/Katalye.Data/Katalye.Data.csproj /src/src/Katalye.Data/
COPY src/Katalye.Host/Katalye.Host.csproj /src/src/Katalye.Host/
COPY tests/Katalye.Components.Tests/Katalye.Components.Tests.csproj /src/tests/Katalye.Components.Tests/
COPY Katalye.sln /src/

RUN dotnet restore

COPY . .

RUN dotnet build \
    && dotnet test

FROM build AS publish
RUN dotnet publish src/Katalye.Host/Katalye.Host.csproj -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Katalye.Host.dll"]
