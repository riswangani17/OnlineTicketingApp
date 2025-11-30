import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      "/api": { target: "https://localhost:5001", secure: false },
      "/Identity": { target: "https://localhost:5001", secure: false },
    },
  },
});
