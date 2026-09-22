# Online Class Management System

A robust, layered .NET 8 Web API solution designed for managing online classes, student enrollments, schedules, subjects, and teaching plans.

---

## 📌 Project Overview

The **Online Class Management System** is built using ASP.NET Core Web API and Entity Framework Core following a decoupled, layered architecture. It provides RESTful APIs for managing sub-classes, registering student enrollments with automatic capacity tracking, managing users/teachers, and handling schedules and teaching plans.

### Tech Stack
- **Framework**: .NET 8.0
- **API Engine**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 8.0 (SQL Server)
- **Documentation**: Swagger / OpenAPI (Swashbuckle)

---

## 🏗️ Architecture & Project Structure

```text
OnlineClassManagementSystem/
├── OnlineClassManagementSystem.Database/       # Database Context & EF Core Entity Models
│   └── Models/
│       ├── AppDbContext.cs
│       ├── TblSubClass.cs
│       ├── TblEnrollment.cs
│       ├── TblUser.cs
│       ├── TblSubject.cs
│       ├── Schedule.cs
│       └── TeachPlan.cs
├── OnlineClassManagementSystem.Domain/         # Business Logic, Feature Services & DTO Models
│   ├── features/
│   │   ├── SubClass/
│   │   │   └── SubClassService.cs
│   │   └── Enrollment/
│   │       └── EnrollmentService.cs
│   └── models/                                 # Request & Response DTO Models
│       ├── SubClass*.cs
│       └── Enrollment*.cs
└── OnlineClassManagementSystem.WebApi/         # REST API Controllers & Program Setup
    ├── Controllers/
    │   ├── SubClassController.cs
    │   └── EnrollmentController.cs
    ├── appsettings.json
    └── Program.cs
```


### Detailed Feature Workflows

#### 1. SubClass Creation Workflow
1. Client submits class details (`ClassName`, `Place`, `OpenDate`, `OpenTime`, `StudentLimit`).
2. `SubClassService` validates required fields and checks for duplicate class names or overlapping place/date/time.
3. Class is saved to `Tbl_SubClass` with initial `StudentCount = 0`.

#### 2. Enrollment Workflow
1. Client requests enrollment with `ClassId` and `StudentId`.
2. `EnrollmentService` verifies:
   - Target `SubClass` exists and is active (`!IsDelete`).
   - Target `Student` exists and is active.
   - Student is not already enrolled in the class.
   - Class capacity has not reached `StudentLimit`.
3. Adds record to `Tbl_Enrollments` and automatically increments `StudentCount` in `Tbl_SubClass`.

#### 3. Enrollment Cancellation / Soft Delete Workflow
1. Client requests deletion of an enrollment by `EnrollmentId`.
2. `EnrollmentService` marks `Tbl_Enrollments.IsDelete = true`.
3. Automatically decrements `StudentCount` in `Tbl_SubClass`.

---

## ✨ Features List

### 🏫 SubClass Management
- **Get All SubClasses**: Fetch list of non-deleted sub-classes.
- **Get SubClass by ID**: Retrieve specific class details.
- **Create SubClass**: Add new class with validations on duplicate name and schedule conflicts.
- **Patch SubClass**: Partial updates for class name, location, dates, times, and student limits.
- **Delete SubClass**: Soft delete sub-class by setting `IsDelete = true`.

### 🎓 Enrollment Management
- **Get All Enrollments**: Retrieve active student class enrollments with class & student details.
- **Get Enrollment by ID**: Fetch enrollment info by ID.
- **Create Enrollment**: Enroll student into a class with capacity limit checks & automatic count increment.
- **Patch Enrollment**: Update enrolled class or student with automatic capacity adjustment across old and new classes.
- **Delete Enrollment**: Soft delete enrollment record and automatically decrement class student count.

### 👤 User & Course Infrastructure
- Data models for `TblUser` (Students & Teachers), `TblSubject`, `Schedule`, and `TeachPlan`.

---

