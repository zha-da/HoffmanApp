using System.Collections.Generic;
using System.Text;

public static class CubaBeba
{
    public static string Decode(Dictionary<char, string> dictionary, string text)
    {
        Dictionary<string, char> dictionary2 = new Dictionary<string, char>();
        foreach (var item in dictionary)
        {
            dictionary2.Add(item.Value, item.Key);
        }
        string key = "";
        StringBuilder sb = new StringBuilder("");
        for (int i = 0; i < text.Length; i++)
        {
            key += text[i];
            if (dictionary2.ContainsKey(key))
            {
                sb.Append(dictionary2[key]);
                key = "";
            }
        }
        return sb.ToString();
    }
    public static string Encode(Dictionary<char, string> dic, string text)
    {
        StringBuilder sb = new StringBuilder("");
        for (int i = 0; i < text.Length; i++)
        {
            sb.Append(dic[text[i]]);
        }
        return sb.ToString();
    }
}