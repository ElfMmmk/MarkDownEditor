namespace CSharpMobileApp.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool Completed { get; set; }
    }
}
