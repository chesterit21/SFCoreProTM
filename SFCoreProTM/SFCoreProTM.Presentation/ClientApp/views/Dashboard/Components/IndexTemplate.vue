<template>
  <div class="dashboard">
    <header class="dashboard__header">
      <div class="header__brand">
        <div class="brand__logo">SF</div>
        <div class="brand__meta">
          <p class="brand__workspace">SFCore Workspace</p>
          <span class="brand__timestamp">{{ todaysDate }}</span>
        </div>
      </div>
      <nav class="header__menu" aria-label="Menu aplikasi">
        <button
          v-for="menu in applicationMenu"
          :key="menu.id"
          type="button"
          class="header__menu-item"
          :class="{ 'header__menu-item--active': activeApplicationMenu === menu.id }"
          @click="handleApplicationMenu(menu)"
        >
          {{ menu.label }}
        </button>
      </nav>
      <div class="header__actions">
        <label class="header__search">
          <span class="header__search-icon">⌕</span>
          <input type="search" placeholder="Cari proyek atau tugas" />
        </label>
        <button type="button" class="header__quick-action">
          Buat Task
        </button>
        <div class="header__avatar">JD</div>
      </div>
    </header>

    <div class="dashboard__body">
      <aside class="dashboard__sidebar">
        <section class="sidebar__section sidebar__section--profile">
          <p class="sidebar__section-title">Halo, Jane!</p>
          <p class="sidebar__section-subtitle">Kelola seluruh aktivitas tim Anda dari sini.</p>
          <button type="button" class="sidebar__cta">Lihat Profil</button>
        </section>

        <section
          v-for="section in sidebarSections"
          :key="section.id"
          class="sidebar__section"
        >
          <p class="sidebar__section-title">{{ section.label }}</p>
          <ul class="sidebar__menu">
            <li
              v-for="item in section.items"
              :key="item.id"
              class="sidebar__menu-item"
              :class="{ 'sidebar__menu-item--active': activeSidebarMenu === item.id }"
              @click="handleSidebarMenu(item)"
            >
              <span class="sidebar__menu-icon">{{ item.icon }}</span>
              <div class="sidebar__menu-text">
                <span class="sidebar__menu-label">{{ item.label }}</span>
                <span v-if="item.caption" class="sidebar__menu-caption">{{ item.caption }}</span>
              </div>
              <span v-if="item.badge" class="sidebar__menu-badge">{{ item.badge }}</span>
            </li>
          </ul>
        </section>

        <section class="sidebar__section sidebar__section--footer">
          <div class="sidebar__footer-card">
            <p class="footer-card__title">Bantuan Cepat</p>
            <p class="footer-card__text">Hubungi tim support atau lihat dokumentasi internal.</p>
            <button type="button" class="footer-card__action">Hubungi Kami</button>
          </div>
        </section>
      </aside>

      <main class="dashboard__main">
        <template v-if="isOverview">
          <section class="main__summary">
            <article
              v-for="summary in summaries"
              :key="summary.id"
              class="summary-card"
            >
              <div class="summary-card__icon">{{ summary.icon }}</div>
              <div class="summary-card__meta">
                <p class="summary-card__label">{{ summary.label }}</p>
                <p class="summary-card__value">{{ summary.value }}</p>
                <span class="summary-card__delta" :class="summary.delta > 0 ? 'summary-card__delta--up' : 'summary-card__delta--down'">
                  {{ summary.delta > 0 ? '+' : '' }}{{ summary.delta }}%
                </span>
              </div>
            </article>
          </section>

          <section class="main__panel">
            <header class="panel__header">
              <h2 class="panel__title">Ringkasan Aktivitas</h2>
              <div class="panel__filters">
                <button
                  v-for="filter in activityFilters"
                  :key="filter.id"
                  type="button"
                  class="panel__filter"
                  :class="{ 'panel__filter--active': activeActivityFilter === filter.id }"
                  @click="handleActivityFilter(filter.id)"
                >
                  {{ filter.label }}
                </button>
              </div>
            </header>
            <div class="panel__content">
              <div
                v-for="activity in filteredActivities"
                :key="activity.id"
                class="activity-card"
              >
                <div class="activity-card__status" :style="{ backgroundColor: activity.color }" />
                <div class="activity-card__meta">
                  <p class="activity-card__title">{{ activity.title }}</p>
                  <p class="activity-card__description">{{ activity.description }}</p>
                </div>
                <span class="activity-card__time">{{ activity.time }}</span>
              </div>
            </div>
          </section>
        </template>
        <section v-else class="main__placeholder" role="status">
          <div class="placeholder-card">
            <h2>{{ currentSidebarItem?.label ?? 'Dashboard' }}</h2>
            <p>{{ currentSidebarItem?.placeholder ?? 'Konten akan tersedia segera.' }}</p>
            <button
              type="button"
              class="placeholder__cta"
              @click="navigateTo('/dashboard')"
            >
              Kembali ke Overview
            </button>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { applicationMenu, dashboardSidebarSections } from '@/views/shared/navigationConfig'

