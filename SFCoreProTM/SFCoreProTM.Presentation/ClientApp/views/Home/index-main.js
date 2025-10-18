//  ClientApp/views/Home/Index-main.js

import { bootstrapVueApp } from '@/bootstrapVueApp'
import IndexTemplate from './Components/IndexTemplate.vue'
// --- Pindahan code dari IndexTemplate.script.js ---
import { ref } from 'vue'
import authService from '@/services/authService'

const scriptLogic = {
  setup() {
    const email = ref('')
    const password = ref('')
    const rememberMe = ref(false)
    const passwordVisible = ref(false)
    const isSubmitting = ref(false)
    const errorMessage = ref('')

    const togglePassword = () => {
      passwordVisible.value = !passwordVisible.value
    }

    const handleForgotPassword = () => {
      console.info('Forgot password clicked')
    }

    const handleSubmit = async () => {
      if (isSubmitting.value) {
        return
      }

      errorMessage.value = ''

      const trimmedEmail = email.value.trim().toLowerCase()
      if (!trimmedEmail || !password.value) {
        errorMessage.value = 'Please enter both email and password.'
        return
      }

      isSubmitting.value = true
      try {
        await authService.signIn({
          email: trimmedEmail,
          password: password.value,
          rememberMe: rememberMe.value
        })

        window.location.href = '/dashboard'
      } catch (error) {
        // error.message mungkin undefined, kita gunakan operator nullish coalescing (??)
        errorMessage.value = error?.message ?? 'Unable to sign in. Please try again.'
      } finally {
        isSubmitting.value = false
      }
    }

    return {
      email,
      password,
      rememberMe,
      passwordVisible,
      isSubmitting,
      errorMessage,
      togglePassword,
      handleForgotPassword,
      handleSubmit
    }
  }
}
// --------------------------------------------------

// Menggabungkan template (IndexTemplate) dan logic (scriptLogic)
const ActionComponent = { // <-- Deklarasi kedua
  ...IndexTemplate,
  ...scriptLogic,
}
bootstrapVueApp(ActionComponent)