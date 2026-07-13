#!/usr/bin/env node
// Run vue-tsc against the real TypeScript 6 compiler. TypeScript 7 does not
// yet expose the programmatic API that vue-tsc needs to process Vue SFCs.

import fs from 'node:fs'
import { createRequire } from 'node:module'

const require = createRequire(import.meta.url)

// The compatibility package is commonly installed through a symlink. Resolve
// from its real location so its private @typescript/old dependency is visible.
const aliasRequire = createRequire(fs.realpathSync(require.resolve('typescript/package.json')))
const realTsc6 = aliasRequire.resolve('@typescript/old/lib/tsc.js')

require('vue-tsc').run(realTsc6)
