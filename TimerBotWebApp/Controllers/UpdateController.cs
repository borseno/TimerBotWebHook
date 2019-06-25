using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TimerBotWebApp.Models;

namespace TimerBotWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private delegate void MessageHandler(TelegramBotClient client, Message msg);
        private event MessageHandler MessageReceived;

        public UpdateController()
        {
            MessageReceived += Functions.TimerHandler;
        }

        // GET api/update
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/update
        [HttpPost]
        public async Task Post([FromBody] Update update)
        {
            if (update.Type == UpdateType.Message)
            {
                var bot = await Bot.GetAsync();

                MessageReceived(bot, update.Message);
            }
        }
    }
}
