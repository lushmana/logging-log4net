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

namespace log4net.Util;

/// <summary>
/// Subclass of <see cref="QuietTextWriter"/> that maintains a count of 
/// the number of bytes written.
/// </summary>
/// <remarks>
/// <para>
/// This writer counts the number of bytes written.
/// </para>
/// </remarks>
/// <author>Nicko Cadell</author>
/// <author>Gert Driesen</author>
public class CountingQuietTextWriter : QuietTextWriter
{
  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="writer">The <see cref="TextWriter" /> to actually write to.</param>
  /// <param name="errorHandler">The <see cref="IErrorHandler" /> to report errors to.</param>
  /// <remarks>
  /// <para>
  /// Creates a new instance of the <see cref="CountingQuietTextWriter" /> class 
  /// with the specified <see cref="TextWriter" /> and <see cref="IErrorHandler" />.
  /// </para>
  /// </remarks>
  public CountingQuietTextWriter(TextWriter writer, IErrorHandler errorHandler) : base(writer, errorHandler) => Count = 0;

  /// <summary>
  /// Writes a character to the underlying writer and counts the number of bytes written.
  /// </summary>
  /// <param name="value">the char to write</param>
  /// <remarks>
  /// <para>
  /// Overrides implementation of <see cref="QuietTextWriter"/>. Counts
  /// the number of bytes written.
  /// </para>
  /// </remarks>
  public override void Write(char value)
  {
    try
    {
      base.Write(value);

      // get the number of bytes needed to represent the 
      // char using the supplied encoding.
      Count += Encoding.GetByteCount(new[] { value });
    }
    catch (Exception e) when (!e.IsFatal())
    {
      ErrorHandler.Error($"Failed to write [{value}].", e, ErrorCode.WriteFailure);
    }
  }

  /// <summary>
  /// Writes a buffer to the underlying writer and counts the number of bytes written.
  /// </summary>
  /// <param name="buffer">the buffer to write</param>
  /// <param name="index">the start index to write from</param>
  /// <param name="count">the number of characters to write</param>
  /// <remarks>
  /// <para>
  /// Overrides implementation of <see cref="QuietTextWriter"/>. Counts
  /// the number of bytes written.
  /// </para>
  /// </remarks>
  public override void Write(char[] buffer, int index, int count)
  {
    if (count > 0)
    {
      try
      {
        base.Write(buffer, index, count);

        // get the number of bytes needed to represent the 
        // char array using the supplied encoding.
        Count += Encoding.GetByteCount(buffer, index, count);
      }
      catch (Exception e) when (!e.IsFatal())
      {
        ErrorHandler.Error("Failed to write buffer.", e, ErrorCode.WriteFailure);
      }
    }
  }

  /// <summary>
  /// Writes a string to the output and counts the number of bytes written.
  /// </summary>
  /// <param name="value">The string data to write to the output.</param>
  /// <remarks>
  /// <para>
  /// Overrides implementation of <see cref="QuietTextWriter"/>. Counts
  /// the number of bytes written.
  /// </para>
  /// </remarks>
  public override void Write(string? value)
  {
    if (value is not null && value.Length > 0)
    {
      try
      {
        base.Write(value);

        // get the number of bytes needed to represent the 
        // string using the supplied encoding.
        Count += Encoding.GetByteCount(value);
      }
      catch (Exception e) when (!e.IsFatal())
      {
        ErrorHandler.Error($"Failed to write [{value}].", e, ErrorCode.WriteFailure);
      }
    }
  }

  /// <summary>
  /// Gets or sets the total number of bytes written.
  /// </summary>
  /// <value>
  /// The total number of bytes written.
  /// </value>
  /// <remarks>
  /// <para>
  /// Gets or sets the total number of bytes written.
  /// </para>
  /// </remarks>
  public long Count { get; set; }
}
