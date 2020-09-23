using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_6.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string List { get; set; }
        public string Address { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public string Trip_Detail { get; set; }
        public string Directions { get; set; }
        public string Image { get; set; }
        public DateTime Posted { get; set; }
        public List<Comment> Comments { get; set; }
        [NotMapped]
        public string TagsString { get; set; }
        //adding the necessary CMS stuff
        public string Footer { get; set; }
        public string Url { get; set; }
        public int? ParentPageId { get; set; }
        public int? SortOrder { get; set; }

    }

    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string CommentText { get; set; }
    }

    public class List
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Comment { get; set; }
    }

    public class Address
    {
        public int? Id { get; set; }
        public string address { get; set; }
    }
}
