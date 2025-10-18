// ClientApp/views/shared/navigationConfig.js

export const applicationMenu = [
  { id: 'projects', label: 'Projects', route: '/projectsMenu' },
  { id: 'issues', label: 'Issue Tracker', route: '/issuesMenu' },
  { id: 'resources', label: 'Resources', route: '/resources' },
  { id: 'teams', label: 'Teams', route: '/teams' }
]

export const dashboardSidebarSections = [
  {
    id: 'navigation',
    label: 'Navigasi Utama',
    items: [
      {
        id: 'overview',
        label: 'Overview',
        icon: '⌂',
        caption: 'Statistik terkini',
        route: '/dashboard',
        placeholder: 'Lihat metrik harian, ringkasan sprint, dan aktivitas terbaru tim.'
      },
      {
        id: 'task-board',
        label: 'Task Board',
        icon: '☰',
        caption: 'Monitor tugas aktif',
        badge: 12,
        route: '/dashboard/taskboard',
        placeholder: 'Papan tugas menampilkan status setiap pekerjaan lintas fungsi.'
      },
      {
        id: 'sprint',
        label: 'Sprint Plan',
        icon: '⚑',
        caption: 'Rencana mingguan',
        route: '/dashboard/sprintplan',
        placeholder: 'Rencanakan sprint, tetapkan kapasitas, dan kunci ruang lingkup.'
      },
      {
        id: 'backlog',
        label: 'Backlog',
        icon: '▤',
        caption: 'Daftar pekerjaan',
        route: '/dashboard/backlog',
        placeholder: 'Susun prioritas backlog dan pantau item yang menunggu estimasi.'
      }
    ]
  },
  {
    id: 'reports',
    label: 'Laporan',
    items: [
      {
        id: 'burn-down',
        label: 'Burn Down',
        icon: '⌛',
        caption: 'Progress sprint',
        route: '/dashboard/burndown',
        placeholder: 'Grafik burn down membantu mengevaluasi progress sprint secara real-time.'
      },
      {
        id: 'team-capacity',
        label: 'Team Capacity',
        icon: '👥',
        caption: 'Analisa kapasitas',
        route: '/dashboard/teamcapacity',
        placeholder: 'Lihat kemampuan tim dan identifikasi potensi blocking lebih awal.'
      },
      {
        id: 'release',
        label: 'Release',
        icon: '⬆',
        caption: 'Status rilis',
        badge: 2,
        route: '/dashboard/release',
        placeholder: 'Kelola jadwal rilis, checklist kualitas, dan komunikasi lintas tim.'
      }
    ]
  }
]
