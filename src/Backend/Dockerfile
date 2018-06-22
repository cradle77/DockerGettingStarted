FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Backend/Backend.csproj Backend/
COPY Entities/Entities.csproj Entities/
RUN dotnet restore Backend/Backend.csproj
COPY . .
WORKDIR /src/Backend
RUN dotnet build Backend.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Backend.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Backend.dll"]
