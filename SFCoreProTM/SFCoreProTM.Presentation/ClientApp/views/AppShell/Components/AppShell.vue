<template>
  <div class="app-shell">
    <aside class="app-shell__sidebar">
      <div class="sidebar__workspace">
        <div class="workspace-logo">PL</div>
        <div class="workspace-meta">
          <span class="workspace-meta__name">Plane Workspace</span>
          <button class="workspace-meta__switch" type="button">Switch</button>
        </div>
      </div>

      <div class="sidebar__section">
        <p class="sidebar__section-title">Main</p>
        <ul class="sidebar__nav">
          <li
            v-for="item in primaryNavigation"
            :key="item.label"
            :class="['sidebar__nav-item', { 'sidebar__nav-item--active': activePrimary === item.label }]"
            @click="activePrimary = item.label"
          >
            <span class="sidebar__nav-icon">{{ item.icon }}</span>
            <span class="sidebar__nav-label">{{ item.label }}</span>
            <span v-if="item.badge" class="sidebar__nav-badge">{{ item.badge }}</span>
          </li>
        </ul>
      </div>

      <div class="sidebar__section">
        <div class="sidebar__section-header">
          <p class="sidebar__section-title">Favorites</p>
          <button class="sidebar__section-action" type="button" aria-label="Add favourite">+</button>
        </div>
        <ul class="sidebar__nav sidebar__nav--secondary">
          <li
            v-for="item in favoriteNavigation"
            :key="item.label"
            class="sidebar__nav-item"
          >
            <span class="sidebar__nav-color" :style="{ backgroundColor: item.color }" />
            <span class="sidebar__nav-label">{{ item.label }}</span>
          </li>
        </ul>
      </div>

      <div class="sidebar__section sidebar__section--bottom">
        <div class="sidebar__hint">
          <p class="sidebar__hint-title">Need help?</p>
          <p class="sidebar__hint-description">Browse documentation and tutorials.</p>
          <button class="sidebar__hint-action" type="button">Open docs</button>
        </div>
        <div class="sidebar__footer">
          <div class="sidebar__footer-avatar">JD</div>
          <div class="sidebar__footer-meta">
            <span class="sidebar__footer-name">Jane Doe</span>
            <button class="sidebar__footer-action" type="button">Sign out</button>
          </div>
        </div>
      </div>
    </aside>

    <div class="app-shell__main">
      <header class="main-header">
        <div class="main-header__primary">
          <nav aria-label="Breadcrumb" class="breadcrumb">
            <span class="breadcrumb__item breadcrumb__item--muted">Workspace</span>
            <span class="breadcrumb__divider">/</span>
            <span class="breadcrumb__item">Planning Board</span>
          </nav>
          <h1 class="main-header__title">Planning Board</h1>
          <p class="main-header__subtitle">Track issues, priorities, and team progress in real time.</p>
        </div>
        <div class="main-header__actions">
          <label class="main-header__search">
            <span class="main-header__search-icon">⌕</span>
            <input type="search" placeholder="Search issues" />
          </label>
          <button type="button" class="main-header__button main-header__button--outline">Share</button>
          <button type="button" class="main-header__button main-header__button--primary">New issue</button>
          <div class="main-header__avatar">JD</div>
        </div>
      </header>

      <section class="main-body">
        <div class="main-body__panel">
          <div class="main-body__panel-header">
            <h2>Overview</h2>
            <button type="button">Customize</button>
          </div>
          <div class="main-body__placeholder">
            <p>This is where your dashboard widgets and issue lists will live.</p>
            <p>Use this area to replicate Plane modules later.</p>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const primaryNavigation = [
  { label: 'Home', icon: '⌂' },
  { label: 'My Issues', icon: '☰', badge: 6 },
  { label: 'Roadmap', icon: '⌖' },
  { label: 'Cycles', icon: '⟳' },
  { label: 'Views', icon: '▣' }
]

const favoriteNavigation = [
  { label: 'Sprint 14', color: '#006399' },
  { label: 'Design backlog', color: '#0f172a' },
  { label: 'Release planning', color: '#2563eb' }
]

const activePrimary = ref('My Issues')
</script>

<style scoped>
.app-shell {
  display: flex;
  min-height: 100vh;
  background: #f5f7fb;
  color: #111827;
  font-family: 'Inter', system-ui, sans-serif;
}

