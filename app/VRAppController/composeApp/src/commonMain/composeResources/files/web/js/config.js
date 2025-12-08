import { getServers } from "./icesettings.js";

export async function getServerConfig() {
  return { useWebSocket: true, startupMode: "public", logging: "dev" };
}

export function getRTCConfiguration() {
  let config = {};
  config.sdpSemantics = "unified-plan";
  config.iceServers = getServers();
  return config;
}
