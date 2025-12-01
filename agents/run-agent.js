import { spawn } from "child_process";

console.log("🤖 QA Agent iniciado! Rodando pipeline de testes QA...\n");

// Executa o mesmo pipeline do script npm "qa"
const qaProcess = spawn("npm", ["run", "qa"], {
    shell: true,
    stdio: "inherit",
});

qaProcess.on("exit", (code) => {
    if (code === 0) {
        console.log("\n✅ Pipeline QA concluído com sucesso.");
    } else {
        console.error(`\n❌ Pipeline QA terminou com código ${code}.`);
    }
});

