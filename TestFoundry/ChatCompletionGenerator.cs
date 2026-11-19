using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using OpenAI.Chat;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;

// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/ai/Azure.AI.Projects/samples/Sample7_CreateClients.md

namespace TestFoundry
{
    internal class ChatCompletionGenerator
    {
        private readonly ChatClient _chatClient;

        public ChatCompletionGenerator(string endpoint, string modelName)
        {
            Console.WriteLine("Create the Azure OpenAI chat client");
            var credential = new DefaultAzureCredential();
            AIProjectClient projectClient = new AIProjectClient(new Uri(endpoint), credential);

            ClientConnection connection = projectClient.GetConnection(typeof(AzureOpenAIClient).FullName!);

            if (!connection.TryGetLocatorAsUri(out Uri? uri) || uri is null)
            {
                throw new InvalidOperationException("Invalid URI.");
            }
            uri = new Uri($"https://{uri.Host}");

            AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(uri, credential);
            _chatClient = azureOpenAIClient.GetChatClient(deploymentName: modelName);
        }

        public async Task<ChatCompletion> GetChatCompletion(string input)
        {
            Console.WriteLine("Complete a chat");
            ChatCompletion result = await _chatClient.CompleteChatAsync(input);
            Console.WriteLine(result.Content[0].Text);
            return result;
        }

    }
}
