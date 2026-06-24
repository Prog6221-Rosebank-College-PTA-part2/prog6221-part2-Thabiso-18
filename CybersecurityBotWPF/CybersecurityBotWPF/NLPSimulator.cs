using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityBotWPF
{
    public class NLPSimulator
    {
        private Dictionary<string, List<string>> intentPatterns;
        private Dictionary<string, string> topicResponses;
        private Random random;

        public NLPSimulator()
        {
            random = new Random();
            InitializeIntentPatterns();
            InitializeTopicResponses();
        }

        private void InitializeIntentPatterns()
        {
            intentPatterns = new Dictionary<string, List<string>>
            {
                // Task intents
                { "add_task", new List<string>
                    {
                        @"add\s+(?:a\s+)?task",
                        @"create\s+(?:a\s+)?task",
                        @"new\s+task",
                        @"set\s+(?:a\s+)?(?:reminder|task)",
                        @"remind\s+me\s+to",
                        @"i\s+need\s+to",
                        @"please\s+remind\s+me"
                    }
                },
                { "list_tasks", new List<string>
                    {
                        @"show\s+(?:my\s+)?tasks",
                        @"view\s+tasks",
                        @"list\s+tasks",
                        @"what\s+(?:are|is)\s+(?:my\s+)?tasks",
                        @"show\s+all\s+tasks"
                    }
                },
                { "list_all_tasks", new List<string>
                    {
                        @"show\s+all\s+tasks",
                        @"view\s+all\s+tasks",
                        @"list\s+all\s+tasks",
                        @"show\s+everything",
                        @"display\s+all"
                    }
                },
                { "complete_task", new List<string>
                    {
                        @"complete\s+task",
                        @"mark\s+(?:as\s+)?complete",
                        @"done\s+task",
                        @"finish\s+task",
                        @"task\s+done"
                    }
                },
                { "delete_task", new List<string>
                    {
                        @"delete\s+task",
                        @"remove\s+task",
                        @"clear\s+task",
                        @"task\s+delete"
                    }
                },
                // Quiz intents
                { "start_quiz", new List<string>
                    {
                        @"start\s+quiz",
                        @"begin\s+quiz",
                        @"take\s+quiz",
                        @"play\s+quiz",
                        @"quiz\s+time",
                        @"i\s+want\s+to\s+quiz"
                    }
                },
                // Greeting intents
                { "greeting", new List<string>
                    {
                        @"^hello",
                        @"^hi",
                        @"^hey",
                        @"^good\s+(?:morning|afternoon|evening)",
                        @"^howdy",
                        @"^yo",
                        @"^hola",
                        @"^namaste"
                    }
                }
            };
        }

        private void InitializeTopicResponses()
        {
            topicResponses = new Dictionary<string, string>
            {
                { "password", "🔐 **Password Security Tips:**\n• Use at least 12 characters\n• Mix uppercase, lowercase, numbers, and symbols\n• Never reuse passwords\n• Use a password manager\n• Enable 2FA whenever possible" },
                { "phishing", "🎣 **Phishing Prevention:**\n• Check sender email addresses\n• Hover over links before clicking\n• Look for spelling errors\n• Never share personal info via email\n• Report suspicious emails" },
                { "2fa", "🔑 **Two-Factor Authentication:**\n• Adds an extra security layer\n• Uses something you know + something you have\n• Common methods: SMS codes, authenticator apps, hardware tokens\n• Highly recommended for all accounts" },
                { "malware", "🛡️ **Malware Protection:**\n• Install antivirus software\n• Keep everything updated\n• Don't download from untrusted sources\n• Use ad-blockers\n• Be careful with email attachments" },
                { "browsing", "🌐 **Safe Browsing:**\n• Look for HTTPS and padlock icon\n• Avoid public Wi-Fi for sensitive activities\n• Use a VPN\n• Clear cookies regularly\n• Use private browsing for sensitive searches" },
                { "social engineering", "🎭 **Social Engineering Awareness:**\n• Always verify identities\n• Be suspicious of urgent requests\n• Don't share sensitive information\n• Trust your instincts\n• Report suspicious behavior" },
                { "ransomware", "💰 **Ransomware Prevention:**\n• Regular backups\n• Update software\n• Use email filtering\n• Be cautious with attachments\n• Have an incident response plan" }
            };
        }

        public bool IsTaskCommand(string input)
        {
            return IsMatchingIntent(input, "add_task");
        }

        public bool IsListTasksCommand(string input)
        {
            return IsMatchingIntent(input, "list_tasks");
        }

        public bool IsListAllTasksCommand(string input)
        {
            return IsMatchingIntent(input, "list_all_tasks");
        }

        public bool IsCompleteTaskCommand(string input)
        {
            return IsMatchingIntent(input, "complete_task");
        }

        public bool IsDeleteTaskCommand(string input)
        {
            return IsMatchingIntent(input, "delete_task");
        }

        public bool IsGreeting(string input)
        {
            return IsMatchingIntent(input, "greeting");
        }

        public bool IsStartQuiz(string input)
        {
            return IsMatchingIntent(input, "start_quiz");
        }

        private bool IsMatchingIntent(string input, string intent)
        {
            if (!intentPatterns.ContainsKey(intent))
                return false;

            foreach (string pattern in intentPatterns[intent])
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }

        public string GetCybersecurityResponse(string input)
        {
            foreach (var topic in topicResponses.Keys)
            {
                if (input.Contains(topic) || input.Contains(topic.ToLower()))
                {
                    return topicResponses[topic];
                }
            }
            return null;
        }

        public string GetSuggestion(string input)
        {
            string[] suggestions = {
                "Try asking about: passwords, phishing, 2FA, malware, or safe browsing.",
                "You can also say 'Add task' to create a reminder, or 'Start quiz' to test your knowledge!",
                "I can help with cybersecurity topics, task management, or quiz challenges.",
                "Type 'help' to see all available commands.",
                "I understand different ways of asking. Try rephrasing your request."
            };

            return suggestions[random.Next(suggestions.Length)];
        }

        public string ExtractTaskTitle(string input)
        {
            // Try to extract task title using patterns
            string[] patterns = {
                @"add\s+(?:a\s+)?task\s+(?:to\s+)?(.+?)(?:\s+(?:remind|with|in)|\s*$)",
                @"remind\s+me\s+to\s+(.+?)(?:\s+in\s+\d+|\s*$)",
                @"new\s+task\s+(?:to\s+)?(.+?)(?:\s+(?:remind|with|in)|\s*$)"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            // If no pattern matches, return a cleaned version of the input
            string[] prefixes = { "add task", "new task", "create task", "remind me to", "i need to" };
            string cleaned = input.ToLower();
            foreach (var prefix in prefixes)
            {
                if (cleaned.Contains(prefix))
                {
                    int startIndex = cleaned.IndexOf(prefix) + prefix.Length;
                    if (startIndex < input.Length)
                    {
                        return input.Substring(startIndex).Trim();
                    }
                }
            }

            return input.Trim();
        }

        public int ExtractTaskId(string input)
        {
            // Look for #number pattern
            Match match = Regex.Match(input, @"#(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            // Look for "task" followed by number
            match = Regex.Match(input, @"task\s+(\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            // Look for "ID" followed by number
            match = Regex.Match(input, @"id\s+(\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return 0;
        }

        public int ExtractReminderDays(string input)
        {
            // Look for "in X days"
            Match match = Regex.Match(input, @"in\s+(\d+)\s+days?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            // Look for "in X weeks"
            match = Regex.Match(input, @"in\s+(\d+)\s+weeks?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value) * 7;
            }

            // Check for "tomorrow"
            if (input.Contains("tomorrow"))
            {
                return 1;
            }

            return 0;
        }
    }
}