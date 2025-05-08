using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nanover.Frame.Import.CIF
{
    /// <summary>
    /// Base importer for files using the CIF format.
    /// </summary>
    public abstract class CifBaseImport : LineByLineParser
    {
        protected CifBaseImport(IProgress<string> progress = null) : base(progress)
        {
        }

        protected override void Parse()
        {
            while (HasLine)
                if (IsDataBlockLine())
                {
                    ParseDataBlockLine();
                    NextLine();
                }
                else if (IsLoop())
                {
                    NextLine();
                    ParseTable();
                }
                else if (IsDataItem())
                {
                    ParseSingleLineTable();
                }
                else if (IsBlankOrComment())
                {
                    NextLine();
                }
                else
                {
                    throw new InvalidOperationException("Unknown line");
                }
        }

        /// <summary>
        /// Does the current line mark the state of a multi line data table.
        /// </summary>
        protected bool IsLoop()
        {
            return CurrentLine.Length >= 5 && CurrentLine.Substring(0, 5).Equals("loop_");
        }

        /// <summary>
        /// Parse a table with one row, with the values directly after the keys
        /// </summary>
        private void ParseSingleLineTable()
        {
            string currentCategory = null;
            var values = new List<string>();
            var keys = new List<string>();
            var isTextBlock = false;
            var shouldParseTable = (bool?) null;
            while (HasLine)
            {
                if (IsBlankOrComment())
                {
                    NextLine();
                }
                else if (IsLoop() || IsDataBlockLine())
                {
                    var handler = GetTableHandler(currentCategory, keys);
                    handler?.Invoke(values);
                    return;
                }
                else if (IsDataItem())
                {
                    var category = CifUtility.GetCategory(CurrentLine);
                    if (!shouldParseTable.HasValue)
                    {
                        shouldParseTable = ShouldParseCategory(category);
                    }

                    if (currentCategory != null && currentCategory != category)
                    {
                        // Start a new table
                        var handler = GetTableHandler(currentCategory, keys);
                        handler?.Invoke(values);
                        return;
                    }

                    currentCategory = category;

                    if (shouldParseTable.Value)
                    {
                        var (_, keyword, value) =
                            CifUtility.GetCategoryAndKeyword(CurrentLine);

                        if (value != null)
                        {
                            if (value[0] == '\"' && value.Last() == '"')
                            {
                                value = value.Substring(1, value.Length - 2);
                            }
                            else if (value[0] == '\'' && value.Last() == '\'')
                            {
                                value = value.Substring(1, value.Length - 2);
                            }
                        }

                        values.Add(value);
                        keys.Add(keyword);
                    }

                    NextLine();
                }
                else if (CurrentLine[0] == ';')
                {
                    if (isTextBlock)
                    {
                        isTextBlock = false;
                    }
                    else
                    {
                        isTextBlock = true;
                    }

                    if (shouldParseTable.Value)
                    {
                        var value = CurrentLine.Substring(1);
                        if (value.Trim().Length > 0)
                        {
                            var lastValueIndex = values.Count - 1;
                            if (values[lastValueIndex] == null)
                                values[lastValueIndex] = value;
                            else
                                values[lastValueIndex] += "\n" + value;
                        }
                    }

                    NextLine();
                }
                else if (CurrentLine[0] == '"' || CurrentLine[0] == '\'')
                {
                    if (shouldParseTable.Value)
                    {
                        var value = CurrentLine.Substring(1, CurrentLine.Length - 2);
                        var lastValueIndex = values.Count - 1;
                        if (values[lastValueIndex] != null)
                            throw new InvalidOperationException(
                                "String value on new line, but value already read in.");
                        values[lastValueIndex] = value;
                    }

                    NextLine();
                }
                else
                {
                    if (isTextBlock)
                    {
                        if (shouldParseTable.Value)
                        {
                            var value = CurrentLine.Trim();
                            if (value.Length > 0)
                            {
                                var lastValueIndex = values.Count - 1;
                                if (values[lastValueIndex] == null)
                                    values[lastValueIndex] = value;
                                else
                                    values[lastValueIndex] += "\n" + value;
                            }
                        }

                        NextLine();
                    }
                    else
                    {
                        var handler = GetTableHandler(currentCategory, keys);
                        handler?.Invoke(values);
                        return;
                    }
                }
            }

            var handler2 = GetTableHandler(currentCategory, keys);
            handler2?.Invoke(values);
        }

        /// <summary>
        /// Should this table category be parsed, or skipped over?
        /// </summary>
        protected virtual bool ShouldParseCategory(string category)
        {
            return true;
        }

        /// <summary>
        /// Parse a multi line table.
        /// </summary>
        private void ParseTable()
        {
            string currentCategory = null;
            var keywords = new List<string>();
            var shouldParseTable = (bool?) null;
            while (HasLine)
            {
                if (IsBlankOrComment())
                {
                }
                else if (IsDataItem())
                {
                    if (!shouldParseTable.HasValue)
                    {
                        var category = CifUtility.GetCategory(CurrentLine);
                        shouldParseTable = ShouldParseCategory(category);
                    }

                    if (shouldParseTable.Value)
                    {
                        var (category, keyword, _) = CifUtility.GetCategoryAndKeyword(CurrentLine);
                        if (currentCategory != null && currentCategory != category)
                            throw new ArgumentException(
                                "Table does not have consistent category");
                        currentCategory = category;
                        keywords.Add(keyword);
                    }
                }
                else
                {
                    break;
                }

                NextLine();
            }

            var handler = shouldParseTable.Value
                              ? GetTableHandler(currentCategory, keywords)
                              : null;

            var dataItemCount = keywords.Count;

            while (HasLine)
            {
                if (IsBlankOrComment())
                {
                }
                else if (IsDataItem() || IsLoop() || IsDataBlockLine())
                {
                    return;
                }
                else
                {
                    var dataItems = SplitWithDelimiter(CurrentLine);
                    
                    // Account for data items being spread across multiple lines
                    while (dataItems.Length < dataItemCount)
                    {
                        NextLine();
                        var existingCount = dataItems.Length;
                        if (CurrentLine[0] == ';')
                        {
                            Array.Resize(ref dataItems, dataItems.Length + 1);
                            dataItems[existingCount] = ParseTextBlock();
                        }
                        else
                        {
                            var moreDataItems = SplitWithDelimiter(CurrentLine);
                            Array.Resize(ref dataItems, dataItems.Length + moreDataItems.Length);
                            moreDataItems.CopyTo(dataItems, existingCount);
                            if (dataItems.Length > dataItemCount)
                                throw new InvalidOperationException("Read in too many parameters");
                        }
                    }

                    handler?.Invoke(dataItems);
                }

                NextLine();
            }
        }

        // Parse a text block starting with ;
        private string ParseTextBlock()
        {
            string val = null;

            var value = CurrentLine.Substring(1);
            if (value.Trim().Length > 0)
            {
                val = value;
            }

            NextLine();
            while (CurrentLine[0] != ';')
            {
                value = CurrentLine.Trim();
                if (value.Length > 0)
                {
                    if (val == null)
                        val = value;
                    else
                        val += "\n" + value;
                }

                NextLine();
            }

            return val;
        }

        /// <summary>
        /// Does the line start with an underscore
        /// </summary>
        private bool IsDataItem()
        {
            return CurrentLine[0] == '_';
        }

        /// <summary>
        /// Is the line blank or starts with a comment character.
        /// </summary>
        private bool IsBlankOrComment()
        {
            return IsBlankLine() || IsCommentLine('#');
        }

        /// <summary>
        /// Does this line mark the start of a data block (data_).
        /// </summary>
        /// <returns></returns>
        private bool IsDataBlockLine()
        {
            return CurrentLine.Length >= 5 && CurrentLine.Substring(0, 5)
                                                         .Equals(
                                                             "data_",
                                                             StringComparison
                                                                 .InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Parse space separate items, but account for quotation marks delimiting strings.
        /// </summary>
        public static string[] SplitWithDelimiter(string currentLine)
        {
            if (!currentLine.Contains("\"") && !currentLine.Contains("'"))
                return currentLine.Split(new[]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries);
            var split = currentLine.Split(' ');
            var arr = new string[split.Length];
            var i = 0;
            var builder = new StringBuilder();
            char? delimiter = null;
            foreach (var part in split)
            {
                var startDelimiter = part.Length > 0
                                  && (part[0] == '"'
                                   || part[0] == '\'')
                                         ? part[0]
                                         : (char?) null;
                var endDelimiter = part.Length > 0
                                && (part[part.Length - 1] == '"'
                                 || part[part.Length - 1] == '\'')
                                       ? part[part.Length - 1]
                                       : (char?) null;
                var isSpace = part.Length == 0 || part == " ";
                if (delimiter.HasValue)
                {
                    if (endDelimiter == delimiter)
                    {
                        builder.Append(" ");
                        builder.Append(part.Substring(0, part.Length - 1));
                        arr[i++] = builder.ToString();
                        delimiter = null;
                    }
                    else if (startDelimiter.HasValue && startDelimiter != delimiter)
                    {
                        throw new InvalidOperationException("Nested delimiters");
                    }
                    else
                    {
                        builder.Append(" ");
                        builder.Append(part);
                    }
                }
                else
                {
                    // Delimited with no space
                    if (startDelimiter.HasValue && startDelimiter == endDelimiter &&
                        part.Length > 2)
                    {
                        arr[i++] = part.Substring(1, part.Length - 2);
                    }
                    else if (startDelimiter.HasValue)
                    {
                        builder.Clear();
                        builder.Append(part.Substring(1, part.Length - 1));
                        delimiter = startDelimiter;
                    }
                    else if (endDelimiter.HasValue)
                    {
                        throw new InvalidOperationException("Nested delimiters");
                    }
                    else if (!isSpace)
                    {
                        arr[i++] = part;
                    }
                }
            }

            Array.Resize(ref arr, i);
            return arr;
        }

        protected delegate void ParseTableRow(IReadOnlyList<string> row);

        /// <summary>
        /// Get the function which handles reading in table rows.
        /// </summary>
        protected virtual ParseTableRow GetTableHandler(string category, List<string> keywords)
        {
            return null;
        }

        /// <summary>
        /// Parse the data block.
        /// </summary>
        protected virtual void ParseDataBlockLine()
        {
        }

        /// <summary>
        /// Parse an int, treating '?' and '.' as null.
        /// </summary>
        protected new int? ParseInt(string str, string errorMsg)
        {
            if (str == "?" || str == ".")
                return null;
            return base.ParseInt(str, errorMsg);
        }

        /// <summary>
        /// Parse a float, treating '?' and '.' as null.
        /// </summary>
        protected new float? ParseFloat(string str, string errorMsg)
        {
            if (str == "?" || str == ".")
                return null;
            return base.ParseFloat(str, errorMsg);
        }

        /// <summary>
        /// Parse a string, treating '?' and '.' as null.
        /// </summary>
        protected new string ParseString(string str)
        {
            if (str == "?" || str == ".")
                return null;
            return base.ParseString(str);
        }

        protected int ParseNonNullInt(string str, string errorMsg)
        {
            return base.ParseInt(str, errorMsg);
        }

        protected float ParseNonNullFloat(string str, string errorMsg)
        {
            return base.ParseFloat(str, errorMsg);
        }
    }
}
