using GoToIt.Constants;
using GoToIt.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoToIt.Logging
{
    /// <summary>
    /// Lots Application Insights telemetry.
    /// </summary>
    static class Telemetry
    {
        private static TelemetryConfiguration _config = TelemetryConfiguration.CreateDefault();
        private static TelemetryClient _client;

        /// <summary>
        /// Initializes the Telemetry.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        public static void Initialize(EnvDTE80.DTE2 dte) {
            _config = TelemetryConfiguration.CreateDefault();
            _client = new TelemetryClient(_config) {
                InstrumentationKey = Signatures.ApplicationInsights_InstrumentationKey
            };

            _client.Context.User.Id = (Environment.MachineName + "\\" + Environment.UserName).ToSha256Base64();
            _client.Context.Session.Id = Guid.NewGuid().ToString();
            _client.Context.Properties.Add("Host", dte.Application.Edition);
            _client.Context.Properties.Add("HostVersion", dte.Version);
            _client.Context.Properties.Add("HostFullVersion", GetFullHostVersion());
            _client.Context.Component.Version = GetVersion();
            _client.Context.Properties.Add("AppVersion", GetFullHostVersion());
            _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            dte.Events.DTEEvents.OnBeginShutdown += OnBeginShutdown;
            
            WriteEvent("GoToIt Telemetry Started");
        }

        /// <summary>
        /// Writes the event to the telemetry client.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public static void WriteEvent(String eventName) {
            if (_client != null) {
                _client.TrackEvent(new EventTelemetry(eventName));
            }
        }

        /// <summary>
        /// Writes the exception to the telemetry client.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="ex">The ex.</param>
        public static void WriteException(String msg, Exception ex) {
            if (_client != null) {
                var telemetry = new ExceptionTelemetry(ex);
                telemetry.Properties.Add("Message", msg);
                _client.TrackException(telemetry);
            }
        }

        /// <summary>
        /// Writes a trace to the telemetry client.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteTrace(String message) {
            if (_client != null) {
                _client.TrackTrace(message);
            }
        }

        /// <summary>
        /// Should be called when shutting down the application to dispose needed objects.
        /// </summary>
        private static void OnBeginShutdown() {
            _client.Flush();
            _client = null;
            _config.Dispose();
        }

        /// <summary>
        /// Gets the full version based on msenv.dll
        /// </summary>
        /// <returns></returns>
        private static String GetFullHostVersion() {
            try {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var devenv = Path.Combine(baseDir, "msenv.dll");
                var version = FileVersionInfo.GetVersionInfo(devenv);
                return version.ProductVersion;
            }
            catch {
                return "";
            }
        }

        /// <summary>
        /// Gets the Telemetry version.
        /// </summary>
        /// <returns></returns>
        private static String GetVersion() {
            var assembly = typeof(Telemetry).Assembly;
            var fileVersion = assembly
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)
                .Cast<AssemblyFileVersionAttribute>()
                .First().Version;
            return fileVersion;
        }        
    }

}
