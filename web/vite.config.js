import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// In dev, /api calls are proxied to the local .NET API (dotnet run -> http://localhost:5000).
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/api': 'http://localhost:5000',
    },
  },
})
