using System.Windows;
using System.Windows.Controls;
using GameGuideApp.ViewModels;

namespace GameGuideApp.Selectors
{
    public class ContentBlockTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeadingTemplate { get; set; }
        public DataTemplate TipTemplate { get; set; }
        public DataTemplate StepTemplate { get; set; }
        public DataTemplate MapTemplate { get; set; }
        public DataTemplate ParagraphTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var block = item as ContentBlock;
            if (block == null) return ParagraphTemplate;
            switch (block.Type)
            {
                case ContentBlockType.Heading: return HeadingTemplate;
                case ContentBlockType.Tip: return TipTemplate;
                case ContentBlockType.Step: return StepTemplate;
                case ContentBlockType.Map: return MapTemplate;
                default: return ParagraphTemplate;
            }
        }
    }
}
