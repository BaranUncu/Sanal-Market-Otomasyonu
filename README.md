# 🛒 Sanal Market Otomasyonu (Retail Management System)

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

Bu proje, temel CRUD işlemlerinin ötesine geçerek güncel kullanıcı deneyimi (UX) standartlarını ve veri güvenliği prensiplerini uygulayan, perakende sektörüne yönelik kurumsal bir masaüstü (Windows Forms) e-ticaret otomasyonudur.

---

## 🔐 Yönetici (Admin) Girişi ve Test Hesapları

Sistemin Yönetici Kontrol Merkezi'ni ve kilitli modüllerini test edebilmeniz için veritabanında varsayılan bir admin hesabı tanımlanmıştır:

- **E-Posta:** `admin@admin.com`
- **Şifre:** `666666`

*(Alternatif: Sisteme yeni bir kullanıcı olarak kayıt olup, SQL Server üzerinden `Kullanicilar` tablosundaki `Yetki` değerini `1` (True) yaparak o hesaba anında admin yetkisi tanımlayabilirsiniz.)*

---

## 🏗️ Proje Mimarisi ve Öne Çıkan Özellikler

- **Kriptografik Güvenlik (SHA-256):** Veri güvenliği amacıyla kayıt ve giriş aşamalarında kullanıcı şifreleri düz metin (plain-text) yerine, uçtan uca SHA-256 algoritması ile hash'lenerek veritabanında saklanmaktadır.
- **Merkezi Veritabanı Yönetimi:** Tüm SQL işlemleri ADO.NET mimarisiyle `DatabaseHelper.cs` sınıfı üzerinden merkezi olarak yönetilerek kod tekrarı (DRY) engellenmiştir.
- **Dinamik Sepet Modülü:** Sepete eklenen ürünlerin buton yapısı anlık olarak `[-] [Adet] [+]` formatına dönüşür ve sepet tutarı dinamik olarak güncellenir.
- **İlişkisel Veri (Cascading):** Adres yönetiminde, İl seçimine bağlı olarak İlçeler veritabanından dinamik ve filtreli bir şekilde çekilir.
- **Arayüz Koruması:** Yönetici panelindeki `DataGridView` tabloları son kullanıcı müdahalesine karşı kilitlenerek tasarım bütünlüğü ve veri güvenliği sağlanmıştır.

---

## 🚀 Kurulum ve Çalıştırma

Sistemi kararlı bir şekilde kendi bilgisayarınızda ayağa kaldırmak için aşağıdaki adımları izlemeniz yeterlidir:

1. **Veritabanını Yükleyin:** Proje klasörünün içindeki `Database` klasöründe yer alan **`MarketDB_Kurulum.sql`** dosyasını SQL Server Management Studio (SSMS) üzerinden açın ve `Execute` (Çalıştır) butonuna basın. Bu işlem tüm tabloları ve varsayılan yönetici hesabını kuracaktır.
2. **Projeyi Açın:** Visual Studio üzerinden `.sln` (Solution) dosyasını açın.
3. **Derleyin:** Üst menüden **Start (Başlat)** butonuna basarak projeyi çalıştırın. `App.config` ayarı gerektirmez, evrensel bağlantı dizesi ile lokal sunucunuzu otomatik tanır.