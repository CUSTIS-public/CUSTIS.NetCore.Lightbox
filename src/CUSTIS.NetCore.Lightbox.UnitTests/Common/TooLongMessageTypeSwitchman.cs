using CUSTIS.NetCore.Outbox.Contracts;

namespace CUSTIS.NetCore.UnitTests.Outbox.Common
{
    internal class TooLongMessageTypeSwitchman : ISwitchman
    {
        public const string MessageType = "slkfvjnsdlkfndlkfnlkfvndlkfjvnsdlkfvnsdlkfjvnsldkjfvnlskdjfvnskldjfvnklsdjfvnklsdjfvnskldjfvnskldjfvnklsdjfnvklsdjfnvklsdjfnvklsdjfnlvksjdfnvkljsdnfvkljsdfnlvkjsdnfvkljsdnflvkjsdnflkvjsdnflkvjnsdlfkvjnsdlfkvjnsdlkfjvn";

        [Switchman(MessageType)]
        public void ProcessMessage()
        {
        }
    }
}