using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace ParadoxNotion
{

    ///Some common string utilities
    public static class StringUtils
    {

        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        readonly private static char[] CHAR_EMPTY_ARRAY = new char[] { ' ' };
        private static Dictionary<string, string> splitCaseCache = new Dictionary<string, string>(StringComparer.Ordinal);

        ///Convert camelCase to words.
        public static string SplitCamelCase(this string s) {
            if ( string.IsNullOrEmpty(s) ) { return s; }

            string result;
            if ( splitCaseCache.TryGetValue(s, out result) ) {
                return result;
            }

            result = s;
            var underscoreIndex = result.IndexOf('_');
            if ( underscoreIndex <= 1 ) {
                result = result.Substring(underscoreIndex + 1);
            }
            result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1").CapitalizeFirst().Trim();
            return splitCaseCache[s] = result;
        }

        ///Capitalize first letter
        public static string CapitalizeFirst(this string s) {
            if ( string.IsNullOrEmpty(s) ) { return s; }
            return s.First().ToString().ToUpper() + s.Substring(1);
        }

        ///Caps the length of a string to max length and adds "..." if more.
        public static string CapLength(this string s, int max) {
            if ( string.IsNullOrEmpty(s) || s.Length <= max || max <= 3 ) { return s; }
            var result = s.Substring(0, Mathf.Min(s.Length, max) - 3);
            result += "...";
            return result;
        }

        ///Gets only the capitals of the string trimmed.
        public static string GetCapitals(this string s) {
            if ( string.IsNullOrEmpty(s) ) {
                return string.Empty;
            }
            var result = "";
            foreach ( var c in s ) {
                if ( char.IsUpper(c) ) {
                    result += c.ToString();
                }
            }
            result = result.Trim();
            return result;
        }

        ///Returns the alphabet letter based on it's index.
        public static string GetAlphabetLetter(int index) {
            if ( index < 0 ) {
                return null;
            }

            if ( index >= ALPHABET.Length ) {
                return index.ToString();
            }

            return ALPHABET[index].ToString();
        }

        ///Get the string result within from to
        public static string GetStringWithin(this string input, string from, string to) {
            var pattern = string.Format(@"{0}(.*?){1}", from, to);
            var regex = new Regex(pattern);
            var match = regex.Match(input);
            return match.Groups[1].ToString();
        }

        ///Returns a simplistic matching score (0-1) vs leaf + optional category.
        ///Lower is better so can be used without invert in OrderBy.
        public static float ScoreSearchMatch(string input, string leafName, string categoryName = "") {
            if ( input == null || leafName == null ) return float.PositiveInfinity;
            if ( categoryName == null ) { categoryName = string.Empty; }

            input = input.ToUpper();
            leafName = leafName.ToUpper().Replace(" ", string.Empty);

            var words = input.Replace('.', ' ').Split(CHAR_EMPTY_ARRAY, StringSplitOptions.RemoveEmptyEntries);
            if ( words.Length == 0 ) {
                return 1;
            }

            if ( input.LastOrDefault() == '.' ) {
                //do score match for category
                leafName = categoryName.ToUpper().Replace(" ", string.Empty);
            }

            var targetLengthBefore = leafName.Length;
            var replaced = leafName;
            for ( var i = 0; i < words.Length; i++ ) {
                var word = words[i];
                replaced = replaced.Replace(word, string.Empty);
            }
            var targetLengthAfter = replaced.Length;

            var score = (float)targetLengthAfter / (float)targetLengthBefore;
            //remember lower is better
            if ( leafName.StartsWith(words[0]) ) {
                score *= 0.25f;
            }
            if ( leafName.StartsWith(words[words.Length - 1]) ) {
                score *= 0.5f;
            }

            return score;
        }

        ///Returns whether or not the input is valid for a search match vs the leaf + optional category.
        public static bool SearchMatch(string input, string leafName, string categoryName = "") {

            if ( input == null || leafName == null ) return false;
            if ( categoryName == null ) { categoryName = string.Empty; }

            if ( leafName.Length <= 1 && input.Length <= 2 ) {
                string alias = null; //usually only operator like searches are less than 2
                if ( ReflectionTools.op_CSharpAliases.TryGetValue(input, out alias) ) {
                    return alias == leafName;
                }
            }

            if ( input.Length <= 1 ) {
                return input == leafName;
            }

            //ignore case
            input = input.ToUpper();
            leafName = leafName.ToUpper().Replace(" ", string.Empty);
            categoryName = categoryName.ToUpper().Replace(" ", string.Empty);
            var fullPath = categoryName + "/" + leafName;

            //treat dot as spaces and split to words
            var words = input.Replace('.', ' ').Split(CHAR_EMPTY_ARRAY, StringSplitOptions.RemoveEmptyEntries);
            if ( words.Length == 0 ) {
                return false;
            }

            //last input char check
            if ( input.LastOrDefault() == '.' ) {
                return categoryName.Contains(words[0]);
            }

            //check match for sequential occurency
            var leftover = fullPath;
            for ( var i = 0; i < words.Length; i++ ) {
                var word = words[i];

                if ( !leftover.Contains(word) ) {
                    return false;
                }

                leftover = leftover.Substring(leftover.IndexOf(word) + word.Length);
            }

            //last word should also be contained in leaf name regardless
            var lastWord = words[words.Length - 1];
            return leafName.Contains(lastWord);
        }


        public static string ReplaceWithin(this string text, char startChar, char endChar, System.Func<string, string> Process) {
            var s = text;
            var i = 0;
            while ( ( i = s.IndexOf(startChar, i) ) != -1 ) {
                var end = s.Substring(i + 1).IndexOf(endChar);
                var input = s.Substring(i + 1, end); //what's in the brackets
                var output = s.Substring(i, end + 2); //what should be replaced (includes brackets)
                var result = Process(input);
                s = s.Replace(output, result);
                i++;
            }

            return s;
        }


        ///A more complete ToString version
        public static string ToStringAdvanced(this object o) {

            if ( o == null || o.Equals(null) ) {
                return "NULL";
            }

            if ( o is string ) {
                return string.Format("\"{0}\"", (string)o);
            }

            if ( o is UnityEngine.Object ) {
                return ( o as UnityEngine.Object ).name;
            }

            var t = o.GetType();
            if ( t.RTIsSubclassOf(typeof(System.Enum)) ) {
                if ( t.RTIsDefined<System.FlagsAttribute>(true) ) {
                    var value = string.Empty;
                    var cnt = 0;
                    var list = System.Enum.GetValues(t);
                    foreach ( var e in list ) {
                        if ( ( Convert.ToInt32(e) & Convert.ToInt32(o) ) == Convert.ToInt32(e) ) {
                            cnt++;
                            if ( value == string.Empty ) {
                                value = e.ToString();
                            } else {
                                value = "Mixed...";
                            }
                        }
                    }
                    if ( cnt == 0 ) {
                        return "Nothing";
                    }
                    if ( cnt == list.Length ) {
                        return "Everything";
                    }
                    return value;
                }
            }

            return o.ToString();
        }
    }
}