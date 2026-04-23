// FILE: LoggerHelper.cs
// ROLE: Buffers Info log lines for flushing into ExtentReports in TearDown.

using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace Task1.Support
{
    public abstract class LoggerHelper
    {
        private readonly List<string> _logBuffer = new();

        public IReadOnlyList<string> GetLogs()  => _logBuffer;
        public void ClearLogs()                  => _logBuffer.Clear();

        public void Info(string message)
        {
            _logBuffer.Add(WebUtility.HtmlEncode(message ?? string.Empty));
            TestContext.WriteLine($"[INFO] {message}");
        }
    }
}
