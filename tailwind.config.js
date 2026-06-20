/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      boxShadow: {
        'soft': '0 10px 30px -10px rgba(0, 0, 0, 0.08)',
        'premium': '0 20px 40px -15px rgba(0, 0, 0, 0.05)',
      }
    },
  },
  plugins: [],
}
