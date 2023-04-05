using System.ComponentModel.DataAnnotations.Schema;
namespace API.Entities
{
    [Table("Tags")]
    public class PostTag
    {
        public int TagId { get; set; }
        public int PostId {get; set;}
        public Post Post {get; set;}
        public Tag Tag {get; set;}
    }
}