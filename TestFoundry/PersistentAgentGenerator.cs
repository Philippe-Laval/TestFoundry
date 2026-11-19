using Azure;
using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using RunStatus = Azure.AI.Agents.Persistent.RunStatus;

// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/ai/Azure.AI.Projects/samples/Sample6_ClassicAgents.md
// Attention : classic agents are deprecated and will be removed in a future release.
// Please use persistent agents instead (see AgentManager.cs).

namespace TestFoundry
{
    public class PersistentAgentGenerator
    {
        private readonly PersistentAgentsClient _agentsClient;
        private PersistentAgent? _agent = null;
        private readonly string _modelName;

        public PersistentAgentGenerator(string endpoint, string modelName)
        {
            _modelName = modelName;
            
            AIProjectClient projectClient = new(new Uri(endpoint), new DefaultAzureCredential());
            _agentsClient = projectClient.GetPersistentAgentsClient();
        }

        public async Task<PersistentAgent> CreateAgent()
        {
            // Create a new persistent agent
            PersistentAgent agent = await _agentsClient.Administration.CreateAgentAsync(
                model: _modelName,
                name: "Math Tutor",
                instructions: "You are a personal math tutor. Write and run code to answer math questions."
            );

            return agent;
        }

        public async Task GetAnswerFromAgent()
        {
            PersistentAgentThread? thread = null;
            ThreadRun? run = null;

            try
            {
                // Create an agent.
                if (_agent == null)
                {
                    _agent = await CreateAgent();
                }

                // Create a thread.
                thread = await _agentsClient.Threads.CreateThreadAsync();

                // Add a message to a thread.
                PersistentThreadMessage message = await _agentsClient.Messages.CreateMessageAsync(
                    thread.Id,
                    MessageRole.User,
                    "I need to solve the equation `3x + 11 = 14`. Can you help me?");

                // Run the agent.
                run = await _agentsClient.Runs.CreateRunAsync(
                    thread.Id,
                    _agent.Id,
                    additionalInstructions: "Please address the user as Jane Doe. The user has a premium account.");

                // Wait for the run to complete
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    run = await _agentsClient.Runs.GetRunAsync(thread.Id, run.Id);
                }
                while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);

                //Assert.That(
                //    RunStatus.Completed,
                //    Is.EqualTo(run.Status),
                //    run.LastError?.Message);

                // List messages in chronological order.
                AsyncPageable<PersistentThreadMessage> messages
                    = _agentsClient.Messages.GetMessagesAsync(
                        threadId: thread.Id, order: ListSortOrder.Ascending);

                await foreach (PersistentThreadMessage threadMessage in messages)
                {
                    Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
                    foreach (MessageContent contentItem in threadMessage.ContentItems)
                    {
                        if (contentItem is MessageTextContent textItem)
                        {
                            Console.Write(textItem.Text);
                        }
                        else if (contentItem is MessageImageFileContent imageFileItem)
                        {
                            Console.Write($"<image from ID: {imageFileItem.FileId}");
                        }
                        Console.WriteLine();
                    }
                }

            }
            finally
            {
                // Clean up the resources

                if (thread is not null)
                {
                    await _agentsClient.Threads.DeleteThreadAsync(threadId: thread.Id);
                }

                if (_agent is not null)
                {
                    await _agentsClient.Administration.DeleteAgentAsync(agentId: _agent.Id);
                }
            }
        }

    }
}
