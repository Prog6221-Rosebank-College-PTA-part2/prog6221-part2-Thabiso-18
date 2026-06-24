using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotWPF
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool isQuizActive;
        private ActivityLogger logger;

        public QuizManager(ActivityLogger logger)
        {
            this.logger = logger;
            InitializeQuestions();
            ResetQuiz();
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                // Multiple Choice Questions
                new QuizQuestion
                {
                    Question = "What is the strongest type of password?",
                    Options = new List<string> { "123456", "password123", "Tr0ub4d0ur&H0rs3", "qwerty" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A strong password uses a mix of uppercase, lowercase, numbers, and symbols. 'Tr0ub4d0ur&H0rs3' is complex and memorable."
                },
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report it as phishing and delete", "Ignore it" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Legitimate companies never ask for passwords via email. Reporting phishing helps protect others from scams."
                },
                new QuizQuestion
                {
                    Question = "How often should you update your passwords?",
                    Options = new List<string> { "Never", "Every 5 years", "Every 3-6 months", "Only when forced" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Regular password updates (every 3-6 months) help protect against undetected breaches and reduce risk."
                },
                new QuizQuestion
                {
                    Question = "What is Two-Factor Authentication (2FA)?",
                    Options = new List<string> { "A second password", "A security code sent to your phone/email", "A fingerprint scan only", "A physical key" },
                    CorrectAnswerIndex = 1,
                    Explanation = "2FA adds a second layer of security by requiring both your password and a verification code sent to your device."
                },
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new List<string> { "Professional language", "Personal greeting", "Urgent action required", "Known sender address" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Phishing emails often create urgency to pressure you into acting without thinking. Always verify suspicious emails."
                },
                new QuizQuestion
                {
                    Question = "It's safe to use the same password for multiple accounts as long as they're not important.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Never reuse passwords! If one account is compromised, all accounts using the same password are at risk."
                },
                new QuizQuestion
                {
                    Question = "Public Wi-Fi is completely safe for banking as long as the website has HTTPS.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Public Wi-Fi can be compromised. Always use a VPN for sensitive transactions, even with HTTPS."
                },
                new QuizQuestion
                {
                    Question = "A strong password should be at least 12 characters long.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "12+ characters with mixed case, numbers, and symbols provides strong protection against brute force attacks."
                },
                new QuizQuestion
                {
                    Question = "You should never click on links in unsolicited text messages.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Text message phishing (smishing) is common. Always verify the sender and don't click suspicious links."
                },
                new QuizQuestion
                {
                    Question = "Software updates are unnecessary and often introduce more bugs.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Security updates patch vulnerabilities. Always install updates to protect against known threats."
                },
                new QuizQuestion
                {
                    Question = "A strong password should contain your birth date for easy remembering.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Personal information like birth dates are easy for attackers to guess. Never use personal info in passwords."
                },
                new QuizQuestion
                {
                    Question = "Using a password manager is a secure way to store passwords.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Password managers generate and store unique, complex passwords securely. They're highly recommended for security."
                }
            };
        }

        public void ResetQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            isQuizActive = false;
        }

        public string StartQuiz()
        {
            ResetQuiz();
            isQuizActive = true;
            logger.LogAction("Quiz started");
            return GetNextQuestion();
        }

        public string GetNextQuestion()
        {
            if (!isQuizActive)
                return "The quiz hasn't been started. Say 'Start quiz' to begin!";

            if (currentQuestionIndex >= questions.Count)
            {
                return EndQuiz();
            }

            var q = questions[currentQuestionIndex];
            string response = $"📝 Question {currentQuestionIndex + 1}/{questions.Count}: {q.Question}\n\n";

            for (int i = 0; i < q.Options.Count; i++)
            {
                char optionLetter = (char)('A' + i);
                response += $"{optionLetter}) {q.Options[i]}\n";
            }

            response += $"\n💡 Score so far: {score}/{currentQuestionIndex}";
            return response;
        }

        public string AnswerQuestion(string userInput)
        {
            if (!isQuizActive)
                return "The quiz hasn't been started. Say 'Start quiz' to begin!";

            if (currentQuestionIndex >= questions.Count)
            {
                return EndQuiz();
            }

            var currentQuestion = questions[currentQuestionIndex];
            int userAnswerIndex = ParseAnswer(userInput);

            if (userAnswerIndex < 0 || userAnswerIndex >= currentQuestion.Options.Count)
            {
                return "❌ Please enter a valid answer (A, B, C, D, or 1-4).";
            }

            bool isCorrect = userAnswerIndex == currentQuestion.CorrectAnswerIndex;

            if (isCorrect)
            {
                score++;
                logger.LogAction($"Quiz: Question {currentQuestionIndex + 1} answered correctly");
            }
            else
            {
                logger.LogAction($"Quiz: Question {currentQuestionIndex + 1} answered incorrectly");
            }

            string response = isCorrect ? "✅ Correct! " : "❌ Incorrect. ";
            response += currentQuestion.Explanation + "\n\n";

            currentQuestionIndex++;

            if (currentQuestionIndex >= questions.Count)
            {
                response += EndQuiz();
            }
            else
            {
                response += GetNextQuestion();
            }

            return response;
        }

        private string EndQuiz()
        {
            isQuizActive = false;
            int totalQuestions = questions.Count;
            double percentage = (double)score / totalQuestions * 100;

            string feedback;
            if (percentage >= 90)
                feedback = "🌟 Outstanding! You're a cybersecurity expert! 🏆";
            else if (percentage >= 70)
                feedback = "👏 Great job! You have strong cybersecurity knowledge!";
            else if (percentage >= 50)
                feedback = "📚 Good effort! Keep learning to improve your security awareness!";
            else
                feedback = "💪 Keep practicing! Cybersecurity is important for everyone!";

            logger.LogAction($"Quiz completed: Score {score}/{totalQuestions} ({percentage:F1}%)");

            return $"🏁 Quiz Complete!\n" +
                   $"📊 Final Score: {score}/{totalQuestions} ({percentage:F1}%)\n" +
                   $"💬 {feedback}\n\n" +
                   "🔄 Say 'Start quiz' to play again!\n" +
                   "📚 Ask me about specific topics to learn more!";
        }

        private int ParseAnswer(string input)
        {
            input = input.Trim().ToUpper();

            // Check for letter answers (A, B, C, D)
            if (input.Length == 1 && char.IsLetter(input[0]))
            {
                int index = input[0] - 'A';
                return index;
            }

            // Check for number answers (1-4)
            if (int.TryParse(input, out int number))
            {
                return number - 1;
            }

            // Check for full words (e.g., "option a", "answer a")
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                if (word.Length == 1 && char.IsLetter(word[0]))
                {
                    int index = word[0] - 'A';
                    if (index >= 0 && index < 4)
                        return index;
                }
            }

            return -1;
        }

        public bool IsQuizActive => isQuizActive;
        public int GetCurrentQuestionIndex() => currentQuestionIndex;
        public int GetTotalQuestions() => questions.Count;
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }
}