using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityBotWPF
{
    public partial class MainWindow : Window
    {
        private ChatBotManager chatBot;
        private ActivityLogger logger;
        private TaskManager taskManager;
        private QuizManager quizManager;
        private NLPSimulator nlpSimulator;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBot();
            AddBotMessage("🛡️ Welcome to CyberGuard! I'm your cybersecurity awareness assistant.");
            AddBotMessage("💡 I can help you with:\n• Cybersecurity tips\n• Task management\n• Quiz challenges\n• Security advice");
            AddBotMessage("📌 Type 'help' to see all commands or start typing!");
            UpdateTaskCount();
        }

        private void InitializeBot()
        {
            logger = new ActivityLogger();
            chatBot = new ChatBotManager(logger);
            taskManager = new TaskManager(logger);
            quizManager = new QuizManager(logger);
            nlpSimulator = new NLPSimulator();

            logger.LogAction("Application started");
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void ProcessUserInput()
        {
            string userInput = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
                return;

            AddUserMessage(userInput);
            txtInput.Clear();

            // Process the input
            string response = ProcessCommand(userInput);
            AddBotMessage(response);

            // Update status
            UpdateTaskCount();
            txtStatus.Text = $"Last action: {DateTime.Now:HH:mm:ss} • Processing...";
            txtStatus.Text = $"Status: Online • Ready to help";
        }

        private string ProcessCommand(string input)
        {
            input = input.ToLower().Trim();

            // Check for quiz commands
            if (input.Contains("start quiz") || input.Contains("begin quiz") || input.Contains("take quiz"))
            {
                return quizManager.StartQuiz();
            }

            // Check for quiz answers
            if (quizManager.IsQuizActive)
            {
                return quizManager.AnswerQuestion(input);
            }

            // Check for task commands
            if (nlpSimulator.IsTaskCommand(input))
            {
                return taskManager.AddTask(input);
            }

            if (nlpSimulator.IsListTasksCommand(input))
            {
                return taskManager.ListTasks();
            }

            if (nlpSimulator.IsListAllTasksCommand(input))
            {
                return taskManager.ListAllTasks();
            }

            if (nlpSimulator.IsCompleteTaskCommand(input))
            {
                return taskManager.CompleteTask(input);
            }

            if (nlpSimulator.IsDeleteTaskCommand(input))
            {
                return taskManager.DeleteTask(input);
            }

            // Check for log commands
            if (input.Contains("show activity log") || input.Contains("what have you done") ||
                input.Contains("show log") || input.Contains("activity log"))
            {
                return logger.GetLogSummary();
            }

            // Check for help
            if (input == "help" || input == "commands" || input == "what can i ask")
            {
                return GetHelpMessage();
            }

            // Check for greetings
            if (nlpSimulator.IsGreeting(input))
            {
                return $"👋 Hello! I'm CyberGuard, your cybersecurity assistant. How can I help you today?";
            }

            // Check for cybersecurity topics using NLP
            string topicResponse = nlpSimulator.GetCybersecurityResponse(input);
            if (topicResponse != null)
            {
                logger.LogAction($"Provided information about: {input}");
                return topicResponse;
            }

            // Default response with NLP suggestions
            string suggestion = nlpSimulator.GetSuggestion(input);
            return $"🤔 I didn't quite understand that. {suggestion}";
        }

        private string GetHelpMessage()
        {
            return @"📚 **Available Commands:**

**🔐 Cybersecurity Topics:**
• Ask about: passwords, phishing, 2FA, malware, safe browsing

**📋 Task Management:**
• Add task: [task description] (remind me in X days)
• Show my tasks
• Show all tasks
• Complete task #ID
• Delete task #ID

**📝 Quiz:**
• Start quiz

**📜 Activity Log:**
• Show activity log
• What have you done for me?

**💬 General:**
• Hello, Hi, Hey
• Help, Commands

💡 I use NLP to understand different ways you phrase requests!";
        }

        private void AddUserMessage(string message)
        {
            lstMessages.Items.Add(new ChatMessage
            {
                Text = message,
                IsUser = true,
                Timestamp = DateTime.Now
            });
            ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            lstMessages.Items.Add(new ChatMessage
            {
                Text = message,
                IsUser = false,
                Timestamp = DateTime.Now
            });
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (lstMessages.Items.Count > 0)
            {
                lstMessages.ScrollIntoView(lstMessages.Items[lstMessages.Items.Count - 1]);
            }
        }

        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            string response = quizManager.StartQuiz();
            AddBotMessage(response);
        }

        private void BtnShowTasks_Click(object sender, RoutedEventArgs e)
        {
            string response = taskManager.ListTasks();
            AddBotMessage(response);
        }

        private void BtnShowLog_Click(object sender, RoutedEventArgs e)
        {
            string response = logger.GetLogSummary();
            AddBotMessage(response);
        }

        private void BtnClearChat_Click(object sender, RoutedEventArgs e)
        {
            lstMessages.Items.Clear();
            AddBotMessage("🔄 Chat cleared. How can I help you today?");
        }

        private void UpdateTaskCount()
        {
            try
            {
                var tasks = new DatabaseHelper().GetTasks(false);
                txtTaskCount.Text = $"Tasks: {tasks.Count}";
            }
            catch
            {
                txtTaskCount.Text = "Tasks: 0";
            }
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}