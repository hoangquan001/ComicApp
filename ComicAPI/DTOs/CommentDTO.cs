using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace ComicApp.Models
{
    public class CommentDTO
    {
        public CommentDTO(Comment comment)
        {
            ID = comment.ID;
            ChapterID = comment.ChapterID;
            ComicID = comment.ComicID;
            UserID = comment.UserID;
            Content = comment.Content;
            CommentedAt = comment.CommentedAt;
            Replies = new List<CommentDTO>();
            UserName = comment.User?.FullName;
            Avatar = comment.User?.Avatar;
            ChapterName = comment.Chapter?.Url.ToString();
            for (int i = 0; i < comment.Replies?.Count; i++)
            {
                Replies.Add(new CommentDTO(comment.Replies[i]));
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