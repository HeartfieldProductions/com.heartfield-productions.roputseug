using System.Collections.Generic;
using System;
using UnityEngine;

namespace Roputseug
{
    public class RoputseugDictionary : MonoBehaviour
    {
        public string inputText;

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

        static void Main(string[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
            {
                Console.WriteLine(Translate(arg[i]) + "\n");
            }
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

        static bool IsSymbol(char a)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (a == symbols[i])
                    return true;
            }

            return false;
        }

        static bool IsNumber(char a)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                if (a == numbers[i])
                    return true;
            }

            return false;
        }

        static bool IsVowel(char c) { return vowels.ContainsKey(c); }

        static bool IsSemiVowel(char a, char b)
        {
            if (!IsVowel(a) || !IsVowel(b))
                return false;

            char ax = vowels[a];
            char bx = vowels[b];

            return (ax == 'u' && IsVowel(bx)) || (bx == 'u' && IsVowel(ax)) || (ax == 'i' && IsVowel(bx)) || (bx == 'i' && IsVowel(ax));
        }

        static bool IsHiato(char a, char b) { return IsVowel(a) && IsVowel(b); }

        static bool IsSeparableDigrafo(char a, char b)
        {
            string s = string.Format("{0}{1}", a, b);
            return Contains(s, separableDigrafos);
        }

        static bool IsNonSeparableDigrafo(char a, char b)
        {
            string s = string.Format("{0}{1}", a, b);
            return Contains(s, nonSeparableDigrafos);
        }

        static bool IsConsonant(char c)
        {
            for (int i = 0; i < consonants.Length; i++)
            {
                if (c == consonants[i])
                    return true;
            }

            return false;
        }

        static bool ConsonantCluster(char a, char b)
        {
            if (a == 'b' || a == 'c' || a == 'd' || a == 'f' || a == 'g' || a == 'k' || a == 'p' || a == 't')
                return b == 'l' || b == 'r' || b == 's' || b == 'h';

            return false;
        }

        static bool IsLMNRS(char a) { return a == 'l' || a == 'm' || a == 'n' || a == 'r' || a == 's'; }

        static bool VowelEndsWithLMNRS(char a, char b) { return IsVowel(a) && IsLMNRS(b); }

        static bool MergeChars(char a, char b)
        {
            bool condA = IsVowel(a) && b == 'r';
            bool condB = IsConsonant(a) && IsVowel(b);
            bool condC = IsHiato(a, b);
            bool semiVowel = IsSemiVowel(a, b);
            bool condD = IsNonSeparableDigrafo(a, b);
            bool condE = !IsSeparableDigrafo(a, b);

            if (condD)
                return true;

            return condA || condB || (condC && !semiVowel && condE);
        }

        public static string Translate(string input)
        {
            List<string> syllables = new List<string>();

            if (input.Length > 3)
            {
                string currSyllable = string.Empty;

                for (int i = 0; i < input.Length; i++)
                {
                    char currChar = input[i];
                    int nextId = i + 1;

                    currSyllable += currChar;

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

                        bool isPlural = nextId == input.Length - 1 && IsVowel(currChar) && nextChar == 's';
                        bool mergeChars = MergeChars(currChar, nextChar);
                        bool consonantCluster = ConsonantCluster(currChar, nextChar);

                        if (!forceSeparation && (isPlural || mergeChars || consonantCluster))
                            continue;
                    }

                    syllables.Add(currSyllable);
                    currSyllable = string.Empty;
                }
            }
            else
                syllables.Add(input);

            string output = string.Empty;

            if (syllables.Count > 0)
            {
                for (int i = 0; i < syllables.Count; i++)
                {
                    output += syllables[i] + " - ";
                }

                output = output.Remove(output.Length - 3);
            }

            return output;
        }
    }
}