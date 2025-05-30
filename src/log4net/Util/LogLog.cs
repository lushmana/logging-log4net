#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace log4net.Util;

/// <summary>
/// LogReceivedEventHandler
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:Use generic event handler instances")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public delegate void LogReceivedEventHandler(object? source, LogReceivedEventArgs e);

/// <summary>
/// Outputs log statements from within the log4net assembly.
/// </summary>
/// <remarks>
/// <para>
/// Log4net components cannot make log4net logging calls. However, it is
/// sometimes useful for the user to learn about what log4net is
/// doing.
/// </para>
/// <para>
/// All log4net internal debug calls go to the standard output stream
/// whereas internal error messages are sent to the standard error output 
/// stream.
/// </para>
/// </remarks>
/// <author>Nicko Cadell</author>
/// <author>Gert Driesen</author>
public sealed class LogLog
{
  /// <summary>
  /// The event raised when an internal message has been received.
  /// </summary>
  public static event LogReceivedEventHandler? LogReceived;

  /// <summary>
  /// The Type that generated the internal message.
  /// </summary>
  public Type Source { get; }

  /// <summary>
  /// The DateTime stamp of when the internal message was received.
  /// </summary>
  public DateTime TimeStamp => TimeStampUtc.ToLocalTime();

  /// <summary>
  /// The UTC DateTime stamp of when the internal message was received.
  /// </summary>
  public DateTime TimeStampUtc { get; }

  /// <summary>
  /// A string indicating the severity of the internal message.
  /// </summary>
  /// <remarks>
  /// "log4net: ", 
  /// "log4net:ERROR ", 
  /// "log4net:WARN "
  /// </remarks>
  public string Prefix { get; }

  /// <summary>
  /// The internal log message.
  /// </summary>
  public string Message { get; }

  /// <summary>
  /// The Exception related to the message.
  /// </summary>
  /// <remarks>
  /// Optional. Will be null if no Exception was passed.
  /// </remarks>
  public Exception? Exception { get; }

  /// <summary>
  /// Formats Prefix, Source, and Message in the same format as the value
  /// sent to Console.Out and Trace.Write.
  /// </summary>
  /// <returns></returns>
  public override string ToString() => $"{Prefix}{Source.Name}: {Message}";

  /// <summary>
  /// Initializes a new instance of the <see cref="LogLog" /> class. 
  /// </summary>
  public LogLog(Type source, string prefix, string message, Exception? exception)
  {
    TimeStampUtc = DateTime.UtcNow;

    Source = source;
    Prefix = prefix;
    Message = message;
    Exception = exception;
  }

  /// <summary>
  /// Static constructor that initializes logging by reading 
  /// settings from the application configuration file.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <c>log4net.Internal.Debug</c> application setting
  /// controls internal debugging. This setting should be set
  /// to <c>true</c> to enable debugging.
  /// </para>
  /// <para>
  /// The <c>log4net.Internal.Quiet</c> application setting
  /// suppresses all internal logging including error messages. 
  /// This setting should be set to <c>true</c> to enable message
  /// suppression.
  /// </para>
  /// </remarks>
  static LogLog()
  {
    try
    {
      InternalDebugging = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Debug"), false);
      QuietMode = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Quiet"), false);
      EmitInternalMessages = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Emit"), true);
    }
    catch (Exception e) when (!e.IsFatal())
    {
      // If an exception is thrown here then it looks like the config file does not
      // parse correctly.
      //
      // We will leave debug OFF and print an Error message
      Error(typeof(LogLog), "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", e);
    }
  }

