FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

RUN git clone --single-branch --branch master https://github.com/HeartofTheForce/Heart.Auth.git

RUN dotnet publish -c Release -o out ./Heart.Auth/Heart.Auth.Api

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/Heart.Auth/Heart.Auth.Api/out .
ENTRYPOINT ["dotnet", "Heart.Auth.Api.dll"]