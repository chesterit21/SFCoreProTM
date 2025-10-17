<template>
  <div class="login-page">
    <div class="login-backdrop" />
    <div class="login-content">
      <header class="login-header">
        <a class="login-brand" href="#">
          <img class="login-logo" src="/plane-logo.svg" alt="Plane" />
        </a>
        <div class="login-header-link">
          <span>New to Plane?</span>
          <a class="login-link" href="#">Sign up</a>
        </div>
      </header>

      <main class="login-body">
        <section class="login-panel">
          <div class="login-panel__heading">
            <h1>Work in all dimensions.</h1>
            <p>Welcome back to Plane.</p>
          </div>
          <form class="login-form" @submit.prevent="handleSubmit">
            <div class="form-field">
              <label for="email">Email</label>
              <input
                id="email"
                v-model="email"
                type="email"
                name="email"
                autocomplete="email"
                placeholder="name@company.com"
                required
              />
            </div>
            <div class="form-field">
              <div class="form-label">
                <label for="password">Password</label>
                <button
                  type="button"
                  class="form-link"
                  @click="togglePassword"
                  :aria-label="passwordVisible ? 'Hide password' : 'Show password'"
                >
                  {{ passwordVisible ? 'Hide' : 'Show' }}
                </button>
              </div>
              <input
                id="password"
                v-model="password"
                :type="passwordVisible ? 'text' : 'password'"
                name="password"
                autocomplete="current-password"
                placeholder="Enter your password"
                required
              />
            </div>
            <button type="submit" class="login-submit">Continue</button>
            <button type="button" class="login-secondary" @click="handleForgotPassword">Forgot password?</button>
          </form>
        </section>
      </main>

      <footer class="login-footer">
        <span>Join 10,000+ teams building with Plane</span>
        <div class="login-footer-logos">
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
import { ref } from 'vue'

const email = ref('')
const password = ref('')
const passwordVisible = ref(false)

const togglePassword = () => {
  passwordVisible.value = !passwordVisible.value
}

const handleForgotPassword = () => {
  console.info('Forgot password clicked')
}

const handleSubmit = () => {
  console.info('Submitting login form with:', {
    email: email.value,
    password: password.value.replace(/./g, '•')
  })
}
</script>

<style scoped>
.login-page {
  position: relative;
  min-height: 100vh;
  background: linear-gradient(180deg, #f8fafc 0%, #f3f4f6 100%);
  color: #171717;
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  overflow: hidden;
}

.login-backdrop {
  position: absolute;
  inset: 0;
  pointer-events: none;
  background: radial-gradient(circle at 10% 20%, rgba(0, 123, 194, 0.12) 0%, rgba(255, 255, 255, 0) 55%),
    radial-gradient(circle at 90% 15%, rgba(0, 99, 153, 0.14) 0%, rgba(255, 255, 255, 0) 60%);
}

.login-content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  padding: 32px clamp(16px, 4vw, 48px) 40px;
}

.login-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
}

.login-brand {
  display: inline-flex;
  align-items: center;
  text-decoration: none;
}

.login-logo {
  width: clamp(120px, 17vw, 170px);
  height: auto;
}

.login-header-link {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.95rem;
  color: rgba(17, 24, 39, 0.8);
}

.login-link {
  color: #006399;
  font-weight: 600;
  text-decoration: none;
}

.login-link:hover {
  text-decoration: underline;
}

.login-body {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: clamp(40px, 6vw, 72px) 0;
}

.login-panel {
  width: min(100%, 360px);
  background: rgba(255, 255, 255, 0.86);
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 28px clamp(20px, 4vw, 30px);
  box-shadow:
    0 10px 30px rgba(15, 23, 42, 0.08),
    0 2px 6px rgba(15, 23, 42, 0.04);
  backdrop-filter: blur(18px);
}

.login-panel__heading {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-bottom: 24px;
}

.login-panel__heading h1 {
  font-size: clamp(1.6rem, 2.2vw, 1.9rem);
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.login-panel__heading p {
  font-size: clamp(1.05rem, 2vw, 1.2rem);
  font-weight: 500;
  color: rgba(55, 65, 81, 0.9);
  margin: 0;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.form-field label {
  font-size: 0.95rem;
  font-weight: 500;
  color: rgba(17, 24, 39, 0.9);
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

.form-field input {
  height: 44px;
  border-radius: 10px;
  border: 1px solid #d1d5db;
  padding: 0 14px;
  font-size: 0.95rem;
  color: #111827;
  background-color: rgba(255, 255, 255, 0.96);
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.form-field input::placeholder {
  color: rgba(156, 163, 175, 0.95);
}

.form-field input:focus {
  border-color: rgba(0, 99, 153, 0.6);
  box-shadow: 0 0 0 4px rgba(0, 99, 153, 0.12);
  outline: none;
}

.login-submit {
  height: 46px;
  border-radius: 12px;
  border: none;
  background: linear-gradient(120deg, #007bc2 0%, #006399 100%);
  color: #ffffff;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.1s ease, box-shadow 0.1s ease;
}

.login-submit:hover {
  box-shadow: 0 12px 24px rgba(0, 99, 153, 0.25);
  transform: translateY(-1px);
}

.login-submit:active {
  transform: translateY(0);
  box-shadow: 0 6px 12px rgba(0, 99, 153, 0.3);
}

.login-secondary {
  height: 46px;
  border-radius: 12px;
  border: 1px solid rgba(0, 99, 153, 0.25);
  background: rgba(255, 255, 255, 0.65);
  color: #006399;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  transition: border 0.15s ease, color 0.15s ease;
}

.login-secondary:hover {
  border-color: rgba(0, 99, 153, 0.5);
  color: #00527a;
}

.login-footer {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  padding-top: clamp(32px, 5vw, 48px);
  color: rgba(55, 65, 81, 0.85);
  font-size: 0.95rem;
}

.login-footer-logos {
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
  background: rgba(229, 231, 235, 0.65);
  color: rgba(31, 41, 55, 0.85);
  font-weight: 600;
  font-size: 0.85rem;
}

@media (max-width: 640px) {
  .login-content {
    padding-top: 24px;
  }

  .login-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .login-header-link {
    order: 2;
  }

  .login-panel {
    width: 100%;
    border-radius: 14px;
  }

  .login-footer {
    padding-top: 28px;
  }
}
</style>
