using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows.Declarative.Kit;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Text;

// beaucoup de code pour gérer Foundry
// https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.projects-readme?view=azure-dotnet#performing-agent-operations

#pragma warning disable AAIP002
#pragma warning disable OPENAI001

namespace TestFoundry
{
    public class AgentManager
    {
        private readonly AIProjectClient _projectClient;
        private readonly string _modelName;

        public AgentManager(string projectEndpoint, string modelName)
        {
            _projectClient = new(
                endpoint: new Uri(projectEndpoint),
                tokenProvider: new DefaultAzureCredential());
            _modelName = modelName;
        }

        public async Task GetResponseAsync()
        {
            ProjectResponsesClient responseClient = _projectClient.ProjectOpenAIClient
               .GetProjectResponsesClientForModel(_modelName);

            ResponseResult response = responseClient.CreateResponse(
                "What is the size of France in square miles?");
            Console.WriteLine($"Response output: {response.GetOutputText()}");
        }


        public async Task CreateAgents()
        {
            DeclarativeAgentDefinition agentDefinition = new(model: _modelName)
            {
                Instructions = "You are a prompt agent."
            };

            ProjectsAgentVersion agentVersion1 = await _projectClient.AgentAdministrationClient.CreateAgentVersionAsync(
                agentName: "myAgent1",
                options: new(agentDefinition));

            Console.WriteLine($"Agent created (id: {agentVersion1.Id}, name: {agentVersion1.Name}, version: {agentVersion1.Version})");

            ProjectsAgentVersion agentVersion2 = await _projectClient.AgentAdministrationClient.CreateAgentVersionAsync(
                agentName: "myAgent2",
                options: new(agentDefinition));

            Console.WriteLine($"Agent created (id: {agentVersion2.Id}, name: {agentVersion2.Name}, version: {agentVersion2.Version})");
        }


        public async Task GetAgent(string agentName)
        {
            ProjectsAgentRecord result = await _projectClient.AgentAdministrationClient.GetAgentAsync(agentName);
            Console.WriteLine($"Agent created (id: {result.Id}, name: {result.Name})");
        }

        public async Task ListAgents()
        {
            await foreach (ProjectsAgentRecord agent in _projectClient.AgentAdministrationClient.GetAgentsAsync())
            {
                Console.WriteLine($"Listed Agent: id: {agent.Id}, name: {agent.Name}");
            }
        }

        public async Task DeleteAgent(string agentName)
        {
            await _projectClient.AgentAdministrationClient.DeleteAgentAsync(agentName);
            Console.WriteLine($"Deleted Agent: name: {agentName}");
        }

        // text-embedding-ada-002
        // grok-4-20-reasoning
        // gpt-6-luna
        // text-embedding-3-small
        // text-embedding-3-large
        // gpt-5.4-mini

        /// <summary>
        /// Récupère et affiche les noms de tous les déploiements disponibles pour le projet.
        /// </summary>
        /// <remarks>Écrit la liste des déploiements dans la sortie standard.</remarks>
        /// <returns>Une tâche qui représente l’opération asynchrone.</returns>
        public async Task<List<string>> GetDeployments()
        {
            List<string> deployedModels = new List<string>();

            Console.WriteLine("List all deployments:");
            foreach (AIProjectDeployment deployment in _projectClient.Deployments.GetDeployments())
            {
                deployedModels.Add(deployment.Name);
            }

            return deployedModels;
        }

        public async Task<List<string>> GetDeploymentsForPublisher(string modelPublisher)
        {
            List<string> deployedModels = new List<string>();

            Console.WriteLine($"List all deployments by the model publisher `{modelPublisher}`:");
            foreach (AIProjectDeployment deployment in _projectClient.Deployments.GetDeployments(modelPublisher: modelPublisher))
            {
                deployedModels.Add(deployment.Name);
            }

            return deployedModels;
        }

        public async Task GetDeploymentDetails(string modelDeploymentName)
        {
            Console.WriteLine($"Get a single model deployment named `{modelDeploymentName}`:");
            ModelDeployment deploymentDetails = (ModelDeployment)_projectClient.Deployments.GetDeployment(modelDeploymentName);
            Console.WriteLine(deploymentDetails);
        }

        
    }
}
