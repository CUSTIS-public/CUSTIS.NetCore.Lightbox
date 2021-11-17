using CUSTIS.NetCore.Outbox.Contracts;
using CUSTIS.NetCore.Outbox.DAL;

namespace CUSTIS.NetCore.UnitTests.Outbox.Common
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