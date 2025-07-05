# Ollama Performance Chat

![Language](https://img.shields.io/badge/Language-C%23-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9-purple.svg)

A simple yet powerful C# console application to chat with and benchmark the performance of local Large Language Models (LLMs) running via [Ollama](https://ollama.com/). This tool uses [Microsoft Semantic Kernel](https://aka.ms/semantic-kernel) to interface with the models and provides real-time performance metrics like response time and tokens per second for each interaction.

You can seamlessly switch between different models during a chat session to compare their responses and performance characteristics side-by-side.

## Demo

Here is a sample session showing the application in action:

```
🤖 Ollama Performance Test Chat
================================

Available models:
1. llama3.2:latest
2. phi3.5:latest
3. phi3:latest
Select model (1-3): 2

Testing connection... ✅
✅ Connected to phi3.5:latest
Type 'quit' to exit, 'model' to switch models, 'stats' for session stats

You: What is Microsoft Semantic Kernel?
AI: Microsoft Semantic Kernel is an open-source SDK that lets you easily build agents that can call your existing code. As a highly extensible SDK, you can use Semantic Kernel with models from OpenAI, Azure OpenAI, Hugging Face, and more! By combining your existing C#, Python, and Java code with these models, you can build agents that answer questions and automate processes.

⏱️  Performance Metrics:
   Total Time: 3,105 ms (3.11s)
   Est. Tokens: ~91
   Speed: ~29.3 tokens/sec
   Time: 14:21:05 - 14:21:08

You: stats

📈 Session Statistics:
   Total Requests: 1
   Successful: 1
   Failed: 0
   Average Response Time: 3.11s
   Fastest Response: 3.11s
   Slowest Response: 3.11s
   Average Speed: ~29.3 tokens/sec
   Total Session Time: 3.1s

You: quit
```

## ✨ Features

-   **Direct Chat Interface**: Interact with your local LLMs in a straightforward console chat.
-   **Performance Benchmarking**: Automatically measures and displays key metrics for every response:
    -   Total generation time (in ms and seconds).
    -   Estimated tokens generated.
    -   Generation speed (tokens/second).
-   **Session Statistics**: Track aggregate performance across your entire chat session (`stats` command).
-   **On-the-Fly Model Switching**: Change the active LLM at any time without restarting the application (`model` command).
-   **Local & Private**: Runs entirely on your machine. No data is sent to the cloud.
-   **Easy Setup**: Built with .NET and connects to a standard Ollama endpoint.

## 🚀 Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites

1.  **.NET 9 SDK**: Ensure you have the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or a newer version installed.
2.  **Ollama**: You must have [Ollama](https://ollama.com/) installed and running on your machine.
3.  **LLM Models**: Download the models used by this application. Open your terminal and run:
    ```sh
    ollama pull llama3.2
    ollama pull phi3.5
    ollama pull phi3
    ```

### Installation & Running

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/donpotts/OllamaPerformanceChat.git
    cd OllamaPerformanceChat
    ```

2.  **Ensure Ollama is running:**
    Open a separate terminal and run `ollama serve`. You should see a confirmation that the server is listening.

3.  **Run the application:**
    In the project directory, run the following command:
    ```sh
    dotnet run
    ```
    The application will start, test the connection to Ollama, and prompt you to select a model.

## 💻 Usage

Once the application is running, you can start chatting with your chosen model.

-   Simply type your message and press `Enter`.
-   The AI's response will be printed, followed by its performance metrics.

### Commands

You can use the following special commands at any time:

| Command | Description                                                |
| :------ | :--------------------------------------------------------- |
| `model` | Prompts you to switch to a different available LLM.        |
| `stats` | Displays a summary of performance statistics for the current session. |
| `quit`  | Exits the application and shows a final session summary.   |

## 🛠️ Built With

-   **[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)** and **[.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)**
-   **[Microsoft Semantic Kernel](https://aka.ms/semantic-kernel)** - For orchestrating prompts and connecting to the AI model.
-   **[Ollama](https://ollama.com/)** - For serving LLMs locally.

---

Tested on local Windows 11 Pro & Windows 365 Cloud PC

**NOTE: This project is designed to run entirely on your local machine. The AI Models require a fast and powerful computer for quick responses. It does not require any cloud services or external APIs, ensuring complete data privacy and control.**

## 📞 Contact

For any questions, feedback, or inquiries, please feel free to reach out.

**Don Potts** - [Don.Potts@DonPotts.com](mailto:Don.Potts@DonPotts.com)