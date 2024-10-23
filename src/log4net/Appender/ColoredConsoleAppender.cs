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
using System.Text;
using log4net.Core;
using log4net.Util;

namespace log4net.Appender;

/// <summary>
/// Appends logging events to the console.
/// </summary>
/// <remarks>
/// <para>
/// ColoredConsoleAppender appends log events to the standard output stream
/// or the error output stream using a layout specified by the 
/// user. It also allows the color of a specific type of message to be set.
/// </para>
/// <para>
/// By default, all output is written to the console's standard output stream.
/// The <see cref="Target"/> property can be set to direct the output to the
/// error stream.
/// </para>
/// <para>
/// NOTE: This appender writes directly to the application's attached console
/// not to the <c>System.Console.Out</c> or <c>System.Console.Error</c> <c>TextWriter</c>.
/// The <c>System.Console.Out</c> and <c>System.Console.Error</c> streams can be
/// programmatically redirected (for example NUnit does this to capture program output).
/// This appender will ignore these redirections because it needs to use Win32
/// API calls to colorize the output. To respect these redirections the <see cref="ConsoleAppender"/>
/// must be used.
/// </para>
/// <para>
/// When configuring the colored console appender, mapping should be
/// specified to map a logging level to a color. For example:
/// </para>
/// <code lang="XML" escaped="true">
/// <mapping>
///   <level value="ERROR" />
///   <foreColor value="White" />
///   <backColor value="Red, HighIntensity" />
/// </mapping>
/// <mapping>
///   <level value="DEBUG" />
///   <backColor value="Green" />
/// </mapping>
/// </code>
/// <para>
/// The Level is the standard log4net logging level and ForeColor and BackColor can be any
/// combination of the following values:
/// <list type="bullet">
/// <item><term>Blue</term><description></description></item>
/// <item><term>Green</term><description></description></item>
/// <item><term>Red</term><description></description></item>
/// <item><term>White</term><description></description></item>
/// <item><term>Yellow</term><description></description></item>
/// <item><term>Purple</term><description></description></item>
/// <item><term>Cyan</term><description></description></item>
/// <item><term>HighIntensity</term><description></description></item>
/// </list>
/// </para>
/// </remarks>
/// <author>Rick Hobbs</author>
/// <author>Nicko Cadell</author>
public class ColoredConsoleAppender : AppenderSkeleton
{
  /// <summary>
  /// The enum of possible color values for use with the color mapping method
  /// </summary>
  /// <remarks>
  /// <para>
  /// The following flags can be combined to form the colors.
  /// </para>
  /// </remarks>
  /// <seealso cref="ColoredConsoleAppender" />
  [Flags]
  public enum Colors
  {
    /// <summary>
    /// color is blue
    /// </summary>
    Blue = 0x0001,

    /// <summary>
    /// color is green
    /// </summary>
    Green = 0x0002,

    /// <summary>
    /// color is red
    /// </summary>
    Red = 0x0004,

    /// <summary>
    /// color is white
    /// </summary>
    White = Blue | Green | Red,

    /// <summary>
    /// color is yellow
    /// </summary>
    Yellow = Red | Green,

    /// <summary>
    /// color is purple
    /// </summary>
    Purple = Red | Blue,

    /// <summary>
    /// color is cyan
    /// </summary>
    Cyan = Green | Blue,

    /// <summary>
    /// color is intensified
    /// </summary>
    HighIntensity = 0x0008,
  }

  private static readonly char[] _windowsNewline = ['\r', '\n'];

  /// <summary>
  /// Initializes a new instance of the <see cref="ColoredConsoleAppender" /> class.
  /// </summary>
  /// <remarks>
  /// The instance of the <see cref="ColoredConsoleAppender" /> class is set up to write 
  /// to the standard output stream.
  /// </remarks>
  public ColoredConsoleAppender()
  { }

  /// <summary>
  /// Target is the value of the console output stream.
  /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
  /// </summary>
  /// <value>
  /// Target is the value of the console output stream.
  /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
  /// </value>
  /// <remarks>
  /// <para>
  /// Target is the value of the console output stream.
  /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
  /// </para>
  /// </remarks>
  public virtual string Target
  {
    get => _writeToErrorStream ? ConsoleError : ConsoleOut;
    set => _writeToErrorStream = StringComparer.OrdinalIgnoreCase.Equals(ConsoleError, value?.Trim());
  }

