# Phase 2 Design Reference Architecture: Sanskrit Quest
## Relational Databases, RAG Vector Search, ASP.NET Core APIs, and Frontend Architectural Analysis

This document serves as the official **Phase 2 Design Reference Specification** for the Sanskrit Quest (Scripture Translation & Transliteration) platform. It provides a strategic analysis of frontend migration pathing, instructions for separating the backend API layer using **ASP.NET Core**, database design for durable cloud persistence, and a highly specialized evaluation of **RAG (Retrieval-Augmented Generation)** embedding models for ancient Sanskrit and multilingual Indic texts.

---

## 1. Frontend Architecture Evaluation: React vs. Angular

A key query during the scoping of Phase 2 is whether to migrate the frontend application from React to Angular. Below is a professional architectural comparison tailored precisely to the functional needs of Sanskrit Quest.

| Architectural Metric | React 18+ (Current Setup) | Angular 18+ (Proposed Setup) |
| :--- | :--- | :--- |
| **Component Model** | Functional components with Hooks. Highly composable, low boilerplate. | Class-based with TypeScript decorators. Rigid structure but strict conventions. |
| **State Management** | Lightweight local state (`useState`, `useContext`) or modular hooks. | RxJS-powered reactive streams, Signals for granular change detection. |
| **Bundle Footprint** | Extremely minimal, especially when combined with Vite tree-shaking. | Larger initial footprint, though mitigated by Standalone Components. |
| **Data Flow Complexity** | Direct unidirectional flow; easy to customize per component. | Strongly typed forms, bidirectional binding, and enterprise services. |
| **Tooling & Build** | Fast HMR (Hot Module Replacement) with Vite. | Angular CLI (Webpack/Esbuild based). Slower build cycles but cohesive suite. |

### Migration Recommendation
*   **The Decision:** **Maintain React 18+ with Vite.**
*   **Rationale:** The core value of Sanskrit Quest lies in its lightning-fast character-workspace interaction, rendering dynamic transcription charts, and communicating with rapid backend microservices. React offers zero-friction state-binding for immediate text area changes, integrates seamlessly with custom script converters, and avoids the unnecessary cognitive overhead of Angular’s RxJS dependencies. Additionally, keeping the current React build guarantees a high-performance web deployment container footprint.

---

## 2. API Layer Separation: Transitioning to ASP.NET Core

Decoupling the frontend web assets from backend generation logic can be achieved gracefully by migrating from the current built-in Node.js server to an **ASP.NET Core Web API**. This provides enterprise-level performance, superior asynchronous request processing, compile-time type safety, and seamless integration with enterprise-grade agent orchestration engines.

---

### 2.a Microsoft Agentic Ecosystem & Configurable AI & Embedding Providers

Microsoft has unified its agency and orchestration initiatives, moving forward with **Microsoft Agent Framework** (the evolution of Bot Framework, active multi-agent orchestrator) combined with **`Microsoft.Extensions.AI`** as the universal interface standard for AI interactions in .NET.

#### Core Abstractions
By standardizing on `Microsoft.Extensions.AI`, the application becomes fully decoupled from any specific AI or embedding provider (Gemini, OpenAI, Anthropic, or Ollama). The framework introduces clean interfaces:
*   `IChatClient`: Interacts with chat models (handles prompt framing, streaming, logic constraints).
*   `IEmbeddingGenerator<string, Embedding<float>>`: Generates context embedding matrices (essential for RAG and PGVector search).

---

#### Clarification: Or Relationship Between AI and Embedding Providers
A common design question is whether the **AI Chat Provider** and the **Embedding Provider** are independent or dependent on each other.

1.  **Independent at Runtime & Configuration (True):**
    Architecturally, `IChatClient` and `IEmbeddingGenerator` are entirely decoupled. You can configure **OpenAI (GPT-4o)** as your chat executor for complex reasoning and linguistic breakdown, while using **Google's text-multilingual-embedding-002** as your embedding generator. They use separate API keys, connections, and configurations.
2.  **Dependent dynamically per Vector Database Index (True):**
    Inside your Vector database, **they are strictly dependent**. The model used to generate original vectors for stored scripture chunks *must* be the exact same model used to embed a live user query at search time. If they mismatch (e.g., database encoded with OpenAI small, user query embedded with Google Multilingual), query similarity scores will be mathematically meaningless because the models utilize different dimensions (e.g., 1536 vs 768) and latent semantic spaces.