.app-shell__sidebar {
  width: 260px;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0.96) 0%, rgba(246, 248, 252, 0.92) 100%);
  border-right: 1px solid #e5e7eb;
  padding: 20px 18px;
  display: flex;
  flex-direction: column;
  gap: 24px;
  box-shadow: 8px 0 24px -16px rgba(15, 23, 42, 0.25);
}

.sidebar__workspace {
  display: flex;
  gap: 12px;
  align-items: center;
}

.workspace-logo {
  height: 40px;
  width: 40px;
  border-radius: 12px;
  background: linear-gradient(135deg, #007bc2 0%, #006399 100%);
  color: #fff;
  display: grid;
  place-items: center;
  font-weight: 700;
  letter-spacing: 0.02em;
}

.workspace-meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.workspace-meta__name {
  font-weight: 600;
  font-size: 1rem;
  color: #0f172a;
}

.workspace-meta__switch {
  padding: 4px 10px;
  border-radius: 999px;
  border: 1px solid rgba(15, 23, 42, 0.12);
  background: rgba(255, 255, 255, 0.85);
  font-size: 0.75rem;
  font-weight: 600;
  color: #006399;
  cursor: pointer;
}

.sidebar__section {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.sidebar__section--bottom {
  margin-top: auto;
}

.sidebar__section-title {
  margin: 0;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: rgba(15, 23, 42, 0.55);
}

.sidebar__section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.sidebar__section-action {
  height: 24px;
  width: 24px;
  border-radius: 8px;
  border: 1px solid rgba(15, 23, 42, 0.1);
  background: rgba(248, 250, 252, 0.9);
  color: #0f172a;
  font-weight: 600;
  cursor: pointer;
}

.sidebar__nav {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.sidebar__nav--secondary .sidebar__nav-item {
  padding-left: 10px;
}

.sidebar__nav-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: 12px;
  font-size: 0.95rem;
  font-weight: 500;
  color: rgba(15, 23, 42, 0.8);
  cursor: pointer;
  transition: background 0.15s ease, color 0.15s ease, box-shadow 0.15s ease;
}

.sidebar__nav-item:hover {
  background: rgba(0, 99, 153, 0.08);
  color: #006399;
}

.sidebar__nav-item--active {
  background: rgba(0, 99, 153, 0.16);
  color: #00527a;
  box-shadow: inset 0 0 0 1px rgba(0, 83, 122, 0.25);
}

.sidebar__nav-icon {
  width: 18px;
  display: inline-flex;
  justify-content: center;
  color: rgba(15, 23, 42, 0.55);
}

.sidebar__nav-badge {
  margin-left: auto;
  padding: 2px 8px;
  border-radius: 999px;
  font-size: 0.7rem;
  font-weight: 600;
  color: #fff;
  background: #0f172a;
}

.sidebar__nav-color {
  height: 8px;
  width: 8px;
  border-radius: 999px;
  border: 2px solid #fff;
  box-shadow: 0 0 0 1px rgba(15, 23, 42, 0.1);
}

.sidebar__hint {
  padding: 16px;
  border-radius: 16px;
  background: rgba(0, 99, 153, 0.08);
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.sidebar__hint-title {
  margin: 0;
  font-weight: 600;
  font-size: 0.95rem;
}

.sidebar__hint-description {
  margin: 0;
  font-size: 0.82rem;
  color: rgba(15, 23, 42, 0.7);
}

.sidebar__hint-action {
  align-self: flex-start;
  padding: 6px 12px;
  border-radius: 10px;
  border: none;
  background: rgba(0, 99, 153, 0.92);
  color: #fff;
  font-size: 0.78rem;
  font-weight: 600;
  cursor: pointer;
}

.sidebar__footer {
  margin-top: 16px;
  padding: 12px;
  border-radius: 14px;
  background: rgba(15, 23, 42, 0.05);
  display: flex;
  gap: 12px;
  align-items: center;
}

.sidebar__footer-avatar {
  height: 36px;
  width: 36px;
  border-radius: 10px;
  background: #0f172a;
  color: #fff;
  display: grid;
  place-items: center;
  font-weight: 600;
}

.sidebar__footer-meta {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.sidebar__footer-name {
  font-size: 0.95rem;
  font-weight: 600;
}

.sidebar__footer-action {
  font-size: 0.82rem;
  font-weight: 600;
  color: #006399;
  background: none;
  border: none;
  padding: 0;
  cursor: pointer;
  align-self: flex-start;
}

.app-shell__main {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: rgba(247, 248, 252, 0.9);
}

.main-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 24px;
  padding: 24px clamp(24px, 5vw, 48px);
  background: rgba(255, 255, 255, 0.92);
  border-bottom: 1px solid rgba(226, 232, 240, 0.8);
  box-shadow: 0 12px 30px -24px rgba(15, 23, 42, 0.45);
  backdrop-filter: blur(20px);
}

.main-header__primary {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.breadcrumb {
  display: flex;
  gap: 8px;
  align-items: center;
  font-size: 0.85rem;
  color: rgba(55, 65, 81, 0.85);
}

.breadcrumb__item--muted {
  color: rgba(55, 65, 81, 0.6);
}

.main-header__title {
  margin: 0;
  font-size: clamp(1.6rem, 2.4vw, 2rem);
  font-weight: 600;
  color: #0f172a;
}

.main-header__subtitle {
  margin: 0;
  font-size: 0.95rem;
  color: rgba(55, 65, 81, 0.85);
}

.main-header__actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.main-header__search {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  border-radius: 12px;
  border: 1px solid rgba(15, 23, 42, 0.08);
  background: rgba(248, 249, 253, 0.9);
  padding: 0 14px;
  height: 44px;
}

.main-header__search input {
  border: none;
  background: transparent;
  font-size: 0.95rem;
  color: #0f172a;
  min-width: 160px;
}

.main-header__search input:focus {
  outline: none;
}

.main-header__search-icon {
  font-size: 0.95rem;
  color: rgba(55, 65, 81, 0.55);
}

.main-header__button {
  height: 44px;
  padding: 0 18px;
  border-radius: 12px;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  border: 1px solid transparent;
  transition: transform 0.1s ease, box-shadow 0.1s ease;
}

.main-header__button--outline {
  background: rgba(248, 249, 253, 0.9);
  border-color: rgba(15, 23, 42, 0.1);
  color: #0f172a;
}

.main-header__button--primary {
  background: linear-gradient(135deg, #007bc2 0%, #006399 100%);
  color: #fff;
  box-shadow: 0 12px 20px -12px rgba(0, 99, 153, 0.6);
}

.main-header__button:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 20px -16px rgba(15, 23, 42, 0.35);
}

.main-header__avatar {
  height: 40px;
  width: 40px;
  border-radius: 12px;
  background: rgba(15, 23, 42, 0.9);
  color: #fff;
  display: grid;
  place-items: center;
  font-weight: 600;
}

.main-body {
  flex: 1;
  padding: clamp(24px, 5vw, 48px);
  overflow: auto;
}

.main-body__panel {
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 20px;
  padding: 24px;
  box-shadow:
    0 18px 32px -24px rgba(15, 23, 42, 0.4),
    0 4px 16px -12px rgba(15, 23, 42, 0.2);
  backdrop-filter: blur(14px);
}

.main-body__panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.main-body__panel-header h2 {
  margin: 0;
  font-size: 1.1rem;
  color: #0f172a;
}

.main-body__panel-header button {
  background: none;
  border: 1px solid rgba(15, 23, 42, 0.12);
  border-radius: 10px;
  padding: 8px 16px;
  font-size: 0.85rem;
  font-weight: 600;
  color: #006399;
  cursor: pointer;
}

.main-body__placeholder {
  border: 1px dashed rgba(15, 23, 42, 0.12);
  border-radius: 16px;
  padding: 32px;
  text-align: center;
  display: flex;
  flex-direction: column;
  gap: 12px;
  color: rgba(55, 65, 81, 0.8);
}

@media (max-width: 1080px) {
  .app-shell__sidebar {
    width: 220px;
  }

  .main-header {
    flex-direction: column;
    align-items: stretch;
  }

  .main-header__actions {
    justify-content: flex-end;
    flex-wrap: wrap;
  }

  .main-header__search {
    flex: 1;
    min-width: 200px;
  }
}

@media (max-width: 820px) {
  .app-shell {
    flex-direction: column;
  }

  .app-shell__sidebar {
    width: 100%;
    flex-direction: row;
    flex-wrap: wrap;
    gap: 16px;
    padding: 16px;
    box-shadow: none;
    border-right: none;
    border-bottom: 1px solid #e5e7eb;
  }

  .sidebar__section {
    flex: 1;
    min-width: 200px;
  }

  .sidebar__section--bottom {
    flex-basis: 100%;
  }
}

@media (max-width: 640px) {
  .main-header__actions {
    flex-direction: column;
    align-items: stretch;
  }

  .main-header__search {
    width: 100%;
  }

  .main-header__button {
    width: 100%;
  }
}
</style>
