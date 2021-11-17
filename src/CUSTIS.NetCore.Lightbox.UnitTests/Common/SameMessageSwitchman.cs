using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
{
    internal class SameMessageSwitchman : ISwitchman
    {
        [Switchman(TestSwitchman.MessageType)]
        public void ProcessMessage()
        {
        }
    }
}