# Fase 1 – Konsolidasi Domain

## Tujuan
- Mendata model domain backend Plane (Django) beserta aturan bisnis utamanya.
- Menginventarisasi entitas dan value object yang sudah tersedia di `SFCoreProTM.Domain`.
- Menyusun matriks pemetaan untuk menilai cakupan saat ini dan menandai gap yang perlu ditangani pada fase implementasi selanjutnya.

## Inventaris Plane Backend (Django)
- Workspaces & Membership: `workspace.py`, `favorite.py`, `sticky.py`, `description.py`, `deploy_board.py`, `exporter.py`, `label.py`.
- Projects & Planning: `project.py`, `module.py`, `cycle.py`, `intake.py`, `estimate.py`, `state.py`, `deploy_board.py` (cross-cut), `favorite.py`.
- Issues & Collaboration: `issue.py`, `issue_type.py`, `label.py`, `description.py`, `notification.py`, `view.py`, `draft.py`.
- Users & Access: `user.py`, `device.py`, `session.py`, `social_connection.py`, `api.py`.
- Integrations & Automation: `integration/base.py`, `integration/slack.py`, `integration/github.py`, `webhook.py`, `importer.py`, `exporter.py`.
- Analytics & Reporting: `analytic.py`, `recent_visit.py`.
- Assets & Attachments: `asset.py`.
- Misc Support: `page.py`, `notification.py`, `intake.py`, `deploy_board.py`, `favorite.py`.
- Base abstractions: `base.py`, mixin soft delete (`SoftDeletionManager`), audit fields, slug helpers.

> Catatan: file `issue.py` mengandung logika kompleks seperti `IssueManager` (filter siap pakai), sequencing via `pg_advisory_lock`, strip HTML, sinkron relasi Many-To-Many dan versioning; `workspace.py` melakukan soft delete slug rewrite; `module.py` mengelola `sort_order` secara otomatis; beberapa model mengisi properti JSON default untuk preferensi UI.

## Inventaris Domain SFCoreProTM (C#)
- Workspaces: `WorkspaceAggregate`, `LabelEntities`, `FavoriteEntities`, `StickyEntities`, `DescriptionEntities`, `DeployBoard`, `ExporterHistory`.
- Projects: `ProjectAggregate`, `StateEntities`, `EstimateEntities`, `ModuleEntities`, `CycleEntities`, `IntakeEntities`, `Importer`.
- Issues: `IssueAggregate` (termasuk Assignee, Label, Blocker, Relation, Mention, Link, Attachment, Activity, Version, DescriptionVersion, Comment, Subscriber, Reaction, Vote, Sequence), `IssueTypeEntities`.
- Views & Preferences: `IssueViewEntities`, `ViewPreferences`, `WorkspaceMemberPreferences`, `ProjectMemberPreferences`.
- Users & Accounts: `UserAggregate`, `DeviceEntities`, `SessionEntities`, `SocialConnectionEntities`.
- Notifications & Automation: `NotificationEntities`, `WebhookEntities`, `Integrations` (Slack/GitHub/workspace integration), `ApiEntities`.
- Content & Collaboration: `PageEntities`, `DraftEntities`, `Assets/FileAsset`, `Analytics/AnalyticView`, `UserActivities/UserRecentVisit`.
- Value Objects: `RichTextContent`, `StructuredData`, `DateRange`, `Url`, `EmailAddress`, `AuditTrail`, `ExternalReference`, `ColorCode`, `Slug`, `ViewPreferences`, `WorkspaceMemberPreferences`.

## Matriks Pemetaan Plane → SFCoreProTM

