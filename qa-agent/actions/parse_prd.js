// actions/parse_prd.js (ESM)
import fs from 'fs';
import * as glob from 'glob';
import path from 'path';

const outDir = 'tmp';
if (!fs.existsSync(outDir)) fs.mkdirSync(outDir, { recursive: true });

function extractFromMd(text) {
	return text
		.split(/\r?\n/)
		.filter(l => /^\d+\./.test(l))
		.map(l => l.replace(/^\d+\.\s*/, '').trim());
}

async function main() {
	const files = glob.sync('BMAD/docs/sprint-artifacts/*.md');
	let all = [];

	files.forEach(f => {
		const txt = fs.readFileSync(f, 'utf8');
		all = all.concat(extractFromMd(txt));
	});

	fs.writeFileSync(path.join(outDir, 'criteria.json'), JSON.stringify(all, null, 2));
	console.log('Extracted criteria:', all.length);
}

main();