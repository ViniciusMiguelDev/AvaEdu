// actions/run_nunit.js (ESM)
import { execSync } from 'child_process';

try {
	// Executa NUnit no projeto AvaEdu, incluindo testes existentes + gerados em Tests/Generated
	execSync('dotnet test AvaEdu.csproj --logger "trx;LogFileName=TestResult.trx" --results-directory ./reports', {
		stdio: 'inherit'
	});
	console.log('dotnet test completed');
} catch (e) {
	console.error('dotnet test failed:', e.message);
	process.exit(1);
}