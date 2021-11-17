namespace CUSTIS.NetCore.UnitTests.Outbox.Common
{
    internal class Dto
    {
        public Dto(string msg)
        {
            Msg = msg;
        }

        public string Msg { get; set; }
    }
}