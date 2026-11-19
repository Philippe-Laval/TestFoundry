using Azure.AI.Projects;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/ai/Azure.AI.Projects/samples/Sample3_Connections.md

/*
MobilitydevMCP
mobilityapisdev-9232
sophiamcp
*/

namespace TestFoundry
{
    public class ConnectionTester
    {
        public async Task TestConnectionAsync()
        {
            var endpoint = Environment.GetEnvironmentVariable("FOUNDRY_PROJECT_ENDPOINT")
                ?? throw new InvalidOperationException("FOUNDRY_PROJECT_ENDPOINT is not set.");
            var deploymentName = Environment.GetEnvironmentVariable("FOUNDRY_MODEL") ?? "gpt-5.4-mini";
            var connectionName = Environment.GetEnvironmentVariable("CONNECTION_NAME");
            AIProjectClient projectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());

            Console.WriteLine("List the properties of all connections:");
            await foreach (AIProjectConnection connection in projectClient.Connections.GetConnectionsAsync())
            {
                Console.WriteLine(connection);
                Console.WriteLine(connection.Name);
            }

            Console.WriteLine("List the properties of all connections of a particular type (e.g., Azure OpenAI connections):");
            await foreach (AIProjectConnection connection in projectClient.Connections.GetConnectionsAsync(connectionType: ConnectionType.AzureOpenAI))
            {
                Console.WriteLine(connection);
            }

            Console.WriteLine($"Get the properties of a connection named `{connectionName}`:");
            AIProjectConnection specificConnection = await projectClient.Connections.GetConnectionAsync(connectionName, includeCredentials: false);
            Console.WriteLine(specificConnection);

            Console.WriteLine("Get the properties of a connection with credentials:");
            AIProjectConnection specificConnectionCredentials = await projectClient.Connections.GetConnectionAsync(connectionName, includeCredentials: true);
            Console.WriteLine(specificConnectionCredentials);

            Console.WriteLine($"Get the properties of the default connection:");
            AIProjectConnection defaultConnection = await projectClient.Connections.GetDefaultConnectionAsync(includeCredentials: false);
            Console.WriteLine(defaultConnection);

            Console.WriteLine($"Get the properties of the default connection with credentials:");
            AIProjectConnection defaultConnectionCredentials = await projectClient.Connections.GetDefaultConnectionAsync(includeCredentials: true);
            Console.WriteLine(defaultConnectionCredentials);
        }
    }
}
