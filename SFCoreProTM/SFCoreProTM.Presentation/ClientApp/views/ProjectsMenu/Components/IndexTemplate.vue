<template>
  <ApplicationPageTemplate
    page-key="projects"
    hero-title="Projects Workspace"
    hero-description="Kelola portfolio produk dan inisiatif lintas tim dalam satu tampilan."
  >
    <section class="projects-grid">
      <article
        v-for="project in projects"
        :key="project.id"
        class="project-card"
      >
        <header class="project-card__header">
          <span class="project-card__badge" :style="{ backgroundColor: project.badgeColor }">{{ project.badge }}</span>
          <span class="project-card__status" :class="`project-card__status--${project.status}`">{{ project.statusLabel }}</span>
        </header>
        <h3>{{ project.name }}</h3>
        <p>{{ project.description }}</p>
        <footer class="project-card__footer">
          <div class="project-card__owner">
            <span class="owner-avatar">{{ project.ownerInitials }}</span>
            <span>{{ project.owner }}</span>
          </div>
          <div class="project-card__progress">
            <span>{{ project.progress }}%</span>
            <div class="project-card__progress-bar">
              <span :style="{ width: `${project.progress}%` }" />
            </div>
          </div>
        </footer>
      </article>
    </section>
  </ApplicationPageTemplate>
</template>

<script setup>
import ApplicationPageTemplate from '@/views/shared/ApplicationPageTemplate.vue'

const projects = [
  {
    id: 'alpha',
    name: 'Plane Mobile App',
    description: 'Iterasi desain terbaru untuk aplikasi mobile dengan fokus peningkatan onboarding.',
    owner: 'Dimas Priatna',
    ownerInitials: 'DP',
    badge: 'Mobile',
    badgeColor: '#0f172a',
    status: 'in-progress',
    statusLabel: 'In Progress',
    progress: 68
  },
  {
    id: 'beta',
    name: 'Plane API Gateway',
    description: 'Refactor modul autentikasi dan penambahan rate limiting untuk integrasi publik.',
    owner: 'Sinta Aprilia',
    ownerInitials: 'SA',
    badge: 'Backend',
    badgeColor: '#1d4ed8',
    status: 'review',
    statusLabel: 'Review',
    progress: 54
  },
  {
    id: 'gamma',
    name: 'Plane Docs Revamp',
    description: 'Menyusun ulang dokumentasi developer untuk meningkatkan discoverability fitur.',
    owner: 'Rafi Mahendra',
    ownerInitials: 'RM',
    badge: 'Docs',
    badgeColor: '#7c3aed',
    status: 'planned',
    statusLabel: 'Planned',
    progress: 25
  }
]
</script>

<style scoped>
.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 20px;
}

.project-card {
  background: #f8fafc;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 20px 40px -34px rgba(15, 23, 42, 0.5);
}

.project-card__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.project-card__badge {
  padding: 6px 12px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
  color: #fff;
}

.project-card__status {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 999px;
  background: rgba(15, 23, 42, 0.08);
  color: #0f172a;
}

.project-card__status--in-progress {
  background: rgba(37, 99, 235, 0.14);
  color: #1d4ed8;
}

.project-card__status--review {
  background: rgba(14, 116, 144, 0.16);
  color: #0f766e;
}

.project-card__status--planned {
  background: rgba(180, 83, 9, 0.18);
  color: #b45309;
}

.project-card h3 {
  margin: 0;
  font-size: 1.15rem;
  font-weight: 700;
}

.project-card p {
  margin: 0;
  font-size: 0.95rem;
  color: #475569;
  line-height: 1.5;
}

.project-card__footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.project-card__owner {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-weight: 600;
  color: #0f172a;
}

.owner-avatar {
  width: 36px;
  height: 36px;
  border-radius: 12px;
  background: rgba(15, 23, 42, 0.1);
  display: grid;
  place-items: center;
  font-weight: 700;
}

.project-card__progress {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 6px;
  min-width: 90px;
}

.project-card__progress span {
  font-size: 0.85rem;
  font-weight: 600;
  color: #1d4ed8;
}

.project-card__progress-bar {
  width: 100%;
  height: 6px;
  background: rgba(37, 99, 235, 0.12);
  border-radius: 999px;
  overflow: hidden;
}

.project-card__progress-bar span {
  display: block;
  height: 100%;
  background: #1d4ed8;
  border-radius: inherit;
  transition: width 0.3s ease;
}
</style>
