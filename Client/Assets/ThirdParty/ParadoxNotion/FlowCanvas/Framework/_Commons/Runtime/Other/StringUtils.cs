using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace ParadoxNotion{

	///Some common string utilities
	public static class StringUtils {

		private static Dictionary<string, string> splitCaseCache = new Dictionary<string, string>(StringComparer.Ordinal);

		///Convert camelCase to words.
		public static string SplitCamelCase(this string s){
			if (string.IsNullOrEmpty(s)){
				return s;
			}

			string result;
			if (splitCaseCache.TryGetValue(s, out result)){
				return result;
			}
			if (s.StartsWith("_")){ s = s.Substring(1); }
			result = s.Replace("_", " ");
			result = char.ToUpper(result[0]) + result.Substring(1);
			result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1").Trim();
			return splitCaseCache[s] = result;
		}

		///Caps the length of a string to max length and adds "..." if more.
		public static string CapLength(this string s, int max){
			if (string.IsNullOrEmpty(s)){ return s; }
			var result = s.Substring(0, Mathf.Min(s.Length, max) );
			if (result.Length < s.Length){ result += "..."; }
			return result;
		}

		///Gets only the capitals of the string trimmed.
		public static string GetCapitals(this string s){
	    	if (string.IsNullOrEmpty(s)){
	    		return string.Empty;
	    	}
	    	var result = "";
	    	foreach(var c in s){
	    		if (char.IsUpper(c)){
	    			result += c.ToString();
	    		}
	    	}
	    	result = result.Trim();
	    	return result;			
		}

		///Returns the alphabet letter based on it's index.
		public static string GetAlphabetLetter(int index){
			if (index < 0){
				return null;
			}

			var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			if (index >= letters.Length){
				return index.ToString();
			}

			return letters[index].ToString();
		}

		///Get the string result within from to
		public static string GetStringWithin(this string input, string from, string to){
			var pattern = string.Format(@"{0}(.*?){1}", from, to);
			var regex = new Regex(pattern);
			var match = regex.Match(input);
			return match.Groups[1].ToString();
		}

		///Returns whether or not the input is valid for a search match vs the target.
		///Done by splitting input to words and checking sequential occurency within target string.
		public static bool SearchMatch(string input, string target){
			//ignore case
			input = input.ToUpper();
			target = target.ToUpper();

			//treat dot as spaces and split to words
			var words = input.Replace('.', ' ').Split(' ');

			//at least one word should also be contained in target leaf name in case we have path
			var leafName = target.Contains('/')? target.Substring(target.LastIndexOf('/') + 1) : target;

			//if fewer than 3 chars, do a direct match check with leaf name
			if (input.Length <= 2){
				return input == leafName;
			}

			var match = false;
			var lastIndex = -1;
			foreach(var word in words){
				if (!target.Contains(word)){
					return false;
				}
				var index = target.IndexOf(word);
				if (lastIndex > index){
					return false;
				}
				if (leafName.Contains(word)){
					match = true;
				}
				lastIndex = index;
			}
			return match;
		}

		///A more complete ToString version
		public static string ToStringAdvanced(this object o) {

			if (o == null || o.Equals(null)){
				return "NULL";
			}

			if (o is string){
				return string.Format("\"{0}\"", (string)o);
			}

			if (o is UnityEngine.Object){
				return (o as UnityEngine.Object).name;
			}

			var t = o.GetType();
			if (t.RTIsSubclassOf(typeof(System.Enum))) {
				if (t.RTIsDefined<System.FlagsAttribute>(true)) {
					var value = "";
					var cnt = 0;
					var list = System.Enum.GetValues(t);
					foreach (var e in list) {
						if ((Convert.ToInt32(e) & Convert.ToInt32(o)) == Convert.ToInt32(e)) {
							cnt++;
							if (value == "") {
								value = e.ToString();
							} else {
								value = "Mixed...";
							}
						}
					}
					if (cnt == 0) {
						return "Nothing";
					}
					if (cnt == list.Length){
						return "Everything";
					}
					return value;
				}
			}

			return o.ToString();
		}
	}
}