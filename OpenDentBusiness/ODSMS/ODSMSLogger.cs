using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Forms; // Assuming this is a Windows Forms application

namespace OpenDentBusiness.ODSMS
{
    internal class ODSMSLogger
    {
        private static readonly Lazy<ODSMSLogger> _instance = new Lazy<ODSMSLogger>(() => new ODSMSLogger());

        public static ODSMSLogger Instance => _instance.Value;

        private readonly string _logDirectory = @"L:\od_logs";
        private StreamWriter _writer;
        private string _currentLogFile;

        private ODSMSLogger()
        {
            try
            {
                Directory.CreateDirectory(_logDirectory);
                OpenLogFile();
            }
            catch (Exception ex)
            {
                LogToEventLog($"Failed to initialize log directory or log file: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void OpenLogFile()
        {
            try
            {
                string logFileName = $"log_{DateTime.Now:yyyyMMdd}.json";
                _currentLogFile = Path.Combine(_logDirectory, logFileName);

                if (_writer != null)
                {
                    _writer.Close();
                }

                // Open file with shared access, just in case there's two
                FileStream logFileStream = new FileStream(_currentLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                _writer = new StreamWriter(logFileStream);
            }
            catch (Exception ex)
            {
                LogToEventLog($"Failed to open log file: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void CheckForNewDay()
        {
            try
            {
                string newLogFileName = $"log_{DateTime.Now:yyyyMMdd}.json";
                string newLogFilePath = Path.Combine(_logDirectory, newLogFileName);

                if (_currentLogFile != newLogFilePath)
                {
                    OpenLogFile();
                }
            }
            catch (Exception ex)
            {
                LogToEventLog($"Failed to check for new log file: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Log(string message, EventLogEntryType severity = EventLogEntryType.Warning, bool logToConsole = true, bool logToEventLog = true, bool logToFile = true)
        {
            try
            {
                var logEntry = new
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    ComputerName = Environment.MachineName,
                    Severity = severity.ToString(),
                    Message = message
                };

                if (logToConsole)
                {
                    Console.WriteLine($"{logEntry.Timestamp} [{logEntry.Severity}] {logEntry.Message}");
                }

                if (logToEventLog)
                {
                    LogToEventLog(message, severity);
                }

                if (logToFile)
                {
                    CheckForNewDay();
                    string json = JsonConvert.SerializeObject(logEntry);
                    _writer.WriteLine(json);
                    _writer.Flush();
                }
            }
            catch (Exception ex)
            {
                LogToEventLog($"Failed to write log message: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Close()
        {
            try
            {
                _writer?.Close();
            }
            catch (Exception ex)
            {
                LogToEventLog($"Failed to close log file: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogToEventLog(string message, EventLogEntryType severity)
        {
            try
            {
                EventLog.WriteEntry("ODSMS", message, severity, 101, 1, new byte[10]);
            }
            catch
            {
                MessageBox.Show("Major failure with logging. Please restart", "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
