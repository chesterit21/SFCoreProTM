<template>
  <div class="workspace-page">
    <header class="workspace-header">
      <button
        type="button"
        class="workspace-header__brand"
        @click="navigateTo('/dashboard')"
        aria-label="Kembali ke Dashboard"
      >
        <span class="brand-logo">SF</span>
        <span class="brand-title">SFCore Workspace</span>
      </button>
      <nav class="workspace-header__menu" aria-label="Menu aplikasi">
        <button
          v-for="menu in applicationMenu"
          :key="menu.id"
          type="button"
          class="workspace-header__menu-item"
          :class="{ 'workspace-header__menu-item--active': activeMenu === menu.id }"
          @click="handleMenuClick(menu)"
        >
          {{ menu.label }}
        </button>
      </nav>
      <div class="workspace-header__actions">
        <button type="button" class="workspace-header__action">Invite Team</button>
        <div class="workspace-header__avatar">JD</div>
      </div>
    </header>

    <main class="workspace-main">
      <section class="workspace-hero">
        <h1>{{ heroTitle }}</h1>
        <p>{{ heroDescription }}</p>
      </section>
      <section class="workspace-content">
        <slot>
          <p class="workspace-placeholder">{{ emptyMessage }}</p>
        </slot>
      </section>
    </main>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { applicationMenu } from '@/views/shared/navigationConfig'

const props = defineProps({
  pageKey: {
    type: String,
    required: true
  },
  heroTitle: {
    type: String,
    required: true
  },
  heroDescription: {
    type: String,
    default: 'Kelola pekerjaan lintas fungsi dengan tampilan yang fokus pada tim.'
  },
  emptyMessage: {
    type: String,
    default: 'Konten halaman akan tersedia setelah integrasi modul utama.'
  }
})

const activeMenu = ref(null)

const normalizePath = (path) => {
  if (!path) {
    return '/'
  }
  return path.replace(/\/+$/, '') || '/'
}

const normalizeRoute = (route) => normalizePath(route?.toLowerCase?.() ?? '')

const syncActiveMenu = () => {
  if (typeof window === 'undefined') {
    return
  }

  const currentPath = normalizeRoute(window.location.pathname)
  const matchedMenu = applicationMenu.find(menu => {
    const menuPath = normalizeRoute(menu.route)
    return currentPath === menuPath || currentPath.startsWith(`${menuPath}/`)
  })

  activeMenu.value = matchedMenu?.id ?? props.pageKey
}

if (typeof window !== 'undefined') {
  syncActiveMenu()
}

onMounted(() => {
  syncActiveMenu()
})

watch(() => props.pageKey, (value) => {
  if (!activeMenu.value) {
    activeMenu.value = value
  }
})

const navigateTo = (route) => {
  if (!route || typeof window === 'undefined') {
    return
  }
  const targetPath = normalizeRoute(route)
  const currentPath = normalizeRoute(window.location.pathname)
  if (targetPath === currentPath) {
    return
  }
  window.location.href = route
}

const handleMenuClick = (menu) => {
  if (!menu) {
    return
  }
  activeMenu.value = menu.id
  navigateTo(menu.route)
}
</script>

<style scoped>
.workspace-page {
  min-height: 100vh;
  background: #f8fafc;
  color: #0f172a;
  font-family: 'Inter', system-ui, sans-serif;
  display: flex;
  flex-direction: column;
}

.workspace-header {
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 24px;
  padding: 22px 40px;
  background: linear-gradient(120deg, rgba(15, 76, 129, 0.95) 0%, rgba(37, 99, 235, 0.82) 100%);
  color: #fff;
  box-shadow: 0 12px 24px -18px rgba(15, 23, 42, 0.4);
}

.workspace-header__brand {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px;
  border-radius: 14px;
  border: none;
  background: rgba(15, 23, 42, 0.22);
  color: inherit;
  cursor: pointer;
}

.brand-logo {
  display: grid;
  place-items: center;
  width: 38px;
  height: 38px;
  border-radius: 12px;
  background: rgba(248, 250, 252, 0.16);
  font-weight: 700;
  letter-spacing: 0.04em;
}

.brand-title {
  font-weight: 600;
  font-size: 1rem;
}

.workspace-header__menu {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 8px;
  background: rgba(15, 23, 42, 0.15);
  border-radius: 999px;
}

.workspace-header__menu-item {
  padding: 10px 18px;
  border: none;
  border-radius: 999px;
  background: transparent;
  color: rgba(248, 250, 252, 0.78);
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease;
}

.workspace-header__menu-item--active {
  background: rgba(248, 250, 252, 0.18);
  color: #fff;
  box-shadow: 0 10px 24px -14px rgba(15, 23, 42, 0.6);
}

.workspace-header__actions {
  display: inline-flex;
  gap: 16px;
  align-items: center;
}

.workspace-header__action {
  padding: 10px 18px;
  border-radius: 12px;
  border: none;
  font-weight: 600;
  background: #facc15;
  color: #0f172a;
  cursor: pointer;
}

.workspace-header__avatar {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: rgba(248, 250, 252, 0.18);
  display: grid;
  place-items: center;
  font-weight: 700;
}

.workspace-main {
  flex: 1;
  padding: 36px 48px 48px;
  display: flex;
  flex-direction: column;
  gap: 28px;
}

.workspace-hero {
  background: #fff;
  border-radius: 24px;
  padding: 32px;
  box-shadow: 0 18px 32px -28px rgba(15, 23, 42, 0.45);
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.workspace-hero h1 {
  margin: 0;
  font-size: 2rem;
  font-weight: 700;
}

.workspace-hero p {
  margin: 0;
  font-size: 1rem;
  color: #475569;
}

.workspace-content {
  flex: 1;
  background: #fff;
  border-radius: 24px;
  padding: 32px;
  box-shadow: 0 20px 40px -32px rgba(15, 23, 42, 0.4);
}

.workspace-placeholder {
  margin: 0;
  color: #64748b;
  font-size: 1rem;
  text-align: center;
}

@media (max-width: 1180px) {
  .workspace-header {
    grid-template-columns: 1fr;
    justify-items: center;
    gap: 16px;
  }

  .workspace-main {
    padding: 32px 24px 40px;
  }
}

@media (max-width: 720px) {
  .workspace-main {
    padding: 28px 20px 32px;
  }

  .workspace-content,
  .workspace-hero {
    padding: 24px;
  }
}
</style>
