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
    internal class ChatClientTester
    {
        public async Task Test()
        {
            var endpoint = System.Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT");
            var modelName = System.Environment.GetEnvironmentVariable("FOUNDRY_MODEL_NAME");
            var connectionName = System.Environment.GetEnvironmentVariable("CONNECTION_NAME");
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
            ChatClient chatClient = azureOpenAIClient.GetChatClient(deploymentName: modelName);

            Console.WriteLine("Complete a chat");
            ChatCompletion result = await chatClient.CompleteChatAsync("List all the rainbow colors");
            Console.WriteLine(result.Content[0].Text);
        }

    }
}
