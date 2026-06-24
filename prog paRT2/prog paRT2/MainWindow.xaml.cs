using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityBotWPF
{
    public partial class MainWindow : Window
    {
        private ChatBotEngine chatBot;
        private SpeechSynthesizer speechSynthesizer;
        private string currentTopic = "";
        private Dictionary<string, List<string>> topicResponses;
        private List<string> conversationHistory;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatBot();
            InitializeVoiceSynthesizer();
            InitializeTopicResponses();
            ShowWelcomeMessage();
        }

        private void InitializeChatBot()
        {
            chatBot = new ChatBotEngine();
            conversationHistory = new List<string>();
        }

        private void InitializeVoiceSynthesizer()
        {
            try
            {
                speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.SetOutputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                StatusText.Text = "⚠️ Voice synthesis not available";
            }
        }

        private void InitializeTopicResponses()
        {
            topicResponses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "🔐 Create strong passwords with at least 12 characters, including uppercase, lowercase, numbers, and symbols!",
                    "🔑 Never reuse passwords across different accounts. Use a password manager to generate and store unique passwords!",
                    "🛡️ Enable Two-Factor Authentication (2FA) whenever possible for an extra layer of security!"
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Always verify the sender's email address before clicking links. Legitimate companies won't ask for sensitive info via email!",
                    "⚠️ Hover over links to see the actual URL before clicking. Look for misspellings or suspicious domain names!",
                    "📧 Be cautious of emails creating urgency or threatening account closure - these are common phishing tactics!"
                },
                ["privacy"] = new List<string>
                {
                    "🔒 Review your privacy settings on social media regularly. Limit what personal information is publicly visible!",
                    "🕵️ Use a VPN when using public Wi-Fi to encrypt your internet traffic and protect your privacy!",
                    "📱 Check app permissions on your phone - many apps request access they don't actually need!"
                },
                ["scam"] = new List<string>
                {
                    "🚨 If something sounds too good to be true, it probably is! Never send money to someone you've only met online!",
                    "📞 Hang up on unsolicited calls asking for personal information. Legitimate companies won't call asking for passwords!",
                    "💸 Never pay fees to claim prizes or inheritances - this is a common advance-fee scam tactic!"
                },
                ["malware"] = new List<string>
                {
                    "🛡️ Keep your antivirus software updated and run regular system scans to detect and remove malware!",
                    "💿 Don't download software from unofficial sources. Stick to official app stores and websites!",
                    "🔍 Be wary of email attachments, even from known senders. Scan everything before opening!"
                }
            };
        }

        private void ShowWelcomeMessage()
        {
            AddBotMessage("✨ Hello! Welcome to CyberGuard, your personal cybersecurity awareness assistant! ✨\n\n");

            Task.Delay(500).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    AddBotMessage("I'm here to help you stay safe online. I can provide tips about:\n" +
                                 "• Password Security 🔐\n" +
                                 "• Phishing Detection 🎣\n" +
                                 "• Privacy Protection 🔒\n" +
                                 "• Scam Prevention 🚨\n" +
                                 "• Malware Defense 🛡️\n\n" +
                                 "May I have your name?");

                    StatusText.Text = "🤖 Bot: Asking for user name";
                });
            });
        }

        private void AddUserMessage(string message)
        {
            ChatListBox.Items.Add(new ChatMessage(message, true));
            ScrollToBottom();
            conversationHistory.Add($"User: {message}");
            StatusText.Text = $"👤 You: {message.Substring(0, Math.Min(30, message.Length))}...";
        }

        private void AddBotMessage(string message, string sentiment = "")
        {
            ChatListBox.Items.Add(new ChatMessage(message, false, sentiment));
            ScrollToBottom();
            conversationHistory.Add($"Bot: {message}");
        }

        private void ScrollToBottom()
        {
            if (ChatListBox.Items.Count > 0)
            {
                ChatListBox.ScrollIntoView(ChatListBox.Items[ChatListBox.Items.Count - 1]);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                ProcessUserInput();
                e.Handled = true;
            }
        }

        private async void ProcessUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            AddUserMessage(userInput);
            InputTextBox.Clear();

            // Detect sentiment
            string sentiment = DetectSentiment(userInput);

            // Process with memory and context
            string response = await Task.Run(() => chatBot.GetResponse(userInput, sentiment, currentTopic, conversationHistory));

            // Update current topic if detected
            string newTopic = DetectTopic(userInput);
            if (!string.IsNullOrEmpty(newTopic))
            {
                currentTopic = newTopic;
            }

            AddBotMessage(response, sentiment);
            StatusText.Text = "🤖 Bot: Responding...";

            // Auto-scroll
            ScrollToBottom();
        }

        private string DetectSentiment(string input)
        {
            input = input.ToLower();

            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious") ||
                input.Contains("nervous") || input.Contains("concerned") || input.Contains("afraid"))
            {
                return "worried";
            }

            if (input.Contains("curious") || input.Contains("interested") || input.Contains("want to learn") ||
                input.Contains("tell me") || input.Contains("explain"))
            {
                return "curious";
            }

            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("angry") ||
                input.Contains("confused") || input.Contains("hard") || input.Contains("difficult"))
            {
                return "frustrated";
            }

            return "";
        }

        private string DetectTopic(string input)
        {
            input = input.ToLower();

            if (input.Contains("password") || input.Contains("passphrase")) return "password";
            if (input.Contains("phish")) return "phishing";
            if (input.Contains("privacy") || input.Contains("private")) return "privacy";
            if (input.Contains("scam") || input.Contains("fraud")) return "scam";
            if (input.Contains("malware") || input.Contains("virus") || input.Contains("malicious")) return "malware";

            return "";
        }

        private async void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (speechSynthesizer != null)
            {
                // Get the last bot message
                var lastMessage = ChatListBox.Items.OfType<ChatMessage>().LastOrDefault(m => !m.IsUser);
                if (lastMessage != null)
                {
                    StatusText.Text = "🔊 Speaking...";
                    await Task.Run(() =>
                    {
                        speechSynthesizer.Speak(lastMessage.Content);
                    });
                    StatusText.Text = "🤖 Ready";
                }
            }
            else
            {
                MessageBox.Show("Voice synthesis is not available on this system.", "Voice Feature",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatListBox.Items.Clear();
            conversationHistory.Clear();
            currentTopic = "";
            ShowWelcomeMessage();
        }
    }

    public class ChatBotEngine
    {
        private string userName = "";
        private string userInterest = "";
        private Dictionary<string, List<string>> keywordResponses;
        private Random random;

        public ChatBotEngine()
        {
            random = new Random();
            InitializeKeywordResponses();
        }

        private void InitializeKeywordResponses()
        {
            keywordResponses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "🔐 Create passwords with 12+ characters, mixing uppercase, lowercase, numbers, and symbols!",
                    "🔑 Never reuse passwords! Use a password manager to generate and store unique passwords for each account.",
                    "🛡️ Enable Two-Factor Authentication (2FA) - it adds a crucial second layer of security!"
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Always check email sender addresses carefully. Look for misspellings or suspicious domains!",
                    "⚠️ Never click links in unsolicited emails. Type the URL directly into your browser instead.",
                    "📧 Legitimate companies never ask for passwords or sensitive info via email. Be suspicious of urgent requests!"
                },
                ["privacy"] = new List<string>
                {
                    "🔒 Review app permissions on your devices. Many apps request access they don't actually need!",
                    "🕵️ Use a VPN on public Wi-Fi to encrypt your traffic and protect your privacy online.",
                    "📱 Limit what you share on social media - oversharing can help scammers target you!"
                },
                ["scam"] = new List<string>
                {
                    "🚨 If something sounds too good to be true, it is! Never send money to online acquaintances.",
                    "📞 Hang up on calls demanding immediate payment or personal info. Hang up and call back on official numbers.",
                    "💸 Never pay fees to claim prizes - legitimate lotteries don't ask for payment to release winnings!"
                },
                ["malware"] = new List<string>
                {
                    "🛡️ Keep antivirus software updated and run regular scans to detect and remove malware!",
                    "💿 Only download software from official sources. Cracked software often contains malware.",
                    "🔍 Be cautious with email attachments, even from known senders. Scan everything before opening!"
                }
            };
        }

        public string GetResponse(string input, string sentiment, string currentTopic, List<string> conversationHistory)
        {
            input = input.ToLower();

            // Handle follow-up requests
            if (input.Contains("another tip") || input.Contains("tell me more") || input.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(currentTopic) && keywordResponses.ContainsKey(currentTopic))
                {
                    string response = GetRandomResponse(currentTopic);
                    return $"Here's another tip about {currentTopic}: {response}";
                }
                return "Sure! What cybersecurity topic would you like to learn more about? (Try: passwords, phishing, privacy, scams, or malware)";
            }

            // Handle name input (first interaction)
            if (string.IsNullOrEmpty(userName) && !input.Contains("cyber") && !input.Contains("security"))
            {
                userName = char.ToUpper(input[0]) + input.Substring(1);
                return $"Nice to meet you, {userName}! 😊\n\nWhat cybersecurity topic would you like to learn about? I can help with passwords, phishing, privacy, scams, or malware protection.";
            }

            // Store user interest if mentioned
            foreach (var topic in keywordResponses.Keys)
            {
                if (input.Contains(topic))
                {
                    userInterest = topic;
                }
            }

            // Check for keyword matches
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    string response = GetRandomResponse(keyword);

                    // Add sentiment-based personalization
                    if (sentiment == "worried")
                    {
                        response = "It's completely understandable to be worried. " + response +
                                  " Remember, being aware is the first step to staying safe! 💙";
                    }
                    else if (sentiment == "curious")
                    {
                        response = "Great question! I love that you're curious about cybersecurity. " + response +
                                  " Would you like another tip on this topic?";
                    }
                    else if (sentiment == "frustrated")
                    {
                        response = "I understand this can be frustrating. Let me break it down simply: " + response +
                                  " Take a deep breath - you've got this! 💪";
                    }

                    // Add personalization if we know user interest
                    if (!string.IsNullOrEmpty(userInterest) && userInterest != keyword)
                    {
                        response += $"\n\nBy the way, since you're interested in {userInterest}, let me know if you want tips about that too!";
                    }

                    return response;
                }
            }

            // Handle general queries
            if (input.Contains("help") || input.Contains("what can you do"))
            {
                return GetHelpMessage();
            }

            if (input.Contains("thank"))
            {
                return "You're welcome! Stay safe online, " + (string.IsNullOrEmpty(userName) ? "friend" : userName) + "! 🛡️";
            }

            // Default response
            return GetDefaultResponse(sentiment);
        }

        private string GetRandomResponse(string keyword)
        {
            if (keywordResponses.ContainsKey(keyword))
            {
                var responses = keywordResponses[keyword];
                return responses[random.Next(responses.Count)];
            }
            return GetDefaultResponse("");
        }

        private string GetHelpMessage()
        {
            return "I can help you with:\n" +
                   "• 🔐 Password security tips\n" +
                   "• 🎣 How to spot phishing attempts\n" +
                   "• 🔒 Protecting your privacy online\n" +
                   "• 🚨 Recognizing common scams\n" +
                   "• 🛡️ Defending against malware\n\n" +
                   "Just ask me about any of these topics! You can also say 'another tip' or 'tell me more' for additional information.\n\n" +
                   "Try asking: 'Tell me about password safety' or 'How do I spot phishing?'";
        }

        private string GetDefaultResponse(string sentiment)
        {
            if (sentiment == "worried")
            {
                return "I understand your concern about cybersecurity. Would you like to learn about specific threats and how to protect yourself? I can help with passwords, phishing, privacy, scams, or malware protection.";
            }

            return "I'm not sure I understand. Could you rephrase that? I can help with cybersecurity topics like passwords, phishing, privacy, scams, and malware protection. What would you like to know?";
        }
    }
}