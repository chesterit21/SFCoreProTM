// ClientApps/plugins/baseComponents.js
import 'primeflex/primeflex.css'
import 'primeicons/primeicons.css'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura';
import Lara from '@primeuix/themes/lara';
import Nora from '@primeuix/themes/nora';
import { definePreset } from '@primeuix/themes';


//import Drawer from 'primevue/drawer'
const myPreset = definePreset(Aura, {
  semantic: {
    formField: {
      paddingX :'0.5rem',
      paddingY: '0.5rem',
    }
  },
  components : {
    inputtext : {
      lgFontSize : '16px',
      smFontSize : '12px',
      paddingX : '0.5rem',
      paddingY: '0.5rem'
    },
    button : {
      marginY: '0.5rem',
      marginX: '0.5rem'
    }
  }
});

export default {
  install(app) {
    // Setup PrimeVue + Theme
    app.use(PrimeVue, {
    // Default theme configuration
    theme: {
        preset: myPreset,
        options: {
            prefix: 'p',
            darkModeSelector: false || 'none',
            cssLayer: false
        }
    },      
      ripple: true,
      inputStyle: 'outlined',
    })

    // Auto-register semua komponen di /components/base dan subfolder
    const components = import.meta.glob('../components/base/**/*.vue', { eager: true })

    for (const path in components) {
      const component = components[path].default
      const name = component.name || path.split('/').pop().replace('.vue', '')
      app.component(name, component)
    }
  },
}