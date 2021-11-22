using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    internal class TestSwitchman2 : ISwitchman, ITestSwitchman2
    {
        [Switchman(nameof(Process2))]
        public void Process2()
        {
        }
    }
}