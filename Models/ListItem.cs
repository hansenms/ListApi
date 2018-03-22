namespace ListApi.Models
{
    public class ListItem {
        public int ListItemId { get; set; }
        public string Description { get; set; }

        public string Owner { get; set;}

        public bool Completed {get; set;}
    } 
}