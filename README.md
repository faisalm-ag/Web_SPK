# 🎌 Web SPK: Sistem Pendukung Keputusan Magang Jepang

[![.NET](https://img.shields.io/badge/.NET-8.0-512bd4.svg)](https://dotnet.microsoft.com/download)
[![Architecture](https://img.shields.io/badge/Architecture-Clean--Architecture-blue)](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

Sistem Pendukung Keputusan (SPK) berbasis Web yang dirancang untuk membantu calon peserta magang (Ginaser) dalam menentukan lokasi prefektur terbaik di Jepang menggunakan integrasi **Big Data e-Stat Jepang** dan algoritma **Simple Additive Weighting (SAW)**.

---

## 🚀 Justifikasi & Arsitektur Sistem

Proyek ini dibangun menggunakan **Clean Architecture** untuk memastikan kode mudah dikelola, diuji, dan dikembangkan di masa depan.

### 1. Struktur Proyek (Layering)
Sistem dibagi menjadi 4 layer utama:
* **SPKDomain**: Inti dari bisnis logika. Berisi entitas (Gaji, CPI, Populasi), Interface, dan Value Objects. Tidak memiliki ketergantungan pada library luar.
* **SPKCore (Application)**: Berisi logika aplikasi seperti `AnalysisService` dan `PredictionService`. Di sinilah proses perhitungan SAW dan analisis tren dilakukan.
* **SPKInfrastructure**: Menangani akses data. Dalam proyek ini, infrastruktur bertugas membaca dan memproses file CSV dari dataset e-Stat.
* **WebAppSPK (Presentation)**: Interface pengguna berbasis ASP.NET Core MVC yang responsif dan modern.

### 2. Algoritma & Fitur Unggulan
* **Simple Additive Weighting (SAW)**: Melakukan normalisasi data dari berbagai kriteria (Benefit & Cost) untuk menghasilkan ranking prefektur yang objektif.
* **Big Data Integration**: Mengolah ribuan baris data ekonomi Jepang (Gaji rata-rata per industri, Cost of Living, Kepadatan Perusahaan, dan Rasio Penduduk).
* **Personalized Analysis**: Menggunakan kuesioner psikologis untuk menentukan bobot preferensi setiap pengguna secara unik.
* **SWOT Insight**: Memberikan analisis otomatis mengenai Kekuatan, Kelemahan, Peluang, dan Ancaman di lokasi pilihan pengguna.



---

## 🛠 Panduan Instalasi & Menjalankan Project

Ikuti langkah-langkah di bawah ini untuk menjalankan proyek di perangkat lokal Anda.

### 1. Prasyarat (Instalasi Diperlukan)
Pastikan Anda sudah menginstal:
* **[.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)** (Versi terbaru).
* **Git** (Untuk clone repository).
* **Visual Studio Code** atau **Visual Studio 2022**.

### 2. Persiapan Project (Terminal/CMD)
Buka terminal/CMD, masuk ke folder tempat Anda ingin menyimpan project, lalu jalankan:

```bash
# 1. Clone repository
git clone [https://github.com/faisalm-ag/Web_SPK.git](https://github.com/faisalm-ag/Web_SPK.git)

# 2. Masuk ke folder utama project
cd Web_SPK

```

### 3. Menghubungkan & Restore Dependencies

Karena proyek ini terdiri dari beberapa sub-proyek (Class Library), kita perlu memastikan semuanya terhubung melalui file Solution (.sln):

```bash
# Restore semua library dan dependencies yang diperlukan
dotnet restore

```

### 4. Menjalankan Aplikasi

Anda harus menjalankan aplikasi dari folder **WebAppSPK** karena itu adalah *entry point* (titik masuk) webnya:

```bash
# Pindah ke folder WebApp
cd WebAppSPK

# Jalankan aplikasi
dotnet run

```

### 5. Akses Aplikasi

Setelah muncul tulisan `Now listening on: http://localhost:5000` (atau port lain), buka browser Anda dan akses:
**`http://localhost:5000`** atau sesuai port yang muncul di terminal Anda.

---

## 📊 Dataset Reference

Data yang digunakan dalam sistem ini bersumber dari:

* *Portal Site of Official Statistics of Japan (e-Stat)*: [https://www.e-stat.go.jp/](https://www.e-stat.go.jp/)
* Data mencakup statistik Gaji (2023-2024), Indeks Harga Konsumen (CPI), dan Kepadatan Industri per wilayah.

---

**Developed by [Faisal]** - Memberikan solusi cerdas untuk karir masa depan di Jepang.