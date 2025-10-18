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
          <a class="signup-link" href="/">Sign in</a>
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
            <div v-if="errorMessage" class="form-alert form-alert--error">
              {{ errorMessage }}
            </div>
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

            <button type="submit" class="signup-submit" :disabled="isSubmitting || !canSubmit">
              <span v-if="isSubmitting" class="button-spinner" aria-hidden="true" />
              <span>{{ isSubmitting ? 'Creating workspace…' : 'Create workspace' }}</span>
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

<script src="./IndexTemplate.script.js"></script>

<style scoped src="./IndexTemplate.style.css"></style>
