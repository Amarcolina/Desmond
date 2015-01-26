using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

public class BraceMatch {
    public int startIndex, endIndex;
    public string contents;

    public BraceMatch(int a, int b, string c) {
        startIndex = a;
        endIndex = b;
        contents = c;
    }
}

public class MatchFunc {
}

public class StringHelper {
    private static Dictionary<KeyValuePair<string, string>, int> _lookupTable = new Dictionary<KeyValuePair<string, string>, int>();

    public static string getBraced(string s) {
        Regex r = new Regex(@"<([^<>]*)>");
        Match m = r.Match(s);
        if (m.Groups.Count > 1) {
            return m.Groups[1].Captures[0].Value;
        }
        return null;
    }

    public delegate bool StringDelegate(string s);

    public static bool doesMatch(string[] split, params StringDelegate[] delegates) {
        for (int i = 0; i < delegates.Length; i++) {
            string value = i < split.Length ? split[i] : null;
            if (!delegates[i](value)) {
                return false;
            }
        }
        return true;
    }

    public static List<string[]> getMatchingBraces(string s, params StringDelegate[] ss) {
        List<string[]> matchesList = new List<string[]>();

        string match;
        while (getBraced(s, out match)) {
            string[] matches = match.Split(' ');
            if (ss.Length == matches.Length) {

                bool didMatchAll = true;
                for (int i = 0; i < ss.Length; i++) {
                    string value = i < matches.Length ? matches[i] : null;
                    if (!ss[i](value)) {
                        didMatchAll = false;
                        break;
                    }
                }

                if (didMatchAll) {
                    matchesList.Add(matches);
                }
            }

            s = replaceBrace(s, "");
        }

        return matchesList;
    }

    public static bool getBraced(string s, out string match) {
        Regex r = new Regex(@"<([^<>]*)>");
        Match m = r.Match(s);
        if (m.Groups.Count > 1) {
            match = m.Groups[1].Captures[0].Value;
            return true;
        }
        match = null;
        return false;
    }

    public static string replaceBrace(string text, string replacement) {
        Regex r = new Regex(@"<[^<>]*>");
        return r.Replace(text, replacement, 1);
    }

    public static string toClassName(string s, bool upFirst = true) {
        bool shouldUpper = upFirst;
        string outString = "";
        foreach(char c in s){
            if (c == ' ') {
                shouldUpper = true;
            }else if (char.IsLetterOrDigit(c)) {
                if (shouldUpper) {
                    outString += char.ToUpper(c);
                    shouldUpper = false;
                } else {
                    outString += c;
                }
            }
        }
        return outString;
    }

    public static string capitalize(string s, int index = 0) {
        if (s.Length == 0) {
            return s;
        }
        char c = s.ToUpper()[index];
        char[] ca = s.ToCharArray();
        ca[index] = c;
        return new string(ca);
    }

    public static int findSimilarity(string a, string b) {
        return findSimilarity(new KeyValuePair<string, string>(a, b));
    }

    private static int findSimilarity(KeyValuePair<string,string> pair) {
        int result;
        if(_lookupTable.TryGetValue(pair, out result)){
            return result;
        }

        string a = pair.Key;
        string b = pair.Value;
        if (a.Length == 0) {
            return b.Length;
        }
        if (b.Length == 0) {
            return a.Length;
        }

        int cost = a[a.Length - 1] == b[b.Length - 1] ? 0 : 1;
        

        string subA = a.Substring(0, a.Length-1);
        string subB = b.Substring(0, b.Length-1);

        result = Mathf.Min(findSimilarity(new KeyValuePair<string, string>(subA, b)) + 1,
                           findSimilarity(new KeyValuePair<string, string>(a, subB)) + 1,
                           findSimilarity(new KeyValuePair<string, string>(subA, subB)) + cost);

        _lookupTable[pair] = result;
        return result;
    }
}
