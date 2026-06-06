FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ClinicManager/ClinicManager.csproj src/ClinicManager/
RUN dotnet restore src/ClinicManager/ClinicManager.csproj
COPY . .
RUN dotnet publish src/ClinicManager/ClinicManager.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClinicManager.dll"]
