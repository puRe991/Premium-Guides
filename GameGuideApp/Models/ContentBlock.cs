namespace GameGuideApp.ViewModels
{
    public class ContentBlock
    {
        public ContentBlockType Type { get; set; }
        public string Text { get; set; }
        public int StepNumber { get; set; }
        public string TipLabel { get; set; } = "TIPP";
    }
}
