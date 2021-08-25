#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# defines which image should be used for running the APP.
# For raspberry pi, use an arm based image
# see list here:
# https://mcr.microsoft.com/v2/dotnet/aspnet/tags/list
ARG BASE_IMAGE=aspnet:5.0

# defines which image should be used for building the APP.
# For raspberry pi use an arm based image.
# See list here:
# https://mcr.microsoft.com/v2/dotnet/sdk/tags/list
ARG BUILD_IMAGE=sdk:5.0

# env variable for slim-buster images
ARG DEBIAN_FRONTEND=noninteractive

FROM mcr.microsoft.com/dotnet/$BASE_IMAGE AS base
WORKDIR /app
run apt-get update
run apt-get upgrade -y
run apt-get install -y curl
run apt-get install -y npm
run npm install -g npm@6.14.13

run curl -sL https://deb.nodesource.com/setup_14.x -o setup_14.sh
run sh ./setup_14.sh
run apt-get update -y
run apt-get install -y nodejs

run node -v
run npm -v
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/$BUILD_IMAGE AS buildbase
run apt-get update
run apt-get upgrade -y
run apt-get install -y curl
run apt-get install -y npm
run npm install -g npm@6.14.13

run curl -sL https://deb.nodesource.com/setup_14.x -o setup_14.sh
run sh ./setup_14.sh
run apt-get update -y
run apt-get install -y nodejs

run node -v
run npm -v

FROM buildbase AS build
WORKDIR /src
COPY . .
WORKDIR "/src"
RUN ls -la
RUN ls -la ../
RUN dotnet restore "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj"
RUN dotnet build "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
VOLUME /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "YAB.ApiWithFrontend.dll"]