const todaysDate = new Intl.DateTimeFormat('id-ID', {
  weekday: 'long',
  day: 'numeric',
  month: 'long',
  year: 'numeric'
}).format(new Date())

const sidebarSections = dashboardSidebarSections

const activityFilters = [
  { id: 'today', label: 'Hari ini' },
  { id: 'week', label: '7 Hari' },
  { id: 'month', label: '30 Hari' }
]

const activities = [
  {
    id: 'activity-1',
    title: 'Update modul autentikasi',
    description: 'Pull request #24 menunggu review dari tim backend.',
    time: '15 menit lalu',
    category: 'today',
    color: '#0f766e'
  },
  {
    id: 'activity-2',
    title: 'Sprint 8 dimulai',
    description: 'Sprint board digenerate dengan 18 tasks prioritas.',
    time: '3 jam lalu',
    category: 'today',
    color: '#2563eb'
  },
  {
    id: 'activity-3',
    title: 'Review desain dashboard',
    description: 'Tim desain mengunggah mockup versi 1.4.',
    time: 'Kemarin',
    category: 'week',
    color: '#9d174d'
  },
  {
    id: 'activity-4',
    title: 'Release build 0.3.2',
    description: 'Build staging tersedia untuk pengujian QA.',
    time: '3 hari lalu',
    category: 'week',
    color: '#1d4ed8'
  },
  {
    id: 'activity-5',
    title: 'Perencanaan kuartal 2',
    description: 'Catatan rapat direvisi dengan tujuan baru.',
    time: '2 minggu lalu',
    category: 'month',
    color: '#7c3aed'
  }
]

const summaries = [
  { id: 'open-tasks', label: 'Tugas Terbuka', value: 34, delta: 8, icon: '📌' },
  { id: 'completed', label: 'Selesai Minggu Ini', value: 19, delta: 12, icon: '✅' },
  { id: 'overdue', label: 'Terlambat', value: 5, delta: -3, icon: '⚠️' }
]

const favoriteNavigation = [
  { label: 'Sprint 14', color: '#006399' },
  { label: 'Design backlog', color: '#0f172a' },
  { label: 'Release planning', color: '#2563eb' }
]

const activeApplicationMenu = ref(null)
const activeSidebarMenu = ref('overview')
const activeActivityFilter = ref(activityFilters[0].id)

const allSidebarItems = computed(() =>
  sidebarSections.flatMap(section => section.items)
)

const currentSidebarItem = computed(() =>
  allSidebarItems.value.find(item => item.id === activeSidebarMenu.value)
)

const isOverview = computed(() => currentSidebarItem.value?.id === 'overview')

const normalizePath = (path) => {
  if (!path) {
    return '/'
  }
  return path.replace(/\/+$/, '') || '/'
}

const normalizeRoute = (route) => normalizePath(route?.toLowerCase?.() ?? '')

const syncActiveStates = () => {
  if (typeof window === 'undefined') {
    return
  }

  const currentPath = normalizeRoute(window.location.pathname)

  const matchedSidebar = allSidebarItems.value.find(item => normalizeRoute(item.route) === currentPath)
  if (matchedSidebar) {
    activeSidebarMenu.value = matchedSidebar.id
  } else {
    activeSidebarMenu.value = 'overview'
  }

  const matchedApplication = applicationMenu.find(menu => {
    const menuPath = normalizeRoute(menu.route)
    return currentPath === menuPath || currentPath.startsWith(`${menuPath}/`)
  })

  activeApplicationMenu.value = matchedApplication?.id ?? null
}

if (typeof window !== 'undefined') {
  syncActiveStates()
}

onMounted(() => {
  syncActiveStates()
})

