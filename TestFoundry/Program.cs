using OpenAI.Chat;

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

            PersistentAgentGenerator persistentAgentTester = new PersistentAgentGenerator(endpoint, modelNameForAgent);
            await persistentAgentTester.Test();


            ChatCompletionGenerator chatClientTester = new ChatCompletionGenerator(endpoint, modelName);
            ChatCompletion chatCompletion = await chatClientTester.GetChatCompletion("List all the rainbow colors");

            EmbeddingGenerator embeddingClientTester = new EmbeddingGenerator(endpoint, modelNameForEmbedding);
            ReadOnlyMemory<float> embeddings = await embeddingClientTester.GetEmbedding("List all the rainbow colors");

            ConnectionTester connectionTester = new ConnectionTester(endpoint, modelName, connectionName);
            await connectionTester.TestConnectionAsync();
        }
    }
}