## ✅ Implementation Checklist

- [x] Solution setup with Clean Architecture layout (.Database, .Domain, .WebApi)
- [x] Entity Framework Core models & DbContext configuration (`AppDbContext`)
- [x] **SubClass Feature**
  - [x] `SubClassService` implementation
  - [x] Request/Response models (`SubClassList`, `SubClassEdit`, `SubClassCreate`, `SubClassPatch`, `SubClassDelete`)
  - [x] `SubClassController` endpoints (`GET`, `POST`, `PATCH`, `DELETE`)
- [x] **Enrollment Feature**
  - [x] `EnrollmentService` implementation
  - [x] Request/Response models (`EnrollmentList`, `EnrollmentEdit`, `EnrollmentCreate`, `EnrollmentPatch`, `EnrollmentDelete`)
  - [x] `EnrollmentController` endpoints (`GET`, `POST`, `PATCH`, `DELETE`)
  - [x] Automatic capacity check & `StudentCount` synchronization
- [x] Dependency Injection configuration in `Program.cs`
- [x] Solution build verification (0 Errors)
- [ ] **Future Enhancements / Backlog**
  - [ ] Subject Service & Controller (`TblSubject`)
  - [ ] Schedule Service & Controller (`Schedule`)
  - [ ] Teach Plan Service & Controller (`TeachPlan`)
  - [ ] Authentication & Authorization (JWT integration)
  - [ ] Unit Tests & Integration Tests

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or Express / Full Instance)

### Configuration
Update the database connection string in `OnlineClassManagementSystem.WebApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DbConnection": "Server=YOUR_SERVER_NAME;Database=OnlineClassDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Running the API

```bash
# Build the solution
dotnet build

# Run the Web API project
dotnet run --project OnlineClassManagementSystem.WebApi
```

Once running, navigate to `https://localhost:7196/swagger` (or your configured port) to test endpoints using Swagger UI.

---

## 📡 API Endpoints Summary

| Feature | Method | Endpoint | Description |
| :--- | :--- | :--- | :--- |
| **SubClass** | `GET` | `/api/SubClass` | Get all active sub-classes |
| **SubClass** | `GET` | `/api/SubClass/{SubClassId}` | Get sub-class details by ID |
| **SubClass** | `POST` | `/api/SubClass` | Create new sub-class |
| **SubClass** | `PATCH` | `/api/SubClass/{id}` | Partially update sub-class details |
| **SubClass** | `DELETE` | `/api/SubClass/{SubClassId}` | Soft delete sub-class |
| **Enrollment** | `GET` | `/api/Enrollment` | Get all active enrollments |
| **Enrollment** | `GET` | `/api/Enrollment/{EnrollmentId}` | Get enrollment by ID |
| **Enrollment** | `POST` | `/api/Enrollment` | Create new student enrollment |
| **Enrollment** | `PATCH` | `/api/Enrollment/{id}` | Update enrollment details |
| **Enrollment** | `DELETE` | `/api/Enrollment/{EnrollmentId}` | Soft delete enrollment |

---

## 📚 Referenced Projects

---

### 🛒 Book Store POS System (MSSQL)

A web-based Point of Sale (POS) system for a bookstore that enables customers to browse and buy books easily while providing staff with tools to track inventory, process orders, and view sales metrics.

---

#### 🧰 Tech Stack & Database Architecture

| Property | Details |
| :--- | :--- |
| **Database Engine** | Microsoft SQL Server (MSSQL) |
| **Architecture** | Relational Database with Soft Delete support for inventory tracking |

---

#### 🗂️ Entity Relationship Overview

| Table | Purpose | Key Attributes |
| :--- | :--- | :--- |
| **Books** | Catalogs bookstore inventory (supports soft delete). | `BookId`, `Title`, `Price`, `StockQuantity`, `IsDeleted` |
| **Orders** | Records individual sales transactions. | `OrderId`, `TotalPrice`, `OrderDate` |
| **OrderItems** | Junction table linking books to specific orders. | `OrderItemId`, `OrderId`, `BookId`, `Quantity`, `UnitPrice` |

