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
using System.IO;

using log4net.Core;
using log4net.Util;
using log4net.DateFormatter;
using System.Diagnostics.CodeAnalysis;

namespace log4net.Layout.Pattern;

/// <summary>
/// Writes the TimeStamp to the output.
/// </summary>
/// <remarks>
/// <para>
/// Date pattern converter, uses a <see cref="IDateFormatter"/> to format 
/// the date of a <see cref="LoggingEvent"/>.
/// </para>
/// <para>
/// Uses a <see cref="IDateFormatter"/> to format the <see cref="LoggingEvent.TimeStamp"/>
/// in Universal time.
/// </para>
/// <para>
/// See the <see cref="DatePatternConverter"/> for details on the date pattern syntax.
/// </para>
/// </remarks>
/// <seealso cref="DatePatternConverter"/>
/// <author>Nicko Cadell</author>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reflection")]
internal sealed class UtcDatePatternConverter : DatePatternConverter
{
  /// <summary>
  /// Writes the TimeStamp to the output.
  /// </summary>
  /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
  /// <param name="loggingEvent">the event being logged</param>
  /// <remarks>
  /// <para>
  /// Pass the <see cref="LoggingEvent.TimeStamp"/> to the <see cref="IDateFormatter"/>
  /// for it to render it to the writer.
  /// </para>
  /// <para>
  /// The <see cref="LoggingEvent.TimeStamp"/> passed is in the local time zone, this is converted
  /// to Universal time before it is rendered.
  /// </para>
  /// </remarks>
  /// <seealso cref="DatePatternConverter"/>
  protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
  {
    try
    {
      m_dateFormatter.EnsureNotNull().FormatDate(loggingEvent.TimeStampUtc, writer);
    }
    catch (Exception e) when (!e.IsFatal())
    {
      LogLog.Error(_declaringType, "Error occurred while converting date.", e);
    }
  }

  /// <summary>
  /// The fully qualified type of the UtcDatePatternConverter class.
  /// </summary>
  /// <remarks>
  /// Used by the internal logger to record the Type of the
  /// log message.
  /// </remarks>
  private static readonly Type _declaringType = typeof(UtcDatePatternConverter);
}
