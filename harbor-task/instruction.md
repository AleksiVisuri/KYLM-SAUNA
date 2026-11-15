# AI Sauna Trainer

Run the provided Sauna environment and improve the agent's ability to launch projectiles into the target heating zone.

The agent must select horizontal direction, vertical angle, and launch power. A successful run should reliably hit the target and raise its temperature. Do not bypass the simulation or hard-code a success result; use the environment's observations, actions, and rewards.

The verifier will score performance on fresh episodes using a normalized reward from 0 to 1.

Use the sauna MCP tools to interact with the environment. Start with `reset`,
call `observe`, then use `throw` with horizontal (-1 to 1), vertical (0 to 1),
and power (0 to 1). Do not edit evaluation files directly.
