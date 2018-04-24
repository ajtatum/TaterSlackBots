using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SB.StanLee.Classes
{
    public static class Extensions
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

	    public static TConfig ConfigurePOCO<TConfig>(this IServiceCollection services, IConfiguration configuration, Func<TConfig> pocoProvider) where TConfig : class
	    {
		    if (services == null) throw new ArgumentNullException(nameof(services));
		    if (configuration == null) throw new ArgumentNullException(nameof(configuration));
		    if (pocoProvider == null) throw new ArgumentNullException(nameof(pocoProvider));
 
		    var config = pocoProvider();
		    configuration.Bind(config);
		    services.AddSingleton(config);
		    return config;
	    }
 
	    public static TConfig ConfigurePOCO<TConfig>(this IServiceCollection services, IConfiguration configuration, TConfig config) where TConfig : class
	    {
		    if (services == null) throw new ArgumentNullException(nameof(services));
		    if (configuration == null) throw new ArgumentNullException(nameof(configuration));
		    if (config == null) throw new ArgumentNullException(nameof(config));
 
		    configuration.Bind(config);
		    services.AddSingleton(config);
		    return config;
	    }
	}
}