---

#### ✨ Key Features

##### 🛍️ Customer / Cashier Side
- **Find Books**: Search inventory by Title, Author, or Genre.
- **View Book Details**: See description, price, and stock status.
- **Shopping Cart**: Add, update, or remove books.
- **Checkout**: Display total prices and complete sales.

##### 🔧 Admin / Staff Side
- **Management**:
  - Add new books (price, initial stock).
  - View current stock levels.
  - Edit book details or update stock.
- **Soft Delete**: Mark outdated books as soft-deleted (`IsDeleted = 1`) to preserve historical sales records without showing them in the active catalog.
- **Sales Dashboard**: Review order history and daily sales totals.

---

#### 📡 API Endpoints

The following REST API endpoints are exposed by the `BookStorePOS.WebApi` project via Swagger:

##### 📖 Book Controller (`/api/book`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/book` | List all books |
| `GET` | `/api/book/{id}` | Get a specific book |
| `POST` | `/api/book` | Add a new book to inventory |
| `PATCH` | `/api/book/{id}` | Update a book's details or stock |
| `DELETE` | `/api/book/{id}` | Remove a book (Soft delete) |

##### 🧾 Order Controller (`/api/order`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/order` | View order history |
| `GET` | `/api/order/{id}` | View a specific order (returns the order and its items) |
| `POST` | `/api/order` | Checkout! Send a list of items. Calculates prices and saves securely. |

---

#### 🗄️ MSSQL Setup Script

Run the following script directly in your existing database:

```sql
-- 1. Create Books Table (Includes soft delete column)
CREATE TABLE Books (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(150) NOT NULL,
    Genre NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0, -- 0 = Active, 1 = Soft Deleted
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- 2. Create Orders Table
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME2 DEFAULT GETDATE(),
    TotalPrice DECIMAL(10, 2) NOT NULL DEFAULT 0.00
);
GO

-- 3. Create OrderItems Table
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    Subtotal AS (Quantity * UnitPrice) PERSISTED, -- Calculated persisted column
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Books FOREIGN KEY (BookId) REFERENCES Books(BookId)
);
GO

-- =========================================================
-- SAMPLE DATA INSERTION
-- =========================================================

-- Insert Sample Books
INSERT INTO Books (Title, Author, Genre, Description, Price, StockQuantity, IsDeleted) VALUES
(N'The Great Gatsby', N'F. Scott Fitzgerald', N'Classic', N'A novel about the American dream in the 1920s.', 12.99, 15, 0),
(N'To Kill a Mockingbird', N'Harper Lee', N'Classic', N'A story of racial injustice and loss of innocence.', 14.50, 10, 0),
(N'Dune', N'Frank Herbert', N'Sci-Fi', N'A science fiction masterpiece set on Arrakis.', 18.99, 8, 0),
(N'1984', N'George Orwell', N'Dystopian', N'A dystopian social science fiction novel.', 11.25, 20, 0),
(N'The Hobbit', N'J.R.R. Tolkien', N'Fantasy', N'The adventure of Bilbo Baggins in Middle-earth.', 15.00, 0, 0),
(N'Outdated IT Manual 2005', N'Tech Writer', N'Technology', N'Old tech manual no longer sold.', 5.00, 0, 1); -- Soft Deleted

-- Insert Sample Orders
INSERT INTO Orders (OrderDate, TotalPrice) VALUES
('2026-08-01 10:30:00', 40.48),
('2026-08-02 14:15:00', 18.99);

-- Insert Sample Order Items
INSERT INTO OrderItems (OrderId, BookId, Quantity, UnitPrice) VALUES
(1, 1, 2, 12.99), -- Alice bought 2 Gatsby
(1, 2, 1, 14.50), -- Alice bought 1 Mockingbird
(2, 3, 1, 18.99); -- Bob bought 1 Dune
GO
```
