using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotWPF
{
    public class ActivityLogger
    {
        private List<LogEntry> logEntries;
        private int maxLogEntries = 100;

        public ActivityLogger()
        {
            logEntries = new List<LogEntry>();
        }

        public void LogAction(string description)
        {
            logEntries.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Description = description
            });

            // Keep log manageable
            if (logEntries.Count > maxLogEntries)
            {
                logEntries.RemoveAt(0);
            }
        }

        public string GetLogSummary(int count = 10)
        {
            if (logEntries.Count == 0)
            {
                return "📜 No activities have been logged yet. Start interacting with me to create a log!";
            }

            var recentEntries = logEntries
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToList();

            string result = "📜 **Recent Activity Log:**\n\n";

            for (int i = 0; i < recentEntries.Count; i++)
            {
                var entry = recentEntries[i];
                result += $"{i + 1}. [{entry.Timestamp:HH:mm:ss}] {entry.Description}\n";
            }

            int totalCount = logEntries.Count;
            if (totalCount > count)
            {
                result += $"\n📊 Showing {count} of {totalCount} entries. Say 'Show all log' to see everything.";
            }
            else
            {
                result += $"\n📊 Total entries: {totalCount}";
            }

            return result;
        }

        public string GetFullLog()
        {
            if (logEntries.Count == 0)
            {
                return "📜 No activities have been logged yet.";
            }

            string result = "📜 **Complete Activity Log:**\n\n";

            for (int i = 0; i < logEntries.Count; i++)
            {
                var entry = logEntries[i];
                result += $"{i + 1}. [{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.Description}\n";
            }

            result += $"\n📊 Total entries: {logEntries.Count}";
            return result;
        }

        public void ClearLog()
        {
            logEntries.Clear();
            LogAction("Activity log cleared");
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
}