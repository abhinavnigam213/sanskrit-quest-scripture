using System;
using System.Collections.Generic;
using System.Text;

namespace SanskritQuest.Main.Common.Utilities;

public static class Transliterator
{
    private static readonly Dictionary<string, string> SlpVowels = new()
    {
        { "अ", "a" }, { "आ", "A" }, { "इ", "i" }, { "ई", "I" }, { "उ", "u" }, { "ऊ", "U" },
        { "ऋ", "f" }, { "ॠ", "F" }, { "ऌ", "x" }, { "ॡ", "X" }, { "ए", "e" }, { "ऐ", "E" },
        { "ओ", "o" }, { "औ", "O" }
    };

    private static readonly Dictionary<string, string> SlpMatras = new()
    {
        { "ा", "A" }, { "ि", "i" }, { "ी", "I" }, { "ु", "u" }, { "ू", "U" }, { "ृ", "f" },
        { "ॄ", "F" }, { "ॢ", "x" }, { "ॣ", "X" }, { "े", "e" }, { "ै", "E" }, { "ो", "o" },
        { "ौ", "O" }
    };

    private static readonly Dictionary<string, string> SlpConsonants = new()
    {
        { "क", "k" }, { "ख", "K" }, { "ग", "g" }, { "घ", "G" }, { "ङ", "N" },
        { "च", "c" }, { "छ", "C" }, { "ज", "j" }, { "झ", "J" }, { "ञ", "Y" },
        { "ट", "w" }, { "ठ", "W" }, { "ड", "q" }, { "ढ", "Q" }, { "ण", "R" },
        { "त", "t" }, { "थ", "T" }, { "द", "d" }, { "ध", "D" }, { "न", "n" },
        { "प", "p" }, { "फ", "P" }, { "ब", "b" }, { "भ", "B" }, { "म", "m" },
        { "य", "y" }, { "र", "r" }, { "ल", "l" }, { "व", "v" },
        { "श", "S" }, { "ष", "z" }, { "स", "s" }, { "ह", "h" },
        { "ळ", "L" }, { "क्ष", "kz" }, { "त्र", "tr" }, { "ज्ञ", "jY" }
    };

    private static readonly Dictionary<string, string> SlpShunya = new()
    {
        { "ं", "M" }, { "ः", "H" }, { "ँ", "~" }
    };

    private static readonly Dictionary<string, string> IastVowels = new()
    {
        { "अ", "a" }, { "आ", "ā" }, { "इ", "i" }, { "ई", "ī" }, { "उ", "u" }, { "ऊ", "ū" },
        { "ऋ", "ṛ" }, { "ॠ", "ṝ" }, { "ऌ", "ḷ" }, { "ॡ", "ḹ" }, { "ए", "e" }, { "ऐ", "ai" },
        { "ओ", "o" }, { "औ", "au" }
    };

    private static readonly Dictionary<string, string> IastMatras = new()
    {
        { "ा", "ā" }, { "ि", "i" }, { "ी", "ī" }, { "ु", "u" }, { "ू", "ū" }, { "ृ", "ṛ" },
        { "ॄ", "ṝ" }, { "ॢ", "ḷ" }, { "ॣ", "ḹ" }, { "े", "e" }, { "ै", "ai" }, { "ो", "o" },
        { "ौ", "au" }
    };

    private static readonly Dictionary<string, string> IastConsonants = new()
    {
        { "क", "k" }, { "ख", "kh" }, { "ग", "g" }, { "घ", "gh" }, { "ङ", "ṅ" },
        { "च", "c" }, { "छ", "ch" }, { "ज", "j" }, { "झ", "jh" }, { "ञ", "ñ" },
        { "ट", "ṭ" }, { "ठ", "ṭh" }, { "ड", "ḍ" }, { "ढ", "ḍh" }, { "ण", "ṇ" },
        { "त", "t" }, { "थ", "th" }, { "द", "d" }, { "ध", "dh" }, { "न", "n" },
        { "प", "p" }, { "फ", "ph" }, { "ब", "b" }, { "भ", "bh" }, { "म", "m" },
        { "य", "y" }, { "र", "r" }, { "ल", "l" }, { "व", "v" },
        { "श", "ś" }, { "ष", "ṣ" }, { "स", "s" }, { "ह", "h" },
        { "ळ", "ḷ" }, { "क्ष", "kṣ" }, { "त्र", "tr" }, { "ज्ञ", "jñ" }
    };

    private static readonly Dictionary<string, string> IastShunya = new()
    {
        { "ं", "ṁ" }, { "ः", "ḥ" }, { "ँ", "m\u0310" } // m̐
    };

