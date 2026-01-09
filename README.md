# Smart Home Management System

A comprehensive, enterprise-grade Smart Home Management System built with .NET 9.0, featuring clean architecture, real-time device communication via MQTT, and robust authentication mechanisms.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Project Rating & Analysis](#project-rating--analysis)
- [Recommended Upgrades](#recommended-upgrades)

## 🎯 Overview

This Smart Home Management System provides a complete backend solution for managing smart homes, rooms, IoT devices, and user access. The system enables users to create and manage multiple homes, organize devices into rooms, control IoT units via MQTT protocol, and manage user subscriptions with role-based access control.

### Key Capabilities

- **Multi-Home Management**: Users can create and manage multiple smart homes with location tracking
- **Room Organization**: Organize IoT devices into rooms within each home
- **Device Authentication**: Secure device registration and authentication using OTP challenges
- **Real-Time Communication**: MQTT-based messaging for real-time device state updates and control
- **User Management**: Comprehensive user authentication, profile management, and home subscription system
- **Email Services**: Account activation and password reset via email
- **Caching**: Redis-based caching for device OTP and MQTT state management

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns across four main layers:

```
┌─────────────────────────────────────────┐
│           API Layer                     │
│  (Controllers, DTOs, Middleware)        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        Application Layer                │
│  (Services, Business Logic, Interfaces) │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│          Domain Layer                    │
│  (Entities, Repository Interfaces)       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Infrastructure Layer               │
│  (Persistence, External Services, MQTT)  │
└─────────────────────────────────────────┘
```

### Layer Responsibilities

- **API**: HTTP endpoints, request/response DTOs, authentication middleware
- **Application**: Business logic, service orchestration, use cases
- **Domain**: Core entities, domain models, repository contracts
- **Infrastructure**: Database access (EF Core), external services (Email, MQTT, Redis), identity management

## ✨ Features

### Authentication & Security
- ✅ User registration with email verification
- ✅ JWT-based authentication with refresh tokens
- ✅ Password reset functionality
- ✅ Device-based authentication (MAC address tracking)
- ✅ OTP challenge system for device verification
- ✅ Account activation via email
- ✅ Secure password hashing

### Home Management
- ✅ Create and manage multiple homes
- ✅ Home location tracking (latitude/longitude)
- ✅ Rename homes
- ✅ Home subscription requests system
- ✅ Multi-user home access management

### Room Management
- ✅ Create and delete rooms within homes
- ✅ Room organization for IoT devices
- ✅ Room-based device grouping

### Device Management
- ✅ Device registration and authentication
- ✅ MAC address-based device identification
- ✅ MQTT integration for real-time device communication
- ✅ Device state management (on/off, brightness, temperature, speed)
- ✅ Control unit management (lights, AC, fans, etc.)

### User Management
- ✅ User profile management (username, display name, phone, image)
- ✅ User dashboard with home listings
- ✅ Home subscription request management
- ✅ Multi-home user access

### Additional Features
- ✅ Image upload and management
- ✅ Email service integration (MailKit)
- ✅ Redis caching for performance
- ✅ Swagger/OpenAPI documentation
- ✅ CORS configuration for frontend integration
- ✅ Docker support for containerization

## 🛠️ Technology Stack

### Backend Framework
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 9.0** - ORM for database access
- **SQL Server** - Primary database

### Authentication & Security
- **ASP.NET Core Identity** - User identity management
- **JWT Bearer Authentication** - Token-based authentication
- **Custom Token Service** - Refresh token management
- **BCrypt/Argon2** - Password hashing

### Messaging & Communication
- **MQTTnet** - MQTT protocol implementation
- **MailKit** - Email service integration

### Caching & Storage
- **Redis** - In-memory caching (StackExchange.Redis)
- **Physical File Storage** - Image and static file storage

### Development Tools
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization support
- **Entity Framework Migrations** - Database versioning

## 📦 Prerequisites

Before setting up the project, ensure you have the following installed:

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** (Express or higher) - [Download](https://www.microsoft.com/sql-server/sql-server-downloads)
- **Redis Server** - [Download](https://redis.io/download) or use Docker: `docker run -d -p 6379:6379 redis`
- **MQTT Broker** (optional, for device communication) - [Mosquitto](https://mosquitto.org/download/) or cloud service
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** - For version control

## 🚀 Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd SmartHomeFullSystem
```

### 2. Database Setup

#### Option A: Using SQL Server Express (Local)

1. Open SQL Server Management Studio (SSMS)
2. Create a new database named `SmartHome_DB`
3. Update the connection string in `API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=YOUR_SERVER_NAME\\SQLEXPRESS;Initial Catalog=SmartHome_DB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"
}
```

#### Option B: Using Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Update connection string:
```json
"DefaultConnection": "Data Source=localhost,1433;Initial Catalog=SmartHome_DB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```

### 3. Run Database Migrations

```bash
cd API
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project .
```

### 4. Redis Setup

#### Option A: Local Installation
- Install Redis for Windows or use WSL
- Ensure Redis is running on `localhost:6379`

#### Option B: Docker
```bash
docker run -d -p 6379:6379 --name redis redis:latest
```

### 5. Configure Application Settings

Edit `API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING",
    "RedisConnection": "localhost:6379"
  },
  "EmailSetting": {
    "Port": 465,
    "From": "your-email@gmail.com",
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "SmtpHost": "smtp.gmail.com"
  },
  "Jwt": {
    "SigningKey": "YOUR_SECRET_KEY_MIN_32_CHARACTERS",
    "Issuer": "SmartHome.Api",
    "Audience": "SmartHome.Client",
    "AccessTokenMinutes": 10,
    "RefreshTokenExpirationDays": 7
  },
  "FronEndInfo": {
    "FronEndUrl": {
      "ActivationUrl": "https://localhost:4200/authentication",
      "ResetPasswordUrl": "https://localhost:4200/authentication"
    },
    "FronEndComponent": {
      "ActivationComponent": "account-activation",
      "ResetPasswordComponent": "reset-password"
    },
    "DefaultUserImageUrl": "https://localhost:7072/General/AnonymousUserImage.png"
  }
}
```

**⚠️ Security Note**: 
- Generate a strong JWT signing key (minimum 32 characters)
- Use environment variables or User Secrets for sensitive data in production
- Never commit secrets to version control

### 6. Configure User Secrets (Recommended for Development)

```bash
cd API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "Jwt:SigningKey" "YOUR_SECRET_KEY"
dotnet user-secrets set "EmailSetting:Password" "YOUR_EMAIL_PASSWORD"
```

### 7. Build the Solution

```bash
dotnet restore
dotnet build
```

### 8. Run the Application

```bash
cd API
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger` (Development only)

### 9. Verify Setup

1. Open Swagger UI at `https://localhost:5001/swagger`
2. Test the health check endpoint (if available)
3. Try registering a new user via `/api/Auth/register`

## ⚙️ Configuration

### Environment-Specific Settings

Create `appsettings.Development.json` for development overrides:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### MQTT Configuration

If using MQTT for device communication, configure the MQTT broker settings in `InfrastructureModule.cs` or add to `appsettings.json`:

```json
{
  "Mqtt": {
    "BrokerHost": "localhost",
    "BrokerPort": 1883,
    "ClientId": "SmartHomeApi",
    "Username": "",
    "Password": ""
  }
}
```

### CORS Configuration

Update CORS policy in `Program.cs` to match your frontend URL:

```csharp
policy.WithOrigins("https://localhost:4200") // Your Angular app URL
```

## 📚 API Documentation

### Authentication Endpoints

- `POST /api/Auth/register` - Register new user
- `POST /api/Auth/login` - User login (requires Device-Mac header)
- `POST /api/Auth/logout` - User logout
- `POST /api/Auth/activate-account` - Activate account with token
- `POST /api/Auth/forgot-password` - Request password reset
- `POST /api/Auth/reset-password` - Reset password with token
- `POST /api/Auth/refresh-token` - Refresh access token
- `GET /api/Auth/check-email` - Check email availability
- `GET /api/Auth/check-username` - Check username availability
- `DELETE /api/Auth/delete-account` - Delete user account

### Home Management Endpoints

- `POST /api/HomeManagement/Create-NewHome` - Create new home
- `PATCH /api/HomeManagement/Rename-Home` - Rename home
- `GET /api/HomeManagement/Get-HomeData/{homeId}` - Get home details
- `GET /api/HomeManagement/Get-Home-SubRequest/{homeId}` - Get subscription requests
- `POST /api/HomeManagement/Add-NewRoom` - Add room to home
- `POST /api/HomeManagement/Add-NewUser` - Add user to home
- `DELETE /api/HomeManagement/Delete-User` - Remove user from home
- `DELETE /api/HomeManagement/Delete-Room` - Delete room

### User Information Endpoints

- `GET /api/UserInfo/Get-Info` - Get user profile
- `GET /api/UserInfo/Get-Homes` - Get user's homes
- `GET /api/UserInfo/Get-All-HSRQ` - Get all subscription requests
- `GET /api/UserInfo/Get-New-HSRQ` - Get new subscription requests
- `PATCH /api/UserInfo/Update-UserName` - Update username
- `PATCH /api/UserInfo/Update-DisplayName` - Update display name
- `PATCH /api/UserInfo/Update-PhoneNumber` - Update phone number
- `PATCH /api/UserInfo/Update-UserImage` - Update profile image
- `POST /api/UserInfo/Subscribe-ToHome` - Request home subscription
- `DELETE /api/UserInfo/Delete-SubRequest/{requestId}` - Delete subscription request

### Device Authentication Endpoints

- `POST /api/DevicesAuth/VerifyOTP` - Verify device OTP
- `POST /api/DevicesAuth/UpdateBrowserId` - Update device identifier

**Note**: Most endpoints require JWT authentication. Include the token in the `Authorization` header:
```
Authorization: Bearer <your-access-token>
```

## 📁 Project Structure

```
SmartHomeFullSystem/
├── API/                          # Presentation Layer
│   ├── Controllers/              # API Controllers
│   ├── ApiDTOs/                  # Request/Response DTOs
│   ├── Program.cs                # Application entry point
│   └── appsettings.json          # Configuration
│
├── Application/                  # Application Layer
│   ├── Auth/                     # Authentication services
│   ├── Home-Management/          # Home management services
│   ├── User-Dashboard/           # User dashboard services
│   ├── Contracts/                # Application contracts
│   └── DI/                       # Dependency injection
│
├── Domain/                       # Domain Layer
│   ├── Entities/                 # Domain entities
│   ├── GenericResult/            # Result pattern
│   └── RepositotyInterfaces/     # Repository contracts
│
└── Infrastructure/               # Infrastructure Layer
    ├── Persistence/              # EF Core & Repositories
    ├── Identity/                 # Identity management
    ├── Security/                 # JWT, hashing services
    ├── Messaging/                # Email & MQTT services
    ├── Images/                   # Image handling
    ├── CasheStorage/             # Redis caching
    └── DI/                       # Infrastructure DI
```

## 📊 Project Rating & Analysis

### Overall Rating: **8.5/10** ⭐⭐⭐⭐

### Strengths ✅

1. **Architecture (9/10)**
   - Excellent clean architecture implementation
   - Clear separation of concerns
   - Proper dependency direction (Domain → Application → Infrastructure → API)
   - Well-organized layer structure

2. **Code Quality (8/10)**
   - Modern .NET 9.0 features
   - Nullable reference types enabled
   - Generic Result pattern for error handling
   - Domain-driven design principles

3. **Security (8/10)**
   - JWT authentication with refresh tokens
   - Device-based authentication
   - Password hashing
   - OTP challenge system
   - ⚠️ **Issue**: JWT secret key in appsettings.json (should use User Secrets/Environment Variables)

4. **Features (8/10)**
   - Comprehensive authentication system
   - Multi-home management
   - MQTT integration for IoT
   - Email services
   - Redis caching

5. **Documentation (6/10)**
   - Basic README (now improved)
   - Swagger/OpenAPI available
   - ⚠️ Missing inline code documentation
   - ⚠️ No architecture diagrams

### Areas for Improvement ⚠️

1. **Security Concerns**
   - Secrets in configuration files
   - No rate limiting
   - Missing input validation attributes
   - No API versioning

2. **Testing**
   - No unit tests
   - No integration tests
   - No test coverage

3. **Error Handling**
   - Inconsistent error responses
   - Missing global exception handler
   - No structured logging

4. **Performance**
   - No response caching
   - Missing pagination for list endpoints
   - No database query optimization visible

5. **Code Issues**
   - MQTT service not fully implemented (throws NotImplementedException)
   - Some typos in naming (e.g., "FronEndInfo" → "FrontEndInfo")
   - Missing SignalR hubs (referenced in Program.cs but not implemented)

## 🔧 Recommended Upgrades

### High Priority 🔴

#### 1. Security Enhancements
- [ ] Move secrets to User Secrets/Environment Variables
- [ ] Implement rate limiting (e.g., AspNetCoreRateLimit)
- [ ] Add input validation with FluentValidation
- [ ] Implement API versioning
- [ ] Add HTTPS enforcement
- [ ] Implement CORS policy restrictions
- [ ] Add request size limits

#### 2. Testing Infrastructure
- [ ] Add xUnit test project
- [ ] Write unit tests for services (target 80%+ coverage)
- [ ] Add integration tests for API endpoints
- [ ] Implement test database seeding
- [ ] Add API contract testing

#### 3. Error Handling & Logging
- [ ] Implement global exception handler middleware
- [ ] Add structured logging (Serilog)
- [ ] Create custom exception types
- [ ] Standardize error response format
- [ ] Add request/response logging middleware

#### 4. Complete MQTT Implementation
- [ ] Implement `MqttBusService` methods
- [ ] Add MQTT configuration to appsettings
- [ ] Implement device state synchronization
- [ ] Add MQTT connection health checks
- [ ] Create MQTT message handlers

### Medium Priority 🟡

#### 5. Performance Optimizations
- [ ] Add response caching for read operations
- [ ] Implement pagination for list endpoints
- [ ] Add database query optimization (AsNoTracking, projections)
- [ ] Implement lazy loading or explicit loading strategies
- [ ] Add Redis caching for frequently accessed data

#### 6. API Improvements
- [ ] Implement SignalR hubs for real-time updates
- [ ] Add API versioning (v1, v2)
- [ ] Create API response wrappers
- [ ] Add request/response compression
- [ ] Implement health check endpoints

#### 7. Code Quality
- [ ] Fix naming inconsistencies (FronEndInfo → FrontEndInfo)
- [ ] Add XML documentation comments
- [ ] Implement code analyzers (StyleCop, SonarAnalyzer)
- [ ] Add .editorconfig file
- [ ] Fix nullable reference warnings

#### 8. Database Enhancements
- [ ] Add database indexes for performance
- [ ] Implement soft delete properly
- [ ] Add database migrations for indexes
- [ ] Consider read replicas for scaling
- [ ] Add database backup strategy

### Low Priority 🟢

#### 9. Documentation
- [ ] Add inline XML documentation
- [ ] Create architecture decision records (ADRs)
- [ ] Add sequence diagrams for complex flows
- [ ] Document deployment procedures
- [ ] Create developer onboarding guide

#### 10. DevOps & CI/CD
- [ ] Add GitHub Actions / Azure DevOps pipeline
- [ ] Implement automated testing in CI
- [ ] Add Docker Compose for local development
- [ ] Create production deployment scripts
- [ ] Add application monitoring (Application Insights)

#### 11. Advanced Features
- [ ] Implement WebSocket support for real-time updates
- [ ] Add device scheduling/automation
- [ ] Implement device groups/scenes
- [ ] Add notification system (push notifications)
- [ ] Create admin dashboard API
- [ ] Add audit logging
- [ ] Implement file upload size limits and validation

#### 12. Code Refactoring
- [ ] Extract common controller logic to base class
- [ ] Implement CQRS pattern for complex operations
- [ ] Add MediatR for request handling
- [ ] Implement repository pattern improvements
- [ ] Add specification pattern for queries

### Implementation Priority Order

1. **Week 1-2**: Security fixes, error handling, logging
2. **Week 3-4**: Testing infrastructure, unit tests
3. **Week 5-6**: Complete MQTT implementation, SignalR
4. **Week 7-8**: Performance optimizations, API improvements
5. **Ongoing**: Documentation, CI/CD, advanced features

## 🤝 Contributing

This is a private project. For contributions, please follow:
1. Create feature branches
2. Write tests for new features
3. Ensure all tests pass
4. Update documentation
5. Submit pull requests

## 📝 License

[Specify your license here]

## 👥 Authors

[Add author information]

## 🙏 Acknowledgments

- .NET team for the excellent framework
- MQTTnet contributors
- Entity Framework Core team

---

**Last Updated**: January 2025
**Version**: 1.0.0
**Status**: Active Development
