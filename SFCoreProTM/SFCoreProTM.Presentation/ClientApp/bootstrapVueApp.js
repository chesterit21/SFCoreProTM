// ClientApp/bootstrapVueApp.js
import { createApp } from 'vue'
import BaseComponents from '@/plugins/baseComponents'
//import vFocus from './core/directives/v-focus'
// Fungsi bootstrap reusable
export function bootstrapVueApp(viewComponent, props = {}) {
  const app = createApp(viewComponent, props)
  // Register semua plugin & komponen global dari satu tempat
  app.use(BaseComponents)
  // Global directives
  //app.directive('focus', vFocus)

  // Mount ke #app
  app.mount('#app')
}
