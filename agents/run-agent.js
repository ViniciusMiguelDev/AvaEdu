import AgentVibes from "agentvibes";
import fs from "fs";
import path from "path";
import readline from "readline";

const agentConfigPath = path.resolve("qa-agent/qa-agent-config.md");

// Carrega instruções do agente
const config = fs.readFileSync(agentConfigPath, "utf8");

// Inicializa o agente
const agent = new AgentVibes({
    name: "QA-Agent",
    instructions: config,
});

// Configura CLI interativo
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

console.log("🤖 QA Agent iniciado! Digite sua pergunta abaixo:");

rl.on("line", async (input) => {
    const response = await agent.run(input);
    console.log("\n" + response + "\n");
    console.log("Digite outra pergunta:");
});
