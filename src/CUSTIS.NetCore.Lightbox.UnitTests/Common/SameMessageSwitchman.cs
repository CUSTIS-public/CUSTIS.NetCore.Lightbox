using CUSTIS.NetCore.Outbox.Contracts;

namespace CUSTIS.NetCore.UnitTests.Outbox.Common
{
    internal class SameMessageSwitchman : ISwitchman
    {
        [Switchman(TestSwitchman.MessageType)]
        public void ProcessMessage()
        {
        }
    }
}