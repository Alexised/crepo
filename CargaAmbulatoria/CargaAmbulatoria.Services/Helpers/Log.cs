using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargaAmbulatoria.Services.Helpers.Log
{
    public class Log
    {
        /// <summary>
        ///     Loggers may be assigned levels. The set of possible levels, that is DEBUG, INFO, WARN, ERROR and FATAL
        ///     are defined in the org.apache.log4j.Level class.
        ///     Esta librería está diseñada bajo el estándar LOG4J y facilita el manejo de los archivos de registro de
        ///     eventos.El uso de lalibrería permitirá guardar registro de eventos con diferentes niveles, los cuales
        ///     se describirán a continuación con sus respectivos ejemplos de uso.
        /// </summary>
        public enum LogSeverity
        {
            /// <summary>
            ///     The TRACE Level designates finer-grained informational events than the DEBUG
            ///     En este nivel se deberán configurar aquellos mensajes que permitan obtener un detalle minucioso de la
            ///     ejecución de la automatización; estado de variables, resultados de validaciones, ingreso o
            ///     salida del algún ciclo.
            /// </summary>
            Trace = 0,

            /// <summary>
            ///     The DEBUG Level designates fine-grained informational events that are most useful to debug an application.
            ///     En este nivel se deberán configurar aquellos mensajes que permitan obtener una idea del ciclo de ejecución
            ///     de la aplicación, ingreso a funciones y valor de los parámetros con las que fueron llamadas, salida de
            ///     funciones y resultado devuelto de la misma.
            /// </summary>
            Debug = 1,

            /// <summary>
            ///     The INFO level designates informational messages that highlight the progress of the application at
            ///     coarse-grained level.
            ///     Eneste nivel se debe configurarmensajes genéricos relacionados con el proceso automatizado, inicio del
            ///     proceso, estados intermedios del proceso, resultado del proceso.
            /// </summary>
            Info = 2,

            /// <summary>
            ///     The WARN level designates potentially harmful situations.
            ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como una excepciónen la
            ///     automatización; pero que no afectan la ejecución de la misma.
            /// </summary>
            Warn = 3,

            /// <summary>
            ///     The ERROR level designates error events that might still allow the application to continue running.
            ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como un erroren la automatización;
            ///     detienen la ejecución del proceso pero no necesariamente provocan elcierre de la automatización.
            /// </summary>
            Error = 4,

            /// <summary>
            ///     The FATAL level designates very severe error events that will presumably lead the application to abort.
            ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como un error en la automatización;
            ///     provocan el cierre de la automatización, por ser requisito indispensable de la ejecución de la misma.
            /// </summary>
            Fatal = 5
        }

        private const long BufferSize = 4096; //ZipFile
        private const long LogFileSize = 5242880; //LOG Maximum allowed size
        private readonly string[] logLevelstring = { "TRACE", "DEBUG", "INFO", "WARN ", "ERROR", "FATAL" };
        private static Log log = new Log();
        private bool logExists;
        private int minLevel;
        private static bool isMultiLog = false;
        private static string customLogName = string.Empty;
        private static string baseDirectory = null;



        /// <summary>
        ///     This will initialize the Log class
        /// </summary>
        /// <param name="application"></param>
        /// <param name="multiLog"></param>
        /// <param name="directory">directory to save the logFile, (add the \\ at the end)</param>
        /// <param name="customLogName"></param>
        public Log()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            const string logFileExtension = ".log";
            string logFileName = "UnicomerPayments";
            this.LastLogName = $"{baseDirectory}{logFileName}{logFileExtension}";
        }

        /// <summary>
        ///     Path of log file location
        /// </summary>
        public string LastLogName { get; set; }
        private static string TemporaryLogName { get; set; }

        /// <summary>
        ///     Sets the log file location to <paramref name="sFileName" /> and determins if the file already exists.
        /// </summary>
        /// <param name="sFileName">New log file location</param>
        public void LogFile(string sFileName)
        {
            this.LastLogName = sFileName;
            this.logExists = File.Exists(sFileName);
        }

        /// <summary>
        ///     Creates a new emptry file in <paramref name="path" /> if it does not yet exist.
        /// </summary>
        /// <param name="path">Log file path</param>
        public void CreateFileLog(string path)
        {
            if (!File.Exists(path))
            {
                using (var create = File.Create(path))
                {
                }
            }
        }

        /// <summary>
        ///     Converts a string from "TRACE" to 0, "DEBUG" to 1, "INFO" to 2, "WARN " to 3, "ERROR" to 4 and "FATAL" to 5
        /// </summary>
        /// <param name="levelStr">"TRACE", "DEBUG", "INFO", "WARN ", "ERROR", "FATAL"</param>
        /// <returns>Value from 0 to 5, or -1 if an unrecognised string was given</returns>
        private int LogLevel(string levelStr)
        {
            try
            {
                int indice = Array.IndexOf(this.logLevelstring, levelStr);

                return indice;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        ///     Converts a int to string, from 0 to "TRACE", 1 to "DEBUG", 2 to "INFO", 3 TO "WARN", 4 to "ERROR", 5 to "FATAL"
        /// </summary>
        /// <param name="iLevel">Log-severity level from 0 (TRACE) to 5 (FATAL)</param>
        /// <returns>
        ///     String with "TRACE", "DEBUG", "INFO", "WARN ", "ERROR", "FATAL", or "" if value is lower than 0 or higher than
        ///     5
        /// </returns>
        private string LogLevelToString(int iLevel)
        {
            if (iLevel < 0 || iLevel > this.logLevelstring.Length - 1)
            {
                return "";
            }
            return this.logLevelstring[iLevel];
        }

        /// <summary>
        ///     Función utilizada para inicializar el archivo de LOG
        /// </summary>
        public bool LogStart()
        {
            try
            {
                this.LogMessageUnformatted("");
                this.LogMessageUnformatted("Archivo de registro de eventos");
                this.LogMessageUnformatted("");
                this.LogInfo(Directory.GetCurrentDirectory(), "Iniciado.");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Logs a message to the set Log file location at TRACE-severity
        ///     The TRACE Level designates finer-grained informational events than the DEBUG
        ///     En este nivel se deberán configurar aquellos mensajes que permitan obtener un detalle minucioso de la
        ///     ejecución de la automatización; estado de variables, resultados de validaciones, ingreso o
        ///     salida del algún ciclo.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogTrace(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogTrace_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }

        /// <summary>
        ///     Logs a message to the set Log file location at DEBUG-severity
        ///     The DEBUG Level designates fine-grained informational events that are most useful to debug an application.
        ///     En este nivel se deberán configurar aquellos mensajes que permitan obtener una idea del ciclo de ejecución
        ///     de la aplicación, ingreso a funciones y valor de los parámetros con las que fueron llamadas, salida de
        ///     funciones y resultado devuelto de la misma.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogDebug(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogDebug_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }

        /// <summary>
        ///     Logs a message to the set Log file location at INFO-severity
        ///     The INFO level designates informational messages that highlight the progress of the application at
        ///     coarse-grained level.
        ///     Eneste nivel se debe configurarmensajes genéricos relacionados con el proceso automatizado, inicio del
        ///     proceso, estados intermedios del proceso, resultado del proceso.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogInfo(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogInfo_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }

        /// <summary>
        ///     Logs a message to the set Log file location at WARN-severity
        ///     The WARN level designates potentially harmful situations.
        ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como una excepciónen la
        ///     automatización; pero que no afectan la ejecución de la misma.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogWarn(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogWarn_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }

        /// <summary>
        ///     Logs a message to the set Log file location at ERROR-severity
        ///     The ERROR level designates error events that might still allow the application to continue running.
        ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como un erroren la automatización;
        ///     detienen la ejecución del proceso pero no necesariamente provocan elcierre de la automatización.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogError(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogError_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }


        /// <summary>
        ///     Logs a message to the set Log file location at FATAL-severity
        ///     The FATAL level designates very severe error events that will presumably lead the application to abort.
        ///     En este nivel se deberán configurar aquellos mensajes, que son reconocidos como un error en la automatización;
        ///     provocan el cierre de la automatización, por ser requisito indispensable de la ejecución de la misma.
        /// </summary>
        /// <param name="nombreMetodo">Method name of the currently executed method</param>
        /// <param name="evento">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        public void LogFatal(string nombreMetodo, string evento, string sMessage = "")
        {
            this.LogFatal_old(nombreMetodo + " -> " + evento + " -> " + sMessage);
        }

        /// <summary>
        ///     Logs the message, with the method name and event with given severity
        /// </summary>
        /// <param name="method">Method name of the currently executed method</param>
        /// <param name="event">The event that is being logged</param>
        /// <param name="sMessage">An optional message.</param>
        /// <param name="severity">
        ///     The severity of the event, according to the Log4j standard.
        ///     Defaults to INFO if not set
        /// </param>
        /// <param name="path"></param>
        public static void LogEvent(string method, string @event, string sMessage = "",
            LogSeverity severity = LogSeverity.Info, string path = "")
        {
            try
            {
                log = new Log();
                log.LastLogName = !string.IsNullOrEmpty(path) ? path : log.LastLogName;
                switch (severity)
                {
                    case LogSeverity.Trace:
                        log.LogTrace(method, @event, sMessage);
                        break;
                    case LogSeverity.Debug:
                        log.LogDebug(method, @event, sMessage);
                        break;
                    case LogSeverity.Info:
                        log.LogInfo(method, @event, sMessage);
                        break;
                    case LogSeverity.Warn:
                        log.LogWarn(method, @event, sMessage);
                        break;
                    case LogSeverity.Error:
                        log.LogError(method, @event, sMessage);
                        break;
                    case LogSeverity.Fatal:
                        log.LogFatal(method, @event, sMessage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("severity");
                }
                GC.SuppressFinalize(log);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception thrown while trying to write log event to file: {ex.Message}");
            }
        }

        public void LogMyEvent(string method, string @event, Exception exception,
            LogSeverity severity = LogSeverity.Error)
        {
            while (exception != null)
            {
                switch (severity)
                {
                    case LogSeverity.Trace:
                        this.LogTrace(method, @event, exception.Message);
                        break;
                    case LogSeverity.Debug:
                        this.LogDebug(method, @event, exception.Message);
                        break;
                    case LogSeverity.Info:
                        this.LogInfo(method, @event, exception.Message);
                        break;
                    case LogSeverity.Warn:
                        this.LogWarn(method, @event, exception.Message);
                        break;
                    case LogSeverity.Error:
                        this.LogError(method, @event, exception.Message);
                        break;
                    case LogSeverity.Fatal:
                        this.LogFatal(method, @event, exception.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
                }
                exception = exception.InnerException;
            }

        }


        public static void LogEvent(string method, string @event, Exception exception,
            LogSeverity severity = LogSeverity.Error, string path = "")
        {
            LogEvent(method, @event, exception, out string _, severity, path);
        }

        public static void LogEvent(string method, string @event, Exception exception, out string messages,
            LogSeverity severity = LogSeverity.Error, string path = "")
        {
            messages = "";
            while (exception != null)
            {
                messages += exception.Message + "|";
                LogEvent(method, @event, exception.Message, severity, path);
                exception = exception.InnerException;
            }
            messages = messages.TrimEnd('|');
        }


        public static void StaticError(string methodName, string evento, string sMessage = "", string path = "")
        {
            try
            {
                log = new Log();
                log.LastLogName = !string.IsNullOrEmpty(path) ? path : log.LastLogName;
                log.LogError(methodName, evento, sMessage);
                GC.SuppressFinalize(log);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception thrown while trying to write log event to file: {ex.Message}");
            }
        }

        private void LogTrace_old(string sMessage)
        {
            this.LogMessage(0, sMessage);
        }

        private void LogDebug_old(string sMessage)
        {
            this.LogMessage(1, sMessage);
        }

        private void LogInfo_old(string sMessage)
        {
            this.LogMessage(2, sMessage);
        }

        private void LogWarn_old(string sMessage)
        {
            this.LogMessage(3, sMessage);
        }

        private void LogError_old(string sMessage)
        {
            this.LogMessage(4, sMessage);
        }

        private void LogFatal_old(string sMessage)
        {
            this.LogMessage(5, sMessage);
        }

        /// <summary>
        ///     Logs a message to the log with the given severity level.
        /// </summary>
        /// <param name="iLevel">
        ///     Severity level: must be between 0 inclusive and 5
        ///     inclusive, or will return method without writing to log. 0 = "TRACE", 1 = "DEBUG", 2 = "INFO", 3 = "WARN", 4 =
        ///     "ERROR", 5 = "FATAL"
        /// </param>
        /// <param name="sMessage">The message to be written to the log</param>
        private void LogMessage(int iLevel, string sMessage = "")
        {
            this.minLevel = (int)LogSeverity.Trace;

            if (iLevel < this.minLevel)
            {
                return;
            }

            sMessage = this.LogFormatDefault(iLevel, sMessage);

            this.LogMessageUnformatted(sMessage);
        }

        private void LogMessageUnformatted(string sMessage = "")
        {
            if (File.Exists(this.LastLogName) == false)
            {
                StreamWriter newFile = File.CreateText(this.LastLogName);
                newFile.Dispose();
            }

            this.LogFile(this.LastLogName);

            if (this.FileGetSize(this.LastLogName) > LogFileSize)
            {
                Array arr = this.PathSplit(this.LastLogName);

                string disk = ((string[])(arr))[1];
                string path = ((string[])(arr))[2];
                string filename = ((string[])(arr))[3];
                string extension = ((string[])(arr))[4];

                string temp = path + " \\auto_logs\\" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + "-" +
                              Guid.NewGuid() +
                              filename;

                if (this.FileMove(this.LastLogName, temp + extension))
                {
                    this.AddFileToZip(temp + ".zip", temp + extension);

                    File.Delete(temp + extension);

                    string[] fileList = Directory.GetFiles(path + "\\auto_logs");

                    foreach (var item in fileList)
                    {
                        DateTime response = File.GetCreationTime(item);
                        DateTime currentDate = DateTime.Now;
                        DateTime fileDate = response;

                        string pathOldLog = disk + path + @"auto_logs\" + item;

                        double difference = (currentDate - fileDate).TotalHours;

                        if (response.Year == DateTime.Now.Year)
                        {
                            if (difference > 200)
                            {
                                File.Delete(pathOldLog);
                            }
                        }
                        else
                        {
                            if (difference > 9000)
                            {
                                File.Delete(pathOldLog);
                            }
                        }
                    }
                }
                this.LogFile(this.LastLogName);
            }

            try
            {
                var sw = new StreamWriter(string.IsNullOrWhiteSpace(TemporaryLogName) ? this.LastLogName : TemporaryLogName, true);
                sw.WriteLine(sMessage);
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
            catch (Exception)
            {
            }

        }

        private string LogFormatDefault(int sLevel, string sMessage)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            int millisecond = DateTime.Now.Millisecond;
            string computerName = Environment.MachineName;
            string logonDomain = Environment.UserDomainName;
            string userName = Environment.UserName;

            LogSeverity logLevel = LogSeverity.Debug;

            switch (sLevel)
            {
                case 0:
                    logLevel = LogSeverity.Trace;
                    break;
                case 1:
                    logLevel = LogSeverity.Debug;
                    break;
                case 2:
                    logLevel = LogSeverity.Info;
                    break;
                case 3:
                    logLevel = LogSeverity.Warn;
                    break;
                case 4:
                    logLevel = LogSeverity.Error;
                    break;
                case 5:
                    logLevel = LogSeverity.Fatal;
                    break;
            }

            return "[" + year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second + "." + millisecond +
                   " " +
                   computerName + " " + logonDomain + @"\" + userName + " " + logLevel + "] " + sMessage;
        }

        private Array PathSplit(string lastLogName)
        {
            string fullRuta = Path.GetFullPath(lastLogName);
            string disco = Path.GetPathRoot(lastLogName);
            string ruta = Path.GetDirectoryName(lastLogName);
            string archivo = Path.GetFileNameWithoutExtension(lastLogName);
            string extension = Path.GetExtension(lastLogName);

            string[] array = { fullRuta, disco, ruta, archivo, extension };

            return array;
        }

        private void AddFileToZip(string zipFilename, string fileToAdd)
        {
            using (Package zip = Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                string destFilename = ".\\" + Path.GetFileName(fileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                {
                    zip.DeletePart(uri);
                }
                PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
                using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                {
                    using (Stream dest = part.GetStream())
                    {
                        CopyStream(fileStream, dest);
                    }
                }
            }
        }

        private static void CopyStream(FileStream inputStream, Stream outputStream)
        {
            long bufferSize = inputStream.Length < BufferSize ? inputStream.Length : BufferSize;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;
            }
        }

        private bool FileMove(string lastLogName, string p1)
        {
            if (String.IsNullOrEmpty(p1) || String.IsNullOrEmpty(lastLogName))
            {
                return false;
            }
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(p1)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(p1));
                }

                File.Move(lastLogName, p1);

                using (var create = File.Create(lastLogName))
                {
                }
                ;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private long FileGetSize(string lastLogName)
        {
            FileInfo fileInf = new FileInfo(lastLogName);
            long size = fileInf.Length;

            return size;
        }
    }
}
