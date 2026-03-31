# 🏦 Banking Services API

A secure and scalable **RESTful Banking API** built using **ASP.NET Core Web API** and **Entity Framework Core**, implementing a clean **3-Tier Architecture**.
The system manages **bank accounts, transactions, authentication (JWT)**, and supports **PDF/CSV report generation**.

---

## 🚀 Features

* 🔐 JWT-based Authentication (Register & Login)
* 👤 Account Management (Create, View, Update & Delete)
* 💰 Transaction Management

  * Deposit
  * Withdraw
* 📄 Transaction Reports

  * Full report (Table format)
  * ATM-style receipt
  * Export as PDF & CSV
* 📊 Pagination support for transactions
* 🔍 Filtering (Deposit / Withdraw)
* 📅 Sorting (by Date)
* 🛡️ Secure API endpoints using Authorization

---

## 🏗️ Architecture

This project follows a **3-Tier Architecture**:

* **Controller Layer** → Handles HTTP requests & responses
* **Service Layer** → Business logic (Transactions, PDF generation, Auth)
* **Data Access Layer** → Entity Framework Core (DB operations)

---

## 🛠️ Tech Stack

### Backend

* ASP.NET Core Web API (.NET 6/7)
* C#
* Entity Framework Core

### Authentication

* JWT (JSON Web Tokens)

### Database

* SQL Server
* Code First Approach (EF Core Migrations)

### Additional Libraries

* BCrypt (Password Hashing)
* QuestPDF (PDF generation)
* CSV Helper (CSV export)

---

## 👤Account APIs

Banking/Screenshots/Single Account.png

## 📂 Project Structure

```
BankingAPI/
│
├── Controllers/        # API Endpoints
├── Services/           # Business Logic (Auth, Transactions, Reports)
├── Models/             # Entities (User, Account, Transaction)
├── Data/               # DbContext
├── Migrations/         # EF Core Migrations
├── DTOs/               # Data Transfer Objects
└── Program.cs          # App configuration
```

---

## 🔐 Authentication Flow

1. User registers with username & password
2. Password is hashed using BCrypt
3. User logs in → JWT token generated
4. Token is required for accessing protected APIs

---

## 🗄️ Database Design

### 🧑 Users Table

* Id (Primary Key)
* Username
* Password (Hashed)

### 🏦 Accounts Table

* Id (Primary Key)
* AccountHolderName
* Email
* Phone
* Balance

### 💳 Transactions Table

* Id (Primary Key)
* AccountId (Foreign Key)
* Amount
* Type (Deposit / Withdraw)
* Description
* Date

---

## ⚙️ Setup Instructions

### 1. Configure Database

Update connection string in:

```
appsettings.json
```

### 2. Apply Migrations

```
Add-Migrations ""
Update-Database
```

### 3. Run Application

```
CRTL + SHIFT + B
CRTL + F5
```

### 5. Access Swagger

```
https://localhost:7157/swagger
```

---

## 📌 API Endpoints (Sample)

### 🔐 Auth

* POST `/api/auth/register`
* POST `/api/auth/login`

### 💰 Transactions

* POST `/api/transactions/deposit`
* POST `/api/transactions/withdraw`
* GET `/api/transactions/account/{accountId}`

### 📄 Reports

* GET `/api/reports/transactions/{accountId}/pdf`
* GET `/api/reports/transactions/{accountId}/csv`

---

## 📊 Pagination Example

```
GET /api/transactions/account/{accountId}/paged?page=1&size=5
```

---

## 🔒 Security

* Passwords stored using **BCrypt hashing**
* Protected endpoints require **JWT Bearer Token**
* Secure API design with proper validation

---

## 🎯 Future Enhancements

* Refresh Tokens
* Transaction history analytics
* Email/SMS notifications
* Deployment on Azure/AWS

---

## 👩‍💻 Author

Developed as part of a **Full Stack Banking Application Project** using modern backend practices.

---

## ⭐ Notes

This project demonstrates:

* Real-world backend architecture
* Secure authentication
* API design best practices
* Database modeling and relationships

---
