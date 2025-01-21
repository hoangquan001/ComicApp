
using ComicAPI.Services;
namespace ComicApp.Models
{
    public class CommentDTO
    {
        public CommentDTO(Comment comment, UrlService urlService)
        {
            ID = comment.ID;
            ChapterID = comment.ChapterID;
            ComicID = comment.ComicID;
            UserID = comment.UserID;
            Content = comment.Content;
            CommentedAt = comment.CommentedAt;
            Replies = new List<CommentDTO>();
            UserName = comment.User?.FullName;
            ChapterName = comment.Chapter?.Url.ToString();
            Avatar = urlService.GetUserImagePath(comment.User?.Avatar);
            for (int i = 0; i < comment.Replies?.Count; i++)
            {
                Replies.Add(new CommentDTO(comment.Replies[i], urlService));
            }
        }
        public int ID { get; set; }
        public int ChapterID { get; set; }
        public int ComicID { get; set; }
        public int UserID { get; set; }
        public string? Content { get; set; }
        public DateTime CommentedAt { get; set; }
        public string? UserName { get; set; }
        public string? ChapterName { get; set; }
        public string? Avatar { get; set; }
        public List<CommentDTO>? Replies { get; set; }

    }
}