using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SB.StanLee.Extensions
{
    public static class StringExtensions
    {
	    /// <summary>
	    /// TrimPunctuation from start and end of string.
	    /// </summary>
	    public static string TrimPunctuation(this string value)
	    {
		    // Count start punctuation.
		    int removeFromStart = 0;
		    for (int i = 0; i < value.Length; i++)
		    {
			    if (char.IsPunctuation(value[i]))
			    {
				    removeFromStart++;
			    }
			    else
			    {
				    break;
			    }
		    }

		    // Count end punctuation.
		    var removeFromEnd = 0;
		    for (var i = value.Length - 1; i >= 0; i--)
		    {
			    if (char.IsPunctuation(value[i]))
			    {
				    removeFromEnd++;
			    }
			    else
			    {
				    break;
			    }
		    }
		    // No characters were punctuation.
		    if (removeFromStart == 0 &&
		        removeFromEnd == 0)
		    {
			    return value;
		    }
		    // All characters were punctuation.
		    if (removeFromStart == value.Length &&
		        removeFromEnd == value.Length)
		    {
			    return "";
		    }
		    // Substring.
		    return value.Substring(removeFromStart,
			    value.Length - removeFromEnd - removeFromStart);
	    }

	    public static string ToCleanMessage(this string value)
	    {
		    var arr = value.ToCharArray();

		    var allowedCharacters = new[] {'-', '\''};


			arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
		                                          || char.IsWhiteSpace(c)
		                                          || allowedCharacters.Contains(c))));
		    return new string(arr);
		}

	    public static string StripAscii(this string value)
	    {
			return Regex.Replace(value, @"[^\u0000-\u007F]+", string.Empty);
		}

		public static string LatinToAscii(this string value)
	    {
		    var newStringBuilder = new StringBuilder();
		    newStringBuilder.Append(value.Normalize(NormalizationForm.FormKD)
			    .Where(x => x < 128)
			    .ToArray());
		    return newStringBuilder.ToString();
	    }
	}
}
