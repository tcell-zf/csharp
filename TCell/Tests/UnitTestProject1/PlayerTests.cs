using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mexxum.MediaPlayerPlugins.PowerPointPlayer2013.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        [TestMethod()]
        public void ExecuteCommandTest()
        {
            Player player = new Player();
            player.StartPlayer(null);
            player.ExecuteCommand(@"play?deviceids=PC_Boss,PC_Slave&path=C:\Projects\Mexxum\UAES\互动赛车体验升级方案-0723 David.ppsx");
            player.ExecuteCommand(@"ppt-control?deviceids=PC_Boss,PC_Slave&path=C:\Projects\Mexxum\UAES\互动赛车体验升级方案-0723 David.ppsx&action=last");
            player.StopPlayer();
            Assert.IsTrue(true);
        }
    }
}