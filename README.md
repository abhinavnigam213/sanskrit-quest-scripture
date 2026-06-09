# Sanskrit & Vedic Scripture Translator & Analyzer 🕉️

A scholarly Sanskrit and Hindu scripture translator, transliterator, and analytical parser. This application is structured as a modern full-stack solution with an **ASP.NET Core 8.0 Web API** backend (`Web.Api`) and a **React 19 + Vite 6** frontend SPA (`Web.Client`).

---

## 📂 Project Directory Structure

Below is the full project hierarchy, including the backend API layer, client SPA, data assets, and root configurations:

```text
Project - Root Folder
├── assets/                     # Platform assets (AI Studio metadata)
├── Web.Api/                    # ASP.NET Core 8.0 Web API Backend
│   ├── Controllers/            # REST API controllers
│   │   ├── AnalyzeController.cs
│   │   ├── AuthController.cs
│   │   ├── DictionaryController.cs
│   │   ├── ScripturesController.cs
│   │   ├── TranslateController.cs
│   │   └── TransliterateController.cs
│   ├── DataFiles/              # JSON datasets (scriptures, dictionaries, archives)
│   ├── Models/                 # C# record type definitions (Types.cs)
│   ├── Properties/             # launchSettings.json (dev server URLs)
│   ├── Services/               # Business logic layer
│   │   ├── AIService.cs        # Gemini AI integration & local fallback engine
│   │   └── DataService.cs      # JSON dataset loader & category partitioner
│   ├── Utils/                  # Transliterator.cs (rule-based script conversion)
│   ├── Program.cs              # API entry point, DI registration, middleware
│   ├── Web.Api.csproj          # .NET 8.0 project file & NuGet dependencies
│   └── appsettings.json        # App configuration & JWT AuthSettings
├── Web.Client/                 # React 19 + Vite 6 Single Page Application (TypeScript)
│   ├── public/                 # Static assets (logo, icons)
│   ├── src/                    # React source files
│   │   ├── components/         # 14 modular UI components (views, tabs, workbench)
│   │   ├── utils/              # apiClient.ts (JWT-aware Axios), transliteration.ts
│   │   ├── App.tsx             # Primary React view and state controller
│   │   ├── main.tsx            # React application entry node
│   │   ├── types.ts            # Shared TypeScript type declarations
│   │   └── index.css           # Global styles, fonts & font-size scaling
│   ├── package.json            # Client dependencies and npm scripts
│   ├── tsconfig.json           # Client TypeScript configuration
│   └── vite.config.ts          # Vite config with API proxy to ASP.NET Core
├── .env.example                # Template for environment configurations
├── .gitignore
├── metadata.json               # AI Studio application metadata
├── package.json                # Root dev scripts (unified concurrently runner)
├── PHASE_2_DESIGN.md           # Phase 2 architecture reference (RAG, pgvector, cloud)
├── SanskritQuest.slnx     # .NET solution file
└── README.md                   # This file
```

---

## 🛠️ Technology Stack

The application leverages a clean separation of concerns across frontend and backend:

### 1. Frontend (`Web.Client`)
- **React 19**: Modern functional components, custom hooks, and state management.
- **Vite 6**: Fast dev server with HMR, API proxy to the .NET backend, and optimized production bundling.
- **Tailwind CSS v4**: Utility-first styling with a serene "Vedic Gold & Sand" design palette.
- **Framer Motion (`motion/react`)**: Graceful layout transitions and staggered visual elements.
- **Lucide React**: Modern and accessible vector iconography.
- **Axios**: HTTP client with JWT Bearer token interceptors for secure API communication.

### 2. Backend (`Web.Api`)
- **ASP.NET Core 8.0 Web API**: High-performance backend with controllers, dependency injection, and middleware pipeline.
- **Microsoft.Extensions.AI**: Provider-agnostic AI abstraction layer for chat completion integration.
- **OpenAI NuGet Package**: Used to connect to Google Gemini via the OpenAI-compatible REST endpoint (`generativelanguage.googleapis.com`).
- **JWT Bearer Authentication**: Secure token-based authentication using `Microsoft.AspNetCore.Authentication.JwtBearer`.
- **Swagger / OpenAPI**: Interactive API documentation and testing via `Swashbuckle.AspNetCore`.
- **DataService**: Loads and partitions JSON datasets (scriptures, dictionaries, archives) from `Web.Api/DataFiles/`.
- **AIService**: Orchestrates Gemini AI calls with structured prompt engineering and local rule-based fallback.

### 3. AI & Transliteration Layer
- **Google Gemini (`gemini-1.5-flash`)**: High-fidelity translation, transliteration, and grammatical breakdowns via the OpenAI-compatible Gemini endpoint.
- **Rule-based & Archive Local Fallback**: Integrated inside `Web.Api/DataFiles/` (containing `popular_scriptures.json`, `specialized_dictionary.json`, `common_dictionary.json`, and `popular_archive.json`) to guarantee 100% service uptime even without an API key.
- **Transliterator.cs**: Server-side rule-based Devanagari ↔ IAST/ITRANS/Phonetic conversion engine.

