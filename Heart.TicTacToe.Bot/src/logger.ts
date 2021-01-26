import winston from "winston";

const serilog = winston.format((info: any) => {
  info["@mt"] = info.message;
  info["@l"] = info.level;
  info["@t"] = info.timestamp;

  delete info.message;
  delete info.level;
  delete info.timestamp;

  return info;
});

export const logger = winston.createLogger({
  level: "info",
  format: winston.format.combine(
    winston.format.timestamp(),
    serilog(),
    winston.format.json()
  ),
  transports: [new winston.transports.Console()],
});

