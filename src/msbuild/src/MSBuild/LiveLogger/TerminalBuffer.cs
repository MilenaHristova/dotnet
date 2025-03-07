// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Build.Logging.LiveLogger
{
    internal class TerminalBufferLine
    {
        private static int Counter = 0;
        private string _text = string.Empty;
        public List<string> WrappedText { get; private set; } = new();
        public int Id;
        public bool ShouldWrapLines;
        public string Text
        {
            get => _text;
            set
            {
                // Set text value and get wrapped lines
                _text = value;
                if (ShouldWrapLines)
                {
                    WrappedText = ANSIBuilder.ANSIWrap(value, Console.BufferWidth);
                }
                else
                {
                    WrappedText = new List<string> { value };
                }
                // Buffer should rerender
                TerminalBuffer.ShouldRerender = true;
            }
        }

        public TerminalBufferLine()
        {
            Id = Counter++;
            Text = string.Empty;
            ShouldWrapLines = false;
        }
        public TerminalBufferLine(string text)
            : this()
        {
            Text = text;
        }
        public TerminalBufferLine(string text, bool shouldWrapLines)
            : this()
        {
            ShouldWrapLines = shouldWrapLines;
            Text = text;
        }
    }

    internal class TerminalBuffer
    {
        private static List<TerminalBufferLine> Lines = new();
        public static string FooterText = string.Empty;
        public static int TopLineIndex = 0;
        public static string Footer = string.Empty;
        internal static bool IsTerminated = false;
        internal static bool ShouldRerender = true;
        internal static int ScrollableAreaHeight
        {
            get
            {
                // Height of the buffer -3 (titlebar, footer, and footer line)
                return Console.BufferHeight - 3;
            }
        }
        public static void Initialize()
        {
            // Configure buffer, encoding and cursor
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write(ANSIBuilder.Buffer.UseAlternateBuffer());
            Console.Write(ANSIBuilder.Cursor.Invisible());
        }

        public static void Terminate()
        {
            IsTerminated = true;
            // Reset configuration for buffer and cursor, and clear screen
            Console.Write(ANSIBuilder.Buffer.UseMainBuffer());
            Console.Write(ANSIBuilder.Eraser.Display());
            Console.Clear();
            Console.Write(ANSIBuilder.Cursor.Visible());
            Lines = new();
        }

        #region Rendering
        public static void Render()
        {
            if (IsTerminated || !ShouldRerender)
            {
                return;
            }

            ShouldRerender = false;
            Console.Write(
                // Write header
                ANSIBuilder.Cursor.Home() +
                ANSIBuilder.Eraser.LineCursorToEnd() + ANSIBuilder.Formatting.Inverse(ANSIBuilder.Alignment.Center("MSBuild - Build in progress")) +
                // Write footer
                ANSIBuilder.Cursor.Position(Console.BufferHeight - 1, 0) + ANSIBuilder.Eraser.LineCursorToEnd() +
                new string('-', Console.BufferWidth) + '\n' + FooterText);

            if (Lines.Count == 0)
            {
                return;
            }

            // Iterate over lines and display on terminal
            string contents = string.Empty;
            int accumulatedLineCount = 0;
            int lineIndex = 0;
            foreach (TerminalBufferLine line in Lines)
            {
                // Continue if accum line count + next lines < scrolling area
                if (accumulatedLineCount + line.WrappedText.Count < TopLineIndex)
                {
                    accumulatedLineCount += line.WrappedText.Count;
                    continue;
                }

                // Break if exceeds scrolling area
                if (accumulatedLineCount - TopLineIndex > ScrollableAreaHeight)
                {
                    break;
                }

                foreach (string s in line.WrappedText)
                {
                    // Get line index relative to scroll area
                    lineIndex = accumulatedLineCount - TopLineIndex;
                    // Print if line in scrolling area
                    if (lineIndex >= 0 && lineIndex < ScrollableAreaHeight)
                    {
                        contents += ANSIBuilder.Cursor.Position(lineIndex + 2, 0) + ANSIBuilder.Eraser.LineCursorToEnd() + s;
                    }

                    accumulatedLineCount++;
                }
            }
            // Iterate for the rest of the screen
            for (int i = lineIndex + 1; i < ScrollableAreaHeight; i++)
            {
                contents += ANSIBuilder.Cursor.Position(i + 2, 0) + ANSIBuilder.Eraser.LineCursorToEnd();
            }
            Console.Write(contents);
        }
        #endregion
        #region Line identification
        public static int GetLineIndexById(int lineId)
        {
            return Lines.FindIndex(x => x.Id == lineId);
        }

        public static TerminalBufferLine? GetLineById(int lineId)
        {
            int index = GetLineIndexById(lineId);
            if (index == -1)
            {
                return null;
            }

            return Lines[index];
        }
        #endregion

        #region Line create, update and delete
        // Write new line
        public static TerminalBufferLine? WriteNewLineAfter(int lineId, string text)
        {
            return WriteNewLineAfter(lineId, text, true);
        }
        public static TerminalBufferLine? WriteNewLineAfter(int lineId, string text, bool shouldWrapLines)
        {
            TerminalBufferLine line = new TerminalBufferLine(text, shouldWrapLines);
            return WriteNewLineAfter(lineId, line);
        }
        public static TerminalBufferLine? WriteNewLineAfter(int lineId, TerminalBufferLine line)
        {
            if (lineId != -1)
            {
                // Get line index
                int lineIndex = GetLineIndexById(lineId);
                if (lineIndex == -1)
                {
                    return null;
                }
                // Get line end index
                Lines.Insert(lineIndex, line);
            }
            else
            {
                Lines.Add(line);
            }
            return line;
        }

        public static TerminalBufferLine? WriteNewLine(string text)
        {
            return WriteNewLine(text, true);
        }
        public static TerminalBufferLine? WriteNewLine(string text, bool shouldWrapLines)
        {
            TerminalBufferLine line = new TerminalBufferLine(text, shouldWrapLines);
            return WriteNewLine(line);
        }
        public static TerminalBufferLine? WriteNewLine(TerminalBufferLine line)
        {
            return WriteNewLineAfter(Lines.Count > 0 ? Lines.Last().Id : -1, line);
        }

        // Update line
        // TODO: Remove. Use line.Text instead
        public static TerminalBufferLine? UpdateLine(int lineId, string text)
        {
            return null;
        }

        // Delete line
        public static void DeleteLine(int lineId)
        {
            // Get line index
            int lineIndex = GetLineIndexById(lineId);
            if (lineIndex == -1)
            {
                return;
            }
            // Delete
            Lines.RemoveAt(lineIndex);
            ShouldRerender = true;
        }
        #endregion
    }
}
