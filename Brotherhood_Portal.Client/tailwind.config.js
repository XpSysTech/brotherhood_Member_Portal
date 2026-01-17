/** @type {import('tailwindcss').Config} */
module.exports = {
  // Angular templates + TS files (for class strings built in TS)
  content: ["./src/**/*.{html,ts}"],

  // Keep theme minimal if you're using @theme in CSS.
  // (You can still extend here later if needed.)
  // TODO: Use as a design language doc to standardize colors, spacing, etc.
  theme: {
    extend: {
      colors: {
        brand: {
          blue: "#2563eb",
          slate: "#0f172a",
          muted: "#64748b",
          border: "#e2e8f0",
        },
      },
    },
  },

  plugins: [],
};
