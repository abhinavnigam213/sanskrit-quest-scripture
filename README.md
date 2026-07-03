# Sanskrit & Vedic Scripture Translator & Analyzer (Backend API) 🕉️

A high-performance C# ASP.NET Core 10.0 Web API backend service providing philological analysis, translation, transliteration, and dictionary search capabilities for ancient Sanskrit texts.

The frontend SPA has been separated into its own independent repository: **Sanskrit-Quest-Scripture-Client**.

---

## 📂 Project Directory Structure

Below is the directory hierarchy of this repository:

```text
Sanskrit-Quest-Scripture (Backend Repository Root)
├── src/                        # Backend .NET Core Solution Directory
│   ├── Business/               # Business logic layers (contracts and providers)
│   ├── Common/                 # Cross-cutting concerns (config, security, and Transliterator.cs)
│   ├── Data/                   # Data layer (contracts, providers, and test console)
│   ├── Services/               # Third-party integrations (AI Service)
│   └── Web/
│       └── Web.Api/            # ASP.NET Core 10.0 Web API Project
│           ├── Controllers/    # REST API controllers
│           ├── DataFiles/      # JSON datasets (scriptures, dictionaries)
│           ├── Models/         # C# record type definitions
│           ├── Properties/     # launchSettings.json (dev server configurations)
│           ├── Program.cs      # API entry point & DI configuration
│           ├── Startup.cs      # Middleware pipeline setup
│           ├── appsettings.json
│           └── Web.Api.csproj
│   └── SanskritQuest.slnx      # .NET solution file
├── .env.example                # Template for backend environment variables
├── .gitignore                  # Git patterns specific to .NET, OS, and VS
├── PHASE_2_DESIGN.md           # Phase 2 architecture reference (RAG, pgvector, cloud)
├── metadata.json               # IDE/AI Studio metadata
└── README.md                   # This file
```

---

## 🛠️ Technology Stack

- **ASP.NET Core 10.0 Web API**: High-performance backend hosting REST controllers with structured dependency injection.
- **Microsoft.Extensions.AI**: Provider-agnostic AI abstraction layer for chat completion integration.
- **OpenAI NuGet Package**: Used to connect to Google Gemini via the OpenAI-compatible REST endpoint.
- **JWT Bearer Authentication**: Secure token-based access validation.
- **Swagger / OpenAPI**: Interactive API documentation and testing interface.
- **Transliterator.cs**: Server-side rule-based Devanagari ↔ IAST/ITRANS/Phonetic conversion engine.
- **Fallback Archives**: Rules and database fallbacks for 100% service uptime even without active LLM keys.

---

## 📡 API Endpoints

The API is exposed under `/api/*`. Cross-Origin Resource Sharing (CORS) is enabled to allow frontend SPAs to securely make API requests.

### Authentication
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/token` | None | Generates a JWT Bearer token from `clientId` + `clientSecret` credentials |

### Scripture & Dictionary
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/scriptures` | Bearer | Returns the full list of pre-configured offline scriptures |
| `GET` | `/api/dictionary` | Bearer | Returns all specialized dictionary entries (partitioned by category) |
| `GET` | `/api/dictionary?word=<query>` | Bearer | Searches the specialized Sanskrit dictionary by exact or partial match |

### Translation & Analysis
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/translate` | Bearer | Translates text between Sanskrit, Hindi, and English |
| `POST` | `/api/transliterate` | Bearer | Converts Indic texts across schemes (Devanagari, IAST, ITRANS, SLP1, Phonetic) |
| `POST` | `/api/analyze` | Bearer | Split Sandhi, parsing grammar, poetic meter analysis, and theological notes |

### System
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/health` | None | Returns API health status and runtime mode |

---

## 🔐 Authentication Flow

All endpoints (except `AuthController` and the health check) are protected with `[Authorize]` tags requiring a valid JWT Bearer token.

### How It Works
1. **Token Acquisition**: Clients send client credentials (`clientId` and `clientSecret`) to `POST /api/auth/token`.
2. **Server Validation**: Credentials are validated against database records or client settings defined in `appsettings.json > AuthSettings`.
3. **Token Issuance**: A temporary JWT bearer token is signed and returned to the caller.
4. **Usage**: The client attaches `Authorization: Bearer <your_jwt_token>` header to all subsequent resource requests.

### Making Direct API Requests (using curl)
```bash
# Step 1: Acquire a token
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"clientId":"SanskritQuestClient123","clientSecret":"SuperSecretSanskritQuestKey456!"}'

# Step 2: Call resources using the returned token
curl http://localhost:5000/api/scriptures \
  -H "Authorization: Bearer <your_jwt_token>"
```

---

## ⚙️ Environment Variables

Create a `.env` file in the root directory (see `.env.example` for the template) or configure these in your system environment:

| Variable | Required | Description |
|---|---|---|
| `GEMINI_API_KEY` | Recommended | Google Gemini API key for AI-powered translations. Without this, the server runs in offline fallback mode using cached archives. |

---

## 🚀 Running the Project

### Local Development Setup

#### 1. Configure Environment
Create a `.env` file in the root directory by copying the template:
```bash
cp .env.example .env           # Edit with your GEMINI_API_KEY
```

#### 2. Start Backend (`src/Web/Web.Api`)
Run the following dotnet run command from the root of the project:
```bash
dotnet run --project src/Web/Web.Api/Web.Api.csproj
```
This builds and starts the backend service on:
- **http://localhost:5000** (HTTP)
- **https://localhost:5001** (HTTPS)

Access the Swagger UI at:
- **http://localhost:5000/swagger** (or `/swagger` on the active HTTPS port)

### Production Build
To publish the backend app:
```bash
dotnet publish src/Web/Web.Api/Web.Api.csproj -c Release -o bin/Release/publish
```

---

## 🐳 Docker Containerization (Backend-only)

You can containerize the ASP.NET Core Web API inside a production Docker image.

```dockerfile
# ==========================================
# STAGE 1: Build the ASP.NET Core API
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ ./
RUN dotnet restore SanskritQuest.slnx
RUN dotnet publish Web/Web.Api/Web.Api.csproj -c Release -o /app/publish

# ==========================================
# STAGE 2: Execution Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SanskritQuest.Web.Api.dll"]
```

---

## 🛠️ NuGet Dependencies (Backend)

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.Extensions.AI` | 9.0.1-preview.1.24570.5 | Provider-agnostic AI abstraction (`IChatClient`) |
| `Microsoft.Extensions.AI.OpenAI` | 9.0.1-preview.1.24570.5 | OpenAI adapter for Microsoft.Extensions.AI |
| `OpenAI` | 2.1.0 | OpenAI SDK (used for Gemini OpenAI-compatible endpoint) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.8 | JWT Bearer authentication middleware |
| `Swashbuckle.AspNetCore` | 6.6.2 | Swagger / OpenAPI documentation generator |
| `System.IdentityModel.Tokens.Jwt` | 8.19.1 | JWT token creation and validation |