const filteredActivities = computed(() => {
  return activities.filter(activity => {
    if (activeActivityFilter.value === 'month') {
      return true
    }

    if (activeActivityFilter.value === 'week') {
      return activity.category === 'week' || activity.category === 'today'
    }

    return activity.category === 'today'
  })
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

const handleApplicationMenu = (menu) => {
  if (!menu) {
    return
  }

  activeApplicationMenu.value = menu.id
  navigateTo(menu.route)
}

const handleSidebarMenu = (item) => {
  if (!item) {
    return
  }

  activeSidebarMenu.value = item.id
  navigateTo(item.route)
}

const handleActivityFilter = (id) => {
  activeActivityFilter.value = id
}
</script>

<style scoped>
.dashboard {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: #f8fafc;
  color: #0f172a;
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

.dashboard__header {
  display: grid;
  grid-template-columns: minmax(0, 320px) 1fr auto;
  align-items: center;
  gap: 28px;
  padding: 24px 40px;
  background: linear-gradient(135deg, rgba(15, 76, 129, 0.95) 0%, rgba(37, 99, 235, 0.85) 100%);
  color: #fff;
  box-shadow: 0 12px 24px -18px rgba(15, 23, 42, 0.4);
}

.header__brand {
  display: flex;
  align-items: center;
  gap: 16px;
}

.brand__logo {
  display: grid;
  place-items: center;
  width: 52px;
  height: 52px;
  border-radius: 16px;
  background: rgba(15, 23, 42, 0.35);
  font-weight: 700;
  font-size: 1.25rem;
  letter-spacing: 0.05em;
}

.brand__meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.brand__workspace {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 700;
}

.brand__timestamp {
  font-size: 0.85rem;
  color: rgba(248, 250, 252, 0.78);
}

.header__menu {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 8px;
  background: rgba(15, 23, 42, 0.16);
  border-radius: 999px;
}

.header__menu-item {
  padding: 10px 18px;
  border-radius: 999px;
  border: none;
  background: transparent;
  color: rgba(248, 250, 252, 0.78);
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease;
}

.header__menu-item--active {
  background: rgba(248, 250, 252, 0.18);
  color: #fff;
  box-shadow: 0 10px 24px -14px rgba(15, 23, 42, 0.65);
}

.header__actions {
  display: flex;
  align-items: center;
  gap: 18px;
}

.header__search {
  position: relative;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: 10px 14px;
  background: rgba(15, 23, 42, 0.18);
  border-radius: 12px;
  color: rgba(248, 250, 252, 0.78);
}

.header__search input {
  background: transparent;
  border: none;
  color: inherit;
  outline: none;
  width: 180px;
  font-size: 0.9rem;
}

.header__search input::placeholder {
  color: rgba(248, 250, 252, 0.68);
}

.header__search-icon {
  font-size: 0.9rem;
}

.header__quick-action {
  padding: 10px 18px;
  border-radius: 12px;
  font-weight: 600;
  background: #facc15;
  border: none;
  cursor: pointer;
  color: #0f172a;
  transition: transform 0.2s ease;
}

.header__quick-action:hover {
  transform: translateY(-1px);
}

.header__avatar {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: rgba(15, 23, 42, 0.25);
  display: grid;
  place-items: center;
  font-weight: 700;
}

.dashboard__body {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: 32px;
  padding: 32px 40px 40px;
}

.dashboard__sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.sidebar__section {
  background: #fff;
  border-radius: 20px;
  padding: 20px;
  box-shadow: 0 18px 32px -26px rgba(15, 23, 42, 0.4);
}

.sidebar__section--profile {
  background: linear-gradient(145deg, #1d4ed8 0%, #7c3aed 100%);
  color: #fff;
  box-shadow: 0 18px 32px -24px rgba(30, 64, 175, 0.55);
}

.sidebar__section-title {
  margin: 0;
  font-size: 1rem;
  font-weight: 700;
  letter-spacing: 0.02em;
}

.sidebar__section-subtitle {
  margin: 8px 0 0;
  font-size: 0.9rem;
  line-height: 1.5;
  color: rgba(248, 250, 252, 0.85);
}

.sidebar__cta {
  margin-top: 18px;
  padding: 10px 16px;
  border-radius: 12px;
  border: none;
  font-weight: 600;
  cursor: pointer;
  background: rgba(15, 23, 42, 0.2);
  color: inherit;
}

.sidebar__menu {
  list-style: none;
  margin: 16px 0 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.sidebar__menu-item {
  display: grid;
  grid-template-columns: 32px 1fr auto;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-radius: 14px;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease, transform 0.2s ease;
  color: #0f172a;
}

.sidebar__menu-item:hover {
  background: rgba(15, 23, 42, 0.04);
}

.sidebar__menu-item--active {
  background: rgba(37, 99, 235, 0.1);
  transform: translateX(4px);
}

.sidebar__menu-icon {
  font-size: 1.1rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.sidebar__menu-text {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.sidebar__menu-label {
  font-weight: 600;
}

.sidebar__menu-caption {
  font-size: 0.78rem;
  color: #6b7280;
}

.sidebar__menu-badge {
  font-size: 0.75rem;
  padding: 4px 8px;
  border-radius: 999px;
  background: rgba(37, 99, 235, 0.18);
  color: #1d4ed8;
  font-weight: 600;
}

.sidebar__footer-card {
  background: #111827;
  color: #f8fafc;
  border-radius: 16px;
  padding: 18px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.footer-card__title {
  margin: 0;
  font-weight: 700;
}

.footer-card__text {
  margin: 0;
  font-size: 0.85rem;
  color: rgba(248, 250, 252, 0.76);
}

.footer-card__action {
  align-self: flex-start;
  padding: 8px 16px;
  border-radius: 12px;
  border: none;
  font-weight: 600;
  cursor: pointer;
  background: #facc15;
  color: #111827;
}

.dashboard__main {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.main__summary {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
}

.summary-card {
  background: #fff;
  border-radius: 18px;
  padding: 20px;
  display: grid;
  grid-template-columns: 54px 1fr;
  gap: 16px;
  align-items: center;
  box-shadow: 0 20px 32px -32px rgba(15, 23, 42, 0.6);
}

.summary-card__icon {
  display: grid;
  place-items: center;
  width: 54px;
  height: 54px;
  border-radius: 16px;
  background: #e0f2fe;
  font-size: 1.3rem;
}

.summary-card__label {
  margin: 0;
  font-size: 0.85rem;
  color: #6b7280;
}

.summary-card__value {
  margin: 4px 0 0;
  font-size: 1.75rem;
  font-weight: 700;
  color: #0f172a;
}

.summary-card__delta {
  font-size: 0.85rem;
  font-weight: 600;
}

.summary-card__delta--up {
  color: #059669;
}

.summary-card__delta--down {
  color: #dc2626;
}

.main__panel {
  background: #fff;
  border-radius: 24px;
  padding: 24px;
  box-shadow: 0 24px 48px -36px rgba(15, 23, 42, 0.45);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.panel__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.panel__title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 700;
}

.panel__filters {
  display: inline-flex;
  gap: 10px;
}

.panel__filter {
  padding: 8px 16px;
  border-radius: 999px;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #334155;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease, border 0.2s ease;
}

.panel__filter--active {
  background: #1d4ed8;
  border-color: #1d4ed8;
  color: #fff;
}

.panel__content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.main__placeholder {
  background: #fff;
  border-radius: 24px;
  padding: 32px;
  box-shadow: 0 24px 48px -36px rgba(15, 23, 42, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 320px;
  text-align: center;
}

.placeholder-card {
  max-width: 440px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.placeholder-card h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #0f172a;
}

.placeholder-card p {
  margin: 0;
  font-size: 0.95rem;
  color: #475569;
  line-height: 1.6;
}

.placeholder__cta {
  align-self: center;
  padding: 10px 24px;
  border-radius: 12px;
  border: none;
  background: #1d4ed8;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.placeholder__cta:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 20px -16px rgba(29, 78, 216, 0.6);
}

.activity-card {
  display: grid;
  grid-template-columns: 8px 1fr auto;
  gap: 16px;
  padding: 16px 18px;
  border-radius: 16px;
  background: #f8fafc;
  align-items: center;
}

.activity-card__status {
  width: 8px;
  height: 100%;
  border-radius: 999px;
}

.activity-card__meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.activity-card__title {
  margin: 0;
  font-weight: 600;
  color: #0f172a;
}

.activity-card__description {
  margin: 0;
  font-size: 0.85rem;
  color: #475569;
}

.activity-card__time {
  font-size: 0.78rem;
  color: #64748b;
  font-weight: 600;
}

@media (max-width: 1180px) {
  .dashboard__header {
    grid-template-columns: 1fr;
    gap: 16px;
    text-align: center;
  }

  .header__brand {
    justify-content: center;
  }

  .header__actions {
    justify-content: center;
  }

  .dashboard__body {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 720px) {
  .main__summary {
    grid-template-columns: 1fr;
  }

  .dashboard__body {
    padding: 28px 20px 32px;
  }
}
</style>
