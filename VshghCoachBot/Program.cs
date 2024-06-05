using APIprot.Clients;
using System;

namespace APIprot
{
    class Program
    {
        static readonly Client _client = new Client();

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var telegramClient = new TelegramClient(_client);
            await telegramClient.Start(Constant.Token);
        }
    }
}

