<template>
  <ApplicationPageTemplate
    page-key="teams"
    hero-title="Teams Directory"
    hero-description="Lihat kapasitas, peran, dan struktur tim untuk mempercepat kolaborasi lintas fungsi."
  >
    <section class="teams-grid">
      <article
        v-for="team in teams"
        :key="team.id"
        class="team-card"
      >
        <header class="team-card__header">
          <div class="team-card__avatar">{{ team.initials }}</div>
          <div>
            <h3>{{ team.name }}</h3>
            <p>{{ team.mission }}</p>
          </div>
        </header>
        <ul class="team-card__members">
          <li
            v-for="member in team.members"
            :key="member.name"
            class="team-member"
          >
            <span class="team-member__name">{{ member.name }}</span>
            <span class="team-member__role">{{ member.role }}</span>
          </li>
        </ul>
        <footer class="team-card__footer">
          <span class="team-card__capacity">Capacity: {{ team.capacity }}%</span>
          <button type="button" class="team-card__cta">Lihat Detail</button>
        </footer>
      </article>
    </section>
  </ApplicationPageTemplate>
</template>

<script setup>
import ApplicationPageTemplate from '@/views/shared/ApplicationPageTemplate.vue'

const teams = [
  {
    id: 'product',
    name: 'Product Crew',
    initials: 'PC',
    mission: 'Drive roadmap dan validasi solusi dengan customer.',
    capacity: 76,
    members: [
      { name: 'Dewi Lestari', role: 'Product Manager' },
      { name: 'Yoga Wicaksana', role: 'Product Designer' },
      { name: 'Zaki Fadhilah', role: 'UX Researcher' }
    ]
  },
  {
    id: 'engineering',
    name: 'Core Engineering',
    initials: 'CE',
    mission: 'Menjaga reliability platform dan pengembangan fitur inti.',
    capacity: 82,
    members: [
      { name: 'Akbar Ramadhan', role: 'Tech Lead' },
      { name: 'Meta Prameswari', role: 'Backend Engineer' },
      { name: 'Joshua Halim', role: 'Frontend Engineer' }
    ]
  },
  {
    id: 'success',
    name: 'Customer Success',
    initials: 'CS',
    mission: 'Mendampingi implementasi pelanggan dan memastikan adoption.',
    capacity: 64,
    members: [
      { name: 'Elisa Wardhani', role: 'CS Manager' },
      { name: 'Fadil Prakoso', role: 'Onboarding Specialist' },
      { name: 'Nia Septiani', role: 'Support Engineer' }
    ]
  }
]
</script>

<style scoped>
.teams-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 20px;
}

.team-card {
  background: #f8fafc;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 22px 42px -34px rgba(15, 23, 42, 0.45);
}

.team-card__header {
  display: flex;
  gap: 12px;
}

.team-card__avatar {
  width: 44px;
  height: 44px;
  border-radius: 14px;
  background: linear-gradient(135deg, #1d4ed8 0%, #7c3aed 100%);
  color: #fff;
  display: grid;
  place-items: center;
  font-weight: 700;
  font-size: 1rem;
}

.team-card__header h3 {
  margin: 0;
  font-size: 1.1rem;
  font-weight: 700;
}

.team-card__header p {
  margin: 4px 0 0;
  font-size: 0.9rem;
  color: #475569;
}

.team-card__members {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.team-member {
  display: flex;
  justify-content: space-between;
  font-size: 0.9rem;
  color: #334155;
}

.team-member__role {
  color: #64748b;
}

.team-card__footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.85rem;
  color: #475569;
}

.team-card__cta {
  padding: 8px 16px;
  border-radius: 12px;
  border: none;
  background: #0f766e;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
}
</style>
