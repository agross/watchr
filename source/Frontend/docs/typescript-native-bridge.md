# Native TypeScript bridge

This frontend uses
[`typescript-native-bridge`](https://github.com/johnsoncodehk/typescript-native-bridge)
as its `typescript` package. The bridge preserves the TypeScript 6 programmatic
API used by vue-tsc, Volar, and typescript-eslint while delegating type checking
to the native TypeScript 7 (tsgo) engine in process.

## Setup

- `typescript` aliases the pinned bridge release.
- The npm `overrides` entry makes all transitive TypeScript consumers use that
  same package.
- `npm run type-check` invokes vue-tsc normally; no compatibility launcher is
  needed.
- The repository's `.vscode/settings.json` asks VS Code to use the frontend's
  workspace TypeScript SDK. Select
  **TypeScript: Select TypeScript Version → Use Workspace Version** once if
  VS Code prompts.

Run `npm install` after changing the bridge version.

## Verification

Run from `source/Frontend`:

```sh
npm run type-check
node --eval "console.log(require.resolve('typescript'))"
```

The first command prints a `TNB ACTIVE` banner. The resolved package path from
the second command should be under this frontend's `node_modules/typescript`.

The bridge uses tsgo checker behavior, which can differ from stock TypeScript.
See the upstream known-differences tracker when diagnostics change.

## Rollback

1. Replace the aliased `typescript` dependency with a stock TypeScript version.
2. Remove the `overrides` entry.
3. Run `npm install`.
