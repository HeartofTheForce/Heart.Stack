FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY ./src/TicTacToe.Api/TicTacToe.Api.csproj ./src/TicTacToe.Api/TicTacToe.Api.csproj
COPY ./src/TicTacToe.Db/TicTacToe.Db.csproj ./src/TicTacToe.Db/TicTacToe.Db.csproj
RUN dotnet restore ./src/TicTacToe.Api

COPY ./src/TicTacToe.UTests/TicTacToe.UTests.csproj ./src/TicTacToe.UTests/TicTacToe.UTests.csproj
RUN dotnet restore ./src/TicTacToe.UTests

COPY . .

RUN find . -name '*Tests.csproj' | xargs -L1 dotnet test

RUN dotnet publish -c Release -o out ./src/TicTacToe.Api

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TicTacToe.Api.dll"]
