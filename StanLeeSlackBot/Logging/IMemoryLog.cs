using Common.Logging;

namespace StanLeeSlackBot.Logging
{
    public interface IMemoryLog : ILog
    {
        string[] FullLog();
    }
}