using Microsoft.SemanticKernel;

var defaultEndpoint = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "http://host.containers.internal:11434"
    : "http://127.0.0.1:11434";

var endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? defaultEndpoint;
var modelId = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";

Console.WriteLine($"Using Ollama endpoint: {endpointValue}");
Console.WriteLine($"Using Ollama model: {modelId}");

if (!Uri.TryCreate(endpointValue, UriKind.Absolute, out var endpoint))
{
    Console.WriteLine($"Invalid OLLAMA_ENDPOINT value: '{endpointValue}'");
    return;
}

var builder = Kernel.CreateBuilder();
builder.AddOllamaChatCompletion(modelId: modelId, endpoint: endpoint);
var kernel = builder.Build();

try
{
    using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    var tagsResponse = await httpClient.GetAsync(new Uri(endpoint, "/api/tags"));
    if (!tagsResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Ollama readiness check failed with status code {(int)tagsResponse.StatusCode}.");
    }

    var prompt = "Olá! Eu sou um desenvolvedor .NET sênior explorando IA. O que é o Semantic Kernel?";
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine(result);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Connection error talking to Ollama: {ex.Message}");
    Console.WriteLine("Hint: host mode -> http://127.0.0.1:11434, container mode -> http://host.containers.internal:11434");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.GetType().Name} - {ex.Message}");
}