using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
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