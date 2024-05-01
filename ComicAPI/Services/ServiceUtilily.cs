using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public class ServiceUtilily
{
    public static string Base64Encode(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    // Hàm giải mã chuỗi Base64
    public static string Base64Decode(string base64EncodedData)
    {
        byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}

public class SlugHelper
{
    public static string CreateSlug(string inputString, bool ignoreDot = true)
    {
        try
        {
            string slug = inputString.ToLowerInvariant().Trim();
            string unaccentedString = RemoveAccents(slug);

            if (ignoreDot)
                unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s]", ""); // Ignore dots
            else
                unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s.]", "");

            unaccentedString = Regex.Replace(unaccentedString, @"\s+", "-");
            unaccentedString = unaccentedString.Trim('-');

            return unaccentedString;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in CreateSlug: {e.Message}");
            return inputString; // Return the input string as a fallback
        }
    }
    public static HashSet<string> GetListKey(string inputString)
    {

        string unaccentedString = RemoveAccents(inputString.ToLowerInvariant().Trim());
        unaccentedString = Regex.Replace(unaccentedString, @"[^\w\s]", ""); // Ignore dots
        unaccentedString = Regex.Replace(unaccentedString, @"\s+", " ");
        return unaccentedString.Split(' ').ToHashSet();
    }

    private static string RemoveAccents(string accentedString)
    {
        string normalizedString = accentedString.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }
}