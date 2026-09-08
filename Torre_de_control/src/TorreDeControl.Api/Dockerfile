# 1. Imagen base para desarrollo dentro del Dev Container (con SDK de .NET)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Imagen base de ejecución para producción (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 3. Imagen del SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el .csproj y restauramos las dependencias NuGet
COPY ["src/TorreDeControl.Api/TorreDeControl.Api.csproj", "src/TorreDeControl.Api/"]
RUN dotnet restore "src/TorreDeControl.Api/TorreDeControl.Api.csproj"

# Copiamos todo el código fuente del proyecto y compilamos
COPY . .
WORKDIR "/src/src/TorreDeControl.Api"
RUN dotnet build "TorreDeControl.Api.csproj" -c Release -o /app/build

# 4. Publicación de los binarios
FROM build AS publish
RUN dotnet publish "TorreDeControl.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. Imagen final de producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TorreDeControl.Api.dll"]