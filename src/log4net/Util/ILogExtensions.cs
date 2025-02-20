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

using log4net.Core;

namespace log4net.Util;

/// <summary>
/// The static class ILogExtensions contains a set of widely used
/// methods that ease the interaction with the ILog interface implementations.
/// </summary>
/// <remarks>
/// <para>
/// This class contains methods for logging at different levels and checks the
/// properties for determining if those logging levels are enabled in the current
/// configuration.
/// </para>
/// </remarks>
/// <example>Simple example of logging messages
/// <code lang="C#">
/// using log4net.Util;
/// 
/// ILog log = LogManager.GetLogger("application-log");
/// 
/// log.InfoExt("Application Start");
/// log.DebugExt("This is a debug message");
/// </code>
/// </example>
public static class LogExtensions
{
  /// <summary>
  /// The fully qualified type of the Logger class.
  /// </summary>
  private static readonly Type _declaringType = typeof(LogExtensions);

  /// <summary>
  /// Log a message object with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>INFO</c>
  /// enabled by reading the value <seealso cref="ILog.IsDebugEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation.  If this logger is <c>INFO</c> enabled, then it converts 
  /// the message object (retrieved by invocation of the provided callback) to a 
  /// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>.
  /// It then proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="DebugExt(ILog,Func{object},Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugExt(this ILog logger, Func<object> callback)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.Debug(callback?.Invoke());
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Debug"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="DebugExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugExt(this ILog logger, Func<object> callback, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.Debug(callback?.Invoke(), exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <overloads>Log a message object with the <see cref="Level.Debug"/> level.</overloads> //TODO
  /// <summary>
  /// Log a message object with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>INFO</c>
  /// enabled by reading the value <seealso cref="ILog.IsDebugEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation. If this logger is <c>INFO</c> enabled, then it converts 
  /// the message object (passed as parameter) to a string by invoking the appropriate
  /// <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then 
  /// proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="DebugExt(ILog,object,Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugExt(this ILog logger, object? message)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.Debug(message);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Debug"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="DebugExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugExt(this ILog logger, object? message, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.Debug(message, exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="DebugExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugFormatExt(this ILog logger, string format, object? arg0)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.DebugFormat(format, arg0);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="DebugExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugFormatExt(this ILog logger, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.DebugFormat(format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information</param>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="DebugExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugFormatExt(this ILog logger, IFormatProvider provider, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.DebugFormat(provider, format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="DebugExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugFormatExt(this ILog logger, string format, object? arg0, object? arg1)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.DebugFormat(format, arg0, arg1);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Debug"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <param name="arg2">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="DebugExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Debug(object)"/>
  /// <seealso cref="ILog.IsDebugEnabled"/>
  public static void DebugFormatExt(this ILog logger, string format, object? arg0, object? arg1, object? arg2)
  {
    try
    {
      if (logger.EnsureNotNull().IsDebugEnabled)
      {
        logger.DebugFormat(format, arg0, arg1, arg2);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>INFO</c>
  /// enabled by reading the value <seealso cref="ILog.IsInfoEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation.  If this logger is <c>INFO</c> enabled, then it converts 
  /// the message object (retrieved by invocation of the provided callback) to a 
  /// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>.
  /// It then proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="InfoExt(ILog,Func{object},Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoExt(this ILog logger, Func<object> callback)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.Info(callback?.Invoke());
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Info"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="InfoExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoExt(this ILog logger, Func<object> callback, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.Info(callback?.Invoke(), exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <overloads>Log a message object with the <see cref="Level.Info"/> level.</overloads> //TODO
  /// <summary>
  /// Log a message object with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>INFO</c>
  /// enabled by reading the value <seealso cref="ILog.IsInfoEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation. If this logger is <c>INFO</c> enabled, then it converts 
  /// the message object (passed as parameter) to a string by invoking the appropriate
  /// <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then 
  /// proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="InfoExt(ILog,object,Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoExt(this ILog logger, object? message)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.Info(message);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Info"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="InfoExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoExt(this ILog logger, object? message, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.Info(message, exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="InfoExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoFormatExt(this ILog logger, string format, object? arg0)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.InfoFormat(format, arg0);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="InfoExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoFormatExt(this ILog logger, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.InfoFormat(format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information</param>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="InfoExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoFormatExt(this ILog logger, IFormatProvider provider, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.InfoFormat(provider, format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="InfoExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoFormatExt(this ILog logger, string format, object? arg0, object? arg1)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.InfoFormat(format, arg0, arg1);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Info"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <param name="arg2">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="InfoExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Info(object)"/>
  /// <seealso cref="ILog.IsInfoEnabled"/>
  public static void InfoFormatExt(this ILog logger, string format, object? arg0, object? arg1, object? arg2)
  {
    try
    {
      if (logger.EnsureNotNull().IsInfoEnabled)
      {
        logger.InfoFormat(format, arg0, arg1, arg2);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>WARN</c>
  /// enabled by reading the value <seealso cref="ILog.IsWarnEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation.  If this logger is <c>WARN</c> enabled, then it converts 
  /// the message object (retrieved by invocation of the provided callback) to a 
  /// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>.
  /// It then proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="WarnExt(ILog,Func{object},Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnExt(this ILog logger, Func<object> callback)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.Warn(callback?.Invoke());
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Warn"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="WarnExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnExt(this ILog logger, Func<object> callback, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.Warn(callback?.Invoke(), exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <overloads>Log a message object with the <see cref="Level.Warn"/> level.</overloads> //TODO
  /// <summary>
  /// Log a message object with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>WARN</c>
  /// enabled by reading the value <seealso cref="ILog.IsWarnEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation. If this logger is <c>WARN</c> enabled, then it converts 
  /// the message object (passed as parameter) to a string by invoking the appropriate
  /// <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then 
  /// proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="WarnExt(ILog,object,Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnExt(this ILog logger, object? message)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.Warn(message);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Warn"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="WarnExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnExt(this ILog logger, object? message, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.Warn(message, exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="WarnExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnFormatExt(this ILog logger, string format, object? arg0)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.WarnFormat(format, arg0);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="WarnExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnFormatExt(this ILog logger, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.WarnFormat(format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information</param>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="WarnExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnFormatExt(this ILog logger, IFormatProvider provider, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.WarnFormat(provider, format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="WarnExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnFormatExt(this ILog logger, string format, object? arg0, object? arg1)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.WarnFormat(format, arg0, arg1);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Warn"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <param name="arg2">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="WarnExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Warn(object)"/>
  /// <seealso cref="ILog.IsWarnEnabled"/>
  public static void WarnFormatExt(this ILog logger, string format, object? arg0, object? arg1, object? arg2)
  {
    try
    {
      if (logger.EnsureNotNull().IsWarnEnabled)
      {
        logger.WarnFormat(format, arg0, arg1, arg2);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>ERROR</c>
  /// enabled by reading the value <seealso cref="ILog.IsErrorEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation.  If this logger is <c>ERROR</c> enabled, then it converts 
  /// the message object (retrieved by invocation of the provided callback) to a 
  /// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>.
  /// It then proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="ErrorExt(ILog,Func{object},Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorExt(this ILog logger, Func<object> callback)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.Error(callback?.Invoke());
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Error"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="ErrorExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorExt(this ILog logger, Func<object> callback, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.Error(callback?.Invoke(), exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <overloads>Log a message object with the <see cref="Level.Error"/> level.</overloads> //TODO
  /// <summary>
  /// Log a message object with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>ERROR</c>
  /// enabled by reading the value <seealso cref="ILog.IsErrorEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation. If this logger is <c>ERROR</c> enabled, then it converts 
  /// the message object (passed as parameter) to a string by invoking the appropriate
  /// <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then 
  /// proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="ErrorExt(ILog,object,Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorExt(this ILog logger, object? message)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.Error(message);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Error"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="ErrorExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorExt(this ILog logger, object? message, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.Error(message, exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="ErrorExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorFormatExt(this ILog logger, string format, object? arg0)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.ErrorFormat(format, arg0);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="ErrorExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorFormatExt(this ILog logger, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.ErrorFormat(format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information</param>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="ErrorExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorFormatExt(this ILog logger, IFormatProvider provider, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.ErrorFormat(provider, format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="ErrorExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorFormatExt(this ILog logger, string format, object? arg0, object? arg1)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.ErrorFormat(format, arg0, arg1);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Error"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <param name="arg2">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="ErrorExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Error(object)"/>
  /// <seealso cref="ILog.IsErrorEnabled"/>
  public static void ErrorFormatExt(this ILog logger, string format, object? arg0, object? arg1, object? arg2)
  {
    try
    {
      if (logger.EnsureNotNull().IsErrorEnabled)
      {
        logger.ErrorFormat(format, arg0, arg1, arg2);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>FATAL</c>
  /// enabled by reading the value <seealso cref="ILog.IsFatalEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation.  If this logger is <c>FATAL</c> enabled, then it converts 
  /// the message object (retrieved by invocation of the provided callback) to a 
  /// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>.
  /// It then proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="FatalExt(ILog,Func{object},Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalExt(this ILog logger, Func<object> callback)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.Fatal(callback?.Invoke());
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Fatal"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="callback">The lambda expression that gets the object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="FatalExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalExt(this ILog logger, Func<object> callback, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.Fatal(callback?.Invoke(), exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <overloads>Log a message object with the <see cref="Level.Fatal"/> level.</overloads> //TODO
  /// <summary>
  /// Log a message object with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <remarks>
  /// <para>
  /// This method first checks if this logger is <c>FATAL</c>
  /// enabled by reading the value <seealso cref="ILog.IsFatalEnabled"/> property.
  /// This check happens always and does not depend on the <seealso cref="ILog"/>
  /// implementation. If this logger is <c>FATAL</c> enabled, then it converts 
  /// the message object (passed as parameter) to a string by invoking the appropriate
  /// <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then 
  /// proceeds to call all the registered appenders in this logger 
  /// and also higher in the hierarchy depending on the value of 
  /// the additivity flag.
  /// </para>
  /// <para><b>WARNING</b> Note that passing an <see cref="Exception"/> 
  /// to this method will print the name of the <see cref="Exception"/> 
  /// but no stack trace. To print a stack trace use the 
  /// <see cref="FatalExt(ILog,object,Exception)"/> form instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalExt(this ILog logger, object? message)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.Fatal(message);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Log a message object with the <see cref="Level.Fatal"/> level including
  /// the stack trace of the <see cref="Exception"/> passed
  /// as a parameter.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="message">The message object to log.</param>
  /// <param name="exception">The exception to log, including its stack trace.</param>
  /// <remarks>
  /// <para>
  /// See the <see cref="FatalExt(ILog, object)"/> form for more detailed information.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalExt(this ILog logger, object? message, Exception? exception)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.Fatal(message, exception);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="FatalExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalFormatExt(this ILog logger, string format, object? arg0)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.FatalFormat(format, arg0);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="FatalExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalFormatExt(this ILog logger, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.FatalFormat(format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information</param>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="args">An Object array containing zero or more objects to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="FatalExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalFormatExt(this ILog logger, IFormatProvider provider, string format, params object?[]? args)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.FatalFormat(provider, format, args);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="FatalExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalFormatExt(this ILog logger, string format, object? arg0, object? arg1)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.FatalFormat(format, arg0, arg1);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }

  /// <summary>
  /// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
  /// </summary>
  /// <param name="logger">The logger on which the message is logged.</param>
  /// <param name="format">A String containing zero or more format items</param>
  /// <param name="arg0">An Object to format</param>
  /// <param name="arg1">An Object to format</param>
  /// <param name="arg2">An Object to format</param>
  /// <remarks>
  /// <para>
  /// The message is formatted using the <c>String.Format</c> method. See
  /// <see cref="string.Format(string, object[])"/> for details of the syntax of the format string and the behavior
  /// of the formatting.
  /// </para>
  /// <para>
  /// This method does not take an <see cref="Exception"/> object to include in the
  /// log event. To pass an <see cref="Exception"/> use one of the <see cref="FatalExt(ILog,object,Exception)"/>
  /// methods instead.
  /// </para>
  /// </remarks>
  /// <seealso cref="ILog.Fatal(object)"/>
  /// <seealso cref="ILog.IsFatalEnabled"/>
  public static void FatalFormatExt(this ILog logger, string format, object? arg0, object? arg1, object? arg2)
  {
    try
    {
      if (logger.EnsureNotNull().IsFatalEnabled)
      {
        logger.FatalFormat(format, arg0, arg1, arg2);
      }
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Exception while logging", e);
    }
  }
}