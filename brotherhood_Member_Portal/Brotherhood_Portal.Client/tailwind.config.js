/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        brand: {
          primary: "#006a4e",
          secondary: "#dc2626",
          accent: "#22c55e",

          blue: "#2563eb",
          blueDark: "#1e40af",

          slate: "#0f172a",
          muted: "#64748b",
          border: "#e2e8f0",
        },
      },
      boxShadow: {
        soft: "0 8px 24px rgba(15, 23, 42, 0.08)",
      },
      fontFamily: {
        sans: [
          "Inter",
          "ui-sans-serif",
          "system-ui",
          "-apple-system",
          "Segoe UI",
          "Roboto",
          "Arial",
        ],
      },
    },
  },
  plugins: [],
};