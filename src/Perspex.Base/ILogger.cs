using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://github.com/serilog/serilog/tree/dev/src/Serilog

namespace Serilog
{
    /// <summary>
    /// The core Serilog logging API, used for writing log events.
    /// </summary>
    /// <example>
    /// var log = new LoggerConfiguration()
    ///     .WithConsoleSink()
    ///     .CreateLogger();
    /// 
    /// var thing = "World";
    /// log.Information("Hello, {Thing}!", thing);
    /// </example>
    /// <remarks>
    /// The methods on <see cref="ILogger"/> (and its static sibling <see cref="Log"/>) are guaranteed
    /// never to throw exceptions. Methods on all other types may.
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        /// Create a logger that enriches log events via the provided enrichers.
        /// </summary>
        /// <param name="enrichers">Enrichers that apply in the context.</param>
        /// <returns>A logger that will enrich log events as specified.</returns>
        // ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers);

        /// <summary>
        /// Create a logger that enriches log events with the specified property.
        /// </summary>
        /// <returns>A logger that will enrich log events as specified.</returns>
        ILogger ForContext(string propertyName, object value, bool destructureObjects = false);

        /// <summary>
        /// Create a logger that marks log events as being from the specified
        /// source type.
        /// </summary>
        /// <typeparam name="TSource">Type generating log messages in the context.</typeparam>
        /// <returns>A logger that will enrich log events as specified.</returns>
        ILogger ForContext<TSource>();

        /// <summary>
        /// Create a logger that marks log events as being from the specified
        /// source type.
        /// </summary>
        /// <param name="source">Type generating log messages in the context.</param>
        /// <returns>A logger that will enrich log events as specified.</returns>
        ILogger ForContext(Type source);

        /// <summary>
        /// Write an event to the log.
        /// </summary>
        /// <param name="logEvent">The event to write.</param>
        void Write(LogEvent logEvent);

