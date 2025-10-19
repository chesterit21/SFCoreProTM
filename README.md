# SFCoreProTM - Panduan Pengembangan untuk Agent AI

Selamat datang di codebase SFCoreProTM! Dokumen ini berisi panduan dan aturan **WAJIB** yang harus diikuti oleh Agent AI saat melakukan pengembangan pada proyek ini.

## 🎯 Tujuan Proyek

SFCoreProTM adalah Sistem Manajemen Proyek yang dibangun menggunakan ASP.NET Core dan Vue 3, mengikuti prinsip Clean Architecture dan Domain-Driven Design (DDD) untuk memastikan kode yang *maintainable*, *scalable*, dan fokus pada *business logic*.

## 🏗️ Arsitektur Inti & Pola Desain

Pemahaman arsitektur ini krusial untuk menghasilkan kode yang konsisten:

1.  **Clean Architecture:** Pemisahan *concerns* yang ketat antar layer:
    * `Domain`: Inti bisnis (Entities, Value Objects, Interfaces).
    * `Application`: Alur kerja & *use cases* (CQRS via MediatR, DTOs, Validation).
    * `Persistence`: Implementasi akses data (EF Core, Repositories, UoW).
    * `Presentation`: Interaksi luar (API Controllers, MVC Views, Frontend SPA).
    * `Shared`: Kode utilitas generik.
    * **Aturan Dependensi:** Hanya boleh mengarah ke dalam (misal: Application boleh ke Domain, tapi Domain tidak boleh ke Application).
2.  **Domain-Driven Design (DDD):** Fokus pada pemodelan domain bisnis menggunakan *Entities*, *Value Objects*, dan *Aggregates*.
3.  **CQRS (Command Query Responsibility Segregation):** Menggunakan **MediatR** untuk memisahkan operasi *write* (Commands) dan *read* (Queries).
4.  **Repository & Unit of Work (UoW):** Abstraksi akses data dan manajemen transaksi database.

(Detail lebih lanjut: `docs/struktur-project.md`, `docs/phase1-domain-consolidation.md`)

## 🛠️ Teknologi Utama

* **Backend:** ASP.NET Core, EF Core, PostgreSQL, MediatR, FluentValidation, AutoMapper, JWT, BCrypt.Net-Next.
* **Frontend:** Vue 3 (Composition API), Vite, PrimeVue, PrimeFlex, Axios, Vanilla JS (integrasi).

## 🤖 Peran Agent AI vs. Manusia

* **Manusia:**
    * Setup awal proyek & infrastruktur.
    * Desain dan definisi Modul, Task, dan Entitas Bisnis (termasuk ERD Definition).
    * Definisi *workflow* / alur kerja Task (Flow Task Definition).
    * *Code review* hasil kerja AI.
    * Pengambilan keputusan arsitektural tingkat tinggi.
* **Agent AI:**
    * **Implementasi fitur** berdasarkan spesifikasi (Task, Flow Task, ERD) yang diberikan manusia.
    * **Menulis kode** C# (Backend) dan Vue/JS (Frontend) sesuai aturan di bawah.
    * **Membuat/Memperbarui** *Commands*, *Queries*, *Handlers*, DTOs, *Entities*, *Value Objects*, *Repositories*, *EF Core Configurations*, *API Controllers*, dan *Vue Components*.
    * **Menulis unit test** dasar (jika diminta).
    * **Refactoring** kode sesuai arahan atau *best practices*.

## 📜 Aturan Wajib untuk Agent AI

**Patuhi aturan ini secara KETAT saat menulis atau memodifikasi kode:**

1.  **Ikuti Clean Architecture:**
    * Tempatkan kode pada layer yang **sesuai**. Jangan melanggar aturan dependensi (misal: Jangan referensi `Persistence` dari `Domain`).
    * Gunakan *interfaces* untuk komunikasi antar layer (misal: `Application` menggunakan `IRepository` dari `Domain`).
2.  **Gunakan MediatR untuk Use Cases:**
    * Setiap *endpoint* API **HARUS** memanggil `_mediator.Send()` dengan *Command* atau *Query*.
    * Buat `IRequest` (Command/Query) dan `IRequestHandler` baru di `Application Layer` untuk setiap *use case* baru.
    * **DILARANG** menaruh *business logic* di *Controller*.
3.  **Akses Data via Repository & UoW:**
    * Interaksi database **HARUS** melalui *Repository Interface*.
    * Implementasikan *query* kompleks di *Repository* (di `Persistence Layer`), bukan di *Handler*.
    * Gunakan `IUnitOfWork` di *Command Handler* untuk operasi *write* yang butuh transaksi.
4.  **Terapkan DDD:**
    * Modifikasi *state* **HARUS** melalui metode di dalam *Aggregate Root*.
    * Gunakan *Value Objects* untuk konsep yang tidak punya identitas (misal: `EmailAddress`, `DateRange`). Pastikan *Value Objects* *immutable*.
    * Gunakan *Entities* untuk objek dengan identitas.
5.  **Validasi di Application Layer:**
    * Tambahkan/Update **FluentValidation** `AbstractValidator<TCommand>` untuk setiap *Command*.
    * Validasi **HARUS** fokus pada *input request*, bukan *business rules* kompleks (itu di Domain).
6.  **Konsistensi Frontend (Vue/JS):**
    * Gunakan **Composition API** (`<script setup>`) untuk komponen Vue baru.
    * Manfaatkan **Base Components** PrimeVue (`BaseButton`, `BaseInput`, dll.).
    * Akses API backend **HARUS** melalui *services* (`authService.js`, dll.) & `axios` (`httpClient.js`).
    * Ikuti struktur folder `views/...` yang ada.
7.  **Coding Standards & Conventions:**
    * Ikuti **konvensi C#** standar Microsoft dan *style* yang sudah ada di codebase.
    * Gunakan `async`/`await` untuk I/O.
    * Gunakan **Records** untuk DTOs/VOs jika memungkinkan.
    * Ikuti *style* frontend yang ada (PrimeFlex, dll.).
8.  **Error Handling:**
    * Tangani *exceptions* yang **diharapkan** di `Application Layer` dan lempar *custom exceptions* (`NotFoundException`, `ConflictException`, `ValidationException`).
    * Jangan menangkap *exception* umum (`catch (Exception e)`) kecuali di *middleware* terluar.
9.  **Logging:**
    * Tambahkan *logging* (`ILogger`) yang relevan di *Handlers* dan *Services* untuk *tracing* dan *debugging*.

**Tujuan Akhir:** Menghasilkan kode yang bersih, konsisten, teruji (jika diminta), dan sesuai dengan arsitektur yang sudah ditetapkan oleh tim manusia.