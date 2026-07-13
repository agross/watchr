# TypeScript 6 + 7 side-by-side setup

This frontend compiles TypeScript sources with TypeScript 7's native compiler,
while tools that need TypeScript's JavaScript API continue using TypeScript 6.
This follows the setup described in the
[TypeScript 7.0 announcement](https://devblogs.microsoft.com/typescript/announcing-typescript-7-0/#running-side-by-side-with-typescript-6.0).

## Why both versions are installed

TypeScript 7 does not yet provide a stable programmatic JavaScript API. Two
development tools still need that API:

- `typescript-eslint` requires TypeScript `<6.1.0` and uses its JavaScript API.
- `vue-tsc` patches the TypeScript compiler at runtime to process Vue SFCs.

The `typescript` dependency therefore aliases Microsoft's TypeScript 6
compatibility package. TypeScript 7 is installed as `@typescript/native`, which
owns the `tsc` binary. The `scripts/vue-tsc6.mjs` wrapper gives `vue-tsc` the
real TypeScript 6 compiler entry point hidden behind the compatibility package.

Relevant upstream work:

- [typescript-eslint/typescript-eslint#10940](https://github.com/typescript-eslint/typescript-eslint/issues/10940)
- [vuejs/language-tools#5381](https://github.com/vuejs/language-tools/issues/5381)
- [microsoft/typescript-go#648](https://github.com/microsoft/typescript-go/issues/648)

## Removing the compatibility setup

After `typescript-eslint` and `vue-tsc` support TypeScript 7's API:

1. Replace `typescript` and `@typescript/native` with one `typescript@7`
   dependency.
2. Upgrade `typescript-eslint` and `vue-tsc` to compatible releases.
3. Change `type-check` back to `vue-tsc --build` and delete
   `scripts/vue-tsc6.mjs`.
4. Delete this document and its README reference.
