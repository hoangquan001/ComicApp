public static class JaccardSearch
{
    public static Dictionary<string, List<string>> ngramCache = new Dictionary<string, List<string>>();

    public static List<string> GetNgrams(string text, int n, bool useCache = true)
    {
        if (useCache && ngramCache.TryGetValue(text, out List<string>? ngrams))
        {
            return ngrams;
        }
        ngrams = new List<string>();
        for (int i = 0; i <= text.Length - n; i++)
        {
            ngrams.Add(text.Substring(i, n));
        }
        if (useCache)
        {
            ngramCache.Add(key: text, ngrams);
        }
        return ngrams;
    }

    public static double CalculateSimilarity(List<string> ngrams1, List<string> ngrams2)
    {
        int matchCount = ngrams1.Intersect(ngrams2).Count();
        // Tổng số N-gram trong cả hai chuỗi
        int totalNgrams = ngrams1.Count + ngrams2.Count;
        // Trả về độ tương đồng N-gram
        return (2.0 * matchCount) / totalNgrams;
    }

    // Hàm tính độ tương đồng N-gram giữa hai chuỗi
    public static int CalNGram(string text)
    {
        return text.Length < 10 ? 2 : text.Length < 20 ? 3 : 4;
    }

}