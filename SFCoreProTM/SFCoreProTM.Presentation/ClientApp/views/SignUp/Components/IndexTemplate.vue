<template>
  <div class="signup-page">
    <div class="signup-backdrop" />
    <div class="signup-content">
      <header class="signup-header">
        <a class="signup-brand" href="#">
          <img class="signup-logo" src="/plane-logo.svg" alt="Plane" />
        </a>
        <div class="signup-header-link">
          <span>Already have an account?</span>
          <a class="signup-link" href="#">Sign in</a>
        </div>
      </header>

      <main class="signup-body">
        <section class="signup-hero">
          <div class="signup-hero__pill">Plane for Teams</div>
          <h1>Workflows that scale with your product velocity.</h1>
          <p>
            Craft sprint plans, collaborate with context, and keep everybody aligned in a single workspace that
            adapts to your team.
          </p>
          <ul class="signup-hero__bullets">
            <li>
              <span class="bullet-icon">✓</span>
              Kick off projects with ready-to-use templates.
            </li>
            <li>
              <span class="bullet-icon">✓</span>
              Real-time updates across issues, cycles, and roadmaps.
            </li>
            <li>
              <span class="bullet-icon">✓</span>
              Invite your teammates instantly—no setup required.
            </li>
          </ul>
        </section>

        <section class="signup-panel">
          <div class="signup-panel__heading">
            <h2>Create your Plane workspace</h2>
            <p>Tell us a little about you and your team to get started.</p>
          </div>

          <form class="signup-form" @submit.prevent="handleSubmit">
            <div class="form-row">
              <div class="form-field">
                <label for="full-name">Full name</label>
                <input
                  id="full-name"
                  v-model="fullName"
                  type="text"
                  name="fullName"
                  autocomplete="name"
                  placeholder="Jane Carter"
                  :class="{ 'input-error': showErrors && !fullName.trim() }"
                  required
                />
                <p v-if="showErrors && !fullName.trim()" class="form-help form-help--error">Please add your name.</p>
              </div>

              <div class="form-field">
                <label for="workspace-name">Workspace name</label>
                <input
                  id="workspace-name"
                  v-model="workspaceName"
                  type="text"
                  name="workspaceName"
                  autocomplete="organization"
                  placeholder="Acme Product Team"
                  :class="{ 'input-error': showErrors && !workspaceName.trim() }"
                  required
                />
                <p v-if="showErrors && !workspaceName.trim()" class="form-help form-help--error">
                  Give your workspace a name.
                </p>
              </div>
            </div>

            <div class="form-field">
              <label for="email">Work email</label>
              <input
                id="email"
                v-model="email"
                type="email"
                name="email"
                autocomplete="email"
                placeholder="you@company.com"
                :class="{ 'input-error': showErrors && !isValidEmail }"
                required
              />
              <p v-if="showErrors && !isValidEmail" class="form-help form-help--error">
                Enter a valid email address.
              </p>
            </div>

            <div class="form-field form-field--password">
              <div class="form-label">
                <label for="password">Create password</label>
                <button
                  type="button"
                  class="form-link"
                  @click="togglePassword('password')"
                  :aria-label="passwordVisibility.password ? 'Hide password' : 'Show password'"
                >
                  {{ passwordVisibility.password ? 'Hide' : 'Show' }}
                </button>
              </div>
              <input
                id="password"
                v-model="password"
                :type="passwordVisibility.password ? 'text' : 'password'"
                name="password"
                autocomplete="new-password"
                placeholder="Use 8+ characters with numbers or symbols"
                :class="{ 'input-error': showErrors && !passwordLengthOk }"
                required
              />
              <div class="password-meter" :class="`password-meter--${passwordStrength.level}`">
                <span class="password-meter__bar" />
                <span class="password-meter__label">{{ passwordStrength.label }}</span>
              </div>
              <ul class="password-hints">
                <li :class="{ passed: hasLetter, failed: !hasLetter }">Contains a letter</li>
                <li :class="{ passed: hasNumberOrSymbol, failed: !hasNumberOrSymbol }">Contains a number or symbol</li>
                <li :class="{ passed: passwordLengthOk, failed: !passwordLengthOk }">At least 8 characters</li>
              </ul>
            </div>

            <div class="form-field">
              <div class="form-label">
                <label for="confirm-password">Confirm password</label>
                <button
                  type="button"
                  class="form-link"
                  @click="togglePassword('confirm')"
                  :aria-label="passwordVisibility.confirm ? 'Hide password' : 'Show password'"
                >
                  {{ passwordVisibility.confirm ? 'Hide' : 'Show' }}
                </button>
              </div>
              <input
                id="confirm-password"
                v-model="confirmPassword"
                :type="passwordVisibility.confirm ? 'text' : 'password'"
                name="confirmPassword"
                autocomplete="new-password"
                placeholder="Re-enter your password"
                :class="{ 'input-error': showErrors && !passwordsMatch }"
                required
              />
              <p v-if="showErrors && !passwordsMatch" class="form-help form-help--error">
                Passwords do not match.
              </p>
            </div>

            <div class="form-field">
              <label for="team-size">Team size</label>
              <div class="form-select-wrapper">
                <select id="team-size" v-model="teamSize" name="teamSize">
                  <option value="1-5">1 - 5 people</option>
                  <option value="6-15">6 - 15 people</option>
                  <option value="16-50">16 - 50 people</option>
                  <option value="51-100">51 - 100 people</option>
                  <option value="100+">100+ people</option>
                </select>
                <span class="form-select__icon">▾</span>
              </div>
            </div>

            <div class="form-checkbox">
              <input
                id="terms"
                v-model="acceptedTerms"
                type="checkbox"
                :class="{ 'input-error': showErrors && !acceptedTerms }"
                required
              />
              <label for="terms">
                I agree to the <a href="#" class="inline-link">Terms of Service</a> and
                <a href="#" class="inline-link">Privacy Policy</a>.
              </label>
            </div>

            <div class="form-hint">
              We’ll keep you posted about product updates and best practices. You can unsubscribe anytime.
            </div>

            <button type="submit" class="signup-submit" :disabled="!canSubmit">
              Create workspace
            </button>
          </form>
        </section>
      </main>

      <footer class="signup-footer">
        <span>Trusted by teams shipping faster with Plane</span>
        <div class="signup-footer-logos">
          <span class="footer-pill">Zerodha</span>
          <span class="footer-pill">Sony</span>
          <span class="footer-pill">Dolby</span>
          <span class="footer-pill">Accenture</span>
        </div>
      </footer>
    </div>
  </div>
