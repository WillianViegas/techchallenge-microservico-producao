#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["techchallenge-microservico-producao/techchallenge-microservico-producao.csproj", "techchallenge-microservico-producao/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infra/Infra.csproj", "Infra/"]

RUN dotnet restore "techchallenge-microservico-producao/techchallenge-microservico-producao.csproj"
COPY . .

WORKDIR "/src/techchallenge-microservico-producao"
RUN dotnet build "techchallenge-microservico-producao.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "techchallenge-microservico-producao.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "techchallenge-microservico-producao.dll"]