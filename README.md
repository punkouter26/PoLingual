# PoLingual — Unified Language Playground

A .NET 10 Blazor WebAssembly application that combines **AI-powered rap debates** with **Victorian English translation**, creating a unified language playground.

## Features

### 🎤 Rap Battle Arena
- AI-generated rap debates between famous rappers using Azure OpenAI (GPT-4o)
- Real-time debate streaming via SignalR
- Text-to-speech with distinct voices per rapper (Azure Speech SDK)
- AI judge with scoring and reasoning
- Win/loss leaderboard with Azure Table Storage persistence

### 🎩 Victorian English Translator
- Translates modern English to Victorian-era style using Azure OpenAI
- Song lyrics library for quick translation demos
- Text-to-speech with British Victorian voice (Azure Speech REST API)
- In-memory translation cache for cost optimization

### 🔧 Diagnostics & Health
- `/health` endpoint with comprehensive health checks
- `/diag` page for live service diagnostics
- OpenTelemetry tracing and custom metrics

## Architecture

- **Framework**: .NET 10, Unified Blazor (SSR + WASM)
- **Architecture**: Vertical Slice Architecture (VSA)
- **UI**: Radzen Blazor components
- **Data**: Azure Table Storage (Azurite for local dev)
- **AI**: Azure OpenAI (GPT-4o)
- **Speech**: Azure Speech SDK + REST API
- **Real-time**: SignalR
- **Observability**: Serilog + OpenTelemetry + Application Insights
- **Validation**: FluentValidation
- **API Docs**: Swagger / OpenAPI
- **Tests**: xUnit v3, bUnit, FluentAssertions, Moq, Playwright

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Azurite)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)

## Getting Started

### 1. Start Azurite (Local Storage Emulator)

```bash
docker-compose up -d
```

### 2. Configure User Secrets

```bash
cd src/PoLingual.Web
dotnet user-secrets set "Azure:AzureOpenAIApiKey" "your-key"
dotnet user-secrets set "Azure:AzureSpeechSubscriptionKey" "your-key"
dotnet user-secrets set "NewsApi:ApiKey" "your-newsapi-key"
```

### 3. Run the Application

```bash
dotnet run --project src/PoLingual.Web
```

Navigate to `https://localhost:7199`

### 4. Run Tests

```bash
# Unit tests
dotnet test tests/PoLingual.UnitTests

# Integration tests (requires Azurite)
dotnet test tests/PoLingual.IntegrationTests

# E2E tests (requires running app)
cd tests/PoLingual.E2ETests
npm install
npx playwright install
npm test
```

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/rappers` | List all rappers |
| GET | `/api/topics` | Get debate topics |
| GET | `/api/news/headlines` | News headlines |
| GET | `/api/debate/state` | Current debate state |
| POST | `/api/debate/reset` | Reset debate |
| POST | `/api/translation` | Translate to Victorian |
| GET | `/api/lyrics/songs` | Available songs |
| GET | `/api/lyrics/random` | Random lyrics |
| POST | `/api/speech/synthesize` | Text to speech |
| GET | `/api/diagnostics` | Run diagnostics |
| GET | `/health` | Health check |

## Deployment

```bash
azd up
```

## Project Structure

```
PoLingual/
├── src/
│   ├── PoLingual.Shared/       # Shared models
│   ├── PoLingual.Web/          # Blazor Server + API
│   └── PoLingual.Web.Client/   # Blazor WASM Client
├── tests/
│   ├── PoLingual.UnitTests/
│   ├── PoLingual.IntegrationTests/
│   └── PoLingual.E2ETests/
└── infra/                      # Bicep templates
```
