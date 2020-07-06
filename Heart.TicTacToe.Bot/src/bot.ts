import faker from "faker";
import { logger } from "./logger";
import { login, getGame, joinQueue, register, takeTurn } from "./requests";

const requestBuffer = 1000;

function sleep(ms: number) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

interface UserContext {
  Id: string;
  AccessToken: string;
}

export class Bot {
  private email: string;
  private password: string;

  private gameId: string | undefined;

  private user?: UserContext;

  constructor() {
    this.email = faker.internet.email();
    this.password = "superPASSWORD123*#";

    this.gameLoop = this.gameLoop.bind(this);

    this.setup();
  }

  async setup() {
    let registerResponse;
    do {
      logger.info("registering", {
        details: {
          email: this.email,
          password: this.password,
        },
      });

      registerResponse = await register(this.email, this.password);
    } while (!registerResponse.ok);
    const userId = (await registerResponse.json()).id;

    logger.info("logging in", {
      details: {
        email: this.email,
        password: this.password,
      },
    });
    let loginResponse = await login(this.email, this.password);
    const accessToken = loginResponse.accessToken;

    this.user = { Id: userId, AccessToken: accessToken };
  }

  async gameLoop() {
    if (!this.user) return sleep(requestBuffer);

    logger.info("joining queue", { userId: this.user.Id });
    const joinQueueResponse = await joinQueue(this.user.AccessToken);
    this.gameId = joinQueueResponse.gameId;
    if (!this.gameId) return sleep(requestBuffer);

    let currentGame = await getGame(this.gameId, this.user.AccessToken);
    logger.info("get game", {
      details: {
        gameId: this.gameId,
        userId: this.user.Id,
        response: currentGame,
      },
    });

    if (currentGame.gameState != "InProgress") return;

    if (currentGame.currentTurnPlayer == this.user.Id) {
      const possibleMoves = [];

      let xIndex = 0;
      let yIndex = 0;
      for (let i = 0; i < currentGame.boardState.length; i++) {
        const char = currentGame.boardState[i];
        if (char == "\n") {
          xIndex = 0;
          yIndex++;
        } else {
          if (char == "-") {
            possibleMoves.push({ x: xIndex, y: yIndex });
          }

          xIndex++;
        }
      }

      if (possibleMoves.length > 0) {
        const randomMove =
          possibleMoves[Math.floor(Math.random() * possibleMoves.length)];
        currentGame = await takeTurn(
          this.gameId,
          randomMove.x,
          randomMove.y,
          this.user.AccessToken
        );

        logger.info("take turn", {
          details: {
            gameId: this.gameId,
            userId: this.user.Id,
            response: currentGame,
          },
        });
      }
    } else return sleep(requestBuffer);
  }
}
