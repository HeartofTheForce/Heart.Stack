import dotnet from "dotenv";
import { Bot } from "./bot";
import { logger } from "./logger";

dotnet.config();

const bots: Bot[] = [];

for (let i = 0; i < Number(process.env.BOT_COUNT); i++) {
  const bot = new Bot();
  bots.push(bot);
}

const requestBuffer = 1000;

function sleep(ms: number) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

const coreLoop = async () => {
  while (true) {
    const promises = [];
    for (let i = 0; i < bots.length; i++) {
      const bot = bots[i];
      promises.push(bot.gameLoop());
    }

    await Promise.all(promises).catch((e) => {
      logger.error("coreLoop error", {
        details: {
          exception: e,
        },
      });
    });
    await sleep(requestBuffer);
  }
};

coreLoop();
