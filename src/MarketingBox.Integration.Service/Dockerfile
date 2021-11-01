FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

WORKDIR /app
COPY . .
ARG app_compilation_date
ARG app_version
ENV APP_COMPILATION_DATE=${app_compilation_date}
ENV APP_VERSION=${app_version}
ENTRYPOINT ["dotnet", "MarketingBox.Integration.Service.dll"]
