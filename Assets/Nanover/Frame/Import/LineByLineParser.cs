using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Nanover.Core.Science;

namespace Nanover.Frame.Import
{
    public abstract class LineByLineParser
    {
        /// <summary>
        /// The current line the parser is looking at.
        /// </summary>
        protected string CurrentLine { get; private set; }

        private int LineNumber { get; set; }

        private TextReader CurrentReader { get; set; }

        /// <summary>
        /// Call the <see cref="Parse()" /> function on a given source, catching exceptions
        /// and wrapping them with the line number.
        /// </summary>
        protected void Parse(TextReader reader)
        {
            CurrentReader = reader;
            CurrentLine = reader.ReadLine();
            LineNumber = 1;
            try
            {
                Parse();
            }
            catch (Exception e)
            {
                throw new ImportException(CurrentLine, LineNumber, e);
            }
        }

        protected abstract void Parse();

        protected bool HasLine => CurrentLine != null;

        protected void NextLine()
        {
            if (CurrentReader.Peek() > -1)
            {
                CurrentLine = CurrentReader.ReadLine();
                LineNumber++;
                LogProgress($"Line {LineNumber}");
            }
            else
            {
                CurrentLine = null;
            }
        }

        protected void SkipLines(int n)
        {
            for (var i = 0; i < n; i++)
            {
                if (CurrentLine == null)
                    return;
                NextLine();
            }
        }

        protected void FinishParsing()
        {
            CurrentLine = null;
        }

        protected bool IsBlankLine()
        {
            return string.IsNullOrEmpty(CurrentLine.Trim());
        }

        protected bool IsCommentLine(char commentChar)
        {
            return CurrentLine.Length > 0 && CurrentLine[0] == commentChar;
        }

        protected float ParseFloat(string str, string errorMsg)
        {
            if (!float.TryParse(str, out var value))
                throw new ArgumentException(errorMsg + $" ({str})");
            return value;
        }

        protected int ParseInt(string str, string errorMsg)
        {
            if (!int.TryParse(str, out var value))
                throw new ArgumentException(errorMsg);
            return value;
        }

        protected string ParseString(string str)
        {
            return str;
        }

        private static Dictionary<string, Element> symbolsToElements =
            new Dictionary<string, Element>();

        // Setup all possible combinations (i.e. Cu, cu, CU, cU), to avoid performance problems of repeated calls of ToLower().
        static LineByLineParser()
        {
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                var symbol = element.GetSymbol();
                if (element == Element.Virtual)
                    continue;
                if (symbol.Length == 1)
                {
                    symbolsToElements.Add(symbol.ToUpper(), element);
                    symbolsToElements.Add(symbol.ToLower(), element);
                }
                else
                {
                    symbolsToElements.Add(symbol.ToUpper(), element);
                    symbolsToElements.Add(symbol.ToLower(), element);
                    symbolsToElements.Add(
                        new string(new[]
                        {
                            char.ToUpper(symbol[0]), char.ToLower(symbol[1])
                        }),
                        element);
                    symbolsToElements.Add(
                        new string(new[]
                        {
                            char.ToLower(symbol[0]), char.ToUpper(symbol[1])
                        }), element);
                }
            }
        }

        private readonly IProgress<string> progress = null;

        /// <summary>
        /// Log a message to a progress reporter if present.
        /// </summary>
        protected void LogProgress(string message)
        {
            progress?.Report(message);
        }

        protected LineByLineParser(IProgress<string> progress = null)
        {
            this.progress = progress;
        }

        protected Element? ParseElementSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return null;
            symbol = symbol.Trim();
            return symbolsToElements.TryGetValue(symbol, out var element)
                       ? (Element?) element
                       : null;
        }

        protected GroupCollection MatchLine(string pattern, string errorMsg)
        {
            var match = Regex.Match(CurrentLine, pattern);
            if (!match.Success)
                throw new ArgumentException(errorMsg);

            return match.Groups;
        }
    }
}