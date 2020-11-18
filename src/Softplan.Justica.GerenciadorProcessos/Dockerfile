#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Softplan.Justica.GerenciadorProcessos/Softplan.Justica.GerenciadorProcessos.csproj", "src/Softplan.Justica.GerenciadorProcessos/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Infra.DataAccess/Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.csproj", "src/Softplan.Justica.GerenciadorProcessos.Infra.DataAccess/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Domain.Models/Softplan.Justica.GerenciadorProcessos.Domain.Models.csproj", "src/Softplan.Justica.GerenciadorProcessos.Domain.Models/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Domain/Softplan.Justica.GerenciadorProcessos.Domain.csproj", "src/Softplan.Justica.GerenciadorProcessos.Domain/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Domain.Dtos/Softplan.Justica.GerenciadorProcessos.Domain.Dtos.csproj", "src/Softplan.Justica.GerenciadorProcessos.Domain.Dtos/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting/Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.csproj", "src/Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting/"]
COPY ["src/Softplan.Justica.GerenciadorProcessos.Application.Services/Softplan.Justica.GerenciadorProcessos.Application.Services.csproj", "src/Softplan.Justica.GerenciadorProcessos.Application.Services/"]
RUN dotnet restore "src/Softplan.Justica.GerenciadorProcessos/Softplan.Justica.GerenciadorProcessos.csproj"
COPY . .
WORKDIR "/src/src/Softplan.Justica.GerenciadorProcessos"
RUN dotnet build "Softplan.Justica.GerenciadorProcessos.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Softplan.Justica.GerenciadorProcessos.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Softplan.Justica.GerenciadorProcessos.dll"]
