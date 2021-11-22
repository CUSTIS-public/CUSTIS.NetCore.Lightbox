using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    internal class TestSwitchman : ISwitchman
    {
        [Switchman(nameof(ProcessMessage))]
        public void ProcessMessage()
        {
        }
    }
}