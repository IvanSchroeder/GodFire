using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/StringExtensions.cs

namespace UnityUtilities {
    /// <summary>
	/// Extensions for string.
	/// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Checks if a string is Null or white space
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>
        /// Checks if a string is Null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Checks if a string contains null, empty or white space.
        /// </summary>
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>
        /// Checks if a string is null and returns an empty string if it is.
        /// </summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        /// <summary>
        /// Shortens a string to the specified maximum length. If the string's length
        /// is less than the maxLength, the original string is returned.
        /// </summary>
        public static string Shorten(this string val, int maxLength) {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }

        /// <summary>Slices a string from the start index to the end index.</summary>
        /// <result>The sliced string.</result>
        public static string Slice(this string val, int startIndex, int endIndex) {
            if (val.IsBlank()) {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (startIndex < 0 || startIndex > val.Length - 1) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length) {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Converts the input string to an alphanumeric string, optionally allowing periods.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <param name="allowPeriods">A boolean flag indicating whether periods should be allowed in the output string.</param>
        /// <returns>
        /// A new string containing only alphanumeric characters, underscores, and optionally periods.
        /// If the input string is null or empty, an empty string is returned.
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            List<char> filteredChars = new List<char>();
            int lastValidIndex = -1;

            // Iterate over the input string, filtering and determining valid start/end indices
            foreach (char character in input
                         .Where(character => char
                             .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                         .Where(character => filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.'))) {

                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // Update lastValidIndex for valid characters
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.') {
                lastValidIndex--;
            }
    
            // Return the filtered string
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1) : string.Empty;
        }
        
        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(this string text) => $"<b>{text}</b>";
        public static string RichItalic(this string text) => $"<i>{text}</i>";
        public static string RichUnderline(this string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";
        public static string RichGradient(this string text, string color1, string color2) => $"<gradient={color1},{color2}>{text}</gradient>";
        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";

        /// <summary>
    	/// returns true ONLY if the entire string is numeric
    	/// </summary>
    	/// <param name="input">the string to test</param>
		public static bool IsNumeric(this string input) {
			// return false if no string
			return (!String.IsNullOrEmpty(input)) && (new Regex(@"^-?[0-9]*\.?[0-9]+$").IsMatch(input.Trim()));
			//Valid user input
		}

		/// <summary>
		/// returns true if any part of the string is numeric
		/// </summary>
		/// <param name="input">the string to test</param>
		public static bool HasNumeric(this string input) {
			// if no string, return false
			return (!String.IsNullOrEmpty(input)) && (new Regex(@"[0-9]+").IsMatch(input));
		}

		/// <summary>
		/// Returns true if value is a date
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsDate(this string value) {
			try {
				DateTime tempDate;
				return DateTime.TryParse(value, out tempDate);
			}

			catch (Exception) {
				return false;
			}
		}

		/// <summary>
		/// Returns true if value is an int
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsInt(this string value) {
			try {
				int tempInt;
				return int.TryParse(value, out tempInt);
			}

			catch (Exception) {
				return false;
			}
		}

		/// <summary>
		/// Like LINQ take - takes the first x characters
		/// </summary>
		/// <param name="value">the string</param>
		/// <param name="count">number of characters to take</param>
		/// <param name="ellipsis">true to add ellipsis (...) at the end of the string</param>
		/// <returns></returns>
		public static string Take(this string value, int count, bool ellipsis = false) {
			// get number of characters we can actually take
			int lengthToTake = Math.Min(count, value.Length);

			// Take and return
			return (ellipsis && lengthToTake < value.Length) ? string.Format("{0}...", value.Substring(0, lengthToTake)) : value.Substring(0, lengthToTake);
		}

		/// <summary>
		/// like LINQ skip - skips the first x characters and returns the remaining string
		/// </summary>
		/// <param name="value">the string</param>
		/// <param name="count">number of characters to skip</param>
		/// <returns></returns>
		public static string Skip(this string value, int count) => value.Substring(Math.Min(count, value.Length) - 1);

		/// <summary>
		/// Reverses the string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string Reverse(this string input) {
			char[] chars = input.ToCharArray();
			Array.Reverse(chars);
			return new String(chars);
		}

		/// <summary>
		/// Returns true if the string is Not null or empty
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNOTNullOrEmpty(this string value) => (!string.IsNullOrEmpty(value));

		/// <summary>
		/// "a string {0}".Formatted("blah") vs string.Format("a string {0}", "blah")
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Formatted(this string format, params object[] args) => string.Format(format, args);

		/// <summary>
		/// ditches html tags - note it doesn't get rid of things like nbsp;
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string StripHtml(this string html) {
			if (html.IsNullOrEmpty()) return string.Empty;

			return Regex.Replace(html, @"<[^>]*>", string.Empty);
		}

		/// <summary>
		/// Returns true if the pattern matches.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static bool Match(this string value, string pattern) => Regex.IsMatch(value, pattern);

		/// <summary>
		/// Remove white space, not line end.
		/// Useful when parsing user input such phone,
		/// price int.Parse("1 000 000".RemoveSpaces(),.....
		/// </summary>
		/// <param name="value"></param>
		public static string RemoveSpaces(this string value) => value.Replace(" ", string.Empty);

		/// <summary>
		/// Converts a null or "" to string.empty. Useful for XML code. Does nothing if <paramref name="value"/> already has a value
		/// </summary>
		/// <param name="value">string to convert</param>
		public static string ToEmptyString(string value) => (string.IsNullOrEmpty(value)) ? string.Empty : value;

		/*
		* Converting a sequence to a nicely-formatted string is a bit of a pain. 
		* The String.Join method definitely helps, but unfortunately it accepts an 
		* array of strings, so it does not compose with LINQ very nicely.
		* 
		* My library includes several overloads of the ToStringPretty operator that 
		* hides the uninteresting code. Here is an example of use:
		* 
		* Console.WriteLine(Enumerable.Range(0, 10).ToStringPretty("From 0 to 9: [", ",", "]"));
		* 
		* The output of this program is:
		* 
		* From 0 to 9: [0,1,2,3,4,5,6,7,8,9]
		*/

		/// <summary>
		/// Returns a comma delimited string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		public static string ToStringPretty<T>(this IEnumerable<T> source) => (source == null) ? string.Empty : ToStringPretty(source, ",");

		/// <summary>
		/// Returns a single string, delimited with <paramref name="delimiter"/> from source.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static string ToStringPretty<T>(this IEnumerable<T> source, string delimiter) => (source == null) ? string.Empty : ToStringPretty(source, string.Empty, delimiter, string.Empty);

		/// <summary>
		/// Returns a delimited string, appending <paramref name="before"/> at the start,
		/// and <paramref name="after"/> at the end of the string
		/// Ex: Enumerable.Range(0, 10).ToStringPretty("From 0 to 9: [", ",", "]")
		/// returns: From 0 to 9: [0,1,2,3,4,5,6,7,8,9]
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="before"></param>
		/// <param name="delimiter"></param>
		/// <param name="after"></param>
		/// <returns></returns>
		public static string ToStringPretty<T>(this IEnumerable<T> source, string before, string delimiter, string after) {
			if (source == null) return string.Empty;

			StringBuilder result = new StringBuilder();
			result.Append(before);

			bool firstElement = true;
			foreach (T elem in source) {
				if (firstElement) firstElement = false;
				else result.Append(delimiter);

				result.Append(elem.ToString());
			}

			result.Append(after);
			return result.ToString();
		}

		/// <summary>
		/// Inverts the case of each character in the given string and returns the new string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The converted string.</returns>
		public static string InvertCase(this string s) => new string(s.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c)) : c).ToArray());

		/// <summary>
		/// Checks whether the given string is null, else if empty after trimmed.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>True if string is Null or Empty, false otherwise.</returns>
		public static bool IsNullOrEmptyAfterTrimmed(this string s) => (s.IsNullOrEmpty() || s.Trim().IsNullOrEmpty());

		/// <summary>
		/// Converts the given string to <see cref="List{Char}"/>.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>Returns a list of char (or null if string is null).</returns>
		public static List<char> ToCharList(this string s) => (s.IsNOTNullOrEmpty()) ? s.ToCharArray().ToList() : null;

		/// <summary>
		/// Extracts the substring starting from 'start' position to 'end' position.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="start">The start position.</param>
		/// <param name="end">The end position.</param>
		/// <returns>The substring.</returns>
		public static string SubstringFromXToY(this string s, int start, int end) {
			if (s.IsNullOrEmpty()) return string.Empty;

			// if start is past the length of the string
			if (start >= s.Length) return string.Empty;

			// if end is beyond the length of the string, reset
			if (end >= s.Length) end = s.Length - 1;

			return s.Substring(start, end - start);
		}

		/// <summary>
		/// Removes the given character from the given string and returns the new string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="c">The character to be removed.</param>
		/// <returns>The new string.</returns>
		public static string RemoveChar(this string s, char c) => (s.IsNOTNullOrEmpty()) ? s.Replace(c.ToString(), string.Empty) : string.Empty;

		/// <summary>
		/// Returns the number of words in the given string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The word count.</returns>
		public static int GetWordCount(this string s) => (new Regex(@"\w+")).Matches(s).Count;

		/// <summary>
		/// Checks whether the given string is a palindrome.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>True if the given string is palindrome, false otherwise.</returns>
		public static bool IsPalindrome(this string s) => s.Equals(s.Reverse());

		/// <summary>
		/// Checks whether the given string is NOT a palindrome.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>True if the given string is NOT palindrome, false otherwise.</returns>
		public static bool IsNotPalindrome(this string s) => s.IsPalindrome().Toggle();

		/// <summary>
		/// Checks whether the given string is a valid IP address using regular expressions.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>True if it is a valid IP address, false otherwise.</returns>
		public static bool IsValidIPAddress(this string s) {
			return Regex.IsMatch(s, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
		}

		/// <summary>
		/// Appends the given separator to the given string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="sep">The separator to be appended.</param>
		/// <returns>The appended string.</returns>
		public static string AppendSep(this string s, string sep) => s + sep;

		/// <summary>
		/// Appends the a comma to the given string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The appended string.</returns>
		public static string AppendComma(this string s) => s.AppendSep(",");

		/// <summary>
		/// Appends \r \n to a string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The appended string.</returns>
		public static string AppendNewLine(this string s) => s.AppendSep("\r\n");

		/// <summary>
		/// Appends \r \n to a string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The appended string.</returns>
		public static string AppendHtmlBr(this string s) => s.AppendSep("<br />");

		/// <summary>
		/// Appends the a space to the given string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The appended string.</returns>
		public static string AppendSpace(this string s) => s.AppendSep(" ");

		/// <summary>
		/// Appends the a hyphen to the given string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <returns>The appended string.</returns>
		public static string AppendHyphen(this string s) => s.AppendSep("-");

		/// <summary>
		/// Appends the given character to the given string and returns the new string.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="sep">The character to be appended.</param>
		/// <returns>The appended string.</returns>
		public static string AppendSep(this string s, char sep) => s.AppendSep(sep.ToString());

		/// <summary>
		/// Returns this string + sep + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <param name="sep">The separator to be introduced in between these two strings.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithSep(this string s, string newString, string sep) => s.AppendSep(sep) + newString;

		/// <summary>
		/// Returns this string + sep + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <param name="sep">The separator to be introduced in between these two strings.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithSep(this string s, string newString, char sep) => s.AppendSep(sep) + newString;

		/// <summary>
		/// Returns this string + "," + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithComma(this string s, string newString) => s.AppendWithSep(newString, ",");

		/// <summary>
		/// Returns this string + "\r\n" + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithNewLine(this string s, string newString) => s.AppendWithSep(newString, "\r\n");

		/// <summary>
		/// Returns this string + "\r\n" + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithHtmlBr(this string s, string newString) => s.AppendWithSep(newString, "<br />");

		/// <summary>
		/// Returns this string + "-" + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithHyphen(this string s, string newString) => s.AppendWithSep(newString, "-");

		/// <summary>
		/// Returns this string + " " + newString.
		/// </summary>
		/// <param name="s">The given string.</param>
		/// <param name="newString">The string to be concatenated.</param>
		/// <returns>The appended string.</returns>
		/// <remarks>This may give poor performance for large number of strings used in loop. Use <see cref="StringBuilder"/> instead.</remarks>
		public static string AppendWithSpace(this string s, string newString) => s.AppendWithSep(newString, " ");

		/// <summary>
		/// Converts the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
		/// </summary>
		/// <param name="mText"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string mText) {
			if (mText.IsNullOrEmpty())  return mText;

			// get globalization info
			System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
			System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

			// convert to title case
			return textInfo.ToTitleCase(mText);
		}

		/// <summary>
		/// Adds extra spaces to meet the total length
		/// </summary>
		/// <param name="s"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string PadRightEx(this string s, int length) {
			// exit if string is already at length
			if ((!s.IsNullOrEmpty()) && (s.Length >= length)) return s;

			// if string is null, then return empty string
			// else, add spaces and exit
			return (s != null) ? string.Format("{0}{1}", s, new string(' ', length - s.Length)) : new string(' ', length);
		}

		/// <summary>
		/// Computes the FNV-1a hash for the input string. 
		/// The FNV-1a hash is a non-cryptographic hash function known for its speed and good distribution properties.
		/// Useful for creating Dictionary keys instead of using strings.
		/// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="str">The input string to hash.</param>
		/// <returns>An integer representing the FNV-1a hash of the input string.</returns>
		public static int ComputeFNV1aHash(this string str) {
			uint hash = 2166136261;
			foreach (char c in str) {
				hash = (hash ^ c) * 16777619;
			}
			return unchecked((int)hash);
		}
    }
}