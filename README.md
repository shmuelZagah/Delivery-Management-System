# 📦 Delivery Management System

A sophisticated, enterprise-grade logistics management platform designed to streamline complex delivery operations with real-time tracking, intelligent routing, and comprehensive fleet management capabilities.

---

## 🎯 Overview

The **Delivery Management System** is a robust desktop application built with modern .NET technologies, providing a complete solution for managing orders, couriers, and real-time delivery simulations. Designed with scalability and maintainability in mind, it implements industry-standard architectural patterns and SOLID principles for maximum flexibility and reliability.

### Key Highlights
- **Real-time Fleet Operations**: Monitor multiple couriers and deliveries simultaneously with live status updates
- **Intelligent Simulation Engine**: Model complex logistics scenarios to optimize delivery routes and predict completion times
- **Secure Role-Based Access**: Separate authenticated interfaces for administrators and courier personnel
- **Advanced Configuration Management**: Dynamic system parameters for speeds, distance ranges, and operational constraints
- **Thread-Safe Data Management**: Concurrent-safe operations ensuring data integrity across multiple users

---

## ✨ Core Features

### 🚚 Order Management
- **Full Order Lifecycle**: Create, assign, track, and complete delivery orders
- **Order Classification**: Support for multiple order types with customizable attributes (weight, volume, priority)
- **Status Tracking**: Real-time status transitions (Open → In Progress → Completed/Canceled)
- **Customer Integration**: Full customer information management with location-based delivery tracking

### 👨‍💼 Courier Management  
- **Courier Onboarding**: Complete courier registration with authentication and profile management
- **Performance Tracking**: Monitor delivery metrics and assignment history
- **Capability Management**: Define courier specializations and delivery constraints
- **Preferred Delivery Types**: Assign courier preferences for order optimization

### 📊 Administrator Dashboard
- **Live Metrics**: Real-time summary of order statuses and system health
- **System Configuration**: Manage fleet speeds, distance ranges, and operational parameters
- **Database Management**: Initialize, reset, and backup database operations
- **Simulation Controls**: Start/pause real-time delivery simulation with configurable intervals

### 🔄 Simulation Engine
- **Real-Time Modeling**: Simulate delivery operations with configurable time acceleration
- **Route Optimization**: Calculate delivery times based on courier capabilities and configurations
- **Time Management**: Advanced clock control (minute, hour, day, month, year increments)
- **Dynamic Updates**: Live observer pattern for instant UI refresh on system changes

---

## 🏗️ Architecture

The system follows a **3-Tier Layered Architecture** with clean separation of concerns:

```
┌─────────────────────────────────────────┐
│  Presentation Layer (PL)                │
│  WPF Desktop Application & UI Logic     │
├─────────────────────────────────────────┤
│  Business Logic Layer (BL)              │
│  Core Business Rules & Operations       │
├─────────────────────────────────────────┤
│  Data Access Layer (DAL)                │
│  Database Abstraction & Persistence     │
└─────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Purpose | Key Components |
|-------|---------|---|
| **PL** | User interface and interaction | Windows, ViewModels, Converters, Styles |
| **BL** | Business logic and workflows | Order operations, Courier logic, Admin functions |
| **DAL** | Data persistence and retrieval | Entity models, Repository patterns, Storage implementations |

---

## 🛠️ Technology Stack

| Category | Technologies |
|----------|---|
| **Language** | C# 12 |
| **Framework** | .NET 8.0 |
| **UI Framework** | Windows Presentation Foundation (WPF) |
| **Data Binding** | INotifyPropertyChanged pattern, XAML Data Binding |
| **Storage** | In-memory collections, XML serialization |
| **Architecture Patterns** | SOLID Principles, Repository Pattern, Singleton, Observer |
| **Threading** | Thread-safe operations with lock mechanisms |

---

## 🎨 Design Patterns & Best Practices

### ✅ SOLID Principles
- **Single Responsibility**: Separated layers handle specific concerns
- **Open/Closed**: Extensible interfaces for data access implementations
- **Liskov Substitution**: Multiple DAL implementations (DalList, DalXml)
- **Interface Segregation**: Focused, role-specific interfaces
- **Dependency Inversion**: Abstraction-based components

### 🔐 Advanced Features
- **Thread-Safe Singleton**: Concurrent-safe data access layer initialization
- **Observer Pattern**: Real-time UI updates triggered by system changes
- **Dependency Injection**: Factory-based service initialization
- **Input Validation**: Comprehensive user input sanitization (TryParse, regex validation)
- **Async/Await Pattern**: Non-blocking operations for responsive UI

### 🎨 UI/UX Enhancements
- **Responsive Data Binding**: Two-way binding for instant UI synchronization
- **Custom Control Templates**: Polished button styles with hover effects
- **Theme System**: Consistent styling across the application
- **Validation Feedback**: Visual indicators for invalid input
- **Behavior Patterns**: XAML-attached behaviors for reusable validation logic

---

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- .NET 8.0 SDK or Runtime
- Visual Studio 2022 or equivalent IDE

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/shmuelZagah/Delivery-Management-System.git
   cd Delivery-Management-System
   ```

