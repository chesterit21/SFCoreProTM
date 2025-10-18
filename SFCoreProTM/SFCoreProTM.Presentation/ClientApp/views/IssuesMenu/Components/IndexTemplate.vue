<template>
  <ApplicationPageTemplate
    page-key="issues"
    hero-title="Issue Tracker"
    hero-description="Monitor bug, incident, dan permintaan fitur dengan alur status yang terstandarisasi."
  >
    <section class="issues-board">
      <div
        v-for="column in issueColumns"
        :key="column.id"
        class="issues-column"
      >
        <header class="issues-column__header">
          <h3>{{ column.title }}</h3>
          <span class="issues-column__count">{{ column.issues.length }}</span>
        </header>
        <ul class="issues-list">
          <li
            v-for="issue in column.issues"
            :key="issue.id"
            class="issue-card"
          >
            <div class="issue-card__meta">
              <span class="issue-card__id">{{ issue.id }}</span>
              <span class="issue-card__priority" :class="`issue-card__priority--${issue.priority}`">{{ issue.priorityLabel }}</span>
            </div>
            <p class="issue-card__title">{{ issue.title }}</p>
            <div class="issue-card__footer">
              <span class="issue-card__assignee">{{ issue.assignee }}</span>
              <span class="issue-card__updated">{{ issue.updated }}</span>
            </div>
          </li>
        </ul>
      </div>
    </section>
  </ApplicationPageTemplate>
</template>

<script setup>
import ApplicationPageTemplate from '@/views/shared/ApplicationPageTemplate.vue'

const issueColumns = [
  {
    id: 'todo',
    title: 'To Do',
    issues: [
      {
        id: 'PLAN-132',
        title: 'Implementasi audit log pada modul tugas',
        assignee: 'Rere Oktaviani',
        priority: 'high',
        priorityLabel: 'High',
        updated: '2 jam lalu'
      },
      {
        id: 'PLAN-129',
        title: 'Tambahkan shortcut keyboard pada kanban board',
        assignee: 'Bagus Wicaksono',
        priority: 'medium',
        priorityLabel: 'Medium',
        updated: 'Kemarin'
      }
    ]
  },
  {
    id: 'in-progress',
    title: 'In Progress',
    issues: [
      {
        id: 'PLAN-118',
        title: 'Refactor modul notifikasi realtime',
        assignee: 'Nadia Prameswari',
        priority: 'high',
        priorityLabel: 'High',
        updated: '45 menit lalu'
      },
      {
        id: 'PLAN-106',
        title: 'Optimasi query backlog report',
        assignee: 'Andra Setiawan',
        priority: 'low',
        priorityLabel: 'Low',
        updated: '3 jam lalu'
      }
    ]
  },
  {
    id: 'review',
    title: 'Review',
    issues: [
      {
        id: 'PLAN-102',
        title: 'Perbarui komponen filter global',
        assignee: 'Eka Wijaya',
        priority: 'medium',
        priorityLabel: 'Medium',
        updated: 'Hari ini'
      },
      {
        id: 'PLAN-94',
        title: 'Integrasi SSO internal untuk tim support',
        assignee: 'Lala Pratiwi',
        priority: 'high',
        priorityLabel: 'High',
        updated: 'Kemarin'
      }
    ]
  }
]
</script>

<style scoped>
.issues-board {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
}

.issues-column {
  background: #f8fafc;
  border-radius: 20px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  box-shadow: 0 20px 42px -34px rgba(15, 23, 42, 0.45);
}

.issues-column__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.issues-column__header h3 {
  margin: 0;
  font-size: 1rem;
  font-weight: 700;
}

.issues-column__count {
  padding: 4px 10px;
  border-radius: 999px;
  background: rgba(37, 99, 235, 0.1);
  color: #1d4ed8;
  font-weight: 600;
  font-size: 0.8rem;
}

.issues-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.issue-card {
  background: #fff;
  border-radius: 16px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  box-shadow: inset 0 0 0 1px rgba(148, 163, 184, 0.1);
}

.issue-card__meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.8rem;
  color: #64748b;
  font-weight: 600;
}

.issue-card__title {
  margin: 0;
  font-size: 0.95rem;
  font-weight: 600;
  color: #0f172a;
}

.issue-card__footer {
  display: flex;
  justify-content: space-between;
  font-size: 0.8rem;
  color: #64748b;
}

.issue-card__priority {
  padding: 2px 8px;
  border-radius: 999px;
  font-size: 0.7rem;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.issue-card__priority--high {
  background: rgba(220, 38, 38, 0.15);
  color: #b91c1c;
}

.issue-card__priority--medium {
  background: rgba(217, 119, 6, 0.14);
  color: #b45309;
}

.issue-card__priority--low {
  background: rgba(22, 163, 74, 0.14);
  color: #15803d;
}
</style>
