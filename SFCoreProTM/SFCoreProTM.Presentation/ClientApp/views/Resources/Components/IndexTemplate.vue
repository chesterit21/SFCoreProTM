<template>
  <ApplicationPageTemplate
    page-key="resources"
    hero-title="Resource Library"
    hero-description="Pusat dokumentasi, pedoman, dan aset internal untuk mendukung kolaborasi tim."
  >
    <section class="resources-grid">
      <article
        v-for="resource in resources"
        :key="resource.id"
        class="resource-card"
      >
        <header class="resource-card__header">
          <div class="resource-card__icon">{{ resource.icon }}</div>
          <span class="resource-card__type">{{ resource.type }}</span>
        </header>
        <h3>{{ resource.title }}</h3>
        <p>{{ resource.description }}</p>
        <footer class="resource-card__footer">
          <span class="resource-card__owner">{{ resource.owner }}</span>
          <button type="button" class="resource-card__action">Buka</button>
        </footer>
      </article>
    </section>
  </ApplicationPageTemplate>
</template>

<script setup>
import ApplicationPageTemplate from '@/views/shared/ApplicationPageTemplate.vue'

const resources = [
  {
    id: 'handbook',
    title: 'Plane Product Handbook',
    description: 'Panduan menyeluruh tentang visi, strategi, dan ritme rilis produk Plane.',
    type: 'Dokumen',
    owner: 'Product Ops',
    icon: '📘'
  },
  {
    id: 'brand-kit',
    title: 'Brand & Asset Kit',
    description: 'Aset logo, panduan warna, dan komponen UI untuk konsistensi identitas.',
    type: 'Asset',
    owner: 'Marketing',
    icon: '🎨'
  },
  {
    id: 'api-guide',
    title: 'API Integration Guide',
    description: 'Tutorial integrasi webhook dan contoh penerapan API Plane untuk partner.',
    type: 'Guide',
    owner: 'Developer Relations',
    icon: '🔌'
  },
  {
    id: 'retro-template',
    title: 'Retrospective Template',
    description: 'Template miro dan catatan standar untuk sesi retrospective mingguan.',
    type: 'Template',
    owner: 'Agile Coach',
    icon: '📝'
  }
]
</script>

<style scoped>
.resources-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
}

.resource-card {
  background: #f8fafc;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 22px 40px -36px rgba(15, 23, 42, 0.45);
}

.resource-card__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.resource-card__icon {
  font-size: 1.6rem;
}

.resource-card__type {
  font-size: 0.75rem;
  padding: 4px 10px;
  border-radius: 999px;
  background: rgba(15, 23, 42, 0.08);
  font-weight: 600;
  color: #0f172a;
}

.resource-card h3 {
  margin: 0;
  font-size: 1.1rem;
  font-weight: 700;
}

.resource-card p {
  margin: 0;
  font-size: 0.95rem;
  color: #475569;
  line-height: 1.5;
}

.resource-card__footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.85rem;
  color: #475569;
}

.resource-card__action {
  padding: 8px 16px;
  border-radius: 12px;
  border: none;
  background: #2563eb;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
}
</style>
