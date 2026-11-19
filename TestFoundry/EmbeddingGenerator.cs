using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using OpenAI.Embeddings;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;

// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/ai/Azure.AI.Projects/samples/Sample7_CreateClients.md

// text-embedding-3-large
// Generated embedding with 3072 dimensions

namespace TestFoundry
{
    internal class EmbeddingGenerator
    {
        private readonly EmbeddingClient _embeddingsClient;

        public EmbeddingGenerator(string endpoint, string embeddingModelName)
        {
            Console.WriteLine("Create the Azure OpenAI embedding client");
            var credential = new DefaultAzureCredential();
            AIProjectClient projectClient = new AIProjectClient(new Uri(endpoint), credential);

            ClientConnection connection = projectClient.GetConnection(typeof(AzureOpenAIClient).FullName!);

            if (!connection.TryGetLocatorAsUri(out Uri? uri) || uri is null)
            {
                throw new InvalidOperationException("Invalid URI.");
            }
            uri = new Uri($"https://{uri.Host}");

            AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(uri, credential);
            _embeddingsClient = azureOpenAIClient.GetEmbeddingClient(deploymentName: embeddingModelName);
        }

        public async Task<ReadOnlyMemory<float>> GetEmbedding(string input)
        {
            Console.WriteLine($"Generate an embedding for {input}");
            OpenAIEmbedding result = await _embeddingsClient.GenerateEmbeddingAsync(input);
            Console.WriteLine($"Generated embedding with {result.ToFloats().Length} dimensions");
            return result.ToFloats();
        }
    }
}
