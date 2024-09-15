
using System.ComponentModel.DataAnnotations;

public class AddCommentDTO
{
    public string? Content { get; set; }

    public int ChapterId { get; set; }

    public int? replyFromCmt { get; set; }

}