namespace EMEDS_Project.ViewModels
{
    public class ChatbotViewModel
    {
        public string? Question { get; set; }

        public string? Answer { get; set; }

        public List<string> Questions { get; set; }
            = new List<string>();
    }
}