</template>

<script setup>
import { computed, reactive, ref } from 'vue'

const fullName = ref('')
const workspaceName = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const teamSize = ref('1-5')
const acceptedTerms = ref(false)
const showErrors = ref(false)

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

  if (!canSubmit.value) return

  console.info('Submitting signup form with:', {
    fullName: fullName.value,
    workspaceName: workspaceName.value,
    email: email.value,
    teamSize: teamSize.value
  })
}
</script>

<style scoped>
.signup-page {
  position: relative;
  min-height: 100vh;
  background: linear-gradient(135deg, #f6f8fb 0%, #edf2f9 100%);
  color: #111827;
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  overflow: hidden;
}

.signup-backdrop {
  position: absolute;
  inset: 0;
  pointer-events: none;
  background:
    radial-gradient(circle at 12% 18%, rgba(0, 109, 179, 0.16) 0%, transparent 55%),
    radial-gradient(circle at 85% 12%, rgba(0, 99, 153, 0.18) 0%, transparent 60%),
    radial-gradient(circle at 50% 85%, rgba(46, 128, 197, 0.12) 0%, transparent 60%);
}

.signup-content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  padding: 32px clamp(16px, 4vw, 56px) 40px;
}

.signup-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
}

.signup-brand {
  display: inline-flex;
  align-items: center;
  text-decoration: none;
}

.signup-logo {
  width: clamp(120px, 17vw, 170px);
  height: auto;
}

.signup-header-link {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.95rem;
  color: rgba(55, 65, 81, 0.9);
}

.signup-link {
  color: #006399;
  font-weight: 600;
  text-decoration: none;
}

.signup-link:hover {
  text-decoration: underline;
}

.signup-body {
  flex: 1;
  display: flex;
  gap: clamp(32px, 6vw, 64px);
  align-items: stretch;
  margin-top: clamp(32px, 8vh, 72px);
  flex-wrap: wrap;
}

.signup-hero {
  flex: 1 1 360px;
  max-width: 460px;
  display: flex;
  flex-direction: column;
  gap: 18px;
  color: rgb(26, 36, 62);
}

.signup-hero__pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 6px 14px;
  border-radius: 999px;
  background: rgba(0, 109, 179, 0.1);
  color: #00527a;
  font-weight: 600;
  font-size: 0.85rem;
}

.signup-hero h1 {
  margin: 0;
  font-size: clamp(2rem, 4vw, 2.6rem);
  line-height: 1.15;
  font-weight: 700;
}

.signup-hero p {
  margin: 0;
  font-size: 1rem;
  color: rgba(55, 65, 81, 0.9);
}

.signup-hero__bullets {
  list-style: none;
  padding: 0;
  margin: 4px 0 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.signup-hero__bullets li {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 0.95rem;
  color: rgba(31, 41, 55, 0.9);
}

.bullet-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: rgba(0, 99, 153, 0.15);
  color: #006399;
  font-size: 0.75rem;
  font-weight: 700;
}