---

## 🎨 Design Philosophy & Themes

The application features a premium, scholarly look inspired by classical Sanskrit manuscripts:

- **Palette**: "Vedic Gold & Sand" — rich warm ivory backgrounds (`#FDFBF7`), subtle gold accent boundaries (`#D97706`), deep charcoal texts (`#2D241E`), and spacious cards. Full dark mode support with deep backgrounds (`#0F0E11`, `#16151A`).
- **Typography**: A curated five-font system loaded from Google Fonts:
  - **Cinzel** — Display headings with classical authority
  - **Crimson Pro** — Elegant serif for verse bodies and translations
  - **Noto Serif Devanagari** — Native Devanagari script rendering
  - **Inter** — Clean sans-serif for UI labels and controls
  - **JetBrains Mono** — Monospace for code, metadata, and technical labels
- **Font Size Scaling**: Four accessibility levels (Small, Normal, Large, Vedic/XL) applied globally via CSS utility overrides, configurable from the settings panel.
- **Theme Modes**: Light, Dark, and System-auto detection with localStorage persistence.

---

## 📡 API Endpoints

The ASP.NET Core backend hosts the following REST API endpoints under `/api/*`:

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
| `POST` | `/api/translate` | Bearer | Translates text between Sanskrit, Hindi, and English using Gemini AI |
| `POST` | `/api/transliterate` | Bearer | Converts Indic texts across schemes (Devanagari ↔ IAST, ITRANS, SLP1, Phonetics) |
| `POST` | `/api/analyze` | Bearer | Deep scripture analysis: Sandhi splitting, word-by-word grammar, poetic meter, spiritual significance |

### System
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/health` | None | Returns API health status and runtime mode |

---

## 🔐 Authentication Flow

All API controllers (except `AuthController` and the health endpoint) are protected with `[Authorize]` attributes requiring a valid JWT Bearer token.

### How It Works

1. **Token Acquisition**: The frontend's `apiClient.ts` automatically calls `POST /api/auth/token` with credentials from environment variables (`VITE_CLIENT_ID`, `VITE_CLIENT_SECRET`).
2. **Server Validation**: The `AuthController` validates credentials against values configured in `Web.Api/appsettings.json > AuthSettings`.
3. **Token Issuance**: On success, a JWT token is returned with a configurable expiry (default: 2 hours).
4. **Automatic Injection**: The Axios interceptor in `apiClient.ts` caches the token and automatically attaches `Authorization: Bearer <token>` to all subsequent API requests.
5. **Auto-Renewal**: Tokens are automatically refreshed when they are within 10 seconds of expiry.

### Making Direct API Requests

To call protected endpoints directly (e.g., via Swagger, Postman, or curl):

```bash
# Step 1: Acquire a token
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"clientId":"SanskritQuestClient123","clientSecret":"SuperSecretSanskritQuestKey456!"}'

# Step 2: Use the token
curl http://localhost:5000/api/scriptures \
  -H "Authorization: Bearer <your_jwt_token>"
```

---

## ⚙️ Environment Variables

Create a `.env` file in the project root (see `.env.example` for the template):

| Variable | Required | Description |
|---|---|---|
| `GEMINI_API_KEY` | Recommended | Google Gemini API key for AI-powered translations. Without this, the server runs in offline fallback mode using cached archives. |
| `VITE_CLIENT_ID` | Yes | Client ID for JWT token acquisition. Must match `AuthSettings:ClientId` in `appsettings.json`. |
| `VITE_CLIENT_SECRET` | Yes | Client Secret for JWT token acquisition. Must match `AuthSettings:ClientSecret` in `appsettings.json`. |
| `VITE_API_TARGET` | Optional | Vite dev server proxy target. Defaults to `http://localhost:5000`. Set to `https://localhost:5001` for HTTPS. |

---

## 🚀 Running the Project

### Quick Start (Unified Dev Runner)

The root `package.json` provides unified scripts that start both the backend and frontend concurrently:

```bash
# Install dependencies
npm install                    # Root dependencies (concurrently)
npm install --prefix Web.Client  # Client dependencies

# Configure environment
cp .env.example .env           # Edit with your API keys and credentials

# Start both servers
npm run dev
```

This runs:
- **Backend**: `dotnet watch run --project Web.Api` → `http://localhost:5000` / `https://localhost:5001`
- **Frontend**: Vite dev server → `http://localhost:5173` (proxies `/api/*` to the .NET backend)

### Individual Server Commands

```bash
# Backend only
npm run dev:server
# Or directly:
cd Web.Api && dotnet run

# Frontend only
npm run dev:client
# Or directly:
cd Web.Client && npm run dev
```

### Production Build

