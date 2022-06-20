using System;
using UnityEngine;
using System.Text;

namespace Heartfield.Roputseug
{
    public sealed class RoputseugDictionary : MonoBehaviour
    {
        [HideInInspector] public string inputText;

        static void Main(string[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
            {
                Console.WriteLine(SeparateSyllables(arg[i]) + "\n");
            }
        }

        public static string SeparateSyllables(string input)
        {
            var syllables = RoputseugUtils.GetSyllables(input);
            var output = new StringBuilder();

            if (syllables.Count > 0)
            {
                const string separator = " - ";

                for (int i = 0; i < syllables.Count; i++)
                {
                    output.Append(syllables[i]);
                    output.Append(separator);
                }

                output = output.Remove(output.Length - 3, 3);
            }

            return output.ToString();
        }

        public static string Translate(string input)
        {
            var syllables = RoputseugUtils.GetSyllables(input);
            var output = new StringBuilder();

            for (int i = 0; i < syllables.Count; i++)
            {
                var arr = syllables[i].ToString().ToCharArray();

                if (RoputseugUtils.ShouldApplyRule(RoputseugUtils.GetCharId(arr[0])))
                    Array.Reverse(arr);

                output.Append(arr);
            }

            return output.ToString();
        }

        public static string TranslateAndSeparateSyllables(string input)
        {
            var translation = Translate(input);
            return SeparateSyllables(translation);
        }
    }
}