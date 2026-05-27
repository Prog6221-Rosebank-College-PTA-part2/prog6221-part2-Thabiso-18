using System;

namespace CybersecurityBotWPF
{
    public class ChatMessage
    {
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
        public string Sentiment { get; set; }
        public string Visibility => string.IsNullOrEmpty(Sentiment) ? "Collapsed" : "Visible";
        public string ShowSentiment => Visibility;

        public ChatMessage(string content, bool isUser, string sentiment = "")
        {
            Content = content;
            IsUser = isUser;
            Timestamp = DateTime.Now;
            Sentiment = sentiment;
        }
    }
}