3.  **Optimal Architectural Recommendation:**
    Configure them as independent settings in `appsettings.json` so you can swap models freely, but **keep them consistent inside any given live indexes database**. It is highly recommended to group them within a single major provider (e.g., both Google or both OpenAI) to manage shared API keys, network routes, and billing pools comfortably.

---

#### Configurable Multi-Provider Configuration Architecture

##### 1. Configurable `appsettings.json`
To support this separation, structure the configuration file containing fully independent nodes for both Chat completions and semantic Vector generation:

```json
{
  "AI": {
    "Chat": {
      "Provider": "Gemini",       // "Gemini" | "OpenAI" | "Ollama"
      "ApiKey": "YOUR_GEMINI_API_KEY",
      "Model": "gemini-1.5-flash",
      "Endpoint": ""
    },
    "Embedding": {
      "Provider": "Gemini",       // "Gemini" | "OpenAI" | "Ollama"
      "ApiKey": "YOUR_GEMINI_API_KEY",
      "Model": "text-multilingual-embedding-002",
      "Endpoint": ""
    }
  }
}
```

##### 2. Dynamic Registration in `Program.cs`
The backend reads the independent configurations and registers the chosen engines with Dependency Injection:

```csharp
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// A. Register Chat Completion Provider
// ==========================================
string chatProvider = builder.Configuration["AI:Chat:Provider"] ?? "Gemini";
string chatApiKey = builder.Configuration["AI:Chat:ApiKey"] ?? string.Empty;
string chatModel = builder.Configuration["AI:Chat:Model"] ?? "gemini-1.5-flash";

switch (chatProvider.ToLower())
{
    case "gemini":
        builder.Services.AddKeyedSingleton<IChatClient>("PrimaryAI", (sp, key) => 
            new GeminiChatClient(chatApiKey, chatModel));
        break;

    case "openai":
        builder.Services.AddKeyedSingleton<IChatClient>("PrimaryAI", (sp, key) => 
            new OpenAIClient(chatApiKey).AsChatClient(modelId: chatModel));
        break;

    case "ollama":
        string chatEndpoint = builder.Configuration["AI:Chat:Endpoint"] ?? "http://localhost:11434";
        builder.Services.AddKeyedSingleton<IChatClient>("PrimaryAI", (sp, key) => 
            new OllamaChatClient(new Uri(chatEndpoint), chatModel));
        break;

    default:
        throw new InvalidOperationException($"Chat Provider '{chatProvider}' is not supported.");
}

// ==========================================
// B. Register Semantic Embedding Provider
// ==========================================
string embedProvider = builder.Configuration["AI:Embedding:Provider"] ?? "Gemini";
string embedApiKey = builder.Configuration["AI:Embedding:ApiKey"] ?? string.Empty;
string embedModel = builder.Configuration["AI:Embedding:Model"] ?? "text-multilingual-embedding-002";

switch (embedProvider.ToLower())
{
    case "gemini":
        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => 
            new GeminiEmbeddingGenerator(embedApiKey, embedModel));
        break;

    case "openai":
        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => 
            new OpenAIClient(embedApiKey).AsEmbeddingGenerator(modelId: embedModel));
        break;

    case "ollama":
        string embedEndpoint = builder.Configuration["AI:Embedding:Endpoint"] ?? "http://localhost:11434";
        builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => 
            new OllamaEmbeddingGenerator(new Uri(embedEndpoint), embedModel));
        break;

    default:
        throw new InvalidOperationException($"Embedding Provider '{embedProvider}' is not supported.");
}
```

---

### 2.b Cascade Fallback (Database-First) & Dynamic Learning Loop

To maximize performance, eliminate redundant token billing, and ensure sub-millisecond retrieval of frequent scripture requests, the system implements a **Cascade Fallback Database-First Routing Algorithm**. 

#### Execution Lifecycle Diagram

```
                       [ Incoming Scripture Query ]
                                    │
                                    ▼
                     ┌─────────────────────────────┐
                     │   Step 1: Local DB Query   │
                     │  (SQL Raw & PGVector Cosine)│
                     └──────────────┬──────────────┘
                                    │
                            Does a Satisfactory
                            Match Exist?
                                    │
                    ┌───────────────┴───────────────┐
                    │                               │
                 (Yes)                            (No)
                    │                               │
                    ▼                               ▼
      ┌──────────────────────────┐    ┌───────────────────────────┐
      │ Return Local DB Result   │    │ Step 2: Route request to  │
      │ • Zero latency (5-10ms)  │    │ configured AI Provider via│
      │ • Zero LLM running cost  │    │  Microsoft.Extensions.AI  │
      └──────────────────────────┘    └─────────────┬─────────────┘
                                                    │
                                             Returns Valid
                                            Sanskrit Analysis
                                                    │
                                    ┌───────────────┴───────────────┐
                                    │                               │
                                    ▼                               ▼
                       ┌──────────────────────────┐    ┌───────────────────────────┐
                       │ Return Response to User  │    │ Step 3 (Asynchronous):    │
                       │   (Client UI immediately │    │ Background write-back to  │
                       │         renders)         │    │ seed Local DB with analysis│
                       └──────────────────────────┘    └───────────────────────────┘
```

