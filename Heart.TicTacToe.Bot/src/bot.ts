import faker from "faker";
import { logger } from "./logger";
import { login, getGame, joinQueue, register, takeTurn } from "./requests";

interface UserContext {
  Id: string;
  AccessToken: string;
}

export class Bot {
  private email: string;
  private password: string;

  private gameId: string | undefined;

  private userId: string | undefined;
  private accessToken: string | undefined;

  constructor() {
    this.email = faker.internet.email();
    this.password = "superPASSWORD123*#";

    this.gameLoop = this.gameLoop.bind(this);
  }

  async setup() {
    if (!this.userId) {
      logger.info("registering", {
        details: {
          email: this.email,
          password: this.password,
        },
      });
      const registerResponse = await register(this.email, this.password);
      this.userId = registerResponse.id;
    }

    logger.info("logging in", {
      details: {
        email: this.email,
        password: this.password,
      },
    });
    let loginResponse = await login(this.email, this.password);
    this.accessToken = loginResponse.accessToken;
  }

  async gameLoop() {
    if (!this.userId || !this.accessToken) return this.setup();

    logger.info("joining queue", { userId: this.userId });
    const joinQueueResponse = await joinQueue(this.accessToken);
    this.gameId = joinQueueResponse.gameId;
    if (!this.gameId) return;

    let currentGame = await getGame(this.gameId, this.accessToken);
    logger.info("get game", {
      details: {
        gameId: this.gameId,
        userId: this.userId,
        response: currentGame,
      },
    });

    if (currentGame.gameState != "InProgress") return;

    if (currentGame.currentTurnPlayer == this.userId) {
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
          this.accessToken
        );

        logger.info("take turn", {
          details: {
            gameId: this.gameId,
            userId: this.userId,
            response: currentGame,
          },
        });
      }
    } else return;
  }
}
