using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace CybersecurityBotWPF
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cybersecurity_tasks.db");
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void InitializeDatabase()
        {
            try
            {
                if (!File.Exists("cybersecurity_tasks.db"))
                {
                    SQLiteConnection.CreateFile("cybersecurity_tasks.db");
                }

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Tasks (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Description TEXT,
                            ReminderDate TEXT,
                            IsCompleted INTEGER DEFAULT 0,
                            CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Tasks (Title, Description, ReminderDate) 
                        VALUES (@Title, @Description, @ReminderDate);
                        SELECT last_insert_rowid();";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ReminderDate",
                            reminderDate.HasValue ? (object)reminderDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);

                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding task: {ex.Message}");
                return -1;
            }
        }

        public List<Task> GetTasks(bool includeCompleted = false)
        {
            var tasks = new List<Task>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = includeCompleted ?
                        "SELECT * FROM Tasks ORDER BY IsCompleted, CreatedDate DESC" :
                        "SELECT * FROM Tasks WHERE IsCompleted = 0 ORDER BY CreatedDate DESC";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new Task
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    ReminderDate = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)),
                                    IsCompleted = reader.GetInt32(4) == 1,
                                    CreatedDate = DateTime.Parse(reader.GetString(5))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tasks: {ex.Message}");
            }
            return tasks;
        }

        public bool UpdateTaskStatus(int taskId, bool isCompleted)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Tasks SET IsCompleted = @IsCompleted WHERE Id = @Id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IsCompleted", isCompleted ? 1 : 0);
                        command.Parameters.AddWithValue("@Id", taskId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task status: {ex.Message}");
                return false;
            }
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @Id";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", taskId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
                return false;
            }
        }
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Status => IsCompleted ? "✅ Completed" : "⏳ Pending";
        public string ReminderInfo => ReminderDate.HasValue ?
            $"⏰ Reminder: {ReminderDate.Value:yyyy-MM-dd HH:mm}" :
            "ℹ️ No reminder set";
    }
}