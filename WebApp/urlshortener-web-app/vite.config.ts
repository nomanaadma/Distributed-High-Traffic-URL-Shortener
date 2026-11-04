import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Keep port 3000 so the localhost redirect URI registered in Azure still matches.
    port: 3000,
    strictPort: true,
  },
  preview: {
    port: 3000,
    strictPort: true,
  },
  build: {
    // Same output folder as Create React App, so the deployment pipeline needs no changes.
    outDir: 'build',
  },
})