  /// <summary>
  /// Add a mapping of level to color - done by the config file
  /// </summary>
  /// <param name="mapping">The mapping to add</param>
  /// <remarks>
  /// <para>
  /// Add a <see cref="LevelColors"/> mapping to this appender.
  /// Each mapping defines the foreground and background colors
  /// for a level.
  /// </para>
  /// </remarks>
  public void AddMapping(LevelColors mapping) => _levelMapping.Add(mapping);

  /// <summary>
  /// This method is called by the <see cref="AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)"/> method.
  /// </summary>
  /// <param name="loggingEvent">The event to log.</param>
  /// <remarks>
  /// <para>
  /// Writes the event to the console.
  /// </para>
  /// <para>
  /// The format of the output will depend on the appender's layout.
  /// </para>
  /// </remarks>
  [System.Security.SecuritySafeCritical]
  [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, UnmanagedCode = true)]
  protected override void Append(LoggingEvent loggingEvent)
  {
    loggingEvent.EnsureNotNull();
    if (_consoleOutputWriter is not null)
    {
      IntPtr consoleHandle = NativeMethods.GetStdHandle(_writeToErrorStream ? NativeMethods.StdErrorHandle : NativeMethods.StdOutputHandle);

      // Default to white on black
      ushort colorInfo = (ushort)Colors.White;

      // see if there is a specified lookup
      if (_levelMapping.Lookup(loggingEvent.Level) is LevelColors levelColors)
      {
        colorInfo = levelColors.CombinedColor;
      }

      // Render the event to a string
      string strLoggingMessage = RenderLoggingEvent(loggingEvent);

      // get the current console color - to restore later
      NativeMethods.GetConsoleScreenBufferInfo(consoleHandle, out NativeMethods.ConsoleScreenBufferInfo bufferInfo);

      // set the console colors
      NativeMethods.SetConsoleTextAttribute(consoleHandle, colorInfo);

      // Using WriteConsoleW seems to be unreliable.
      // If a large buffer is written, say 15,000 chars
      // Followed by a larger buffer, say 20,000 chars
      // then WriteConsoleW will fail, last error 8
      // 'Not enough storage is available to process this command.'
      // 
      // Although the documentation states that the buffer must
      // be less that 64KB (i.e. 32,000 WCHARs) the longest string
      // that I can write out a the first call to WriteConsoleW
      // is only 30,704 chars.
      //
      // Unlike the WriteFile API the WriteConsoleW method does not 
      // seem to be able to partially write out from the input buffer.
      // It does have a lpNumberOfCharsWritten parameter, but this is
      // either the length of the input buffer if any output was written,
      // or 0 when an error occurs.
      //
      // All results above were observed on Windows XP SP1 running
      // .NET runtime 1.1 SP1.
      //
      // Old call to WriteConsoleW:
      //
      // WriteConsoleW(
      //     consoleHandle,
      //     strLoggingMessage,
      //     (UInt32)strLoggingMessage.Length,
      //     out (UInt32)ignoreWrittenCount,
      //     IntPtr.Zero);
      //
      // Instead of calling WriteConsoleW we use WriteFile which 
      // handles large buffers correctly. Because WriteFile does not
      // handle the codepage conversion as WriteConsoleW does we 
      // need to use a System.IO.StreamWriter with the appropriate
      // Encoding. The WriteFile calls are wrapped up in the
      // System.IO.__ConsoleStream internal class obtained through
      // the System.Console.OpenStandardOutput method.
      //
      // See the ActivateOptions method below for the code that
      // retrieves and wraps the stream.


      // The windows console uses ScrollConsoleScreenBuffer internally to
      // scroll the console buffer when the display buffer of the console
      // has been used up. ScrollConsoleScreenBuffer fills the area uncovered
      // by moving the current content with the background color 
      // currently specified on the console. This means that it fills the
      // whole line in front of the cursor position with the current 
      // background color.
      // This causes an issue when writing out text with a non default
      // background color. For example; We write a message with a Blue
      // background color and the scrollable area of the console is full.
      // When we write the newline at the end of the message the console
      // needs to scroll the buffer to make space available for the new line.
      // The ScrollConsoleScreenBuffer internals will fill the newly created
      // space with the current background color: Blue.
      // We then change the console color back to default (White text on a
      // Black background). We write some text to the console, the text is
      // written correctly in White with a Black background, however the
      // remainder of the line still has a Blue background.
      // 
      // This causes a disjointed appearance to the output where the background
      // colors change.
      //
      // This can be remedied by restoring the console colors before causing
      // the buffer to scroll, i.e. before writing the last newline. This does
      // assume that the rendered message will end with a newline.
      //
      // Therefore we identify a trailing newline in the message and don't
      // write this to the output, then we restore the console color and write
      // a newline. Note that we must AutoFlush before we restore the console
      // color otherwise we will have no effect.
      //
      // There will still be a slight artefact for the last line of the message
      // will have the background extended to the end of the line, however this
      // is unlikely to cause any user issues.
      //
      // Note that none of the above is visible while the console buffer is scrollable
      // within the console window viewport, the effects only arise when the actual
      // buffer is full and needs to be scrolled.

      char[] messageCharArray = strLoggingMessage.ToCharArray();
      int arrayLength = messageCharArray.Length;
      bool appendNewline = false;

      // Trim off last newline, if it exists
      if (arrayLength > 1 && messageCharArray[arrayLength - 2] == '\r' && messageCharArray[arrayLength - 1] == '\n')
      {
        arrayLength -= 2;
        appendNewline = true;
      }

      // Write to the output stream
      _consoleOutputWriter.Write(messageCharArray, 0, arrayLength);

      // Restore the console back to its previous color scheme
      NativeMethods.SetConsoleTextAttribute(consoleHandle, bufferInfo.wAttributes);

      if (appendNewline)
      {
        // Write the newline, after changing the color scheme
        _consoleOutputWriter.Write(_windowsNewline, 0, 2);
      }
    }
  }

  /// <summary>
  /// This appender requires a <see cref="Layout"/> to be set.
  /// </summary>
  protected override bool RequiresLayout => true;

  /// <summary>
  /// Initializes the options for this appender.
  /// </summary>
  [System.Security.SecuritySafeCritical]
  [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, UnmanagedCode = true)]
  public override void ActivateOptions()
  {
    base.ActivateOptions();
    _levelMapping.ActivateOptions();

    // Use the Console methods to open a Stream over the console std handle
    Stream consoleOutputStream = _writeToErrorStream ? Console.OpenStandardError() : Console.OpenStandardOutput();

    // Look up the codepage encoding for the console
    Encoding consoleEncoding = EncodingWithoutPreamble.Get(Encoding.GetEncoding(NativeMethods.GetConsoleOutputCP()));

    // Create a writer around the console stream
    _consoleOutputWriter = new StreamWriter(consoleOutputStream, consoleEncoding, 0x100)
    {
      AutoFlush = true
    };

    // SuppressFinalize on consoleOutputWriter because all it will do is flush
    // and close the file handle. Because we have set AutoFlush the additional flush
    // is not required. The console file handle should not be closed, so we don't call
    // Dispose, Close or the finalizer.
    GC.SuppressFinalize(_consoleOutputWriter);
  }

  /// <summary>
  /// The <see cref="Target"/> to use when writing to the Console 
  /// standard output stream.
  /// </summary>
  public const string ConsoleOut = "Console.Out";

  /// <summary>
  /// The <see cref="Target"/> to use when writing to the Console 
  /// standard error output stream.
  /// </summary>
  public const string ConsoleError = "Console.Error";

  /// <summary>
  /// Flag to write output to the error stream rather than the standard output stream
  /// </summary>
  private bool _writeToErrorStream;

  /// <summary>
  /// Mapping from level object to color value
  /// </summary>
  private readonly LevelMapping _levelMapping = new();

  /// <summary>
  /// The console output stream writer to write to
  /// </summary>
  /// <remarks>
  /// <para>
  /// This writer is not thread safe.
  /// </para>
  /// </remarks>
  private StreamWriter? _consoleOutputWriter;

  /// <summary>
  /// A class to act as a mapping between the level that a logging call is made at and
  /// the color it should be displayed as.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Defines the mapping between a level and the color it should be displayed in.
  /// </para>
  /// </remarks>
  public class LevelColors : LevelMappingEntry
  {
    /// <summary>
    /// The mapped foreground color for the specified level
    /// </summary>
    public Colors ForeColor { get; set; }

    /// <summary>
    /// The mapped background color for the specified level
    /// </summary>
    public Colors BackColor { get; set; }

    /// <summary>
    /// Initialize the options for the object
    /// </summary>
    /// <remarks>
    /// <para>
    /// Combine the <see cref="ForeColor"/> and <see cref="BackColor"/> together.
    /// </para>
    /// </remarks>
    public override void ActivateOptions()
    {
      base.ActivateOptions();
      CombinedColor = (ushort)((int)ForeColor + (((int)BackColor) << 4));
    }

    /// <summary>
    /// The combined <see cref="ForeColor"/> and <see cref="BackColor"/> suitable for 
    /// setting the console color.
    /// </summary>
    internal ushort CombinedColor { get; private set; }
  }
}