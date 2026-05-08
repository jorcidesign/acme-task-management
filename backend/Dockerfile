# Etapa 1: Build y Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["TaskApi.csproj", "./"]
RUN dotnet restore "TaskApi.csproj"

# Copiar el resto del código y compilar
COPY . .
RUN dotnet build "TaskApi.csproj" -c Release -o /app/build
RUN dotnet publish "TaskApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Imagen final para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Exponer el puerto de la API
EXPOSE 8080 
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskApi.dll"]