    private static bool TryMatchConsonant(Dictionary<string, string> dict, string text, int index, out string value, out int matchLen)
    {
        value = null;
        matchLen = 0;
        int remaining = text.Length - index;
        int maxLen = Math.Min(2, remaining);
        for (int len = maxLen; len >= 1; len--)
        {
            string key = text.Substring(index, len);
            if (dict.TryGetValue(key, out value))
            {
                matchLen = len;
                return true;
            }
        }
        return false;
    }

    public static string DevanagariToSlp1(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var result = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == 'ॐ')
            {
                result.Append("oM");
                continue;
            }

            if (TryMatchConsonant(SlpConsonants, text, i, out var baseSlp, out var matchLen))
            {
                int afterBase = i + matchLen;
                char? next = (afterBase < text.Length) ? text[afterBase] : null;
                string nextStr = next.HasValue ? next.Value.ToString() : null;

                if (next == '्')
                {
                    result.Append(baseSlp);
                    i += matchLen; // advance past matched base; for-loop will add one more to skip halant
                }
                else if (next.HasValue && SlpMatras.TryGetValue(nextStr, out var matraVal))
                {
                    result.Append(baseSlp).Append(matraVal);
                    i += matchLen; // advance past matched base; for-loop will add one more to skip matra
                }
                else
                {
                    result.Append(baseSlp).Append('a');
                    i += matchLen - 1; // consumed multiple chars of base, adjust index
                }

                continue;
            }

            string cur = text.Substring(i, 1);
            if (SlpVowels.TryGetValue(cur, out var vowelVal))
            {
                result.Append(vowelVal);
                continue;
            }

            if (SlpShunya.TryGetValue(cur, out var shunyaVal))
            {
                result.Append(shunyaVal);
                continue;
            }

            result.Append(cur switch
            {
                "।" => "|",
                "॥" => "||",
                "ऽ" => "'",
                _ => cur
            });
        }

        return result.ToString();
    }

    public static string DevanagariToIast(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var result = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == 'ॐ')
            {
                result.Append("oṁ");
                continue;
            }

            if (TryMatchConsonant(IastConsonants, text, i, out var baseIast, out var matchLen))
            {
                int afterBase = i + matchLen;
                char? next = (afterBase < text.Length) ? text[afterBase] : null;
                string nextStr = next.HasValue ? next.Value.ToString() : null;

                if (next == '्')
                {
                    result.Append(baseIast);
                    i += matchLen; // advance past matched base; for-loop will add one more to skip halant
                }
                else if (next.HasValue && IastMatras.TryGetValue(nextStr, out var matraVal))
                {
                    result.Append(baseIast).Append(matraVal);
                    i += matchLen; // advance past matched base; for-loop will add one more to skip matra
                }
                else
                {
                    result.Append(baseIast).Append('a');
                    i += matchLen - 1; // consumed multiple chars of base, adjust index
                }

                continue;
            }

            string cur = text.Substring(i, 1);
            if (IastVowels.TryGetValue(cur, out var vowelVal))
            {
                result.Append(vowelVal);
                continue;
            }

            if (IastShunya.TryGetValue(cur, out var shunyaVal))
            {
                result.Append(shunyaVal);
                continue;
            }

            result.Append(cur switch
            {
                "।" => "|",
                "॥" => "||",
                _ => cur
            });
        }

        return result.ToString();
    }

    public static string IastToPhonetic(string iast)
    {
        if (string.IsNullOrEmpty(iast)) return string.Empty;

        return iast
            .Replace("ś", "sh").Replace("Ś", "Sh")
            .Replace("ṣ", "sh").Replace("Ṣ", "Sh")
            .Replace("ṛ", "ri").Replace("Ṛ", "Ri")
            .Replace("ā", "a").Replace("Ā", "A")
            .Replace("ī", "i").Replace("Ī", "I")
            .Replace("ū", "u").Replace("Ū", "U")
            .Replace("ṭ", "t").Replace("Ṭ", "T")
            .Replace("ḍ", "d").Replace("Ḍ", "D")
            .Replace("ṇ", "n").Replace("Ṇ", "N")
            .Replace("ṁ", "m").Replace("Ṁ", "M")
            .Replace("ḥ", "h").Replace("Ḥ", "H")
            .Replace("|", "")
            .Replace("\r", "")
            .Replace("\n", " ");
    }

    public static string IastToItrans(string iast)
    {
        if (string.IsNullOrEmpty(iast)) return string.Empty;

        return iast.ToLower()
            .Replace("ā", "aa")
            .Replace("ī", "ii")
            .Replace("ū", "uu")
            .Replace("ṛ", "RRi")
            .Replace("ñ", "~n")
            .Replace("ṅ", "N")
            .Replace("ś", "sh_")
            .Replace("ṣ", "Sh")
            .Replace("ṭ", "T")
            .Replace("ḍ", "D")
            .Replace("ṇ", "N")
            .Replace("ḥ", "H")
            .Replace("ṁ", "M");
    }
}
