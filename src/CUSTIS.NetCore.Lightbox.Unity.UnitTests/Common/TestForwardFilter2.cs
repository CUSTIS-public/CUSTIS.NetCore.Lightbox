using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Filters;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    public class TestForwardFilter2 : IOutboxForwardFilter
    {
        /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
        public Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token)
        {
            return next(context, token);
        }
    }
}