using System.Collections.ObjectModel;
using GameGuideApp.Core.Models;

namespace GameGuideApp.ViewModels
{
    public class GameItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string ThumbEmoji { get; set; }
        public string ThumbColor { get; set; }
        public ObservableCollection<Guide> Guides { get; set; } = new ObservableCollection<Guide>();
    }
}
