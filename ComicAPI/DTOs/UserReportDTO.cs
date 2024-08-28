using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;


public class ErrorReportDTO
{
    public string name { get; set; } = string.Empty;
    public string errorType { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
    public int chapterid { get; set; } = 0;
}
