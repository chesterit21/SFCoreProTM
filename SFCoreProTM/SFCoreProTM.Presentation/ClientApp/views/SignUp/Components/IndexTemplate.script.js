import { computed, reactive, ref } from 'vue'
import authService from '@/services/authService'

export default {
  setup() {
    const fullName = ref('')
    const workspaceName = ref('')
    const email = ref('')
    const password = ref('')
    const confirmPassword = ref('')
    const teamSize = ref('1-5')
    const acceptedTerms = ref(false)
    const showErrors = ref(false)
    const isSubmitting = ref(false)
    const errorMessage = ref('')

    const passwordVisibility = reactive({
      password: false,
      confirm: false
    })

    const togglePassword = (field) => {
      passwordVisibility[field] = !passwordVisibility[field]
    }

    const isValidEmail = computed(() => /\S+@\S+\.\S+/.test(email.value))

    const hasLetter = computed(() => /[A-Za-z]/.test(password.value))
    const hasNumberOrSymbol = computed(() => /[\d\W_]/.test(password.value))
    const passwordLengthOk = computed(() => password.value.length >= 8)

    const passwordStrength = computed(() => {
      const score = [hasLetter.value, hasNumberOrSymbol.value, passwordLengthOk.value].filter(Boolean).length
      if (password.value.length === 0) return { level: 'empty', label: 'Create a password' }
      if (score <= 1) return { level: 'weak', label: 'Weak password' }
      if (score === 2) return { level: 'medium', label: 'Good password' }
      return { level: 'strong', label: 'Strong password' }
    })

    const passwordsMatch = computed(
      () => password.value.length > 0 && confirmPassword.value.length > 0 && password.value === confirmPassword.value
    )

    const canSubmit = computed(
      () =>
        fullName.value.trim().length > 0 &&
        workspaceName.value.trim().length > 0 &&
        isValidEmail.value &&
        passwordLengthOk.value &&
        passwordsMatch.value &&
        acceptedTerms.value
    )

    const handleSubmit = () => {
      showErrors.value = !canSubmit.value
      errorMessage.value = ''

      if (!canSubmit.value || isSubmitting.value) return

      const nameParts = fullName.value.trim().split(/\s+/)
      const [firstName = '', ...rest] = nameParts
      const payload = {
        email: email.value.trim(),
        password: password.value,
        displayName: fullName.value.trim(),
        firstName,
        lastName: rest.join(' '),
        workspaceName: workspaceName.value.trim(),
        teamSize: teamSize.value
      }

      isSubmitting.value = true

      authService
        .signUp(payload)
        .then(() => {
          window.location.href = '/'
        })
        .catch((error) => {
          errorMessage.value = error?.message || 'Unable to create your account. Please try again.'
        })
        .finally(() => {
          isSubmitting.value = false
        })
    }

    return {
      fullName,
      workspaceName,
      email,
      password,
      confirmPassword,
      teamSize,
      acceptedTerms,
      showErrors,
      isSubmitting,
      errorMessage,
      passwordVisibility,
      togglePassword,
      isValidEmail,
      hasLetter,
      hasNumberOrSymbol,
      passwordLengthOk,
      passwordStrength,
      passwordsMatch,
      canSubmit,
      handleSubmit
    }
  }
}
