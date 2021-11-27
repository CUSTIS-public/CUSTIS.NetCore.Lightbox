using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
{
    internal class MultiArgSwitchman : ISwitchman
    {
        [Switchman(nameof(ProcessTwoArgs))]
        public void ProcessTwoArgs(Dto dto, string s)
        {
        }
    }
}