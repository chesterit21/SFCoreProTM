// ClientApp/vite.config.js

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import fs from 'fs'
import path, { resolve } from 'path'

const viewsDir = path.resolve(__dirname, 'views')
const outputBase = path.resolve(__dirname, '..', 'wwwroot', 'pages')

// 🧹 Hapus seluruh folder wwwroot/pages sebelum build
function deleteFolderRecursive(folderPath) {
  if (fs.existsSync(folderPath)) {
    fs.readdirSync(folderPath).forEach(file => {
      const curPath = path.join(folderPath, file)
      if (fs.lstatSync(curPath).isDirectory()) {
        deleteFolderRecursive(curPath)
      } else {
        fs.unlinkSync(curPath)
      }
    })
    fs.rmdirSync(folderPath)
  }
}

// 💥 Bersihkan seluruh isi wwwroot/pages
deleteFolderRecursive(outputBase)

// 📁 Buat ulang folder root-nya
fs.mkdirSync(outputBase, { recursive: true })

// 🧩 Generate input untuk setiap [controller]/[action]-main.js
const input = {}

if (fs.existsSync(viewsDir)) {
  const controllerFolders = fs.readdirSync(viewsDir).filter(folder =>
    fs.statSync(path.join(viewsDir, folder)).isDirectory()
  )

  controllerFolders.forEach(controller => {
    const controllerPath = path.join(viewsDir, controller)

    // Fungsi rekursif untuk mencari semua file di dalam direktori
    const findFilesRecursively = (dir) => {
      let results = []
      const list = fs.readdirSync(dir)
      list.forEach(file => {
        const fullPath = path.join(dir, file)
        const stat = fs.statSync(fullPath)
        if (stat && stat.isDirectory()) {
          results = results.concat(findFilesRecursively(fullPath))
        } else {
          results.push(fullPath)
        }
      })
      return results
    }

    const allFiles = findFilesRecursively(controllerPath)

    allFiles.forEach(filePath => {
      const file = path.basename(filePath)
      if (/^[a-zA-Z0-9_-]+-main\.js$/.test(file)) {
        const actionName = file.replace('-main.js', '')
        const key = `${controller}/${actionName}`
        input[key] = filePath
      }
    })
  })
}

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': resolve(__dirname)
    }
  },
  build: {
    outDir: outputBase,
    emptyOutDir: false, // false karena kita bersihin manual
    minify: 'esbuild',
    sourcemap: false,
    manifest: true,
    rollupOptions: Object.keys(input).length > 0
      ? {
          input,
          output: {
            manualChunks(id) {
              if (id.includes('node_modules')) {
                if (id.includes('@fortawesome')) return 'fontawesome'
                if (id.includes('primevue')) return 'primevue'
                if (id.includes('primeicons')) return 'primeicons'
                return 'vendor'
              }
            },
            entryFileNames: '[name]-main.js',
            chunkFileNames: 'lib/[name]-[hash].js',
            assetFileNames: 'assets/[name]-[hash][extname]'
          }
        }
      : undefined
  }
})