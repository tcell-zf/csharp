using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mexxum.WindowsServicePlugins.ModbusCommand.Tests
{
    [TestClass()]
    public class ReceiverTests
    {
        [TestMethod()]
        public void StartReceiverTest()
        {
            Receiver receiver = new Receiver();
            receiver.StartReceiver();
            receiver.StopReceiver();
            Assert.IsTrue(true);
        }
    }
}