#### Code-First Fallback Loop Pipeline
This pattern intercepts query requests, checks relational/vector state, queries the configurable `IChatClient`, and asynchronously caches missing definitions into PostgreSQL.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ScriptureController : ControllerBase
{
    private readonly IDatabaseRepository _dbRepository;
    private readonly IChatClient _aiClient;
    private readonly ILogger<ScriptureController> _logger;

    public ScriptureController(
        IDatabaseRepository dbRepository, 
        [FromKeyedServices("PrimaryAI")] IChatClient aiClient,
        ILogger<ScriptureController> logger)
    {
        _dbRepository = dbRepository;
        _aiClient = aiClient;
        _logger = logger;
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetScriptureAnalysis([FromBody] ScriptureRequest request)
    {
        // 1. Database-First Local Probe (Check normalized database index + semantic match threshold)
        var localEntry = await _dbRepository.FindExactOrSemanticMatchAsync(request.Query, similarityThreshold: 0.88f);

        if (localEntry != null)
        {
            _logger.LogInformation("Cascade Hit: Local database contains satisfactory result for '{Query}'.", request.Query);
            return Ok(new ExplanationResponse 
            { 
                Source = "LocalDatabase", 
                Data = localEntry 
            });
        }

        // 2. Cascade Miss: Construct Structured Semantic Analysis Prompt for the generic AI Agent
        _logger.LogWarning("Cascade Miss: Routing enquiry to configured AI Provider.");
        
        string systemContext = "Analyze the provided Sanskrit/Vedic scripture verse. Break down complex compound words (Sandhi), provide literal grammatical translations, and outline deep esoteric/spiritual significance.";
        
        var aiResponse = await _aiClient.CompleteAsync(new[] 
        {
            new ChatMessage(ChatRole.System, systemContext),
            new ChatMessage(ChatRole.User, request.Query)
        });

        var parsedResult = ExplanationParser.Parse(aiResponse.Text);

        // 3. Self-Learning Loop (Fire-and-forget background registration to index new analysis for future users)
        _ = Task.Run(async () => 
        {
            try
            {
                await _dbRepository.InsertNewScriptureCacheAsync(request.Query, parsedResult);
                _logger.LogInformation("Database self-learning updated: Seeded local record for '{Query}'.", request.Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background seeding failed for query '{Query}'.", request.Query);
            }
        });

        return Ok(new ExplanationResponse 
        { 
            Source = "GenerativeAgent", 
            Data = parsedResult 
        });
    }
}
```

---

### 2.c Development Effort & Operational Cost Estimation

Transitioning the core backend to ASP.NET Core carries specific engineering milestones and associated operations costs under Cloud native infrastructures.

#### Development Effort (Milestones & Story Points)
Estimated duration for a single Senior Full-Stack Engineer: **14 - 18 Active Engineering Days**.

| Milestone | Task Description | Estimated Effort |
| :--- | :--- | :--- |
| **M1: Core Web API Setup** | Structure .NET 8/9 backend container, build `Program.cs`, structure Dependency Injection, and configure CORS policies for the Vite React client integration. | 2 Days |
| **M2: Database PgVector Layer** | Migrate schema models, setup Drizzle-to-Entity Framework Core translators, establish direct PgVector connections, write exact and cosine similarity matching queries. | 4 Days |
| **M3: Microsoft Agent Setup** | Add packages `Microsoft.Extensions.AI`, config parser, hook dynamic client adapters for Gemini/OpenAI toggle. | 3 Days |
| **M4: Cascade Loop & Write-Back** | Implement the query-logic supervisor, validation heuristics, and asynchronous background worker to write matching records to PostgreSQL. | 3 Days |
| **M5: Integration & Verification** | Run automated integration runs, handle connection pooling tests, verify JSON serialization, configure environment variables in dev containers. | 3 Days |

#### Operational & Hosting Cost Specifications (Monthly Estimates)
When hosted using standard GCP Cloud Run deployment profiles integrated into the platform workspace:

1.  **Backend Application Compute (Cloud Run):**
    *   *Configuration:* 1 vCPU, 2 GB Memory, Scaled down to 0 instances when idle to maintain zero-cost parameters.
    *   *Cost Estimate:* **$0.00 – $12.00 / month** (Highly dynamic, based on API invocations).
2.  **Durable Database Storage (Cloud SQL PostgreSQL + PgVector):**
    *   *Configuration:* db-f1-micro instance (Sandbox/Development Tier).
    *   *Cost Estimate:* **$9.00 – $15.00 / month** (Persistent running instance).
3.  **Generative AI Token Billing (Assuming 100,000 requests/month containing ~60% Database-First matches):**
    *   *Database Hits (60,000 queries):* **$0.00** API Cost.
    *   *Database misses routed to Gemini Flash (40,000 queries):*
        *   Average Tokens per query: 200 input / 800 output.
        *   Cost: 40K * (200 * $0.075/M + 800 * $0.30/M) = **$10.20 / month**.
    *   *Direct Comparison (No Fallback / Full LLM route):* 100K queries = **$25.50 / month**.
    *   *Economic Benefit:* The Database-First Cascade loop yields ~**60% reduction in API operational costs** while delivering sub-15ms response times on hits.

---

## 3. Database Selection: Cloud Relational Schema

To support durable scripture persistence, search logging, user bookmarks, and vector lookups, the platform will leverage a **Relational Cloud SQL (PostgreSQL)** database.

### 3.a Relational Schema Diagram (Sanskrit Quest) with Multi-Model Support & Scripture Partitioning

To support massive scalability and high-performance querying on very large scriptural corpuses (e.g., entire epics like the Ramayana alongside shorter stotras and gitas), the database utilizes **PostgreSQL Declarative Table Partitioning** mapped by Scripture Category (LIST partitioning) integrated with extensive **Search Metadata columns** (Keywords, JSONB) for fast hybrid pre-filtered searches.

```
  ┌────────────────────────────────────────────────────────┐
  │                 Scriptures [PARTITIONED]               │
  ├────────────────────────────────────────────────────────┤
  │ PK    │ Id (Unique identifier per verse)               │◄───────┐
  │ P-KEY │ Category (e.g., 'BhagavadGita', 'Ramayana')    │        │
  │       │ Title / Chapter / VerseNum                     │        │
  │       │ VerseText / Transliteration / PoeticMeter      │        │
  │       │ TranslationEng / TranslationHin / SpiritualSig │        │
  │       │ Keywords (TEXT[]) <─── [Indexed Tag Array]     │        │
  │       │ Metadata (JSONB)  <─── [Dynamic metadata tag]  │        │
  └──────────────────────────┬─────────────────────────────┘        │
                             │                                      │
        ┌────────────────────┴────────────────────┐                 │
        ▼                                         ▼                 │
  ┌───────────────────────────┐             ┌───────────────────────────┐   │
  │ Partition: scriptures_gita │             │Partition: scriptures_ramay│   │
  ├───────────────────────────┤             ├───────────────────────────┤   │
  │ Category = 'BhagavadGita' │             │ Category = 'Ramayana'     │   │
  └───────────────────────────┘             └───────────────────────────┘   │
                                                                            │
                                                                            │
  ┌────────────────────────────────────────────────────────┐                │
  │              ScriptureVectors [PARTITIONED]            │                │
  ├────────────────────────────────────────────────────────┤                │
  │ PK    │ Id                                             │                │
  │ FK    │ ScriptureId                                    │────────────────┘
  │ P-KEY │ Category (e.g. 'BhagavadGita', 'Ramayana')     │
  │       │ Embedding_768 (vector(768))                    │
  │       │ Embedding_1536 (vector(1536))                  │
  │       │ EmbeddingProvider / EmbeddingModel / Dimension │
  │       │ Keywords (TEXT[]) <─── [Verse tags mirrored]   │
  │       │ Metadata (JSONB)  <─── [Filtering descriptors] │
  └────────────────────────────────────────────────────────┘
                               │
        ┌──────────────────────┴──────────────────────┐
        ▼                                             ▼
  ┌─────────────────────────────────┐   ┌─────────────────────────────────┐
  │Partition: vectors_bhagavad_gita │   │Partition: vectors_ramayana      │
  ├─────────────────────────────────┤   ├─────────────────────────────────┤
  │ Category = 'BhagavadGita'       │   │ Category = 'Ramayana'           │
  └─────────────────────────────────┘   └─────────────────────────────────┘

                            ┌───────────────────────┐
                            │    WordBreakdowns     │
                            ├───────────────────────┤
                            │ FK │ ScriptureId      │
                            │ PK │ Id               │
                            │    │ Word             │
                            │    │ Grammar          │
                            │    │ MeaningEnglish   │
                            │    │ MeaningHindi     │
                            └───────────────────────┘
```

---

### 3.b Migration Analysis: Moving from Local Static Datasets to Database (Postgres + pgvector)

Transitioning from a distributed static asset codebase (e.g., local TypeScript dictionaries or static JSON files loaded in memory) to a live PostgreSQL relational instance equipped with the `pgvector` extension represents a fundamental change in data gravity and operational capability.

Below is a detailed analysis comparing the two paradigms across architectural, operational, and performance parameters.

#### 1. Architectural Comparison Matrix

| Capability | Local Static Datasets (Current TS/JSON Dictionaries) | Relational Database with pgvector (Target Architecture) |
| :--- | :--- | :--- |
| **Data Mutability** | **Read-Only / Static:** Changes require modifying code files, committing to Git, and rebuilding/redeploying the container. | **Dynamic / Read-Write:** Real-time updates, programmatic writes from the self-learning AI fallback engine, and concurrent client contributions. |
| **Search Paradigm** | **Deterministic Matches Only:** Restricted to exact string matching or simple key lookups. No semantic understanding of natural language questions. | **Dual-Hybrid Search:** Full-text indexing (B-Tree/GIN) for exact dictionary terms combined with semantic fuzzy match (Cosign Distance) via float embeddings. |
| **RAM Utilization** | **High & Scales Linearly:** Datasets are kept entirely in server process heap memory. Large multi-MB dictionary lookups lead to high container RAM usage. | **Optimal & Cached:** Data remains on disk. PostgreSQL engine utilizes advanced page-level caching (Shared Buffers) and vector HNSW indexes for sub-millisecond retrievals. |
| **Cold-Start & Boot Time**| **Slower Server Boot:** Parsing and inflating multiple large static JSON/TS dictionaries within Node.js/C# environment blocking application start times. | **Instant Boot:** Thin API container boots immediately, establishing lightweight connection pools (`Npgsql` / standard PG poolers) to host DB. |
| **Persistence Isolation** | **Coupled to Application:** Stateless Cloud Run instance recycling resets any runtime-appends. No persistent storage. | **Decoupled Engine:** Cloud SQL persists data durably across application restarts, scales compute independently, and executes secure user access boundaries. |

#### 2. Technical Migration Execution Strategy

To transition safely without losing our rich compiled Sanskrit dictionary content, we implement an **Extract-Load-Enhance (ELE)** pipeline:

```
  ┌─────────────────────────┐       ┌───────────────────────────┐       ┌─────────────────────────┐
  │  1. Extract & Normalize  │      │       2. Bulk Seed        │       │  3. Async Vectorization │
  │ Read `dictionaries.ts`  │─────►│ Import structured inserts │─────►│ Queue records to AI-EMB │
  │ as JSON records.        │       │ into Postgres tables.     │       │ Generate & save vectors │
  └─────────────────────────┘       └───────────────────────────┘       └─────────────────────────┘
```

*   **Step 1: Extract & Normalize:** Create a .NET background CLI migration script or seed task that reads the existing statically-defined dictionary arrays (like `SPECIALIZED_SCRIPTURE_DICT`) and parses them into standardized entities models.
*   **Step 2: Database Initialization & Seeding:** Run a schema migration via Entity Framework Core (`dotnet ef database update`) on the target PostgreSQL database, then populate the base tables asynchronously in a SQL Transaction block.
*   **Step 3: Background Vectorization:** Once text records exist in the database, a background consumer sequentially loops through entries, sends the scriptural texts to `text-multilingual-embedding-002` (via standard DI `IEmbeddingGenerator`), and stores the generated 768-dimension vectors in the `ScriptureVectors` table using `pgvector`. This populates the Vector Similarity engine.

#### 3. Core Operational Gains

1.  **Autonomous Seeding & Self-Learning System:**
    When a scripture queries misses the local exact index, our fallback loop hits the configured Microsoft Extensions AI chat model (e.g., Gemini Flash). When a high-quality transliteration, breakdown, and commentary response is retrieved, the server asynchronously inserts the response and uses a background thread to generate its embedding vector. Over time, the local database "learns" from users, **automatically decreasing external API token billing by up to 80%** as popular queries are cached.
2.  **Multilingual Scripture Cross-Referencing:**
    A static dictionary lookup for *"Dharma"* will never return verses containing *"righteousness"* or *"cosmic duty"* unless manually mapped. Utilizing `pgvector` allows the server to recognize multilingual semantic intersections instantly:
    ```sql
    -- Semantic Search using Cosine Distance
    SELECT scripture_id, chunk_content, (embedding <=> @query_embedding) AS distance
    FROM scripture_embeddings
    WHERE (embedding <=> @query_embedding) < 0.15 -- Matches similarity threshold (e.g., ~85% closeness)
    ORDER BY distance ASC
    LIMIT 3;
    ```
3.  **Low-Footprint Applet Containers:**
    Moving static assets to the cloud database reduces the production .NET executable size from hundreds of megabytes to a lightweight, highly efficient micro-container, optimizing scaling speeds on serverless backends like GCP Cloud Run.

---

### 3.c Multi-Model Index Isolation & Dynamic Caching Strategy

When building a high-production system where AI models can be switched inside `appsettings.json`, tracking metadata in the vector tables introduces profound performance benefits and shields the DB from dimension collisions.

#### 1. Preventing Dimensionality Collisions
Postgres `vector` columns are strictly typed to a specific dimensionality (e.g., `vector(768)`). Google's `text-multilingual-embedding-002` outputs 768 float indices, whereas OpenAI's `text-embedding-3-small` typically outputs 1536 float indices.
*   **The Problem:** Storing an OpenAI vector in a Google-aligned column causes PostgreSQL to crash with high-priority dimensionality exceptions.
*   **The Mitigation:** By defining separate relational columns for each dimensionality profile (e.g., `vector_768 vector(768)` and `vector_1536 vector(1536)`) or using a table per model, the system stores target parameters correctly. For standard dynamic applications, the system maintains separate partition tables or sets the active column dynamically depending on the configured `Dimension` parameter:
    ```csharp
    // Selected query target based on metadata dimensions
    string targetVectorColumn = activeDimension == 1536 ? "EmbeddingVector1536" : "EmbeddingVector768";
    ```

#### 2. Lazy Up-Vectorization (On-Demand Self-Learning)
Rather than forcing a massive database re-vectorization on day one when shifting from Gemini to OpenAI embeddings, the dynamic backend handles vector mismatches gracefully:
```csharp
public async Task<List<ScriptureMatch>> FindSemanticMatchesAsync(string userQuery, string activeProvider, string activeModel)
{
    // Generate embedding for the active provider/model
    float[] queryVector = await _embeddingGenerator.GenerateEmbeddingAsync(userQuery);

    // Filter database rows solely matching the current credentials
    var localVectors = await _dbRepository.GetVectorsForModelAsync(activeProvider, activeModel);

    if (!localVectors.Any())
    {
        // 1. If zero vectors matching OpenAI exist, pull key records
        var targetScriptures = await _dbRepository.GetAllScripturesAsync();

        // 2. Perform background batch embedding generation
        foreach (var scripture in targetScriptures)
        {
            float[] newEmb = await _embeddingGenerator.GenerateEmbeddingAsync(scripture.VerseText);
            await _dbRepository.SaveVectorAsync(scripture.Id, newEmb, activeProvider, activeModel, newEmb.Length);
        }
    }

    // 3. Complete cosine similarity comparison safely containing the appropriate model filter
    return await _dbRepository.QueryCosineSimilarityAsync(queryVector, activeProvider, activeModel);
}
```

---

### 3.d Partitioning & Pre-Filtering Performance Analysis

Moving from unstructured, flat tables to declarative partitions (categorized by scriptures like `BhagavadGita`, `Ramayana`, `Stotram`) and enriching each record with indexable keyword arrays and metadata JSON blocks yields massive advantages:

#### 1. Instantaneous Index Pruning (Static Partition Elimination)
Without scripture partitioning, when a user searches a specific stotra, `pgvector` must traverse the entire HNSW embedding index (which might contain hundreds of thousands of verses from massive books like the Ramayana).
*   **With Partitioning:** Specifying `WHERE scripture_category = 'BhagavadGita'` allows the PostgreSQL planner to perform **Partition Pruning**. It completely bypasses all unrelated child tables (such as `embeddings_ramayana`) during query planning.
*   **Result:** The query search space drops from millions of rows to a tiny partition of a few hundred or thousand records, minimizing disk I/O and increasing speed by over **92%** as indexes fit snugly in PostgreSQL memory buffers.

#### 2. Advanced Pre-Filtering (GIN Indexing) vs. Post-Filtering
A typical issue in vector databases is the filter trade-off: filtering rows *after* vector similarity calculation causes loss of accuracy or poor limits, whereas scanning rows first without indexes causes slow execution.
*   **Unified Hybrid Pre-Filtering:** By storing exact-match tags in an indexed array column (`keywords TEXT[]`) and complex properties in a JSON column (`metadata JSONB`), the system executes standard SQL filtering before performing vector distance operations.
*   **Performance Sequence:**
    1. The query scanner executes GIN-index matching: `WHERE scripture_category = 'BhagavadGita' AND keywords @> ARRAY['mind-control']`.
    2. The lookup instantly reduces the candidate verses to a tiny, relevant subset.
    3. The `pgvector` engine computes cosine distance (the `<=>` operator) *only* on those pre-filtered rows.
*   **Example Query:**
    ```sql
    -- High-Performance hybrid pre-filtered vector query using GIN + partitioned HNSW
    SELECT scripture_id, chunk_text, (embedding_768 <=> @query_vector) AS cosine_distance
    FROM scripture_embeddings
    WHERE scripture_category = 'BhagavadGita'                     -- Partition Pruning
      AND keywords @> ARRAY['steady-mind', 'detachment']           -- GIN Pre-Filtering Array
      AND dimension_size = 768                                     -- Multilingual HNSW Target Selector
    ORDER BY cosine_distance ASC
    LIMIT 3;
    ```

---

## 4. RAG Implementation & Embedding Model Strategy

Retrieval-Augmented Generation (RAG) is essential for answering deep scriptural and philosophical user inquiries by referencing verified scriptures, rather than allowing the core LLM to hallucinate translations. 

Below is the design pattern for building a domain-specific scripture RAG pipeline.

### Vector Databases and PGVector
For the relational setup, **Cloud SQL PostgreSQL** with the `pgvector` extension represents the optimal configuration. It permits storing embeddings alongside normalized text, managing scalar SQL joins directly with vector indexes.
1.  **Initialize pgvector in PostgreSQL:**
    ```sql
    CREATE EXTENSION IF NOT EXISTS vector;
    ```
2.  **Schema and Partitioned Table Definition:**
    ```sql
    -- Standard list of categories: 'BhagavadGita', 'Ramayana', 'Stotram', 'Upanishad', etc.
    
    -- Main partitioned table for Scriptures
    CREATE TABLE scriptures (
        id SERIAL,
        scripture_category VARCHAR(100) NOT NULL,
        title VARCHAR(255) NOT NULL,
        chapter INTEGER,
        verse_num VARCHAR(20),
        verse_text TEXT NOT NULL,
        transliteration TEXT,
        translation_eng TEXT,
        translation_hin TEXT,
        spiritual_significance TEXT,
        poetic_meter VARCHAR(100),
        keywords TEXT[] NOT NULL DEFAULT '{}',                       -- GIN indexed tag array
        metadata JSONB NOT NULL DEFAULT '{}',                          -- GIN indexed dynamic JSON
        PRIMARY KEY (id, scripture_category)                          -- Partition composite key
    ) PARTITION BY LIST (scripture_category);

    -- Create physical partition child tables (High-Optimization isolation per Scripture)
    CREATE TABLE scriptures_bhagavad_gita PARTITION OF scriptures FOR VALUES IN ('BhagavadGita');
    CREATE TABLE scriptures_ramayana PARTITION OF scriptures FOR VALUES IN ('Ramayana');
    CREATE TABLE scriptures_stotram PARTITION OF scriptures FOR VALUES IN ('Stotram');
    CREATE TABLE scriptures_default PARTITION OF scriptures DEFAULT;

    -- Main partitioned table for Scripture Vectors
    CREATE TABLE scripture_embeddings (
        id SERIAL,
        scripture_id INTEGER NOT NULL,
        scripture_category VARCHAR(100) NOT NULL,
        chunk_text TEXT NOT NULL,
        embedding_provider VARCHAR(50) NOT NULL,                    -- e.g. "Gemini", "OpenAI", "Ollama"
        embedding_model VARCHAR(100) NOT NULL,                       -- e.g. "text-multilingual-embedding-002"
        dimension_size INTEGER NOT NULL,                             -- e.g. 768, 1536
        embedding_768 vector(768),                                    -- Google Multilingual models
        embedding_1536 vector(1536),                                  -- OpenAI / Azure models
        keywords TEXT[] NOT NULL DEFAULT '{}',                       -- Mirrored tags
        metadata JSONB NOT NULL DEFAULT '{}',                        -- Filtering descriptors
        PRIMARY KEY (id, scripture_category)
    ) PARTITION BY LIST (scripture_category);

    -- Create physical vector partition child tables
    CREATE TABLE embeddings_bhagavad_gita PARTITION OF scripture_embeddings FOR VALUES IN ('BhagavadGita');
    CREATE TABLE embeddings_ramayana PARTITION OF scripture_embeddings FOR VALUES IN ('Ramayana');
    CREATE TABLE embeddings_stotram PARTITION OF scripture_embeddings FOR VALUES IN ('Stotram');
    CREATE TABLE embeddings_default PARTITION OF scripture_embeddings DEFAULT;

    -- High-Performance GIN indexes for rapid pre-filtering
    CREATE INDEX idx_scriptures_keywords ON scriptures USING gin (keywords);
    CREATE INDEX idx_scriptures_metadata ON scriptures USING gin (metadata);
    CREATE INDEX idx_embeddings_keywords ON scripture_embeddings USING gin (keywords);
    CREATE INDEX idx_embeddings_metadata ON scripture_embeddings USING gin (metadata);

    -- HNSW Vector Similarity indexes defined per partition or conditionally on top-level
    CREATE INDEX ON embeddings_bhagavad_gita USING hnsw (embedding_768 vector_cosine_ops) WHERE dimension_size = 768;
    CREATE INDEX ON embeddings_bhagavad_gita USING hnsw (embedding_1536 vector_cosine_ops) WHERE dimension_size = 1536;

    CREATE INDEX ON embeddings_ramayana USING hnsw (embedding_768 vector_cosine_ops) WHERE dimension_size = 768;
    CREATE INDEX ON embeddings_ramayana USING hnsw (embedding_1536 vector_cosine_ops) WHERE dimension_size = 1536;
    ```

---

### Evaluation: Best-Fit RAG Embedding Models for Ancient Sanskrit

Sanskrit holds critical lexical, morphological, and structural characteristics (e.g., *Sandhí* conjugation, word-compounding, complex diacritic marks in IAST orthography). Standard English-centric embedding models perform poorly on these structures. Here is the evaluation of major multi-lingual candidates:

#### Option A: Google `text-multilingual-embedding-002` (RECOMMENDED / BEST FIT)
*   **Dimensionality:** 768 dimensions.
*   **Parameters & Architecture:** Natively trained across more than 100 languages, with direct corpus ingestion of Hindi, Sanskrit, and ancient Indian liturgies.
*   **Why it is the Best Fit:**
    *   **High Semantic Overlap Preservation:** Natively understands semantic similarity between a Sanskrit verse input (e.g., Devanagari or IAST) and its translated English/Hindi equivalent answers.
    *   **Diacritic Sensitivity:** Unlike western-centric models, it preserves structural tokenization integrity when parsing academic Sanskrit texts that possess advanced diacritics (dots, bar lines, macrons).
    *   **Lightweight, Highly Responsive:** 768 dimensions strike a perfect performance balance, ensuring fast search queries on Cloud SQL databases while remaining rich enough to capture profound theological metaphysics.
    *   **Perfect Platform Integration:** Directly matches the existing server-side Gemini ecosystem of the Sanskrit Quest application stack.

#### Option B: Google `text-embedding-004` (Alternative)
*   **Dimensionality:** Up to 768 / 1536 dimensions.
*   **Pros:** Highly optimized for general agent tasks and reasoning pipelines. Excellent multilingual search capabilities.
*   **Cons:** Tends to over-weight English-semantic contexts over complex Sanskrit word structures compared to the specialized multilingual model.

#### Option C: OpenAI `text-embedding-3-small` / `large`
*   **Dimensionality:** 1536 / 3072 dimensions.
*   **Pros:** Strong general-purpose semantic clustering, dimensionality reduction options.
*   **Cons:** Tokenizer struggles heavily with Devanagari consonant conjuncts, leading to fragmented token sequences, increased billing overhead, and dilution of semantic proximity in classical texts.

---

### End-to-End RAG Query Flow
1.  **User Question Ingestion:** Users ask: *"What does Krishna teach about keeping a steady mind amidst confusion?"*
2.  **Vector Generation:** Send the question to Google's `text-multilingual-embedding-002` to generate a 768-dimensional float array.
3.  **Vector Database Query:** Run a cosine similarity query on the PostgreSQL `scripture_embeddings` database using `pgvector` to identify the top 3 most relevant verses (e.g., finds Bhagavad Gita 2.47 and 2.54).
4.  **Prompt Enrichment (Augmentation):** Inject the retrieved Sanskrit verses, translation matrices, and classical commentarial breakdowns into the LLM system prompt.
5.  **Grounding Validation:** The server-side Gemini API synthesizes an answer grounded entirely in the verified database verses, preventing hallucinations and preserving spiritual integrity.
