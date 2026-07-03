using System.Text.RegularExpressions;

namespace SanskritQuest.Common.Utilities
{
	public enum SanskritScheme
	{
		Unknown,
		Devanagari,
		IAST,
		SLP1
	}

	public static class SanskritScriptDetector
	{
		// Devanagari Unicode Block range (U+0900 to U+097F)
		private static readonly Regex DevanagariRegex = new Regex(@"[\u0900-\u097F]", RegexOptions.Compiled);

		// IAST unique diacritic markers (macrons, dots below/above, tilde)
		private static readonly Regex IastRegex = new Regex(@"[āīūṛṝḷḻṃḥṅñṭḍṇśṣḗĀĪŪṚṜḶḺṂḤṄÑṬḌṆŚṢ]", RegexOptions.Compiled);

		// SLP1 unique signature characters (capital consonants, specific vowel extensions not used in generic English text)
		private static readonly Regex Slp1UniqueRegex = new Regex(@"[KCEGJNwWqQfFxXzZORL]", RegexOptions.Compiled);

		public static SanskritScheme DetectScheme(string inputText)
		{
			if (string.IsNullOrWhiteSpace(inputText))
			{
				return SanskritScheme.Unknown;
			}

			// 1. Check for Devanagari Script
			if (DevanagariRegex.IsMatch(inputText))
			{
				return SanskritScheme.Devanagari;
			}

			// 2. Check for IAST (International Alphabet of Sanskrit Transliteration)
			if (IastRegex.IsMatch(inputText))
			{
				return SanskritScheme.IAST;
			}

			// 3. Check for SLP1 (Sanskrit Library Phonetic Basic)
			// SLP1 uses specific upper-case ASCII letters for unique phonetic sounds
			if (Slp1UniqueRegex.IsMatch(inputText))
			{
				return SanskritScheme.SLP1;
			}

			// 4. Default Fallback
			// If it is standard lowercase English characters without diacritics, it could technically 
			// be basic SLP1 or plain text. You can treat it as SLP1 or return Unknown depending on context.
			return SanskritScheme.Unknown;
		}
	}
}
