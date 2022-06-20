using System.Text;
using System.Collections.Generic;

namespace Heartfield.Roputseug
{
    static class RoputseugUtils
    {
        static readonly char[] symbols = new char[] { '!', '@', '#', '$', '%', '&', '*', '{', '[', '(', ')', ']', '}', '£', '¢', '?', '/', '|', '\u005C', ';', ':', ',', '.', '<', '>', '*', '-', '_', '+', '=', '§', '"' };
        static readonly char[] numbers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        static readonly char[] consonants = new char[] { 'b', 'c', 'ç', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        static readonly Dictionary<char, char> vowels = new Dictionary<char, char>()
        {
            {'a','a' },
            {'á','a' },
            {'à','a' },
            {'â','a' },
            {'ã','a' },

            {'e','e' },
            {'é','e' },
            {'è','e' },
            {'ê','e' },

            {'i','i' },
            {'í','i' },
            {'ì','i' },
            {'î','i' },

            {'o','o' },
            {'ó','o' },
            {'ò','o' },
            {'ô','o' },
            {'õ','o' },

            {'u','u' },
            {'ú','u' },
            {'ù','u' },
            {'û','u' },
        };
        static readonly string[] separableDigrafos = new string[] { "rr", "ss", "sc", "sç", "xc" };
        static readonly string[] nonSeparableDigrafos = new string[] { "ch", "lh", "nh", "gu", "qu" };

        static readonly Dictionary<char, int> charId = new Dictionary<char, int>()
        {
            { 'a', 1 },
            { 'b', 2 },
            { 'c', 3 },
            { 'ç', 3 },
            { 'd', 4 },
            { 'e', 5 },
            { 'f', 6 },
            { 'g', 7 },
            { 'h', 8 },
            { 'i', 9 },
            { 'j', 10 },
            { 'k', 11 },
            { 'l', 12 },
            { 'm', 13 },
            { 'n', 14 },
            { 'o', 15 },
            { 'p', 16 },
            { 'q', 17 },
            { 'r', 18 },
            { 's', 19 },
            { 't', 20 },
            { 'u', 21 },
            { 'v', 22 },
            { 'w', 23 },
            { 'x', 24 },
            { 'y', 25 },
            { 'z', 26 },
        };

        internal static bool ShouldApplyRule(int charId)
        {
            return charId % 2 == 0 || charId % 5 == 0;
        }

        internal static int GetCharId(char c)
        {
            return charId[char.ToLower(c)];
        }

        static bool Contains(string input, string[] c)
        {
            for (int i = 0; i < c.Length; i++)
            {
                if (input == c[i])
                    return true;
            }

            return false;
        }

        internal static bool IsSymbol(char a)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (a == symbols[i])
                    return true;
            }

            return false;
        }

        internal static bool IsNumber(char a)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                if (a == numbers[i])
                    return true;
            }

            return false;
        }

        internal static bool IsVowel(char c)
        {
            return vowels.ContainsKey(char.ToLower(c));
        }

        internal static bool IsSemiVowel(char a, char b)
        {
            if (!IsVowel(a) || !IsVowel(b))
                return false;

            char ax = vowels[char.ToLower(a)];
            char bx = vowels[char.ToLower(b)];

            return (ax == 'u' && IsVowel(bx)) || (bx == 'u' && IsVowel(ax)) || (ax == 'i' && IsVowel(bx)) || (bx == 'i' && IsVowel(ax));
        }

        internal static bool IsHiato(char a, char b)
        {
            return IsVowel(a) && IsVowel(b);
        }

        internal static bool IsSeparableDigrafo(char a, char b)
        {
            string s = $"{char.ToLower(a)}{char.ToLower(b)}";
            return Contains(s, separableDigrafos);
        }

        internal static bool IsNonSeparableDigrafo(char a, char b)
        {
            string s = $"{char.ToLower(a)}{char.ToLower(b)}";
            return Contains(s, nonSeparableDigrafos);
        }

        internal static bool IsConsonant(char c)
        {
            for (int i = 0; i < consonants.Length; i++)
            {
                if (char.ToLower(c) == consonants[i])
                    return true;
            }

            return false;
        }

        internal static bool ConsonantCluster(char a, char b)
        {
            var aLower = char.ToLower(a);
            var bLower = char.ToLower(b);

            if (aLower == 'b' || aLower == 'c' || aLower == 'd' || aLower == 'f' || aLower == 'g' || aLower == 'k' || aLower == 'p' || aLower == 't')
                return bLower == 'l' || bLower == 'r' || bLower == 's' || bLower == 'h';

            return false;
        }

        internal static bool IsLMNRS(char a)
        {
            var aLower = char.ToLower(a);
            return aLower == 'l' || aLower == 'm' || aLower == 'n' || aLower == 'r' || aLower == 's';
        }

        internal static bool VowelEndsWithLMNRS(char a, char b)
        {
            return IsVowel(a) && IsLMNRS(b);
        }

        internal static bool MergeChars(char a, char b)
        {
            bool condA = IsVowel(a) && char.ToLower(b) == 'r';
            bool condB = IsConsonant(a) && IsVowel(b);
            bool condC = IsHiato(a, b);
            bool condD = IsNonSeparableDigrafo(a, b);
            bool condE = !IsSeparableDigrafo(a, b);

            if (condD)
                return true;

            return condA || condB || (condC && !IsSemiVowel(a, b) && condE);
        }

        internal static List<StringBuilder> GetSyllables(string input)
        {
            var syllables = new List<StringBuilder>();

            if (input.Length <= 3)
            {
                syllables.Add(new StringBuilder(input));
            }
            else
            {
                var currSyllable = new StringBuilder();

                for (int i = 0; i < input.Length; i++)
                {
                    char currChar = input[i];
                    int nextId = i + 1;

                    currSyllable.Append(currChar);

                    if (nextId < input.Length)
                    {
                        char nextChar = input[nextId];
                        bool forceSeparation = false;
                        int afterNextId = nextId + 1;

                        if (afterNextId < input.Length)
                        {
                            if (IsVowel(currChar))
                            {
                                char charAfterNext = input[afterNextId];
                                bool afterLMNRS_isVowel = IsLMNRS(nextChar) && IsVowel(charAfterNext);

                                if (VowelEndsWithLMNRS(currChar, nextChar) && !afterLMNRS_isVowel && !IsNonSeparableDigrafo(nextChar, charAfterNext))
                                    continue;

                                forceSeparation = !afterLMNRS_isVowel;
                            }
                        }

                        if (i > 0)
                        {
                            char previousChar = input[i - 1];

                            if (IsNonSeparableDigrafo(previousChar, currChar))
                            {
                                if (IsVowel(nextChar))
                                    continue;
                            }
                        }

                        bool isPlural = nextId == input.Length - 1 && IsVowel(currChar) && char.ToLower(nextChar) == 's';
                        bool mergeChars = MergeChars(currChar, nextChar);
                        bool consonantCluster = ConsonantCluster(currChar, nextChar);

                        if (!forceSeparation && (isPlural || mergeChars || consonantCluster))
                            continue;
                    }

                    var sb = new StringBuilder();
                    sb.Append(currSyllable);
                    syllables.Add(sb);
                    currSyllable.Clear();
                }
            }

            return syllables;
        }

        internal static byte[] ConvertToByteArray(char[] arr)
        {
            return Encoding.ASCII.GetBytes(arr);
        }
    }
}