| Bounded Context | Plane Models & Logic | SFCoreProTM Entities/VO | Cakupan Saat Ini | Gap & Aksi Lanjutan |
| --- | --- | --- | --- | --- |
| Workspace Core | `workspace.py` (slug validation, soft delete rename), `workspace_member`, default props JSON | `Workspaces/WorkspaceAggregate`, `WorkspaceMemberPreferences`, `WorkspaceMember` | Struktur domain tersedia | Replikasi slug rewrite & soft delete perlu diimplementasi pada domain service/repository; inisialisasi default props perlu dibuat sebagai factory/service |
| Workspace Utilities | `favorite.py`, `sticky.py`, `description.py`, `deploy_board.py`, `exporter.py` | `FavoriteEntities`, `StickyEntities`, `DescriptionEntities`, `DeployBoard`, `ExporterHistory` | Entitas sudah ada | Porting behaviour (auto-color, ordering, history logging) ke perintah/domain event |
| Label Management | `label.py` (warna random, unique constraints) | `Workspaces/LabelEntities` | Ada | Tambahkan penetapan warna default & unique scope seperti constraint Django |
| Project Aggregate | `project.py` (identifier uppercase, network visibility, timezone) | `Projects/ProjectAggregate` | Field & metode setara | Tambah logika trimming identifier + default state fallback saat create/update; validasi timezone via service |
| Project Membership | `project_member`, `project_member_invite` | `ProjectMember`, `ProjectMemberInvite` | Ada | Replikasi auto sort order + token generator |
| Modules | `module.py` (auto sort order, status choices, M2M members) | `Projects/ModuleEntities` | Status enum & member mgmt tersedia | Implementasi auto sort order (min-10000) & default view properties + archived flag |
| Cycles | `cycle.py` (cadence windows, triage linkage) | `Projects/CycleEntities` | Ada | Tambahkan validasi tanggal, stage gating & auto-link issue snapshots |
| Intake | `intake.py` (forms, snapshot config) | `Projects/IntakeEntities` | Ada | Map JSON defaults, status transitions, embed validation rules |
| Estimates & States | `estimate.py`, `state.py` (triage toggle, default) | `EstimateEntities`, `StateEntities` | Ada | Rekam constraint default state (non-triage) & poin estimasi maksimum |
| Issues Core | `issue.py` (IssueManager filter, `save` sequencing + `pg_advisory_lock`, sort order, default state) | `Issues/IssueAggregate`, `IssueSequence` | Struktur lengkap | Implement service untuk default state lookup, concurrency lock (advisory) & sort-order increment via repository/UoW |
| Issue Collaboration | `issue.py` (Assignee, Label, Blocker, Relation, Mention, Link, Attachment, Activity, Version, DescriptionVersion, Vote), `issue_comment.py` | `IssueAssignee`, `IssueLabel`, `IssueBlocker`, `IssueRelation`, `IssueMention`, `IssueLink`, `IssueAttachment`, `IssueActivity`, `IssueVersion`, `IssueDescriptionVersion`, `IssueVote`, `IssueComment` | Semua entitas tersedia | Translasikan behaviour (strip HTML, attachment limit, version snapshots, comment access) ke domain services |
| Issue Preferences & Views | `issue_user_property`, `view.py` | `IssueUserProperty`, `Views/IssueViewEntities`, `ViewPreferences` | Ada | Pastikan default filter/property JSON diinisialisasi ketika entitas dibuat |
| Drafts | `draft.py` (autosave, binary/html sync) | `DraftEntities` | Ada | Implement TTL & `last_saved_at` update + file storage bridging |
| Pages | `page.py` (cover image, publish workflow) | `Pages/PageEntities` | Ada | Replikasi slug uniqueness, publish/unpublish toggles |
| Analytics | `analytic.py` | `Analytics/AnalyticView` | Ada | Pusk validasi snapshot & aggregator mapping |
| Notifications | `notification.py` (snooze, read, batch send) | `NotificationEntities` | Ada | Tulis domain service untuk scheduling (snooze/archive) dan email log | 
| Webhooks & Integrations | `webhook.py`, `integration/*.py`, `importer.py`, `exporter.py` | `Webhooks`, `Integrations`, `Importer`, `ExporterHistory` | Entitas tersedia | Detail provider configuration (Slack scopes, GitHub repo sync) perlu service dan DTO mapping |
| Assets | `asset.py` (upload path, size validation) | `Assets/FileAsset` | Ada | Tambah validasi ukuran & generator path seperti Django `get_upload_path` |
| Users & Sessions | `user.py`, `device.py`, `session.py`, `social_connection.py` | `Users/UserAggregate`, `DeviceEntities`, `SessionEntities`, `SocialConnectionEntities` | Ada | Implement activation/invite flows, session expiry kebijakan |
| API Tokens | `api.py` (token hashing, scope flags, activity log) | `Api/ApiEntities` | Ada | Terapkan hashing, scope management, audit log persisten |
| User Activity | `recent_visit.py` | `UserActivities/UserRecentVisit` | Ada | Pastikan limit & ordering sesuai (mis. last N visited) |

## Gap Utama yang Perlu Diakselerasi
- **Concurrency & Sequencing**: butuh implementasi mekanisme locking (advisory atau alternatif) dan generator sequence seperti `IssueManager.save` serta `Module.save`.
- **Default State & Preference Bootstrapping**: factory/domain service untuk mengisi default JSON (filters, display, preferences) agar porting UI props konsisten.
- **Soft Delete Semantik**: workspace slug rewrite dan filter `deleted_at` (IssueManager) perlu diterapkan via interceptors/global query filter di EF Core.
- **Validation & Constraints**: replikasi constraint unik bersyarat (Django `UniqueConstraint(condition=...)`) menggunakan konfigurasi EF Core (`HasFilter`) dan validator domain.
- **Mapping DTO ↔ Domain**: siapkan profil AutoMapper per bounded context agar `StructuredData`/`RichTextContent` tetap ter-hidrat.
- **Domain Services Pendukung**: HTML stripping, batas ukuran file, random color, sort order management perlu disediakan sebagai domain service atau helper infrastruktur.

## Rekomendasi Tahap Berikutnya
1. Susun backlog prioritas per bounded context berdasarkan gap di atas (Fase 2 – Penajaman Aggregate).
2. Fokus pada Issues & Projects terlebih dahulu untuk menjamin CRUD utama sesuai logika Plane sebelum modul minor.
3. Siapkan kerangka unit/integration test untuk mengecek sequence, soft delete, dan default preference saat implementasi C# dimulai.
