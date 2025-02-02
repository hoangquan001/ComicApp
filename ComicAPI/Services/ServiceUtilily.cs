using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ComicApp.Models;

public class ServiceUtilily
{
    static Random random = new Random();

    public static string Base64Encode(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string GetGlobalConfig(string base64EncodedData)
    {
        return Base64Decode(base64EncodedData);
    }
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Define a regular expression for validating email format
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
    }
    // Hàm giải mã chuỗi Base64
    public static string Base64Decode(string base64EncodedData)
    {
        try
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        catch (System.Exception)
        {
            return "";
        }
    }
    public static ServiceResponse<T> GetDataRes<T>(T? data)
    {
        var res = new ServiceResponse<T>();

        if (data == null || (data is bool d && d == false))
        {
            res.Status = 0;
            res.Message = "Error";

        }
        else
        {
            res.Data = data;
            res.Status = 1;
            res.Message = "Success";
        }

        return res;

    }

    public static void SuffleList<T>(List<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string AddTimestampToUrl(string url)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return $"{url}?timestamp={timestamp}";
    }

    public static List<T> SampleList<T>(List<T> list, List<float> weights, int sampleSize)
    {
        if (list.Count != weights.Count)
            throw new ArgumentException("The number of values must match the number of weights.");
        List<T> sampledValues = new List<T>();
        for (int i = 0; i < sampleSize && weights.Count > 0; i++)
        {
            double totalWeight = weights.Sum();
            double rnd = random.NextDouble() * totalWeight;

            double cumulativeWeight = 0.0;
            int selectedIndex = -1;

            for (int j = 0; j < weights.Count; j++)
            {
                cumulativeWeight += weights[j];
                if (rnd < cumulativeWeight)
                {
                    selectedIndex = j;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                sampledValues.Add(list[selectedIndex]);
                weights.RemoveAt(selectedIndex);
                list.RemoveAt(selectedIndex);
            }
        }

        return sampledValues;
    }

}

public class GlobalConfig
{
    static Dictionary<string, string> _globalConfig = new Dictionary<string, string>();
    static GlobalConfig()
    {
        // string filePath = "./Properties/globalconfig.txt";
        // string[] lines = File.ReadAllLines(filePath);
        // foreach (var line in lines)
        // {
        //     var parts = line.Split(new char[] { ':' }, 2);
        //     if (parts.Length == 2)
        //     {
        //         string key = parts[0].Trim();
        //         string value = parts[1].Trim();
        //         _globalConfig[key] = value;
        //     }
        // }
    }

    public static string GetString(string key)
    {
        if (!_globalConfig.ContainsKey(key))
        {
            return "";
        }
        return _globalConfig[key];

    }


}
public class SlugHelper
{
    public static Dictionary<string, string> _slugCache = new Dictionary<string, string>();
    public static string CreateSlug(string inputString, bool ignoreDot = true, bool useCache = false)
    {
        if (useCache && _slugCache.ContainsKey(inputString))
        {
            return _slugCache[inputString];
        }
        string slug = inputString.ToLowerInvariant().Trim();
        string unaccentedString = RemoveAccents(slug);
        if (ignoreDot)
            unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s]", ""); // Ignore dots
        else
            unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s.]", "");
        unaccentedString = Regex.Replace(unaccentedString, @"\s+", "-");
        unaccentedString = unaccentedString.Trim('-');
        if (useCache)
        {
            _slugCache[inputString] = unaccentedString;
        }
        return unaccentedString;


    }
    public static HashSet<string> GetListKey(string inputString)
    {

        string unaccentedString = RemoveAccents(inputString.ToLowerInvariant().Trim());
        unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s]", ""); // Ignore dots
        unaccentedString = Regex.Replace(unaccentedString, @"\s+", " ");
        return unaccentedString.Split(' ').ToHashSet();
    }
    public static bool GetTermFrequencyVector(string s, ref Dictionary<string, int> vector)
    {
        var words = s.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (vector.ContainsKey(word))
                vector[word]++;
            else
                vector[word] = 1;
        }

        return true;
    }
    public static string RemoveAccents(string accentedString)
    {
        string normalizedString = accentedString.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

}