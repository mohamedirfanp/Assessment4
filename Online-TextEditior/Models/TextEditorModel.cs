namespace Online_TextEditior.Models
{
	public class TextEditorModel
	{
        public int Id { get; set; }

		public string Title	{ get; set; }

        public string TextDescription { get; set; }
        public  string ShortDescription { get; set; }

        public string SharedUsers { get; set; }

        public string UserEmail { get; set; }
    }
}
