using Microsoft.Extensions.Logging;

namespace HardwareStore.Web.Logging;

public sealed class PlainTextFileLoggerProvider(string filePath) : ILoggerProvider
{
    private readonly object _lock = new();

    public ILogger CreateLogger(string categoryName) => new PlainTextFileLogger(filePath, categoryName, _lock);

    public void Dispose()
    {
    }

    private sealed class PlainTextFileLogger(string path, string categoryName, object sync) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line = $"{DateTime.UtcNow:O} [{logLevel}] {categoryName}: {formatter(state, exception)}{Environment.NewLine}";
            lock (sync)
            {
                File.AppendAllText(path, line);
            }
        }
    }
}
