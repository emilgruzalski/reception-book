FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies first so that this layer is cached between builds.
COPY ReceptionBook.sln .
COPY ReceptionBook/ReceptionBook.csproj ReceptionBook/
COPY ReceptionBook.Presentation/ReceptionBook.Presentation.csproj ReceptionBook.Presentation/
COPY Contracts/Contracts.csproj Contracts/
COPY Entities/Entities.csproj Entities/
COPY LoggerService/LoggerService.csproj LoggerService/
COPY Repository/Repository.csproj Repository/
COPY Service/Service.csproj Service/
COPY Service.Contracts/Service.Contracts.csproj Service.Contracts/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore ReceptionBook/ReceptionBook.csproj

COPY . .
RUN dotnet publish ReceptionBook/ReceptionBook.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Log directories must be writable by the non-root user.
RUN mkdir -p logs internal_logs && chown $APP_UID logs internal_logs

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "ReceptionBook.dll"]
