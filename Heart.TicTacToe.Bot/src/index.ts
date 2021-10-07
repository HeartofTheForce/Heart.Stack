import dotnet from "dotenv";
import { Bot } from "./bot";
import { logger } from "./logger";

dotnet.config();

const requestBufferMs = Number(process.env.REQUEST_BUFFER_MS);
const botCount = Number(process.env.BOT_COUNT);

const bots: Bot[] = [];
for (let i = 0; i < botCount; i++) {
  const bot = new Bot();
  bots.push(bot);
}

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

    await sleep(requestBufferMs);
  }
};

coreLoop();
