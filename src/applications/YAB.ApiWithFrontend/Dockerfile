#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
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

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj", "src/applications/YAB.ApiWithFrontend/"]
RUN dotnet restore "src/applications/YAB.ApiWithFrontend/YAB.ApiWithFrontend.csproj"
COPY . .
WORKDIR "/src/src/applications/YAB.ApiWithFrontend"
RUN dotnet build "YAB.ApiWithFrontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YAB.ApiWithFrontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YAB.ApiWithFrontend.dll"]