2. **Open the solution**
   ```bash
   dotnet build dotNet5786_0814_8695.sln
   ```

3. **Run the application**
   ```bash
   dotnet run --project PL/PL.csproj
   ```

---

## 🔓 Default Credentials

### Administrator Access
```
Username: 222222222
Password: admin100
```

### Courier Access
```
Username: 234567890
Password: 1234567890
```

---

## 📖 Usage Guide

### For Administrators

1. **Login** with admin credentials
2. **Dashboard Overview**: Monitor order statistics and fleet status
3. **Manage Orders**: Create, view, and assign delivery orders
4. **Manage Couriers**: Register new couriers and manage assignments
5. **Configure System**: Adjust speeds, ranges, and operational parameters
6. **Run Simulation**: Start real-time delivery simulation

### For Couriers

1. **Login** with courier credentials
2. **View Assignments**: See assigned orders and delivery details
3. **Update Status**: Mark orders as picked up, in transit, or delivered
4. **Track Metrics**: Monitor personal delivery statistics

---

## 🏆 System Capabilities

### Performance Metrics
- **Concurrent Users**: Thread-safe operations support multiple simultaneous sessions
- **Order Capacity**: Efficient handling of large order volumes
- **Simulation Speed**: Configurable time acceleration (real-time to 1000x speed)

### Configuration Parameters
- Average speeds for car, motorcycle, bicycle, and walking
- Maximum air delivery range
- Company base coordinates (latitude/longitude)
- Maximum supply time and activity windows

---

## 🔒 Security Features

- **Authentication Layer**: Secure login with credential validation
- **Role-Based Access Control**: Separate interfaces for Admin and Courier roles
- **Thread-Safe Operations**: Lock mechanisms prevent race conditions
- **Input Validation**: All user inputs validated before processing

---

## 📁 Project Structure

```
Delivery-Management-System/
├── PL/                      # Presentation Layer (WPF)
│   ├── MainWindow.xaml      # Admin Dashboard
│   ├── LoginWindow.xaml     # Authentication
│   ├── Order/               # Order management UI
│   ├── Courier/             # Courier management UI
│   └── Helpers/             # Converters, Styles, Utilities
├── BL/                      # Business Logic Layer
│   └── BL.cs                # Core business operations
├── DalFacade/               # Data Access Facade
│   ├── DO/                  # Data Objects (Models)
│   └── DalApi/              # Interfaces & Contracts
├── DalList/                 # In-Memory Implementation
├── DalXml/                  # XML-Based Implementation
└── DataBase/                # Database & Utilities
```

---

## 🧪 Testing & Quality Assurance

The system includes comprehensive test projects:
- **BITest**: Business layer validation
- **DalTest**: Data access layer testing
- Extensive input validation with TryParse operations
- Real-world scenario simulations

---

## 🚀 Future Enhancements

- **Database Integration**: SQL Server or PostgreSQL backend
- **REST API**: RESTful web services for third-party integration
- **Mobile App**: Companion mobile application for couriers
- **Advanced Analytics**: Machine learning for route optimization
- **Real Geolocation**: Google Maps/GPS integration for true positioning
- **Notification System**: SMS/Push notifications for order updates

---

## 📝 License

This project is provided as-is for educational and commercial purposes.

---

## 👤 Authors

**Shmuel Zagah**  
**Daniel Bar**

---

## 📧 Contact & Support

For questions, feedback, or collaboration opportunities:
- **GitHub**: [@shmuelZagah](https://github.com/shmuelZagah)
- **Issues**: [GitHub Issues](https://github.com/shmuelZagah/Delivery-Management-System/issues)

---

*Last Updated: April 14, 2026*