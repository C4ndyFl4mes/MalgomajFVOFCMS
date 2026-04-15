import { defineConfig } from 'vite';
import path from "node:path";

export default defineConfig({
    build: {
        emptyOutdir: false,
        outDir: path.resolve(__dirname, "Server/wwwroot/js"),
        lib: {
            entry: path.resolve(__dirname, "Server/wwwroot/QuillEditor/main.editor.js"),
            formats: ["es"],
            fileName: () => "editor.bundle.js"
        },
        rolldownOptions: {
            output: {
                codeSplitting: false
            }
        }
    }
});