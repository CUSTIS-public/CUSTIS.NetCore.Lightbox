using CUSTIS.NetCore.Outbox.Contracts;

namespace CUSTIS.NetCore.UnitTests.Outbox.Common
{
    internal class MultiArgSwitchman : ISwitchman
    {
        public const string MessageType = "msg";

        [Switchman(MessageType)]
        public void ProcessMessage(Dto dto, string s)
        {
        }
    }
}