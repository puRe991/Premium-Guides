using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameGuideApp.ViewModels;

namespace GameGuideApp.Tests
{
    [TestClass]
    public class ParseContentTests
    {
        [TestMethod]
        public void ParseContent_Empty_ReturnsEmptyList()
        {
            var blocks = MainViewModel.ParseContent(string.Empty);
            Assert.AreEqual(0, blocks.Count);
        }

        [TestMethod]
        public void ParseContent_Heading_ReturnsHeadingBlock()
        {
            var blocks = MainViewModel.ParseContent("## Heading");
            Assert.AreEqual(1, blocks.Count);
            Assert.AreEqual(ContentBlockType.Heading, blocks[0].Type);
        }

        [TestMethod]
        public void ParseContent_Multiline_ReturnsExpectedSequence()
        {
            var raw = "## H\nText\n>> TIPP | Tip\n- Step\n[MAP] Area";
            var blocks = MainViewModel.ParseContent(raw);
            Assert.AreEqual(5, blocks.Count);
            Assert.AreEqual(ContentBlockType.Heading, blocks[0].Type);
            Assert.AreEqual(ContentBlockType.Paragraph, blocks[1].Type);
            Assert.AreEqual(ContentBlockType.Tip, blocks[2].Type);
            Assert.AreEqual(ContentBlockType.Step, blocks[3].Type);
            Assert.AreEqual(ContentBlockType.Map, blocks[4].Type);
        }
    }
}
