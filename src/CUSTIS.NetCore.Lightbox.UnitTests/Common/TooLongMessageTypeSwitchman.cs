using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
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