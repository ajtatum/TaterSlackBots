namespace StanLeeSlackBot.Classes
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
		    int removeFromEnd = 0;
		    for (int i = value.Length - 1; i >= 0; i--)
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
	}
}
