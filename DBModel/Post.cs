using System;
using System.Collections.Generic;

namespace DBModel
{
    public class APost : RankByPost
    {
        public string Title { get; set; }
        public string Href { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
    }

    public class OnePost
    {
        public string Board { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public string Author { get; set; }
        public string Nickname { get; set; }
        public string Date { get; set; }
        public string Content { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public string Tag { get; set; }
        public string Userid { get; set; }
        public string Content { get; set; }
        public string IPdatetime { get; set; }
    }

    public class APagePosts
    {
        public int Page { get; set; }
        public Board BoardInfo { get; set; }
        public List<APost> PostList { get; set; }
        public Message Message { get; set; }
    }

    public class Board
    {
        public string Name { get; set; }
        public string Nuser { get; set; }
        public string Class { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public int MaxSize { get; set; }
    }

    public class Message
    {
        public string Error { get; set; }
        public string Metadata { get; set; }
    }

    public class RankByPost
    {
        public string Board { get; set; }
        public string AID { get; set; }
        public int Goup { get; set; }
        public int Down { get; set; }
    }

}
