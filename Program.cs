using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OllamaPerformanceChat
{
    class Program
    {
        private static Kernel? _kernel;
        private static readonly string[] AvailableModels = 
        {
            "llama3.2:latest",
            "phi3.5:latest", 
            "phi3:latest"
        };
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("🤖 Ollama Performance Test Chat");
            Console.WriteLine("================================");
            
            // Model selection
            string selectedModel = SelectModel();
            
            // Initialize Semantic Kernel
            await InitializeKernel(selectedModel);
            
            Console.WriteLine($"\n✅ Connected to {selectedModel}");
            Console.WriteLine("Type 'quit' to exit, 'model' to switch models, 'stats' for session stats\n");
            
            var sessionStats = new SessionStats();
            
            // Chat loop
            while (true)
            {
                Console.Write("You: ");
                string? userInput = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(userInput))
                    continue;
                    
                if (userInput.ToLower() == "quit")
                    break;
                    
                if (userInput.ToLower() == "model")
                {
                    selectedModel = SelectModel();
                    await InitializeKernel(selectedModel);
                    Console.WriteLine($"✅ Switched to {selectedModel}\n");
                    continue;
                }
                
                if (userInput.ToLower() == "stats")
                {
                    DisplaySessionStats(sessionStats);
                    continue;
                }
                
                // Measure performance
                var metrics = await GetResponseWithMetrics(userInput);
                
                // Display response
                Console.WriteLine($"\nAI: {metrics.Response}");
                
                // Display performance metrics
                DisplayMetrics(metrics);
                
                // Update session stats
                sessionStats.Update(metrics);
                
                Console.WriteLine();
            }
            
            // Final session summary
            Console.WriteLine("\n📊 Final Session Summary:");
            DisplaySessionStats(sessionStats);
        }
        
        private static string SelectModel()
        {
            Console.WriteLine("\nAvailable models:");
            for (int i = 0; i < AvailableModels.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {AvailableModels[i]}");
            }
            
            while (true)
            {
                Console.Write("Select model (1-3): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && 
                    choice >= 1 && choice <= AvailableModels.Length)
                {
                    return AvailableModels[choice - 1];
                }
                Console.WriteLine("Invalid selection. Please try again.");
            }
        }
        
        private static async Task InitializeKernel(string modelId)
        {
            var builder = Kernel.CreateBuilder();
            
            // Add Ollama as OpenAI-compatible endpoint
            builder.AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: "not-needed", // Ollama doesn't require API key
                endpoint: new Uri("http://localhost:11434/v1"));
            
            // Optional: Add logging to see what's happening
            builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Warning));
            
            _kernel = builder.Build();
            
            // Test connection with a quick prompt
            Console.Write("Testing connection... ");
            try
            {
                var result = await _kernel.InvokePromptAsync("Say 'Ready!'");
                Console.WriteLine("✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}");
                Console.WriteLine("Make sure Ollama is running: 'ollama serve'");
                Environment.Exit(1);
            }
        }
        
        private static async Task<ResponseMetrics> GetResponseWithMetrics(string prompt)
        {
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.Now;
            
            try
            {
                var result = await _kernel!.InvokePromptAsync(prompt);
                string response = result.ToString();
                
                stopwatch.Stop();
                
                return new ResponseMetrics
                {
                    Response = response,
                    TotalTimeMs = stopwatch.ElapsedMilliseconds,
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    TokenCount = EstimateTokenCount(response),
                    Success = true
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                return new ResponseMetrics
                {
                    Response = $"Error: {ex.Message}",
                    TotalTimeMs = stopwatch.ElapsedMilliseconds,
                    StartTime = startTime,
                    EndTime = DateTime.Now,
                    TokenCount = 0,
                    Success = false
                };
            }
        }
        
        private static void DisplayMetrics(ResponseMetrics metrics)
        {
            Console.WriteLine($"\n⏱️  Performance Metrics:");
            Console.WriteLine($"   Total Time: {metrics.TotalTimeMs:N0} ms ({metrics.TotalTimeMs / 1000.0:F2}s)");
            
            if (metrics.Success && metrics.TokenCount > 0)
            {
                double tokensPerSecond = metrics.TokenCount / (metrics.TotalTimeMs / 1000.0);
                Console.WriteLine($"   Est. Tokens: ~{metrics.TokenCount}");
                Console.WriteLine($"   Speed: ~{tokensPerSecond:F1} tokens/sec");
            }
            
            Console.WriteLine($"   Time: {metrics.StartTime:HH:mm:ss} - {metrics.EndTime:HH:mm:ss}");
        }
        
        private static void DisplaySessionStats(SessionStats stats)
        {
            if (stats.TotalRequests == 0)
            {
                Console.WriteLine("No requests made yet.");
                return;
            }
            
            Console.WriteLine($"\n📈 Session Statistics:");
            Console.WriteLine($"   Total Requests: {stats.TotalRequests}");
            Console.WriteLine($"   Successful: {stats.SuccessfulRequests}");
            Console.WriteLine($"   Failed: {stats.FailedRequests}");
            Console.WriteLine($"   Average Response Time: {stats.AverageResponseTime:F2}s");
            Console.WriteLine($"   Fastest Response: {stats.FastestResponse:F2}s");
            Console.WriteLine($"   Slowest Response: {stats.SlowestResponse:F2}s");
            Console.WriteLine($"   Average Speed: ~{stats.AverageTokensPerSecond:F1} tokens/sec");
            Console.WriteLine($"   Total Session Time: {stats.TotalSessionTime:F1}s");
        }
        
        // Simple token estimation (rough approximation)
        private static int EstimateTokenCount(string text)
        {
            // Rough estimate: ~4 characters per token for English text
            return text.Length / 4;
        }
    }
    
    public class ResponseMetrics
    {
        public string Response { get; set; } = "";
        public long TotalTimeMs { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TokenCount { get; set; }
        public bool Success { get; set; }
    }
    
    public class SessionStats
    {
        private readonly List<ResponseMetrics> _metrics = new();
        
        public int TotalRequests => _metrics.Count;
        public int SuccessfulRequests => _metrics.Count(m => m.Success);
        public int FailedRequests => _metrics.Count(m => !m.Success);
        
        public double AverageResponseTime => 
            _metrics.Any() ? _metrics.Where(m => m.Success).Average(m => m.TotalTimeMs) / 1000.0 : 0;
            
        public double FastestResponse => 
            _metrics.Any(m => m.Success) ? _metrics.Where(m => m.Success).Min(m => m.TotalTimeMs) / 1000.0 : 0;
            
        public double SlowestResponse => 
            _metrics.Any(m => m.Success) ? _metrics.Where(m => m.Success).Max(m => m.TotalTimeMs) / 1000.0 : 0;
            
        public double AverageTokensPerSecond
        {
            get
            {
                var successful = _metrics.Where(m => m.Success && m.TokenCount > 0).ToList();
                if (!successful.Any()) return 0;
                
                return successful.Average(m => m.TokenCount / (m.TotalTimeMs / 1000.0));
            }
        }
        
        public double TotalSessionTime => 
            _metrics.Any() ? (_metrics.Max(m => m.EndTime) - _metrics.Min(m => m.StartTime)).TotalSeconds : 0;
        
        public void Update(ResponseMetrics metrics)
        {
            _metrics.Add(metrics);
        }
    }
}