/** @type {import('tailwindcss').Config} */
export default {
    content: ["./index.html", "./src/**/*.{js,jsx,ts,tsx}"],
    theme: {
        extend: {
            colors: {
                // RAS brand — tuned to your blue and the script coral
                "ras-blue": {
                    50: "#EEF0FF",
                    100: "#E1E3FF",
                    200: "#C9CBFF",
                    300: "#A7ACFF",
                    400: "#8A8FE8",
                    500: "#6E73D0",
                    600: "#585BB5",
                    700: "#3B3A8A",   // logo blue
                    800: "#2D2C6B",
                    900: "#1F1F4D"
                },
                "ras-coral": {
                    50: "#FFECEF",
                    100: "#FFD6DC",
                    200: "#FFB7C1",
                    300: "#FF99A8",
                    400: "#F7788F",
                    500: "#E35C73",
                    600: "#CF5367",   // script color
                    700: "#B64559",
                    800: "#953948",
                    900: "#772F3B"
                }
            },
            fontFamily: {
                // body
                sans: ["Inter", "system-ui", "Segoe UI", "Roboto", "Arial", "sans-serif"],
                // titles
                display: ["Poppins", "Inter", "sans-serif"],
                // decorative script (sparingly)
                script: ["Satisfy", "cursive"]
            },
            boxShadow: {
                card: "0 10px 30px -8px rgba(31,31,77,.22)"
            },
            borderRadius: {
                "3xl": "1.5rem"
            }
        }
    },
    plugins: []
};