  /// <summary>
  /// Gets or sets a value indicating whether log4net internal logging
  /// is enabled or disabled.
  /// </summary>
  /// <value>
  /// <c>true</c> if log4net internal logging is enabled, otherwise 
  /// <c>false</c>.
  /// </value>
  /// <remarks>
  /// <para>
  /// When set to <c>true</c>, internal debug level logging will be 
  /// displayed.
  /// </para>
  /// <para>
  /// This value can be set by setting the application setting 
  /// <c>log4net.Internal.Debug</c> in the application configuration
  /// file.
  /// </para>
  /// <para>
  /// The default value is <c>false</c>, i.e. debugging is
  /// disabled.
  /// </para>
  /// </remarks>
  /// <example>
  /// <para>
  /// The following example enables internal debugging using the 
  /// application configuration file :
  /// </para>
  /// <code lang="XML" escaped="true">
  /// <configuration>
  ///    <appSettings>
  ///      <add key="log4net.Internal.Debug" value="true" />
  ///    </appSettings>
  /// </configuration>
  /// </code>
  /// </example>
  public static bool InternalDebugging { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether log4net should generate no output
  /// from internal logging, not even for errors. 
  /// </summary>
  /// <value>
  /// <c>true</c> if log4net should generate no output at all from internal 
  /// logging, otherwise <c>false</c>.
  /// </value>
  /// <remarks>
  /// <para>
  /// When set to <c>true</c> will cause internal logging at all levels to be 
  /// suppressed. This means that no warning or error reports will be logged. 
  /// This option overrides the <see cref="InternalDebugging"/> setting and 
  /// disables all debug also.
  /// </para>
  /// <para>This value can be set by setting the application setting
  /// <c>log4net.Internal.Quiet</c> in the application configuration file.
  /// </para>
  /// <para>
  /// The default value is <c>false</c>, i.e. internal logging is not
  /// disabled.
  /// </para>
  /// </remarks>
  /// <example>
  /// The following example disables internal logging using the 
  /// application configuration file :
  /// <code lang="XML" escaped="true">
  /// <configuration>
  ///    <appSettings>
  ///      <add key="log4net.Internal.Quiet" value="true" />
  ///    </appSettings>
  /// </configuration>
  /// </code>
  /// </example>
  public static bool QuietMode { get; set; }

  /// <summary>
  /// 
  /// </summary>
  public static bool EmitInternalMessages { get; set; } = true;

  /// <summary>
  /// Raises the LogReceived event when an internal messages is received.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="prefix"></param>
  /// <param name="message"></param>
  /// <param name="exception"></param>
  public static void OnLogReceived(Type source, string prefix, string message, Exception? exception) => LogReceived?.Invoke(null, new LogReceivedEventArgs(new LogLog(source, prefix, message, exception)));

  /// <summary>
  /// Test if LogLog.Debug is enabled for output.
  /// </summary>
  /// <value>
  /// <c>true</c> if Debug is enabled
  /// </value>
  /// <remarks>
  /// <para>
  /// Test if LogLog.Debug is enabled for output.
  /// </para>
  /// </remarks>
  public static bool IsDebugEnabled => InternalDebugging && !QuietMode;

  /// <summary>
  /// Writes log4net internal debug messages to the 
  /// standard output stream.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="message">The message to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal debug messages are prepended with 
  ///  the string "log4net: ".
  /// </para>
  /// </remarks>
  public static void Debug(Type source, string message)
  {
    if (IsDebugEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitOutLine(Log4NetPrefix + message);
      }

      OnLogReceived(source, Log4NetPrefix, message, null);
    }
  }

