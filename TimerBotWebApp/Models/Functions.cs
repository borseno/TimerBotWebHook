using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static TimerBotWebApp.Models.Constants;

namespace TimerBotWebApp.Models
{
    internal static class Functions
    {
        public static async void TimerHandler(object obj, MessageEventArgs args)
        {
            var text = args.Message?.Text;

            if (!(obj is TelegramBotClient client))
                return;

            if (string.IsNullOrEmpty(text))
                return;

            if (text == StartCommand)
            {
                await client.SendTextMessageIgnoreAPIErrorsAsync(args.GetChatId(), StartAnswer);
                return;
            }

            if (!TimeSpan.TryParseExact(text, TimeSpanFormat, CultureInfo.InvariantCulture, out var timeSpan))
                return;

            var sec = timeSpan.TotalSeconds;

            var infoMsg = await client.SendTextMessageAsync(args.GetChatId(),
                $@"The timer for {timeSpan.ToString(TimeSpanFormat)} is being started...");

            DateTime startTime = DateTime.Now;
            while (true)
            {
                var delayTask = Task.Delay(1000);

                var leftSeconds = (DateTime.Now - startTime).TotalSeconds;

                if (leftSeconds >= sec)
                    break;

                var editTask =
                    client.EditMessageTextAsync(
                        args.GetChatId(),
                        infoMsg.MessageId,
                        $"Timer for {timeSpan.ToString(TimeSpanFormat)} is running! {Environment.NewLine}" +
                        $"Current remaining time: {timeSpan - new TimeSpan(0, 0, seconds: (int)leftSeconds)}");

                Task.WaitAll(editTask, delayTask);
            }

            var editAfterRunTask =
                client.EditMessageTextAsync(
                    args.GetChatId(),
                    infoMsg.MessageId,
                    $"Timer for {timeSpan.ToString(TimeSpanFormat)} has run out!"
                    );

            var sendTask =
                client.SendTextMessageAsync(
                    args.GetChatId(),
                    $"@{args.GetSenderUserName()} The timer has ran out!");

            await Task.WhenAll(editAfterRunTask, sendTask);
        }

        public static async void TimerHandler(TelegramBotClient client, Message message)
        {
            var text = message?.Text;

            if (string.IsNullOrEmpty(text))
                return;

            if (text == StartCommand)
            {
                await client.SendTextMessageIgnoreAPIErrorsAsync(message.GetChatId(), StartAnswer);
                return;
            }

            if (!TimeSpan.TryParseExact(text, TimeSpanFormat, CultureInfo.InvariantCulture, out var timeSpan))
                return;

            var sec = timeSpan.TotalSeconds;

            var infoMsg = await client.SendTextMessageAsync(message.GetChatId(),
                $@"The timer for {timeSpan.ToString(TimeSpanFormat)} is being started...");

            DateTime startTime = DateTime.Now;
            while (true)
            {
                var delayTask = Task.Delay(1000);

                var leftSeconds = (DateTime.Now - startTime).TotalSeconds;

                if (leftSeconds >= sec)
                    break;

                var editTask =
                    client.EditMessageTextAsync(
                        message.GetChatId(),
                        infoMsg.MessageId,
                        $"Timer for {timeSpan.ToString(TimeSpanFormat)} is running! {Environment.NewLine}" +
                        $"Current remaining time: {timeSpan - new TimeSpan(0, 0, seconds: (int)leftSeconds)}");

                Task.WaitAll(editTask, delayTask);
            }

            var editAfterRunTask =
                client.EditMessageTextAsync(
                    message.GetChatId(),
                    infoMsg.MessageId,
                    $"Timer for {timeSpan.ToString(TimeSpanFormat)} has run out!"
                    );

            var sendTask =
                client.SendTextMessageAsync(
                    message.GetChatId(),
                    $"@{message.GetSenderUserName()} The timer has ran out!");

            await Task.WhenAll(editAfterRunTask, sendTask);
        }
    }

    #region extensions
    public static class MessageEventArgsExtensions
    {
        public static long GetChatId(this MessageEventArgs args)
            => args.Message.Chat.Id;

        public static string GetSenderUserName(this MessageEventArgs args)
    => args.Message.From.Username;
    }

    public static class MessageExtensions
    {
        public static long GetChatId(this Message message)
    => message.Chat.Id;

        public static string GetSenderUserName(this Message message)
    => message.From.Username;
    }

    public static class TelegramBotClientExtensions
    {
        public static async Task<Message> SendTextMessageIgnoreAPIErrorsAsync(this TelegramBotClient client,
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup
            replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            Message result = null;
            try
            {
                result = await client.SendTextMessageAsync(chatId,
                    text,
                    parseMode,
                    disableWebPagePreview,
                    disableNotification,
                    replyToMessageId,
                    replyMarkup,
                    cancellationToken);
            }
            catch (Exception e)
            {
                if (!(e is Telegram.Bot.Exceptions.ApiRequestException))
                    throw;
            }

            return result;
        }
    }

    public static class StringExtensions
    {
        public static string RemoveAtEnd(this string value, string substring)
        {
            var index = value.LastIndexOf(substring);

            return value.Substring(0, index);
        }
    }
    #endregion
}
