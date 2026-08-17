# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs
RUN npm install -g @angular/cli

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Korp_Teste_MarcosMaciel.Server/Korp_Teste_MarcosMaciel.Server.csproj", "Korp_Teste_MarcosMaciel.Server/"]
COPY ["korp_teste_marcosmaciel.client/korp_teste_marcosmaciel.client.esproj", "korp_teste_marcosmaciel.client/"]
RUN dotnet restore "./Korp_Teste_MarcosMaciel.Server/Korp_Teste_MarcosMaciel.Server.csproj"
COPY . .
WORKDIR "/src/Korp_Teste_MarcosMaciel.Server"
RUN dotnet build "./Korp_Teste_MarcosMaciel.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Korp_Teste_MarcosMaciel.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Korp_Teste_MarcosMaciel.Server.dll"]
