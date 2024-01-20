using Microsoft.Extensions.Logging;

namespace AsyncDemo.Services
{
    public class CommonLoggerConsole : ICommonLogger, ILogger
    {
        public void TrackException(Exception exception, string message)
        {
            Console.WriteLine($"TrackException:\t {message}\n {exception.Message}");
        }
        public void TrackEvent(string message)
        {
            // Implement your custom event tracking logic here
            Console.WriteLine($"TrackEvent:\t {message}");
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine($"Log:\t {formatter(state, exception)}");
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}

