using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Filters;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    public class TestPutFilter2 : IOutboxPutFilter
    {
        public static PutContext PutContext { get; private set; }

        public static void Reset()
        {
            PutContext = null;
        }

        /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
        public Task PutMessage(PutContext context, PutDelegate next, CancellationToken token)
        {
            PutContext = context;
            return next(context, token);
        }
    }
}