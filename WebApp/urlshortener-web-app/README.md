# URL Shortener Web App

React 19 + TypeScript + Vite front end for the URL Shortener, using Azure AD (MSAL) for sign-in.

## Setup

This project uses [pnpm](https://pnpm.io). The exact version is pinned in `package.json` (`packageManager`), so the easiest way to get it is through Corepack, which ships with Node.js:

```bash
corepack enable              # one-time; makes the pinned pnpm version available
pnpm install
cp .env.example .env.local   # then fill in your values
pnpm dev                     # http://localhost:3000
```

## Environment variables

Vite only exposes variables prefixed with `VITE_`.

| Variable            | Was (CRA)                | Purpose                          |
| ------------------- | ------------------------ | -------------------------------- |
| `VITE_CLIENT_ID`    | `REACT_APP_CLIENT_ID`    | Azure AD app (client) ID         |
| `VITE_AUTHORITY`    | `REACT_APP_AUTHORITY`    | `https://login.microsoftonline.com/<tenant-id>` |
| `VITE_API_ENDPOINT` | `REACT_APP_API_ENDPOINT` | Base URL of the URL shortener API |

These are baked in at build time, so set them in your CI/deployment pipeline as well.

## Scripts

| Command           | What it does                                    |
| ----------------- | ----------------------------------------------- |
| `pnpm dev`       | Dev server on port 3000                         |
| `pnpm build`     | Type-check and build to `build/`                |
| `pnpm preview`     | Serve the production build on port 3000         |
| `pnpm lint`      | Lint with oxlint                                |

## Notes

- The dev server is pinned to port 3000 so the `http://localhost:3000` redirect URI in the Azure app registration still works.
- The build output stays in `build/` (same as CRA), so an Azure Static Web Apps `output_location: build` keeps working.
