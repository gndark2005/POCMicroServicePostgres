ARG DOTNET_VERSION_TAG=6.0-alpine3.15

FROM ghcr.io/buildinglink/dotnet-sdk:$DOTNET_VERSION_TAG AS build

# Add a GitHub private feed 
ARG GITHUB_TOKEN
RUN dotnet nuget add source https://nuget.pkg.github.com/buildinglink/index.json --name github.com --username github --password $GITHUB_TOKEN --store-password-in-clear-text

# Restore packages
WORKDIR /src

COPY src/BuildingLink.ModuleServiceTemplate/BuildingLink.ModuleServiceTemplate.csproj BuildingLink.ModuleServiceTemplate/
COPY src/BuildingLink.ModuleServiceTemplate.Api/BuildingLink.ModuleServiceTemplate.Api.csproj BuildingLink.ModuleServiceTemplate.Api/
COPY src/BuildingLink.ModuleServiceTemplate.Services/BuildingLink.ModuleServiceTemplate.Services.csproj BuildingLink.ModuleServiceTemplate.Services/
COPY src/BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst/BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst.csproj BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst/
COPY src/BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst/BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.csproj BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst/
RUN dotnet restore BuildingLink.ModuleServiceTemplate.Api/BuildingLink.ModuleServiceTemplate.Api.csproj

# Build application
COPY src ./
RUN dotnet build BuildingLink.ModuleServiceTemplate.Api/BuildingLink.ModuleServiceTemplate.Api.csproj -c Release

FROM build AS publish
RUN dotnet publish BuildingLink.ModuleServiceTemplate.Api/BuildingLink.ModuleServiceTemplate.Api.csproj --no-build -c Release -o /app/publish

# Final image
FROM ghcr.io/buildinglink/dotnet-runtime:$DOTNET_VERSION_TAG AS final

COPY --from=publish /app/publish /bms/api/

WORKDIR /bms/api/

ENTRYPOINT ["dotnet"]
CMD ["BuildingLink.ModuleServiceTemplate.Api.dll"]
