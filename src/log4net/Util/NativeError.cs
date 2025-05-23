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
using System.Globalization;
using System.Runtime.InteropServices;

namespace log4net.Util;

/// <summary>
/// Represents a Win32 native error code and message.
/// </summary>
/// <author>Nicko Cadell</author>
/// <author>Gert Driesen</author>
public sealed class NativeError
{
  /// <summary>
  /// Create an instance of the <see cref="NativeError" /> class with the specified 
  /// error number and message.
  /// </summary>
  /// <param name="number">The number of the native error.</param>
  /// <param name="message">The message of the native error.</param>
  private NativeError(int number, string? message)
  {
    Number = number;
    Message = message;
  }

  /// <summary>
  /// Gets the number of the native error.
  /// </summary>
  /// <value>
  /// The number of the native error.
  /// </value>
  /// <remarks>
  /// <para>
  /// Gets the number of the native error.
  /// </para>
  /// </remarks>
  public int Number { get; }

  /// <summary>
  /// Gets the message of the native error.
  /// </summary>
  public string? Message { get; }

  /// <summary>
  /// Creates a new instance of the <see cref="NativeError" /> class for the last Windows error.
  /// </summary>
  /// <returns>
  /// An instance of the <see cref="NativeError" /> class for the last windows error.
  /// </returns>
  /// <remarks>
  /// <para>
  /// The message for the <see cref="Marshal.GetLastWin32Error"/> error number is lookup up using the 
  /// native Win32 <c>FormatMessage</c> function.
  /// </para>
  /// </remarks>
  [System.Security.SecuritySafeCritical]
  [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, UnmanagedCode = true)]
  public static NativeError GetLastError()
  {
    int number = Marshal.GetLastWin32Error();
    return new NativeError(number, GetErrorMessage(number));
  }

  /// <summary>
  /// Create a new instance of the <see cref="NativeError" /> class.
  /// </summary>
  /// <param name="number">the error number for the native error</param>
  /// <returns>
  /// An instance of the <see cref="NativeError" /> class for the specified 
  /// error number.
  /// </returns>
  /// <remarks>
  /// <para>
  /// The message for the specified error number is lookup up using the 
  /// native Win32 <c>FormatMessage</c> function.
  /// </para>
  /// </remarks>
  public static NativeError GetError(int number) => new NativeError(number, GetErrorMessage(number));

  /// <summary>
  /// Retrieves the message corresponding with a Win32 message identifier.
  /// </summary>
  /// <param name="messageId">Message identifier for the requested message.</param>
  /// <returns>
  /// The message corresponding with the specified message identifier.
  /// </returns>
  /// <remarks>
  /// <para>
  /// The message will be searched for in system message-table resource(s)
  /// using the native <c>FormatMessage</c> function.
  /// </para>
  /// </remarks>
  [System.Security.SecuritySafeCritical]
  [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, UnmanagedCode = true)]
  public static string? GetErrorMessage(int messageId)
  {
    // Win32 constants
    int formatMessageAllocateBuffer = 0x00000100;  // The function should allocates a buffer large enough to hold the formatted message
    int formatMessageIgnoreInserts = 0x00000200;    // Insert sequences in the message definition are to be ignored
    int formatMessageFromSystem = 0x00001000;    // The function should search the system message-table resource(s) for the requested message

    string? msgBuf = "";        // buffer that will receive the message
    IntPtr sourcePtr = new();  // Location of the message definition, will be ignored
    IntPtr argumentsPtr = new();  // Pointer to array of values to insert, not supported as it requires unsafe code

    if (messageId != 0)
    {
      // If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character
      int messageSize =  NativeMethods.FormatMessage(
        formatMessageAllocateBuffer | formatMessageFromSystem | formatMessageIgnoreInserts,
        ref sourcePtr,
        messageId,
        0,
        ref msgBuf,
        255,
        argumentsPtr);

      if (messageSize > 0)
      {
        // Remove trailing null-terminating characters (\r\n) from the message
        msgBuf = msgBuf.TrimEnd(_newlines);
      }
      else
      {
        // A message could not be located.
        msgBuf = null;
      }
    }
    else
    {
      msgBuf = null;
    }

    return msgBuf;
  }

  /// <summary>
  /// Return error information string
  /// </summary>
  /// <returns>error information string</returns>
  /// <remarks>
  /// <para>
  /// Return error information string
  /// </para>
  /// </remarks>
  public override string ToString() 
    => string.Format(CultureInfo.InvariantCulture, "0x{0:x8}", Number) + (Message is not null ? ": " + Message : string.Empty);

  private static readonly char[] _newlines = ['\r', '\n'];
}
