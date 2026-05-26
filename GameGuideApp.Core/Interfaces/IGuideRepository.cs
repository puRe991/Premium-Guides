using System.Collections.Generic;
using GameGuideApp.Core.Models;

namespace GameGuideApp.Core.Interfaces
{
    public interface IGuideRepository
    {
        IList<Guide> LoadAll();
        void SaveAll(IList<Guide> guides);
    }
}
