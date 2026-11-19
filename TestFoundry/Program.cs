using Azure.AI.Projects.Memory;
using OpenAI.Chat;

#pragma warning disable AAIP001
#pragma warning disable OPENAI001

namespace TestFoundry
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var endpoint = System.Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT");
            var modelName = System.Environment.GetEnvironmentVariable("FOUNDRY_MODEL_NAME");
            var modelNameForAgent = System.Environment.GetEnvironmentVariable("FOUNDRY_MODEL_AGENT");      
            var modelNameForEmbedding = System.Environment.GetEnvironmentVariable("EMBEDDING_FOUNDRY_MODEL_NAME");
            var connectionName = Environment.GetEnvironmentVariable("CONNECTION_NAME");

            if (string.IsNullOrEmpty(endpoint) ||
                string.IsNullOrEmpty(modelName) ||
                string.IsNullOrEmpty(modelNameForAgent) ||
                string.IsNullOrEmpty(modelNameForEmbedding) ||
                string.IsNullOrEmpty(connectionName))
            {
                Console.WriteLine("Please set the FOUNDRY_PROJECT_ENDPOINT, FOUNDRY_MODEL_NAME, EMBEDDING_FOUNDRY_MODEL_NAME, and CONNECTION_NAME environment variables.");
                return;
            }

            //MemoryStoreManager memoryStoreManager = new MemoryStoreManager(endpoint, modelNameForAgent, modelNameForEmbedding);
            //await memoryStoreManager.CreateMemoryStore("testMemoryStore", "Memory store demo.");
            // MemoryStore memoryStore = await memoryStoreManager.GetMemoryStore("testMemoryStore");
            //await memoryStoreManager.AddInformationInMemoryStore("testMemoryStore", "Flower", "My favourite flower is Cephalocereus euphorbioides.");
            //await memoryStoreManager.QuestionTheMemoryStore("testMemoryStore", "Flower", "What is my favourite flower?");
            //await memoryStoreManager.DeleteMemoryStore("testMemoryStore");


            AgentManager agentManager = new AgentManager(endpoint, modelNameForAgent);

            // OpenAI, xAI
            List<string> deploymentsForOpenAI = await agentManager.GetDeploymentsForPublisher("OpenAI");
            foreach (var deployment in deploymentsForOpenAI)
            {
                Console.WriteLine($"Deployment: {deployment}");
            }

            List<string> deployments = await agentManager.GetDeployments();
            foreach (var deployment in deployments)
            {
                Console.WriteLine($"Deployment: {deployment}");
            }

            await agentManager.GetDeploymentDetails("gpt-5.4-mini");


            await agentManager.CreateAgents();

            PersistentAgentGenerator persistentAgentTester = new PersistentAgentGenerator(endpoint, modelNameForAgent);
            await persistentAgentTester.GetAnswerFromAgent();


            ChatCompletionGenerator chatClientTester = new ChatCompletionGenerator(endpoint, modelName);
            ChatCompletion chatCompletion = await chatClientTester.GetChatCompletion("List all the rainbow colors");

            EmbeddingGenerator embeddingClientTester = new EmbeddingGenerator(endpoint, modelNameForEmbedding);
            ReadOnlyMemory<float> embeddings = await embeddingClientTester.GetEmbedding("List all the rainbow colors");

            ConnectionTester connectionTester = new ConnectionTester(endpoint, modelName, connectionName);
            await connectionTester.TestConnectionAsync();
        }
    }
}
