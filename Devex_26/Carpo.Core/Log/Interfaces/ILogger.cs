namespace Carpo.Core.Log
{
    public interface ILogger
    {
        LogLevel LogLevel { get; }

        void Debug(string opis, string text, params object[] args);

        void Info(string opis, string text, params object[] args);

        void Warn(string opis, string text, params object[] args);

        void Error(string text, string opis, params object[] args);
    }
}