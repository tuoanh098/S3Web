import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
export default defineConfig({
    plugins: [react()],
    server: {
        proxy: {
            '/identity': 'https://localhost:7171',
            '/api': 'https://localhost:7181'
        }
    }
})
