# syntax=docker/dockerfile:1

# FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app
run apt-get update -y && apt-get install -y curl npm && npm config set unsafe-perm true && npm install -g npm@6.14.13

run curl -sL https://deb.nodesource.com/setup_14.x -o setup_14.sh
run sh ./setup_14.sh
run apt-get install -y nodejs

EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:5.0 AS buildbase
ARG TARGETPLATFORM
ARG BUILDPLATFORM
RUN echo "I am running on $BUILDPLATFORM, building for $TARGETPLATFORM" > /log

run apt-get update -y && apt-get install -y curl npm && npm config set unsafe-perm true && npm install -g npm@6.14.13

run curl -sL https://deb.nodesource.com/setup_14.x -o setup_14.sh
run sh ./setup_14.sh
run apt-get install -y nodejs

FROM buildbase AS build
WORKDIR /src
COPY . .
WORKDIR "/src"
RUN dotnet restore "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj"
# RUN if [ -z "$rpi" ] ; then dotnet restore "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" ; else dotnet restore "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -r linux-arm ; fi

RUN dotnet build "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/build
# RUN if [ -z "$rpi" ] ; then dotnet build "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/build ; else dotnet build "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/build -r linux-arm ; fi

FROM build AS publish
RUN dotnet publish "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/publish
# RUN if [ -z "$rpi" ] ; then dotnet publish "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/publish ; else dotnet publish "/src/src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj" -c Release -o /app/publish -r linux-arm  --self-contained false; fi
WORKDIR /app/publish
RUN ls -la
RUN ls -la ../

FROM base AS final
WORKDIR /app
VOLUME /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443

# ENTRYPOINT ["dotnet", "YAB.ApiWithFrontend.dll"]
ENTRYPOINT ["ls", "-la", "/mnt"]
# ENTRYPOINT ["realpath", "."]