```bash
# Build client + publish .NET API
npm run build

# Start production server
npm start
```

### Interactive API Documentation

Once the backend is running, access the Swagger UI at:
- **http://localhost:5000/swagger** — Interactive API explorer with JWT Bearer token support

---

### Docker Containerization (Recommended for Cloud)

You can containerize the **Web.Api** and **Web.Client** together inside a multi-stage Dockerfile. This is the cleanest approach for Azure, Google Cloud (Cloud Run), and AWS.

Here is a recommended production `Dockerfile` to put in your root folder:

```dockerfile
# ==========================================
# STAGE 1: Build React 19 Frontend SPA Node
# ==========================================
FROM node:20-alpine AS client-build
WORKDIR /app
COPY Web.Client/package*.json ./
RUN npm ci
COPY Web.Client/ ./
RUN npm run build

# ==========================================
# STAGE 2: Build ASP.NET Core 8.0 Web API
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /src
COPY Web.Api/Web.Api.csproj ./
RUN dotnet restore
COPY Web.Api/ ./
RUN dotnet publish -c Release -o /app/publish

# ==========================================
# STAGE 3: Single Unified Production Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy dotnet binaries
COPY --from=server-build /app/publish .

# Copy built React files to wwwroot for ASP.NET to serve statically
COPY --from=client-build /app/dist ./wwwroot

ENTRYPOINT ["dotnet", "SanskritQuest.Web.Api.dll"]
```

---

### Deploying to Cloud Providers

#### ☁️ 1. Google Cloud Platform (GCP — Cloud Run)
```bash
# Build the container image
gcloud builds submit --tag gcr.io/your-project-id/sanskritquest-api .

# Deploy to Cloud Run
gcloud run deploy sanskritquest-api \
  --image gcr.io/your-project-id/sanskritquest-api \
  --platform managed \
  --allow-unauthenticated \
  --port 8080
```

#### ☁️ 2. Microsoft Azure (Azure App Service / Container Apps)
- **App Service Linux Code Deployment**: Publish `Web.Api` directly using Visual Studio or VS Code Azure extensions.
- **Azure Container Apps**: Push the Docker image to Azure Container Registry (ACR), then spin up a container app.

#### ☁️ 3. Amazon Web Services (AWS ECS Fargate / Elastic Beanstalk)
- Build and push the multi-stage Docker image to AWS ECR (Elastic Container Registry).
- Launch an AWS ECS Service with AWS Fargate serverless tasks, exposing port `8080`.

---

## 📐 Frontend Components

The React SPA is composed of 14 modular components:

| Component | Purpose |
|---|---|
| `TopBar` | Global navigation header with theme/font controls |
| `SidebarNavigation` | Desktop & mobile sidebar with nav links and settings |
| `PresetScriptures` | Browsable preset verse library (card & list views) |
| `TranslationWorkbench` | Input workspace for entering text and triggering operations |
| `TranslationTab` | Displays bilingual translations (English/Hindi/Sanskrit) |
| `GrammarTab` | Padapatha word-by-word grammatical breakdown |
| `TransliterationTab` | Multi-script transliteration matrix (Devanagari, IAST, ITRANS, SLP1, Phonetic) |
| `DictionaryTab` | Specialized scripture dictionary browser with search and filters |
| `ApiPlayground` | Developer sandbox for direct API endpoint testing |
| `AboutView` | Application information and credits |
| `VedicAlerts` | Dynamic contextual alerts for AI engine status |
| `Footer` | Global themed footer with quick-nav links |
| `Header` | Supplementary header component |
| `SanskritQuestLogo` | SVG logo component |

---

## 📖 Phase 2 Design Reference

The project includes a comprehensive **Phase 2 architecture document** at `PHASE_2_DESIGN.md` covering:
- Frontend architecture evaluation (React vs. Angular — decision to stay with React)
- ASP.NET Core API migration strategy with `Microsoft.Extensions.AI`
- Cascade Fallback (Database-First) & Dynamic Learning Loop
- PostgreSQL + pgvector relational schema with table partitioning
- RAG embedding model evaluation for ancient Sanskrit (recommends Google `text-multilingual-embedding-002`)
- Development effort estimates and cloud hosting cost projections

---

## 🛠️ NuGet Dependencies (Backend)

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.Extensions.AI` | 9.0.1-preview | Provider-agnostic AI abstraction (`IChatClient`) |
| `Microsoft.Extensions.AI.OpenAI` | 9.0.1-preview | OpenAI adapter for Microsoft.Extensions.AI |
| `OpenAI` | 2.1.0 | OpenAI SDK (used for Gemini OpenAI-compatible endpoint) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.8 | JWT Bearer authentication middleware |
| `Swashbuckle.AspNetCore` | 6.6.2 | Swagger / OpenAPI documentation generator |
| `System.IdentityModel.Tokens.Jwt` | 8.0.1 | JWT token creation and validation |
