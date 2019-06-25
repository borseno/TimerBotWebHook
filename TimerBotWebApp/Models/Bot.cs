using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TimerBotWebApp.Controllers;
using static TimerBotWebApp.Models.Constants;

namespace TimerBotWebApp.Models
{
    public static class Bot
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private static TelegramBotClient client;

        public static async Task<TelegramBotClient> GetAsync()
        {
            if (client != null)
                return client;
            
            await semaphore.WaitAsync();

            try
            {
                if (client == null)
                {
                    client = new TelegramBotClient(ApiToken);

                    await client.SetWebhookAsync($"{TimerBotWebApp.Models.Constants.WebApiUrl}/api/{nameof(UpdateController).RemoveAtEnd(nameof(Controller)).ToLower()}");
                }
            }
            finally
            {
                semaphore.Release();
            }

            return client;
        }
    }
}
