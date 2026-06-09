# 🛒 Retail Management System (Sanal Market)

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

This project is a corporate desktop (Windows Forms) e-commerce automation tailored for the retail sector. It goes beyond basic CRUD operations to implement modern user experience (UX) standards and data security principles.

---

## 🔐 Administrator (Admin) Login and Test Accounts

A default admin account is configured in the database so you can easily test the Administrator Control Center and restricted modules:

- **Email:** `admin@admin.com`
- **Password:** `666666`

*(Alternative: You can register as a new user from the login screen and instantly grant admin privileges by setting the `Yetki` (Authority) value to `1` (True) in the `Kullanicilar` table via SQL Server.)*

---

## 🏗️ Project Architecture and Key Features

- **Cryptographic Security (SHA-256):** To ensure data security, user passwords are not stored as plain text. Instead, they are hashed end-to-end using the SHA-256 algorithm during the registration and login phases.
- **Centralized Database Management:** All SQL operations are centrally managed through the `DatabaseHelper.cs` class using the ADO.NET architecture, strictly adhering to the DRY (Don't Repeat Yourself) principle.
- **Dynamic Shopping Cart Module:** The button structure of items added to the cart instantly transforms into a `[-] [Quantity] [+]` format, and the total cart amount is updated dynamically.
- **Relational Data (Cascading):** In address management, districts are fetched dynamically and filtered from the database based on the selected city.
- **UI Protection:** `DataGridView` tables in the admin panel are locked against end-user manipulation to ensure design integrity and data security.

---

## 🚀 Installation and Execution

To run the system stably on your local machine, simply follow these steps:

1. **Install the Database:** Open the **`MarketDB_Kurulum.sql`** file located in the `Database` folder via SQL Server Management Studio (SSMS) and click the `Execute` button. This will automatically set up all tables and the default admin account.
2. **Open the Project:** Open the `.sln` (Solution) file using Visual Studio.
3. **Compile and Run:** Click the **Start** button on the top menu to run the project. No `App.config` configuration is required; the universal connection string will automatically detect your local server.

---

*Developer: Baran*
*Academic Project - Eskişehir Osmangazi University (ESOGÜ)*