using Azure.AI.Projects;
using Azure.AI.Projects.Memory;
using Azure.Identity;
using OpenAI.Responses;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable AAIP001
#pragma warning disable OPENAI001

namespace TestFoundry
{
    /// <summary>
    /// Gère les opérations de création, mise à jour, récupération, suppression et alimentation de magasins de mémoire
    /// via un projet Azure AI en utilisant des modèles de conversation et d’embedding configurés.
    /// </summary>
    /// <remarks>Utilise <see cref="DefaultAzureCredential"/> pour l’authentification et un
    /// <c>AIProjectClient</c> initialisé avec l’endpoint du projet. Les opérations d’alimentation attendent la fin du
    /// traitement des mémoires et lèvent une exception en cas d’échec.</remarks>
    public class MemoryStoreManager
    {
        private readonly AIProjectClient _projectClient;
        private readonly string _chatModelName;
        private readonly string _embeddingModelName;

        public MemoryStoreManager(string projectEndpoint, string chatModelName, string embeddingModelName)
        {
            _projectClient = new(
                endpoint: new Uri(projectEndpoint),
                tokenProvider: new DefaultAzureCredential());
            _chatModelName = chatModelName;
            _embeddingModelName = embeddingModelName;
        }

        public async Task<MemoryStore> CreateMemoryStore(string storeName, string storeDescription)
        {
            var memoryStoreDefinition = new MemoryStoreDefaultDefinition(
                chatModel: _chatModelName,
                embeddingModel: _embeddingModelName
            );
            memoryStoreDefinition.Options = new(isUserProfileEnabled: true, isChatSummaryEnabled: true);
            
            MemoryStore memoryStore = await _projectClient.MemoryStores.CreateMemoryStoreAsync(
                name: storeName,
                definition: memoryStoreDefinition,
                description: storeDescription
            );

            Console.WriteLine($"Memory store with id {memoryStore.Id}, name {memoryStore.Name} and description {memoryStore.Description} was created.");

            return memoryStore;
        }

        public async Task<MemoryStore> UpdateMemoryStore(string storeName, string storeDescription)
        {
            MemoryStore memoryStore = await _projectClient.MemoryStores.UpdateMemoryStoreAsync(name: storeName, description: storeDescription);
            Console.WriteLine($"Memory store with id {memoryStore.Id}, name {memoryStore.Name} now has description: {memoryStore.Description}.");
            return memoryStore;
        }

        public async Task<MemoryStore> GetMemoryStore(string storeName)
        {
            MemoryStore memoryStore = await _projectClient.MemoryStores.GetMemoryStoreAsync(name: storeName);
            Console.WriteLine($"Memory store with id {memoryStore.Id}, name {memoryStore.Name} and description {memoryStore.Description} was retrieved.");
            return memoryStore;
        }

        public async Task<List<MemoryStore>> GetMemoryStores()
        {
            List<MemoryStore> memoryStores = new List<MemoryStore>();

            await foreach (MemoryStore store in _projectClient.MemoryStores.GetMemoryStoresAsync())
            {
                Console.WriteLine($"Memory store id: {store.Id}, name: {store.Name}, description: {store.Description}.");
                memoryStores.Add(store);
            }

            return memoryStores;
        }

        public async Task DeleteMemoryStore(string storeName)
        {
            DeleteMemoryStoreResponse deleteResponse = await _projectClient.MemoryStores.DeleteMemoryStoreAsync(name: storeName);
            string status = deleteResponse.IsDeleted ? "" : " not";
            Console.WriteLine($"The memory store {deleteResponse.Name} was{status} deleted.");
        }

        public async Task RemoveScope(string storeName, string scope)
        {
            MemoryStoreDeleteScopeResponse deleteScopeResponse =
                await _projectClient.MemoryStores.DeleteScopeAsync(name: storeName, scope: scope);
            string status = deleteScopeResponse.IsDeleted ? "" : " not";
            Console.WriteLine($"The scope {deleteScopeResponse.Name} was{status} deleted.");
        }

        public async Task AddInformationInMemoryStore(string storeName, string scope, string inputTextContent)
        {
            var memoryOptions = new MemoryUpdateOptions(scope);
            memoryOptions.UpdateDelay = 0; 
            var userMessageItem = ResponseItem.CreateUserMessageItem(inputTextContent);

            memoryOptions.Items.Add(userMessageItem);
            MemoryUpdateResult updateResult;
            try
            {
                // Le service de mémoire retarde souvent le traitement d'une mise à jour (un délai de l'ordre de 5 minutes par défaut, de mémoire). 
                updateResult = await _projectClient.MemoryStores
                    .WaitForMemoriesUpdateAsync(memoryStoreName: storeName, options: memoryOptions, pollingInterval: 500)
                    .WaitAsync(TimeSpan.FromMinutes(2));
            }
            catch (TimeoutException)
            {
                throw new TimeoutException($"The memory update for store '{storeName}' did not complete within 2 minutes.");
            }

            if (updateResult.Status == MemoryStoreUpdateStatus.Failed)
            {
                throw new InvalidOperationException(updateResult.ErrorDetails);
            }

            Console.WriteLine($"The update operation {updateResult.UpdateId} has finished with {updateResult.Status} status.");
        }

        public async Task QuestionTheMemoryStore(string storeName, string scope, string inputTextContent)
        {
            var opts = new MemorySearchOptions(scope)
            {
                Items = { ResponseItem.CreateUserMessageItem(inputTextContent) },
            };

            MemoryStoreSearchResponse resp = await _projectClient.MemoryStores.SearchMemoriesAsync(
                memoryStoreName: storeName,
                options: opts
            );
            Console.WriteLine("==The output from memory tool.==");

            foreach (MemorySearchItem item in resp.Memories)
            {
                Console.WriteLine(item.MemoryItem.Content);
            }
            
            Console.WriteLine("==End of memory tool output.==");
        }
        


    }
}
