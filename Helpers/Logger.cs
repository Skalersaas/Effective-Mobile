namespace Effective_Mobile.Helpers
{
    /// <summary>
    /// Class for logging. First you need to configure this class adding paths information to the appsettings.json
    /// Then When logging, it will add information to the files on that path
    /// </summary>
    public class Logger
    {
        private static string DeliveryLog = "";
        private static string DeliveryOrder = "";

        /// <summary>
        /// Configuration method, accepting section gets needed pathes 
        /// </summary>
        /// <param name="sec">Section from appsettings.json</param>
        public static void Configure(IConfigurationSection sec)
        {
            DeliveryLog = sec["DeliveryLog"] ?? "";
            DeliveryOrder = sec["DeliveryOrder"] ?? "";
        }
        public static void Log(LogLevel level, string content)
        {
            if (DeliveryLog == "")
            {
                AbsentPath("DeliveryLog");
                return;
            }
            File.AppendAllText(DeliveryLog, 
                $"[{DateTime.Now}] {level}: {content}\n");
        }
        public static void LogInformation(string content) => Log(LogLevel.Info, content);
        public static void LogWarning(string content) => Log(LogLevel.Warn, content);
        public static void LogError(string content) => Log(LogLevel.Error, content);
        public static void LogOrder(string content)
        {
            if (DeliveryOrder == "")
            {
                AbsentPath("DeliveryOrder");
                return;
            }
            File.AppendAllText(DeliveryOrder, 
                $"[{DateTime.Now}]: {content}\n");
        }
        private static void AbsentPath(string path)
        {
            Console.WriteLine($"\u001b[93mWARNING\u001b[39m: '{path}' path is empty, add log path to the appsetting.json and restart the app");
        }
    }
    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }
}
