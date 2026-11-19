namespace TestFoundry
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //ConnectionTester connectionTester = new ConnectionTester();
            //await connectionTester.TestConnectionAsync();

            ChatClientTester chatClientTester = new ChatClientTester();
            await chatClientTester.Test();
        }
    }
}
