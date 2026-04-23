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
- Container support: ConsoleAppAITest/Dockerfile
- Solution: ConsoleAppAITest.slnx

## Prerequisites

1. .NET SDK 10+
2. Podman (if running Ollama in a container)
3. Ollama model available (example: llama3)

## Configuration

The app reads these environment variables:

- OLLAMA_ENDPOINT
- OLLAMA_MODEL

Defaults used by the app:

- Host run default endpoint: http://127.0.0.1:11434
- Container run default endpoint: http://host.containers.internal:11434
- Default model: llama3

## Run Modes

### 1) Run App on Windows Host

Use endpoint:

- http://127.0.0.1:11434

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

### 2) Run App in Container

When the app runs in a container, localhost points to the app container itself, not your host machine.
Use endpoint:

- http://host.containers.internal:11434

If you run from Visual Studio Docker profile, this endpoint is already set in launchSettings.json.

If you run manually, set env vars in your app container:

~~~powershell
podman run --rm \
  -e OLLAMA_ENDPOINT=http://host.containers.internal:11434 \
  -e OLLAMA_MODEL=llama3 \
  <your-app-image>
~~~

## Podman Setup for Ollama

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

From the Ollama container itself:

~~~powershell
podman exec -it ollama ollama list
~~~

## Troubleshooting

### Error: Connection refused

Common causes:

1. Ollama container is not running
2. Wrong endpoint for the current run mode
3. Port mapping missing or conflicting
4. Model not downloaded yet

Fix checklist:

1. Start Ollama container
2. Confirm port 11434 is exposed by Podman
3. Confirm endpoint:
   - Host run -> http://127.0.0.1:11434
   - Container run -> http://host.containers.internal:11434
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

## Visual Studio Launch Profiles (Screenshot-Style Steps)

### Host Profile (ConsoleAppAITest)

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

### Docker Profile (Container (Dockerfile))

[Step 1 - Ensure Ollama Is Running]

- Confirm Ollama container is up and exposing port 11434.

[Step 2 - Select Profile]

- In the startup target dropdown, choose Container (Dockerfile).

[Step 3 - Confirm Container Endpoint]

- Open ConsoleAppAITest/Properties/launchSettings.json.
- Confirm OLLAMA_ENDPOINT is set to http://host.containers.internal:11434.
- Confirm OLLAMA_MODEL is set to llama3.

[Step 4 - Run]

- Start with F5 or Ctrl+F5.
- Verify the app prints the container endpoint and receives a model response.

### What You Should See

[Host Mode Expected]

- Using Ollama endpoint: http://127.0.0.1:11434
- Using Ollama model: llama3
- Model response text

[Container Mode Expected]

- Using Ollama endpoint: http://host.containers.internal:11434
- Using Ollama model: llama3
- Model response text

### FAQ (Visual Studio Profile Selection)

Q1: Why do I get connection refused only when using the Docker profile?

- Cause: The app is running in a container, but endpoint is set to localhost/127.0.0.1.
- Fix: Use Container (Dockerfile) profile with OLLAMA_ENDPOINT set to http://host.containers.internal:11434.

Q2: Why does it work from terminal but fail in Visual Studio?

- Cause: Visual Studio is launching a different startup profile than expected.
- Fix: Check the startup target dropdown and select the intended profile before running:
   - Host run: ConsoleAppAITest
   - Container run: Container (Dockerfile)

## Notes

- The app prints the selected endpoint and model at startup.
- The app performs a quick readiness check against Ollama before invoking the prompt.
