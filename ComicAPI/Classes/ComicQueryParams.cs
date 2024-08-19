using ComicAPI.Enums;

namespace ComicAPI.Classes;
public class ComicQueryParams
{
    public int page { get; set; } = 1;
    public int step { get; set; } = 10;
    public ComicStatus status { get; set; } = ComicStatus.All;
    public SortType sort { get; set; } = SortType.TopAll;
    public int genre { get; set; } = -1;
    public int hot { get; set; } = -1;

}