namespace TimerBotWebApp.Models
{
    internal static class Constants
    {
        internal const string ApiToken = "743943650:AAEuST7OZGd33pVO9j5xhpP1Udyor1DcGXE";
        internal const string WebApiUrl = @"https://timer-telegram-bot.herokuapp.com";
        internal const string TimeSpanFormat = @"hh\:mm\:ss";
        internal const string StartCommand = @"/start";
        internal const string StartAnswer = @"Hello! You can set a timer to wait and get a notification as the timer runs out.
Use the following format: " + TimeSpanFormat;
    }
}
