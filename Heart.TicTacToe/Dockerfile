FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./src/TicTacToe.Api/TicTacToe.Api.csproj ./src/TicTacToe.Api/TicTacToe.Api.csproj
COPY ./src/TicTacToe.Db/TicTacToe.Db.csproj ./src/TicTacToe.Db/TicTacToe.Db.csproj
COPY ./src/TicTacToe.UTests/TicTacToe.UTests.csproj ./src/TicTacToe.UTests/TicTacToe.UTests.csproj

RUN dotnet restore ./src/TicTacToe.Api
RUN dotnet restore ./src/TicTacToe.UTests

COPY . .

RUN find . -name '*Tests.csproj' | xargs dotnet build -p:GenerateFullPaths=true
RUN find . -wholename '*/bin/*Tests.dll' | xargs dotnet vstest --Parallel

RUN dotnet publish -c Release -o out ./src/TicTacToe.Api

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TicTacToe.Api.dll"]
