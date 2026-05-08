/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./src/**/*.{html,ts}'],
    theme: {
        extend: {
            fontFamily: {
                sans: ['"Inter"', '-apple-system', 'BlinkMacSystemFont', '"Segoe UI"', 'sans-serif'],
            },
            colors: {
                // Paleta Notion/Apple
                surface: {
                    DEFAULT: '#ffffff',
                    subtle: '#f9fafb', // fondo de página
                    hover: '#f3f4f6',
                },
                border: {
                    DEFAULT: '#e5e7eb', // gray-200
                    strong: '#d1d5db',
                },
                ink: {
                    DEFAULT: '#111827', // casi negro, texto principal
                    muted: '#6b7280', // texto secundario
                    faint: '#9ca3af', // placeholder
                },
                accent: {
                    DEFAULT: '#2563eb', // azul pálido, único color de acento
                    hover: '#1d4ed8',
                    light: '#eff6ff',
                },
            },
            borderRadius: {
                DEFAULT: '0.375rem', // 6px – radios sutiles
            },
            boxShadow: {
                card: '0 1px 3px 0 rgb(0 0 0 / 0.06), 0 1px 2px -1px rgb(0 0 0 / 0.04)',
                float: '0 4px 16px 0 rgb(0 0 0 / 0.08)',
            },
            fontSize: {
                '2xs': ['0.6875rem', { lineHeight: '1rem' }],
            },
        },
    },
    plugins: [],
};