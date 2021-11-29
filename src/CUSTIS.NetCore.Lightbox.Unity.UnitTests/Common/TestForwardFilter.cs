using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Filters;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    public class TestForwardFilter : IOutboxForwardFilter
    {
        public static ForwardContext ForwardContext { get; private set; }

        public static void Reset()
        {
            ForwardContext = null;
        }

        /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
        public Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token)
        {
            ForwardContext = context;

            return next(context, token);
        }
    }
}