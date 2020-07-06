import fetch from "node-fetch";

export const register = (email: string, password: string) => {
  return fetch(`${process.env.TICTACTOE_URL}/api/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      email: email,
      password: password,
      alias: email,
    }),
  }).then((res) => res.json());
};

export const login = (email: string, password: string) => {
  return fetch(`${process.env.TICTACTOE_URL}/api/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      email: email,
      password: password,
    }),
  }).then((res) => res.json());
};

export const joinQueue = (accessToken: string) => {
  return fetch(`${process.env.TICTACTOE_URL}/api/game/join-queue`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  }).then((res) => res.json());
};

export const getGame = (gameId: string, accessToken: string) => {
  return fetch(`${process.env.TICTACTOE_URL}/api/game?id=${gameId}`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  }).then((res) => res.json());
};

export const takeTurn = (
  gameId: string,
  x: number,
  y: number,
  accessToken: string
) => {
  return fetch(
    `${process.env.TICTACTOE_URL}/api/game/take-turn?id=${gameId}&x=${x}&y=${y}`,
    {
      method: "POST",
      headers: {
        authorization: `Bearer ${accessToken}`,
      },
    }
  ).then((res) => res.json());
};
