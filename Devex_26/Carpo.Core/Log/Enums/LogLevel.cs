namespace Carpo.Core.Log
{
    public enum LogLevel
    {
        Test = 33,
        Debug = 6,
        Info = 5,
        InfoData = 4,
        Warn = 3,
        Error = 2,
        InfoCore = 1,
        None = 0
    }

    public enum LogType
    {
        InfoData,
        Warning,
        Error
    }

    //public enum LogType
    //{
    //    Layout = 8,
    //    Control = 7,
    //    Data = 6,
    //    Statemant = 5,
    //    Finder = 4,
    //    Event = 3,
    //    Logic = 2,
    //    Core = 1,
    //    Undefined = 0
    //}
}