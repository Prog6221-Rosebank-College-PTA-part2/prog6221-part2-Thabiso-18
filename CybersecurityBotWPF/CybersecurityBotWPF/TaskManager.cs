using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotWPF
{
    public class TaskManager
    {
        private DatabaseHelper dbHelper;
        private ActivityLogger logger;
        private NLPSimulator nlp;

        public TaskManager(ActivityLogger logger)
        {
            dbHelper = new DatabaseHelper();
            dbHelper.InitializeDatabase();
            this.logger = logger;
            nlp = new NLPSimulator();
        }

        public string AddTask(string userInput)
        {
            // Parse task using NLP
            string taskTitle = nlp.ExtractTaskTitle(userInput);
            string description = ExtractDescription(userInput);
            int reminderDays = nlp.ExtractReminderDays(userInput);
            DateTime? reminderDate = reminderDays > 0 ? DateTime.Now.AddDays(reminderDays) : (DateTime?)null;

            if (string.IsNullOrEmpty(taskTitle))
            {
                return "I couldn't understand the task. Please specify a task title like 'Add task: Enable 2FA'";
            }

            int taskId = dbHelper.AddTask(taskTitle, description, reminderDate);

            if (taskId > 0)
            {
                logger.LogAction($"Task added: '{taskTitle}' (ID: {taskId})" +
                    (reminderDate.HasValue ? $" Reminder set for {reminderDate.Value:yyyy-MM-dd HH:mm}" : ""));

                string response = $"✅ Task added successfully!\n" +
                                 $"📋 Title: {taskTitle}\n" +
                                 $"📝 Description: {description ?? "No description provided"}\n";

                if (reminderDate.HasValue)
                {
                    response += $"⏰ Reminder set for: {reminderDate.Value:yyyy-MM-dd HH:mm}\n";
                }
                else
                {
                    response += "ℹ️ No reminder set. You can set one later if needed.\n";
                }

                response += "\n💡 Would you like to view all your tasks? Just say 'Show my tasks'";
                return response;
            }

            return "❌ Failed to add task. Please try again.";
        }

        public string ListTasks()
        {
            var tasks = dbHelper.GetTasks(false);
            var allTasks = dbHelper.GetTasks(true);

            if (tasks.Count == 0)
            {
                int completedCount = allTasks.Count(t => t.IsCompleted);
                if (completedCount > 0)
                {
                    return $"📋 You have no pending tasks! 🎉\n" +
                           $"You've completed {completedCount} task(s). Great job! 👏\n" +
                           "Say 'Show all tasks' to see everything.";
                }
                return "📋 You have no tasks yet. Add one by saying 'Add task: [your task]'";
            }

            string response = "📋 Your Cybersecurity Tasks:\n\n";
            response += "─" + new string('─', 50) + "\n";

            foreach (var task in tasks)
            {
                response += $"📌 #{task.Id}: {task.Title}\n";
                response += $"   📝 {task.Description ?? "No description"}\n";
                response += $"   {task.ReminderInfo}\n";
                response += $"   Status: {task.Status}\n";
                response += "─" + new string('─', 40) + "\n";
            }

            int pendingCount = tasks.Count;
            int totalCount = allTasks.Count;

            response += $"\n📊 Summary: {pendingCount} pending, {totalCount - pendingCount} completed";
            response += "\n\n💡 Actions: Say 'Complete task #ID' or 'Delete task #ID'";

            logger.LogAction($"Viewed tasks: {pendingCount} pending, {totalCount - pendingCount} completed");
            return response;
        }

        public string ListAllTasks()
        {
            var allTasks = dbHelper.GetTasks(true);

            if (allTasks.Count == 0)
            {
                return "📋 No tasks found. Start adding tasks to stay organized!";
            }

            string response = "📋 All Tasks:\n\n";
            response += "─" + new string('─', 50) + "\n";

            foreach (var task in allTasks)
            {
                response += $"📌 #{task.Id}: {task.Title}\n";
                response += $"   📝 {task.Description ?? "No description"}\n";
                response += $"   {task.ReminderInfo}\n";
                response += $"   Status: {task.Status}\n";
                response += "─" + new string('─', 40) + "\n";
            }

            return response;
        }

        public string CompleteTask(string userInput)
        {
            int taskId = nlp.ExtractTaskId(userInput);

            if (taskId <= 0)
            {
                return "❌ Please specify a valid task ID. Example: 'Complete task #3'";
            }

            var tasks = dbHelper.GetTasks(true);
            var task = tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                return $"❌ Task #{taskId} not found. Use 'Show my tasks' to see your task IDs.";
            }

            if (task.IsCompleted)
            {
                return $"✅ Task #{taskId} is already completed! Great work! 🎉";
            }

            if (dbHelper.UpdateTaskStatus(taskId, true))
            {
                logger.LogAction($"Task completed: '{task.Title}' (ID: {taskId})");
                return $"✅ Task #{taskId} marked as completed! 🎉\n" +
                       $"📌 '{task.Title}'\n" +
                       "Keep up the great work on your cybersecurity journey! 💪";
            }

            return "❌ Failed to complete task. Please try again.";
        }

        public string DeleteTask(string userInput)
        {
            int taskId = nlp.ExtractTaskId(userInput);

            if (taskId <= 0)
            {
                return "❌ Please specify a valid task ID. Example: 'Delete task #3'";
            }

            var tasks = dbHelper.GetTasks(true);
            var task = tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                return $"❌ Task #{taskId} not found. Use 'Show all tasks' to see your task IDs.";
            }

            if (dbHelper.DeleteTask(taskId))
            {
                logger.LogAction($"Task deleted: '{task.Title}' (ID: {taskId})");
                return $"🗑️ Task #{taskId} deleted successfully.\n" +
                       $"📌 '{task.Title}' has been removed from your list.";
            }

            return "❌ Failed to delete task. Please try again.";
        }

        private string ExtractDescription(string input)
        {
            // Try to find description after a colon or dash
            if (input.Contains(":"))
            {
                var parts = input.Split(':');
                if (parts.Length >= 2)
                {
                    return parts[1].Trim();
                }
            }

            if (input.Contains("-"))
            {
                var parts = input.Split('-');
                if (parts.Length >= 2)
                {
                    return parts[1].Trim();
                }
            }

            return null;
        }
    }
}