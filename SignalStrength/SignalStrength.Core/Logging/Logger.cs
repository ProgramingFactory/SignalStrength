namespace SignalStrength.Core.Logging
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;


    /// <summary>
    /// Simple logger
    /// </summary>
    public class Logger
    {

        static Logger()
        {
#if DEBUG
            LogFileDirectory = Path.Combine(Environment.CurrentDirectory,"Logs");
#else
            LogFileDirectory = Path.Combine(AppConfigurationFile,"Logs");
#endif

            LogFileFullPath= $"{LogFileDirectory}\\{DateTime.Now.ToString("dd.MM.yyyy")}.log";
        }

        /// <summary>
        /// Path where logger save files.
        /// </summary>
        public static string LogFileFullPath { get;private set; }

        /// <summary>
        /// Directory where is \\Logs\\ today file saved.
        /// </summary>
        public static string LogFileDirectory { get;private set; }


        /// <summary>
        ///  Directory  where ConfigurationManager save settings
        /// </summary>
        private static string AppConfigurationFile
        {
            get
            {
                var level = ConfigurationUserLevel.PerUserRoamingAndLocal;
                var configuration = ConfigurationManager.OpenExeConfiguration(level);
                return System.IO.Path.GetDirectoryName(configuration.FilePath);
            }
        }

        /// <summary>
        /// Log to <see cref="Logger.AppConfigurationFile"/>\\Logs\\ today file. 
        /// Must init <see cref="Logger.AppConfigurationFile"/>.
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="level"><see cref="LogLevel"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Log(string message, LogLevel level)
        {
            var log = InitialStringBilder(message, level);
            WriteToFile(log);
        }

        private static string InitialStringBilder(string msg, LogLevel level)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var format = String.Format($"{DateTime.Now.ToString("dd:MM:yyyy   HH:mm:ss:fff")}      {level}       {msg}");
            stringBuilder.AppendFormat(format);
            return stringBuilder.ToString();
        }
        
        private static void WriteToFile(string log)
        {
            var logFile = LogFileFullPath;
           
            try
            {

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(logFile));
                var todayFile = new System.IO.FileInfo(logFile);
                //GrantAccess(todayFile);

                using (var fileW = new System.IO.StreamWriter(logFile, true))
                {
                    fileW.WriteLine(log);
                }
            }

            catch (Exception debug)
            {
                System.Diagnostics.Debug.WriteLine(debug.Message);
            }
        }


        private static void GrantAccess(System.IO.FileInfo file)
        {
            var parentDir = Path.GetDirectoryName(file.FullName);
            if (!Directory.Exists(parentDir)) return;

            DirectoryInfo dInfo = new DirectoryInfo(parentDir);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }


        public enum LogLevel
        {
            LOGGING_OFF = 0x00000000,
            FATAL_EXCEPTION = 0x00000001,
            ERROR_EXCEPTION = 0x00000003,
            WARN_EXCEPTION = 0x00000005,
            TRACE_INFO = 0x00000006,
            DEBUG_INFO = 0x00000008,
            APPLICATION_STARTED = 0x00000010,
            APPLICATION_STOPPED = 0x00000012,
            APPLICATION_RUNNING = 0x00000014,
            INFO = 0x000000016
        }

        public static void UnderlineToday()
        {
            var logFile = LogFileFullPath;
            try
            {

                System.IO.Directory.CreateDirectory(LogFileDirectory);
                //GrantAccess(todayFile);

                using (var fileW = new System.IO.StreamWriter(logFile, true))
                {
                    fileW.WriteLine();
                    fileW.WriteLine($"**********************************| {DateTime.Now.ToString("dd:MM:yyyy  HH:mm:ss")} |**********************************");
                    fileW.WriteLine();
                }
            }

            catch (Exception debug)
            {
                Console.WriteLine(debug.Message);
            }
        }

        public static void DelateLogsOlderOFXXXDays(double days)
        {
            var path = LogFileFullPath;

            if (System.IO.Directory.Exists(path))
            {
                var files = new System.IO.DirectoryInfo(path).EnumerateFiles().OrderBy(x => x.CreationTime);

                foreach (System.IO.FileInfo file in files)
                {
                    //GrantAccess(file);
                    if (file.CreationTime < (DateTime.Now - TimeSpan.FromDays(days)))
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        {
                            Log($"Cant delate logs older of days {days}{Environment.NewLine}" +
                                $"ERROR: {ex.Message}",LogLevel.ERROR_EXCEPTION);
                        }
                    }
                }
            }
        }
    }
}
