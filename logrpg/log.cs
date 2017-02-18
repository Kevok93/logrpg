using System;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using PrintF;

namespace logrpg {
    public static class Log {
        public static ILog instance;
        static Log() {
            SetupLogging();
            instance = log4net.LogManager.GetLogger ("logrpg");
            instance.Debug ("Logger initialized");
        }

        public static void SetupLogging() {
            var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();

            var patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "[%05thread] %-5level %20logger - %message%newline";
            patternLayout.ActivateOptions();

            var console = new ManagedColoredConsoleAppender();
            var TRACE   = new ManagedColoredConsoleAppender.LevelColors(); TRACE.Level = Level.Trace; TRACE.ForeColor = ConsoleColor.White  ;
            var DEBUG   = new ManagedColoredConsoleAppender.LevelColors(); DEBUG.Level = Level.Debug; DEBUG.ForeColor = ConsoleColor.Green  ;
            var INFO    = new ManagedColoredConsoleAppender.LevelColors(); INFO .Level = Level.Info ; INFO .ForeColor = ConsoleColor.Cyan   ;
            var WARN    = new ManagedColoredConsoleAppender.LevelColors(); WARN .Level = Level.Warn ; WARN .ForeColor = ConsoleColor.Yellow ;
            var ERROR   = new ManagedColoredConsoleAppender.LevelColors(); ERROR.Level = Level.Error; ERROR.ForeColor = ConsoleColor.Magenta;
            var FATAL   = new ManagedColoredConsoleAppender.LevelColors(); FATAL.Level = Level.Fatal; FATAL.ForeColor = ConsoleColor.Red    ;

            console.AddMapping(TRACE);
            console.AddMapping(DEBUG);
            console.AddMapping(INFO );
            console.AddMapping(WARN );
            console.AddMapping(ERROR);
            console.AddMapping(FATAL);

            console.ActivateOptions();
            console.Layout = patternLayout;
            hierarchy.Root.AddAppender(console);

            hierarchy.Root.Level = Level.Trace;
            hierarchy.Configured = true;
        }

        public static void Trace(this ILog log, string message, Exception exception) {
            log.Logger.Log(typeof(LogImpl), Level.Trace  , message, exception);
        }
        public static void Verbose(this ILog log, string message, Exception exception) {
            log.Logger.Log(typeof(LogImpl), Level.Verbose, message, exception);
        }

        public static void Trace(this ILog log, string message) {log.Trace(message, null);}
        public static void Verbose(this ILog log, string message) {log.Verbose(message, null);}


        public static void Trace(string message) {instance.Trace(message);}
        public static void Debug(string message) {instance.Debug(message);}
        public static void Info (string message) {instance.Info (message);}
        public static void Warn (string message) {instance.Warn (message);}
        public static void Error(string message) {instance.Error(message);}
        public static void Fatal(string message) {instance.Fatal(message);}

        public static void Trace(string message, Exception e) {instance.Trace(message,e);}
        public static void Debug(string message, Exception e) {instance.Debug(message,e);}
        public static void Info (string message, Exception e) {instance.Info (message,e);}
        public static void Warn (string message, Exception e) {instance.Warn (message,e);}
        public static void Error(string message, Exception e) {instance.Error(message,e);}
        public static void Fatal(string message, Exception e) {instance.Fatal(message,e);}

        public static void FormatTrace(string format, params Object[] args) {instance.Trace(SPrintF.sprintf(format,args));}
        public static void FormatDebug(string format, params Object[] args) {instance.Debug(SPrintF.sprintf(format,args));}
        public static void FormatInfo (string format, params Object[] args) {instance.Info (SPrintF.sprintf(format,args));}
        public static void FormatWarn (string format, params Object[] args) {instance.Warn (SPrintF.sprintf(format,args));}
        public static void FormatError(string format, params Object[] args) {instance.Error(SPrintF.sprintf(format,args));}
        public static void FormatFatal(string format, params Object[] args) {instance.Fatal(SPrintF.sprintf(format,args));}
    }
}