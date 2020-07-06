import dotnet from "dotenv";
import { Bot } from "./bot";

dotnet.config();

const bots: Bot[] = [];

for (let i = 0; i < Number(process.env.BOT_COUNT); i++) {
  const bot = new Bot();
  bots.push(bot);
}

const coreLoop = async () => {
  while (true) {
    const promises = [];
    for (let i = 0; i < bots.length; i++) {
      const bot = bots[i];
      promises.push(bot.gameLoop());
    }

    await Promise.all(promises);
  }
};

coreLoop();
