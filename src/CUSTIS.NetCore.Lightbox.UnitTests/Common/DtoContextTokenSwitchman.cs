using System.Threading;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
{
    internal class DtoContextTokenSwitchman : ISwitchman
    {
        [Switchman(nameof(ProcessDtoContextToken))]
        public void ProcessDtoContextToken(Dto dto, ForwardContext context, CancellationToken token)
        {
        }
    }
}