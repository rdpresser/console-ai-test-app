# ConsoleAppAITest

Simple .NET console app that uses Microsoft Semantic Kernel to call a local Ollama model.

## What This Project Does

- Builds a Semantic Kernel instance
- Connects to an Ollama endpoint
- Sends a sample prompt to model llama3
- Prints the model response in the console

## Project Basics

- Framework: .NET 10.0
- Entry point: ConsoleAppAITest/Program.cs
- Solution: ConsoleAppAITest.slnx

## Prerequisites

1. .NET SDK 10+
2. Ollama local service available at port 11434
3. Ollama model available (example: llama3)

## Configuration

The app reads these environment variables:

- OLLAMA_ENDPOINT
- OLLAMA_MODEL

Defaults used by the app:

- Endpoint: http://127.0.0.1:11434
- Default model: llama3

## Run

Use endpoint: http://127.0.0.1:11434

Command:

~~~powershell
dotnet run --project .\ConsoleAppAITest\ConsoleAppAITest.csproj
~~~

Optional explicit env vars:

~~~powershell
$env:OLLAMA_ENDPOINT="http://127.0.0.1:11434"
$env:OLLAMA_MODEL="llama3"
dotnet run --project .\ConsoleAppAITest\ConsoleAppAITest.csproj
~~~

## Optional: Ollama in Podman (Only Ollama, Not This App)

Start Ollama container:

~~~powershell
podman run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
~~~

Pull or run model:

~~~powershell
podman exec -it ollama ollama run llama3
~~~

Check models:

~~~powershell
podman exec -it ollama ollama list
~~~

Check container logs:

~~~powershell
podman logs ollama
~~~

## Quick Connectivity Checks

From Windows host:

~~~powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 11434
Invoke-WebRequest http://127.0.0.1:11434
~~~

Expected HTTP body includes:

- Ollama is running

## Troubleshooting

### Error: Connection refused

Common causes:

1. Ollama container is not running
2. Endpoint not set to http://127.0.0.1:11434
3. Port mapping missing or conflicting
4. Model not downloaded yet

Fix checklist:

1. Start Ollama container
2. Confirm port 11434 is exposed by Podman
3. Confirm endpoint: http://127.0.0.1:11434
4. Confirm model exists:
   - podman exec -it ollama ollama list

### Error: Model not found

Run:

~~~powershell
podman exec -it ollama ollama run llama3
~~~

Then retry app.

## Build

~~~powershell
dotnet build .\ConsoleAppAITest.slnx
~~~

## Visual Studio Launch Profile (Screenshot-Style Steps)

### Profile: ConsoleAppAITest

[Step 1 - Open Solution]

- Open ConsoleAppAITest.slnx in Visual Studio.

[Step 2 - Select Profile]

- In the startup target dropdown (next to Run), choose ConsoleAppAITest.

[Step 3 - Confirm Endpoint]

- Open ConsoleAppAITest/Properties/launchSettings.json.
- Confirm OLLAMA_ENDPOINT is set to http://127.0.0.1:11434.
- Confirm OLLAMA_MODEL is set to llama3.

[Step 4 - Run]

- Press F5 (Debug) or Ctrl+F5 (Run without debugging).
- In the console output, confirm startup lines show the selected endpoint and model.

### What You Should See

[Host Mode Expected]

- Using Ollama endpoint: http://127.0.0.1:11434
- Using Ollama model: llama3
- Model response text

### FAQ (Visual Studio Profile Selection)

Q1: Why does it work from terminal but fail in Visual Studio?

- Cause: Visual Studio is launching a different startup profile than expected.
- Fix: Check the startup target dropdown and select ConsoleAppAITest.

Q2: Why do I still get connection refused?

- Cause: Ollama is not listening on local port 11434 or model is missing.
- Fix: Confirm http://127.0.0.1:11434 returns "Ollama is running" and run podman exec -it ollama ollama list.

## Notes

- The app prints the selected endpoint and model at startup.
- The app performs a quick readiness check against Ollama before invoking the prompt.
