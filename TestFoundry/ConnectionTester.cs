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
        private readonly string _endpoint;
        private readonly string _deploymentName;
        private readonly string _connectionName;

        public ConnectionTester(string endpoint, string deploymentName, string connectionName)
        {
            _endpoint = endpoint;
            _deploymentName = deploymentName;
            _connectionName = connectionName;
        }

        public async Task TestConnectionAsync()
        {
            AIProjectClient projectClient = new AIProjectClient(new Uri(_endpoint), new DefaultAzureCredential());

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

            Console.WriteLine($"Get the properties of a connection named `{_connectionName}`:");
            AIProjectConnection specificConnection = await projectClient.Connections.GetConnectionAsync(_connectionName, includeCredentials: false);
            Console.WriteLine(specificConnection);

            Console.WriteLine("Get the properties of a connection with credentials:");
            AIProjectConnection specificConnectionCredentials = await projectClient.Connections.GetConnectionAsync(_connectionName, includeCredentials: true);
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
