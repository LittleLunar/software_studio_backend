# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /WebAPI
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /WebAPI
COPY --from=build-env /WebAPI/publish .
ENTRYPOINT ["dotnet", "software_studio_backend.dll"]