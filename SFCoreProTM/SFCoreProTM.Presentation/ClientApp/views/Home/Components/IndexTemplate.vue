<!-- ClientApp/views/Home/Components/IndexTemplate.vue -->
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
          <a class="login-link" href="/signup">Sign up</a>
        </div>
      </header>

      <main class="login-body">
        <section class="login-panel">
          <div class="login-panel__heading">
            <h1>Work in all dimensions.</h1>
            <p>Welcome back to Plane.</p>
          </div>
          <form class="login-form" @submit.prevent="handleSubmit" novalidate>
            <div v-if="errorMessage" class="form-alert form-alert--error" role="alert">
              {{ errorMessage }}
            </div>
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
            <div class="form-checkbox">
              <input id="remember-me" v-model="rememberMe" type="checkbox" name="rememberMe" />
              <label for="remember-me">Remember this device</label>
            </div>
            <button type="submit" class="login-submit" :disabled="isSubmitting">
              <span v-if="isSubmitting">Signing in…</span>
              <span v-else>Continue</span>
            </button>
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

<script>
// Skrip di sini hanyalah untuk ekspor template agar bisa diimpor, 
// logika utamanya akan disuntikkan di Index-main.js

// Tidak perlu menambahkan logika di sini karena sudah dipindahkan
export default {
  // Hanya mengekspor objek kosong atau yang diperlukan untuk kompiler Vue
  // Misalnya, jika Anda ingin menambahkan nama komponen:
  name: 'IndexTemplate'
}
</script>
<style scoped src="./IndexTemplate.style.css"></style>