        /// <summary>
        /// Write a log event with the specified level.
        /// </summary>
        /// <param name="level">The level of the event.</param>
        /// <param name="messageTemplate"></param>
        /// <param name="propertyValues"></param>
        // [MessageTemplateFormatMethod("messageTemplate")]
        //void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the specified level and associated exception.
        /// </summary>
        /// <param name="level">The level of the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        // [MessageTemplateFormatMethod("messageTemplate")]
        //void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Determine if events at the specified level will be passed through
        /// to the log sinks.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is enabled; otherwise, false.</returns>
        //bool IsEnabled(LogEventLevel level);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Verbose("Staring into space, wondering if we're alone.");
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Verbose(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Debug(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Debug(ex, "Swallowing a mundane exception.");
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Information(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Information(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Warning(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Warning(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Error(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal("Process terminating.");
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Fatal(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        /// <example>
        /// Log.Fatal(ex, "Process terminating.");
        /// </example>
        // [MessageTemplateFormatMethod("messageTemplate")]
        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);
    }

}


namespace Serilog.Core
{
    /// <summary>
    /// Applied during logging to add additional information to log events.
    /// </summary>
    public interface ILogEventEnricher
    {
        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        void Enrich(LogEvent logEvent); //, ILogEventPropertyFactory propertyFactory);
    }
}

namespace Serilog.Core.Enrichers
 {
        /// <summary>
        /// Adds a new property encricher to the log event.
        /// </summary>
        public class PropertyEnricher : ILogEventEnricher
        {
            readonly string _name;
            readonly object _value;
            readonly bool _destructureObjects;

            /// <summary>
            /// Create a new property enricher.
            /// </summary>
            /// <param name="name">The name of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>A handle to later remove the property from the context.</returns>
            /// <param name="destructureObjects">If true, and the value is a non-primitive, non-array type,
            /// then the value will be converted to a structure; otherwise, unknown types will
            /// be converted to scalars, which are generally stored as strings.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public PropertyEnricher(string name, object value, bool destructureObjects = false)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Property name must not be null or empty.", "name");
                _name = name;
                _value = value;
                _destructureObjects = destructureObjects;
            }

            /// <summary>
            /// Enrich the log event.
            /// </summary>
            /// <param name="logEvent">The log event to enrich.</param>
            /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
            public void Enrich(LogEvent logEvent) // , ILogEventPropertyFactory propertyFactory)
            {
                if (logEvent == null) throw new ArgumentNullException("logEvent");
                //if (propertyFactory == null) throw new ArgumentNullException("propertyFactory");
                //var property = propertyFactory.CreateProperty(_name, _value, _destructureObjects);
                //logEvent.AddPropertyIfAbsent(property);
            }
        }
    }


namespace Serilog.Events
{
    /// <summary>
    /// A log event.
    /// </summary>
    public class LogEvent
    {
        readonly DateTimeOffset _timestamp;
      //  readonly LogEventLevel _level;
        readonly Exception _exception;
       // readonly MessageTemplate _messageTemplate;
        //readonly Dictionary<string, LogEventPropertyValue> _properties;

        /// <summary>
        /// Construct a new <seealso cref="LogEvent"/>.
        /// </summary>
        /// <param name="timestamp">The time at which the event occurred.</param>
        /// <param name="level">The level of the event.</param>
        /// <param name="exception">An exception associated with the event, or null.</param>
        /// <param name="messageTemplate">The message template describing the event.</param>
        /// <param name="properties">Properties associated with the event, including those presented in <paramref name="messageTemplate"/>.</param>
        public LogEvent(DateTimeOffset timestamp)
            // , LogEventLevel level, Exception exception, MessageTemplate messageTemplate, IEnumerable<LogEventProperty> properties)
        {
            //if (messageTemplate == null) throw new ArgumentNullException("messageTemplate");
            //if (properties == null) throw new ArgumentNullException("properties");
            //_timestamp = timestamp;
            //_level = level;
            //_exception = exception;
            //_messageTemplate = messageTemplate;
            //_properties = new Dictionary<string, LogEventPropertyValue>();
            //foreach (var p in properties)
            //    AddOrUpdateProperty(p);
        }

        /// <summary>
        /// The time at which the event occurred.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// The level of the event.
        /// </summary>
        //public LogEventLevel Level
        //{
        //    get { return _level; }
        //}

        /// <summary>
        /// The message template describing the event.
        /// </summary>
        //public MessageTemplate MessageTemplate
        //{
        //    get { return _messageTemplate; }
        //}

        /// <summary>
        /// Render the message template to the specified output, given the properties associated
        /// with the event.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        //public void RenderMessage(TextWriter output, IFormatProvider formatProvider = null)
        //{
        //    MessageTemplate.Render(Properties, output, formatProvider);
        //}

        /// <summary>
        /// Render the message template given the properties associated
        /// with the event, and return the result.
        /// </summary>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        //public string RenderMessage(IFormatProvider formatProvider = null)
        //{
        //    return MessageTemplate.Render(Properties, formatProvider);
        //}

        /// <summary>
        /// Properties associated with the event, including those presented in <see cref="LogEvent.MessageTemplate"/>.
        /// </summary>
        //public IReadOnlyDictionary<string, LogEventPropertyValue> Properties
        //{
        //    get { return _properties; }
        //}

        /// <summary>
        /// An exception associated with the event, or null.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Add a property to the event if not already present, otherwise, update its value. 
        /// </summary>
        /// <param name="property">The property to add or update.</param>
        /// <exception cref="ArgumentNullException"></exception>
        //public void AddOrUpdateProperty(LogEventProperty property)
        //{
        //    if (property == null) throw new ArgumentNullException("property");
        //    _properties[property.Name] = property.Value;
        //}

        /// <summary>
        /// Add a property to the event if not already present. 
        /// </summary>
        /// <param name="property">The property to add.</param>
        /// <exception cref="ArgumentNullException"></exception>
        //public void AddPropertyIfAbsent(LogEventProperty property)
        //{
        //    if (property == null) throw new ArgumentNullException("property");
        //    if (!_properties.ContainsKey(property.Name))
        //        _properties.Add(property.Name, property.Value);
        //}

        /// <summary>
        /// Remove a property from the event, if present. Otherwise no action
        /// is performed.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove.</param>
        //public void RemovePropertyIfPresent(string propertyName)
        //{
        //    _properties.Remove(propertyName);
        //}
    }
}