  /// <summary>
  /// Writes log4net internal debug messages to the 
  /// standard output stream.
  /// </summary>
  /// <param name="source">The Type that generated this message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="exception">An exception to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal debug messages are prepended with 
  ///  the string "log4net: ".
  /// </para>
  /// </remarks>
  public static void Debug(Type source, string message, Exception? exception)
  {
    if (IsDebugEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitOutLine(Log4NetPrefix + message);
        if (exception is not null)
        {
          EmitOutLine(exception.ToString());
        }
      }

      OnLogReceived(source, Log4NetPrefix, message, exception);
    }
  }

  /// <summary>
  /// Test if LogLog.Warn is enabled for output.
  /// </summary>
  /// <value>
  /// <c>true</c> if Warn is enabled
  /// </value>
  public static bool IsWarnEnabled => !QuietMode;

  /// <summary>
  /// Writes log4net internal warning messages to the 
  /// standard error stream.
  /// </summary>
  /// <param name="source">The Type that generated this message.</param>
  /// <param name="message">The message to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal warning messages are prepended with 
  ///  the string "log4net:WARN ".
  /// </para>
  /// </remarks>
  public static void Warn(Type source, string message)
  {
    if (IsWarnEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitErrorLine(WarnPrefix + message);
      }

      OnLogReceived(source, WarnPrefix, message, null);
    }
  }

  /// <summary>
  /// Writes log4net internal warning messages to the 
  /// standard error stream.
  /// </summary>
  /// <param name="source">The Type that generated this message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="exception">An exception to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal warning messages are prepended with 
  ///  the string "log4net:WARN ".
  /// </para>
  /// </remarks>
  public static void Warn(Type source, string message, Exception? exception)
  {
    if (IsWarnEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitErrorLine(WarnPrefix + message);
        if (exception is not null)
        {
          EmitErrorLine(exception.ToString());
        }
      }

      OnLogReceived(source, WarnPrefix, message, exception);
    }
  }

  /// <summary>
  /// Test if LogLog.Error is enabled for output.
  /// </summary>
  /// <value>
  /// <c>true</c> if Error is enabled
  /// </value>
  /// <remarks>
  /// <para>
  /// Test if LogLog.Error is enabled for output.
  /// </para>
  /// </remarks>
  public static bool IsErrorEnabled => !QuietMode;

  /// <summary>
  /// Writes log4net internal error messages to the 
  /// standard error stream.
  /// </summary>
  /// <param name="source">The Type that generated this message.</param>
  /// <param name="message">The message to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal error messages are prepended with 
  ///  the string "log4net:ERROR ".
  /// </para>
  /// </remarks>
  public static void Error(Type source, string message)
  {
    if (IsErrorEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitErrorLine(ErrPrefix + message);
      }

      OnLogReceived(source, ErrPrefix, message, null);
    }
  }

  /// <summary>
  /// Writes log4net internal error messages to the 
  /// standard error stream.
  /// </summary>
  /// <param name="source">The Type that generated this message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="exception">An exception to log.</param>
  /// <remarks>
  /// <para>
  ///  All internal debug messages are prepended with 
  ///  the string "log4net:ERROR ".
  /// </para>
  /// </remarks>
  public static void Error(Type source, string message, Exception? exception)
  {
    if (IsErrorEnabled)
    {
      if (EmitInternalMessages)
      {
        EmitErrorLine(ErrPrefix + message);
        if (exception is not null)
        {
          EmitErrorLine(exception.ToString());
        }
      }

      OnLogReceived(source, ErrPrefix, message, exception);
    }
  }

  /// <summary>
  /// Writes output to the standard output stream.  
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <remarks>
  /// <para>
  /// Writes to both Console.Out and System.Diagnostics.Trace.
  /// </para>
  /// <para>
  /// If the AppDomain is not configured with a config file then
  /// the call to System.Diagnostics.Trace may fail. This is only
  /// an issue if you are programmatically creating your own AppDomains.
  /// </para>
  /// </remarks>
  private static void EmitOutLine(string message)
  {
    try
    {
      Console.Out.WriteLine(message);
      Trace.WriteLine(message);
    }
    catch (Exception e) when (!e.IsFatal())
    {
      // Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
    }
  }

  /// <summary>
  /// Writes output to the standard error stream.  
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <remarks>
  /// <para>
  /// Writes to both Console.Error and System.Diagnostics.Trace.
  /// Note that the System.Diagnostics.Trace is not supported
  /// on the Compact Framework.
  /// </para>
  /// <para>
  /// If the AppDomain is not configured with a config file then
  /// the call to System.Diagnostics.Trace may fail. This is only
  /// an issue if you are programmatically creating your own AppDomains.
  /// </para>
  /// </remarks>
  private static void EmitErrorLine(string message)
  {
    try
    {
      Console.Error.WriteLine(message);
      Trace.WriteLine(message);
    }
    catch (Exception e) when (!e.IsFatal())
    {
      // Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
    }
  }

  private const string Log4NetPrefix = "log4net: ";
  private const string ErrPrefix = "log4net:ERROR ";
  private const string WarnPrefix = "log4net:WARN ";

  /// <summary>
  /// Subscribes to the LogLog.LogReceived event and stores messages
  /// to the supplied IList instance.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists")]
  public class LogReceivedAdapter : IDisposable
  {
    private readonly LogReceivedEventHandler _handler;

    /// <inheritdoc/>
    public LogReceivedAdapter(List<LogLog> items)
    {
      Items = items;
      _handler = LogLog_LogReceived;
      LogReceived += _handler;
    }

    void LogLog_LogReceived(object? source, LogReceivedEventArgs e)
    {
      lock (((ICollection)Items).SyncRoot)
      {
        Items.Add(e.LogLog);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public List<LogLog> Items { get; }

    /// <inheritdoc/>
    public void Dispose() => LogReceived -= _handler;
  }
}

/// <summary>
/// 
/// </summary>
public class LogReceivedEventArgs : EventArgs
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="loglog"></param>
  public LogReceivedEventArgs(LogLog loglog)
  {
    this.LogLog = loglog;
  }

  /// <summary>
  /// 
  /// </summary>
  public LogLog LogLog { get; }
}
