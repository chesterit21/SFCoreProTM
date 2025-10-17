# STRUKTUR PROJECT APLIKASI ASP.NETOCRE MVC 
## PATTERN CLEAN ARCHITECTURE + MEDIATR + DDD + UOW + REPOSITORY

---


*best practices* dan struktur yang lengkap untuk *global helpers* adalah langkah yang tepat. Dalam **Clean Architecture**, penempatan *helper* sangat krusial untuk mencegah **ketergantungan yang tidak terkelola** dan menjaga inti bisnis tetap bersih.

Prinsip utamanya adalah: **Fungsi yang sangat umum dan tidak bergantung pada *Domain* atau *Framework* apa pun harus berada di *Shared Kernel* atau *Core Utilities* terpisah.**

Saya akan memberikan rekomendasi struktur **terbaik dan terlengkap** dengan menambahkan proyek khusus untuk *Global Helpers* (Utilities) dan *Cross-Cutting Concerns*.

-----

## 5\. `{AppName}.Shared` (Global Helpers/Utilities) 🌟

Anda benar, *Helpers* yang bersifat universal (tidak spesifik untuk *Domain* maupun *Persistence*) harus diisolasi dalam proyek mereka sendiri, umumnya dinamakan **Shared** atau **Utilities**.

Proyek ini **tidak bergantung pada proyek lain mana pun** dan dapat digunakan oleh *Domain*, *Application*, *Persistence*, dan *Api*.

```
├── {AppName}.Shared
│   ├── Extensions
│   │   ├── StringExtensions.cs (Ekstensi untuk manipulasi string, misal: IsValidEmail(), ToBase64())
│   │   └── DateTimeExtensions.cs (Ekstensi untuk validasi DateTime, misal: IsWeekend())
│   ├── Helpers
│   │   ├── GuidHelper.cs (Misal: untuk menghasilkan GUID yang berurutan)
│   │   └── ValidationHelper.cs (Fungsi validasi format data umum)
│   ├── Security (Cross-Cutting Concern: Keamanan)
│   │   ├── IEncryptionService.cs (Interface/Kontrak Enkripsi)
│   │   ├── AesEncryptionService.cs (Implementasi Enkripsi AES)
│   │   └── PasswordHasher.cs (Hash password)
│   └── Caching (Cross-Cutting Concern: Caching)
│       └── ICacheService.cs (Interface untuk MemoryCache/Redis)
```

**Penting:** Karena ini hanya berisi *interface* dan *extension methods* yang murni C\#, proyek ini dapat digunakan di mana saja tanpa menimbulkan *circular dependency*.

-----

## Update Struktur Final (Full Version Terbaik)

Berikut adalah ringkasan lengkap 5 proyek dengan penempatan *class* yang mencerminkan praktik terbaik Clean Architecture + DDD + UoW + MediatR:

### 1\. `{AppName}.Domain`

*Core Business Logic & Contracts. **Bergantung pada: Shared***

```
├── {AppName}.Domain -> {AppName}.Shared 
│   ├── Entities (Domain Models)
│   │   └── Issue.cs
│   ├── Exceptions (Business Exceptions)
│   │   ├── DomainException.cs 
│   │   └── IssueNotFoundException.cs 
│   ├── Services (Domain Services)
│   │   └── IssuePrioritizationService.cs
│   └── Interfaces (Repository & Domain Contracts)
│       └── IIssueRepository.cs
```

-----

### 2\. `{AppName}.Application`

*Business Workflow & Orchestration (MediatR). **Bergantung pada: Domain, Shared***

```
├── {AppName}.Application -> {AppName}.Domain
│                         -> {AppName}.Shared
│   ├── Features (Use Cases)
│   │   ├── Issues
│   │   │   ├── Commands (MediatR Input)
│   │   │   └── Handlers (MediatR Logic)
│   │   └── ...
│   ├── Interfaces (Application-specific Contracts)
│   │   ├── IUnitOfWork.cs
│   │   └── IDateTimeProvider.cs (Untuk mendapatkan waktu yang dapat diuji)
│   └── Behaviors (MediatR Pipeline)
│       └── ValidationBehavior.cs
```

-----

### 3\. `{AppName}.Persistence`

*Database Implementation (EF Core). **Bergantung pada: Domain, Application, Shared***

```
├── {AppName}.Persistence -> {AppName}.Domain
│                         -> {AppName}.Application
│                         -> {AppName}.Shared
│   ├── Data
│   │   └── ApplicationDbContext.cs
│   ├── Repositories (Implementasi Kontrak Domain)
│   │   └── IssueRepository.cs
│   └── UnitOfWork (Implementasi Kontrak Application)
│       └── UnitOfWork.cs
```

-----

### 4\. **`{AppName}.Api` (Presentation Layer)**

*Entry Point & Framework-specific Logic. **Bergantung pada: Application, Persistence, Shared***

```
├── {AppName}.Api -> {AppName}.Application
│                 -> {AppName}.Persistence
│                 -> {AppName}.Shared
│   ├── Controllers (Hanya Menerima & Mengirim MediatR)
│   │   └── IssuesController.cs
│   ├── Middlewares (Global Exception Handling)
│   │   └── GlobalExceptionHandlerMiddleware.cs (Mengubah DomainException menjadi HTTP Status Code)
│   ├── Helpers (API-specific Helpers)
│   │   └── ApiResponseHelper.cs (Memformat Respons HTTP)
│   ├── Extensions (DI Setup)
│   │   └── ServiceCollectionExtensions.cs (Tempat mendaftarkan semua service/repository/middleware)
│   └── Program.cs
```

-----

### 5\. **`{AppName}.Shared` (Global Helpers) - DIKUNCI**

*Utilities Murni. **Tidak bergantung pada proyek lain.***

**Penempatan Implementasi Caching & Security:**

Dalam struktur ini, **`IEncryptionService`** dan **`ICacheService`** berada di proyek **`{AppName}.Shared`** sebagai *interface* umum. Namun, implementasi konkretnya (`AesEncryptionService` atau implementasi Redis Cache) **harus didaftarkan di `Program.cs` (Api Layer)**.

Ini adalah praktik terbaik:

1.  *Application Layer* hanya tahu tentang **`ICacheService`** (kontrak).
2.  *Api Layer* yang bertanggung jawab untuk memilih dan mendaftarkan implementasi konkret (misalnya, `services.AddScoped<ICacheService, RedisCacheService>()`).

Struktur 5-proyek ini adalah kerangka kerja yang sangat kuat dan sangat disarankan untuk proyek Anda.