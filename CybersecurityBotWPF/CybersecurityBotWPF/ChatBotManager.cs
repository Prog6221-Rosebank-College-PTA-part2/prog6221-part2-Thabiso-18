using System;
using System.Collections.Generic;

namespace CybersecurityBotWPF
{
    public class ChatBotManager
    {
        private Dictionary<string, string> responses;
        private ActivityLogger logger;

        public ChatBotManager(ActivityLogger logger)
        {
            this.logger = logger;
            InitializeResponses();
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, string>
            {
                { "how are you", "I'm operating securely! Thanks for asking. How can I help you stay safe online today?" },
                { "purpose", "My purpose is to educate and protect users from cybersecurity threats. I provide tips on password safety, phishing prevention, and secure browsing!" },
                { "what can i ask", "You can ask me about:\n• Password safety tips\n• How to identify phishing emails\n• Safe browsing practices\n• General cybersecurity questions" },
                { "password", "🔐 Strong passwords should be at least 12 characters long, include uppercase, lowercase, numbers, and symbols. Never reuse passwords across different sites!" },
                { "phishing", "🎣 Phishing emails often have urgent language, suspicious links, or ask for personal info. Always verify the sender's email address before clicking anything!" },
                { "safe browsing", "🌐 Look for 'https://' and the padlock icon in your browser. Avoid using public Wi-Fi for sensitive transactions, and keep your browser updated!" },
                { "2fa", "Two-Factor Authentication adds an extra security layer. Even if someone steals your password, they can't access your account without the second verification step!" },
                { "malware", "🛡️ Always install antivirus software, keep it updated, and avoid downloading files from untrusted sources to protect against malware!" },
                { "firewall", "🔥 A firewall monitors incoming and outgoing network traffic. It's your first line of defense against unauthorized access to your system!" },
                { "social engineering", "🎭 Social engineering manipulates people into revealing confidential information. Always verify identities before sharing sensitive data!" }
            };
        }

        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            foreach (var keyword in responses.Keys)
            {
                if (input.Contains(keyword))
                {
                    logger.LogAction($"Responded to query about: {keyword}");
                    return responses[keyword];
                }
            }

            return null;
        }
    }
}