# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY SandStats.csproj ./
RUN dotnet restore SandStats.csproj

# Copiar todo el código y publicar
COPY . .
RUN dotnet publish SandStats.csproj -c Release -o /app

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Render espera que la app escuche en el puerto 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SandStats.dll"]
