import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const args = process.argv.slice(2)
if (args.length < 2) {
  console.error('❌ Format: node create-view-folder.js [Controller] [Action] [-action]')
  process.exit(1)
}
const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

const [controller, action, modeFlag] = args
const isActionOnly = modeFlag === '-action'
const pascalController = controller.charAt(0).toUpperCase() + controller.slice(1)

const root = path.resolve(__dirname, '..')
const viewsPath = path.join(root, 'Views', pascalController)
const controllerPath = path.join(root, 'Controllers', `${pascalController}Controller.cs`)
const razorFile = path.join(viewsPath, `${action}.cshtml`)

const vueBase = path.join(__dirname, 'views', pascalController)
const jsEntry = path.join(vueBase, `${action.toLowerCase()}-main.js`)
const vueComponent = path.join(vueBase, 'Components', `${action}Template.vue`)

// ✅ Create Razor View
fs.mkdirSync(viewsPath, { recursive: true })
fs.writeFileSync(razorFile, `@{\n    ViewData["Title"] = "${pascalController}";\n}`)

// ✅ Create Vue structure
fs.mkdirSync(path.dirname(jsEntry), { recursive: true })
fs.mkdirSync(path.dirname(vueComponent), { recursive: true })

fs.writeFileSync(jsEntry, `import { bootstrapVueApp } from '@/bootstrapVueApp.js'\nimport ActionComponent from './Components/${action}Template.vue'\n\nbootstrapVueApp(ActionComponent)`)

fs.writeFileSync(vueComponent, `<template>\n  <div class="p-4 surface-100 border-round">\n    <h2>${pascalController} - ${action}</h2>\n    <p>This is the default template for ${action}.</p>\n  </div>\n</template>\n\n<script>\nexport default {\n  name: '${action}Template'\n}\n</script>`)

// ✅ Create Controller (jika mode bukan -action)
if (!isActionOnly && !fs.existsSync(controllerPath)) {
  const controllerCode = `
using Microsoft.AspNetCore.Mvc;

namespace ${path.basename(root)}.Controllers;
public class ${pascalController}Controller : Controller
{
    public ${pascalController}Controller()
    {
    }

    public IActionResult ${action}()
    {
        return View();
    }
}

`.trim()
  fs.writeFileSync(controllerPath, controllerCode)
}

console.log(`✅ ${isActionOnly ? 'Action-only' : 'Full'} scaffolded: ${pascalController}/${action}`)