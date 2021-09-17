#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["SudokuCollective.Api.csproj", "SudokuCollective.Api/"]
RUN dotnet restore "SudokuCollective.Api/SudokuCollective.Api.csproj"
WORKDIR "/src/SudokuCollective.Api"
COPY . .
RUN dotnet build "SudokuCollective.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SudokuCollective.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet SudokuCollective.Api.dll