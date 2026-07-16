using System;
using System.Collections.Generic;
using System.Text;

namespace SanskritQuest.Database.Tools.Ingestion
{
    public static class SanskritTransliterator
    {
        private static readonly Dictionary<char, string> DevaToIastMap = new()
        {
            // Vowels
            {'अ', "a"}, {'आ', "ā"}, {'इ', "i"}, {'ई', "ī"}, {'उ', "u"}, {'ऊ', "ū"},
            {'ऋ', "ṛ"}, {'ॠ', "ṝ"}, {'ऌ', "ḷ"}, {'ॡ', "ḹ"}, {'ए', "e"}, {'ऐ', "ai"},
            {'ओ', "o"}, {'औ', "au"},
            // Matras (Vowel signs)
            {'ा', "ā"}, {'ि', "i"}, {'ी', "ī"}, {'ु', "u"}, {'ू', "ū"},
            {'ृ', "ṛ"}, {'ॄ', "ṝ"}, {'ॢ', "ḷ"}, {'ॣ', "ḹ"}, {'े', "e"}, {'ै', "ai"},
            {'ो', "o"}, {'ौ', "au"},
            // Consonants
            {'क', "ka"}, {'ख', "kha"}, {'ग', "ga"}, {'घ', "gha"}, {'ङ', "ṅa"},
            {'च', "ca"}, {'छ', "cha"}, {'ज', "ja"}, {'झ', "jha"}, {'ञ', "ña"},
            {'ट', "ṭa"}, {'ठ', "ṭha"}, {'ड', "ḍa"}, {'ढ', "ḍha"}, {'ण', "ṇa"},
            {'त', "ta"}, {'थ', "tha"}, {'द', "da"}, {'ध', "dha"}, {'न', "na"},
            {'प', "pa"}, {'फ', "pha"}, {'ब', "ba"}, {'भ', "bha"}, {'म', "ma"},
            {'य', "ya"}, {'र', "ra"}, {'ल', "la"}, {'व', "va"},
            {'श', "śa"}, {'ष', "ṣa"}, {'स', "sa"}, {'ह', "ha"},
            // Others
            {'ं', "ṃ"}, {'ः', "ḥ"}, {'ँ', "m̐"},
            // Digits
            {'०', "0"}, {'१', "1"}, {'२', "2"}, {'३', "3"}, {'४', "4"},
            {'५', "5"}, {'६', "6"}, {'७', "7"}, {'८', "8"}, {'९', "9"}
        };

        private static readonly Dictionary<char, string> DevaToSlp1Map = new()
        {
            // Vowels
            {'अ', "a"}, {'आ', "A"}, {'इ', "i"}, {'ई', "I"}, {'उ', "u"}, {'ऊ', "U"},
            {'ऋ', "f"}, {'ॠ', "F"}, {'ऌ', "x"}, {'ॡ', "X"}, {'ए', "e"}, {'ऐ', "E"},
            {'ओ', "o"}, {'औ', "O"},
            // Matras
            {'ा', "A"}, {'ि', "i"}, {'ी', "I"}, {'ु', "u"}, {'ू', "U"},
            {'ृ', "f"}, {'ॄ', "F"}, {'ॢ', "x"}, {'ॣ', "X"}, {'े', "e"}, {'ै', "E"},
            {'ो', "o"}, {'ौ', "O"},
            // Consonants
            {'क', "ka"}, {'ख', "Ka"}, {'ग', "ga"}, {'घ', "Ga"}, {'ङ', "Na"},
            {'च', "ca"}, {'छ', "Ca"}, {'ज', "ja"}, {'झ', "Ja"}, {'ञ', "Ya"},
            {'ट', "wa"}, {'ठ', "Wa"}, {'ड', "qa"}, {'ढ', "Qa"}, {'ण', "Ra"},
            {'त', "ta"}, {'थ', "Ta"}, {'द', "da"}, {'ध', "Da"}, {'न', "na"},
            {'प', "pa"}, {'फ', "Pa"}, {'ब', "ba"}, {'भ', "Ba"}, {'म', "ma"},
            {'य', "ya"}, {'र', "ra"}, {'ल', "la"}, {'व', "va"},
            {'श', "za"}, {'ष', "Sa"}, {'स', "sa"}, {'ह', "ha"},
            // Others
            {'ं', "M"}, {'ः', "H"}, {'ँ', "~"},
            // Digits
            {'०', "0"}, {'१', "1"}, {'२', "2"}, {'३', "3"}, {'४', "4"},
            {'५', "5"}, {'६', "6"}, {'७', "7"}, {'८', "8"}, {'९', "9"}
        };

        public static string ToIast(string devanagari)
        {
            if (string.IsNullOrWhiteSpace(devanagari)) return string.Empty;
            return Transliterate(devanagari, DevaToIastMap, "a", "āīūṛṝḷḹeaiou");
        }

        public static string ToSlp1(string devanagari)
        {
            if (string.IsNullOrWhiteSpace(devanagari)) return string.Empty;
            return Transliterate(devanagari, DevaToSlp1Map, "a", "AIUfFxxXeEO");
        }

        private static string Transliterate(string text, Dictionary<char, string> mapping, string schwa, string longVowels)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (mapping.TryGetValue(c, out var val))
                {
                    // Check if this is a consonant
                    bool isConsonant = IsDevanagariConsonant(c);

                    if (isConsonant)
                    {
                        // Check next character
                        if (i + 1 < text.Length)
                        {
                            char next = text[i + 1];
                            if (IsMatra(next))
                            {
                                // Consonant is followed by a matra, drop the implicit 'a'
                                sb.Append(val[..^1]); // Remove the trailing 'a'
                            }
                            else if (next == '्')
                            {
                                // Halant, drop the implicit 'a' and skip the halant character
                                sb.Append(val[..^1]);
                                i++; 
                            }
                            else
                            {
                                // Followed by another consonant or space/punctuation, keep the implicit 'a'
                                sb.Append(val);
                            }
                        }
                        else
                        {
                            // End of string, keep the implicit 'a' (or drop it for modern Hindi, but keep for classical Sanskrit)
                            sb.Append(val);
                        }
                    }
                    else
                    {
                        sb.Append(val);
                    }
                }
                else if (c != '्') // skip halant if it fell through
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static bool IsDevanagariConsonant(char c)
        {
            return c >= 'क' && c <= 'ह';
        }

        private static bool IsMatra(char c)
        {
            return (c >= 'ा' && c <= 'ौ') || c == '्';
        }
    }
}
