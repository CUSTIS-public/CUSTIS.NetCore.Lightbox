using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
{
    internal class DbDependentSwitchman : ISwitchman
    {
        private readonly IOutboxMessageRepository _repository;

        internal DbDependentSwitchman(IOutboxMessageRepository repository)
        {
            _repository = repository;
        }
    }
}