.signup-panel {
  flex: 1 1 420px;
  max-width: 480px;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid rgba(226, 232, 240, 0.9);
  border-radius: 20px;
  padding: clamp(24px, 4vw, 36px);
  box-shadow:
    0px 32px 70px rgba(15, 23, 42, 0.08),
    0px 12px 24px rgba(15, 23, 42, 0.05);
  backdrop-filter: blur(18px);
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.signup-panel__heading h2 {
  margin: 0;
  font-size: 1.65rem;
  font-weight: 600;
  color: #101827;
}

.signup-panel__heading p {
  margin: 6px 0 0;
  font-size: 0.95rem;
  color: rgba(55, 65, 81, 0.72);
}

.signup-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-row {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

.form-row .form-field {
  flex: 1 1 180px;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.form-field label {
  font-size: 0.95rem;
  font-weight: 600;
  color: rgba(26, 36, 62, 0.95);
}

.form-field input,
.form-select-wrapper select {
  height: 46px;
  border-radius: 12px;
  border: 1px solid rgba(209, 213, 219, 0.95);
  padding: 0 14px;
  font-size: 0.95rem;
  color: #0f172a;
  background-color: rgba(255, 255, 255, 0.98);
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.form-field input::placeholder {
  color: rgba(148, 163, 184, 0.95);
}

.form-field input:focus,
.form-select-wrapper select:focus {
  border-color: rgba(0, 99, 153, 0.6);
  box-shadow: 0 0 0 4px rgba(0, 99, 153, 0.15);
  outline: none;
}

.form-link {
  background: none;
  border: none;
  padding: 0;
  margin: 0;
  font-size: 0.85rem;
  font-weight: 600;
  color: #006399;
  cursor: pointer;
}

.form-link:hover {
  text-decoration: underline;
}

.form-select-wrapper {
  position: relative;
}

.form-select-wrapper select {
  width: 100%;
  appearance: none;
}

.form-select__icon {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
  color: rgba(55, 65, 81, 0.6);
  font-size: 0.85rem;
}

.form-help {
  margin: 0;
  font-size: 0.8rem;
  color: rgba(71, 85, 105, 0.76);
}

.form-help--error {
  color: #dc2626;
}

.form-checkbox {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  font-size: 0.9rem;
  color: rgba(55, 65, 81, 0.9);
}

.form-checkbox input[type='checkbox'] {
  margin-top: 4px;
  width: 20px;
  height: 20px;
  border-radius: 6px;
  border: 1px solid rgba(148, 163, 184, 0.9);
}

.inline-link {
  color: #006399;
  text-decoration: none;
  font-weight: 600;
}

.inline-link:hover {
  text-decoration: underline;
}

.form-hint {
  font-size: 0.85rem;
  color: rgba(71, 85, 105, 0.85);
}

.signup-submit {
  height: 50px;
  border-radius: 14px;
  border: none;
  background: linear-gradient(125deg, #007bc2 0%, #006399 100%);
  color: #ffffff;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.15s ease, box-shadow 0.15s ease, opacity 0.15s ease;
}

.signup-submit:hover:enabled {
  transform: translateY(-1px);
  box-shadow: 0px 16px 32px -16px rgba(0, 99, 153, 0.4);
}

.signup-submit:disabled {
  opacity: 0.65;
  cursor: not-allowed;
  box-shadow: none;
}

.signup-footer {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  padding-top: clamp(40px, 6vw, 60px);
  color: rgba(55, 65, 81, 0.9);
  font-size: 0.95rem;
}

.signup-footer-logos {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 16px;
}

.footer-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 8px 16px;
  border-radius: 999px;
  background: rgba(229, 231, 235, 0.75);
  color: rgba(31, 41, 55, 0.9);
  font-weight: 600;
  font-size: 0.85rem;
}

.password-meter {
  position: relative;
  height: 6px;
  border-radius: 999px;
  background: rgba(226, 232, 240, 0.8);
  margin-top: 4px;
}

.password-meter__bar {
  position: absolute;
  inset: 0;
  border-radius: 999px;
  transform-origin: left center;
}

.password-meter__label {
  display: block;
  margin-top: 8px;
  font-size: 0.8rem;
  color: rgba(71, 85, 105, 0.85);
  font-weight: 500;
}

.password-meter--empty .password-meter__bar {
  transform: scaleX(0);
}

.password-meter--weak .password-meter__bar {
  transform: scaleX(0.4);
  background: #f87171;
}

.password-meter--medium .password-meter__bar {
  transform: scaleX(0.7);
  background: #f59e0b;
}

.password-meter--strong .password-meter__bar {
  transform: scaleX(1);
  background: #22c55e;
}

.password-hints {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  margin: 6px 0 0;
  padding: 0;
}

.password-hints li {
  list-style: none;
  font-size: 0.78rem;
  padding: 4px 10px;
  border-radius: 999px;
  background: rgba(226, 232, 240, 0.7);
  color: rgba(71, 85, 105, 0.85);
}

.password-hints li.passed {
  background: rgba(34, 197, 94, 0.16);
  color: #15803d;
}

.password-hints li.failed {
  background: rgba(248, 113, 113, 0.16);
  color: #b91c1c;
}

.input-error {
  border-color: rgba(239, 68, 68, 0.8) !important;
  box-shadow: 0 0 0 4px rgba(239, 68, 68, 0.12) !important;
}

@media (max-width: 1024px) {
  .signup-body {
    flex-direction: column;
    align-items: stretch;
  }

  .signup-hero {
    max-width: none;
  }

  .signup-panel {
    max-width: none;
  }
}

@media (max-width: 640px) {
  .signup-content {
    padding: 24px 20px 32px;
  }

  .signup-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .signup-panel {
    padding: 22px 20px;
  }

  .form-row {
    flex-direction: column;
  }
}
</style>
