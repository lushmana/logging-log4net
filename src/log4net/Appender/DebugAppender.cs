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

#define DEBUG

using log4net.Layout;
using log4net.Core;

namespace log4net.Appender;

/// <summary>
/// Appends log events to the <see cref="System.Diagnostics.Debug"/> system.
/// </summary>
/// <remarks>
/// <para>
/// The application configuration file can be used to control what listeners 
/// are actually used. See the MSDN documentation for the 
/// <see cref="System.Diagnostics.Debug"/> class for details on configuring the
/// debug system.
/// </para>
/// <para>
/// Events are written using the <see cref="System.Diagnostics.Debug.Write(string,string)"/>
/// method. The event's logger name is passed as the value for the category name to the Write method.
/// </para>
/// </remarks>
/// <author>Nicko Cadell</author>
public class DebugAppender : AppenderSkeleton
{
  /// <summary>
  /// Gets or sets a value that indicates whether the appender will 
  /// flush at the end of each write.
  /// </summary>
  /// <remarks>
  /// <para>The default behavior is to flush at the end of each 
  /// write. If the option is set to<c>false</c>, then the underlying 
  /// stream can defer writing to physical medium to a later time. 
  /// </para>
  /// <para>
  /// Avoiding the flush operation at the end of each append results 
  /// in a performance gain of 10 to 20 percent. However, there is safety
  /// trade-off involved in skipping flushing. Indeed, when flushing is
  /// skipped, then it is likely that the last few log events will not
  /// be recorded on disk when the application exits. This is a high
  /// price to pay even for a 20% performance gain.
  /// </para>
  /// </remarks>
  public bool ImmediateFlush { get; set; } = true;

  /// <summary>
  /// Formats the category parameter sent to the Debug method.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Defaults to a <see cref="Layout.PatternLayout"/> with %logger as the pattern which will use the logger name of the current 
  /// <see cref="LoggingEvent"/> as the category parameter.
  /// </para>
  /// </remarks>
  public PatternLayout? Category { get; set; } = new("%logger");

  /// <summary>
  /// Flushes any buffered log data.
  /// </summary>
  /// <param name="millisecondsTimeout">The maximum time to wait for logging events to be flushed.</param>
  /// <returns><c>True</c> if all logging events were flushed successfully, else <c>false</c>.</returns>
  public override bool Flush(int millisecondsTimeout)
  {
    // Nothing to do if ImmediateFlush is true
    if (ImmediateFlush)
    {
      return true;
    }

    // System.Diagnostics.Debug is thread-safe, so no need for lock(this).
    System.Diagnostics.Debug.Flush();

    return true;
  }

  /// <summary>
  /// Writes the logging event to the <see cref="System.Diagnostics.Debug"/> system.
  /// </summary>
  /// <param name="loggingEvent">The event to log.</param>
  /// <remarks>
  /// <para>
  /// Writes the logging event to the <see cref="System.Diagnostics.Debug"/> system.
  /// If <see cref="ImmediateFlush"/> is <c>true</c> then the <see cref="System.Diagnostics.Debug.Flush"/>
  /// is called.
  /// </para>
  /// </remarks> 
  protected override void Append(LoggingEvent loggingEvent)
  {
    //
    // Write the string to the Debug system
    //
    if (Category is null)
    {
      System.Diagnostics.Debug.Write(RenderLoggingEvent(loggingEvent));
    }
    else
    {
      string category = Category.Format(loggingEvent);
      if (string.IsNullOrEmpty(category))
      {
        System.Diagnostics.Debug.Write(RenderLoggingEvent(loggingEvent));
      }
      else
      {
        System.Diagnostics.Debug.Write(RenderLoggingEvent(loggingEvent), category);
      }
    }
    //
    // Flush the Debug system if needed
    //
    if (ImmediateFlush)
    {
      System.Diagnostics.Debug.Flush();
    }
  }

  /// <summary>
  /// This appender requires a <see cref="Layout"/> to be set.
  /// </summary>
  protected override bool RequiresLayout => true;
}