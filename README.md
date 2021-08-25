# Heart.Stack
A mock service stack, with a focus on maintainability, monitoring, security and stability.

# Built On
- [.NET Core](https://dotnet.microsoft.com/download/dotnet-core)
- [Docker](https://www.docker.com/)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [EntityFramework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Kibana](https://www.elastic.co/products/kibana)
- [Elasticsearch](https://www.elastic.co/products/elasticsearch)
- [Filebeat](https://www.elastic.co/products/beats/filebeat)

![Dashboard](https://s7.gifyu.com/images/tictactoe.gif)

# Getting Started
Make sure you have access to [Docker](https://www.docker.com/)

Run `./scripts/run-stack.sh`

# Interesting Things

- ### TicTacToe Game State Compression
    ```
    A single game has up to 9 turns
    Each turn has 9 - n possible moves
    Store the index of the available moves of each turn inside the bits of an integer as follows:
    1 - 00000000000000000000000000001111
    2 - 00000000000000000000000011110000
    3 - 00000000000000000000011100000000
    4 - 00000000000000000011100000000000
    5 - 00000000000000011100000000000000
    6 - 00000000000011100000000000000000
    7 - 00000000001100000000000000000000
    8 - 00000000110000000000000000000000
    9 - 00000001000000000000000000000000
    Each slot has a minimum value of (9 - n) + 1 to allow for 0 to be used to indicate an untaken turn
    ```
