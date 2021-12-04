using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Sending;

namespace CUSTIS.NetCore.Lightbox.TestsBase
{
    /// <summary> Тесты на <see cref="IMessageBox"/> </summary>
    public interface IMessageBoxTests
    {
        /// <summary> Сообщение без DTO успешно помещается в БД </summary>
        Task Put_NoDto_MessageInDb();

        /// <summary> Сообщение с DTO успешно помещается в БД </summary>
        Task Put_WithDto_MessageInDb();
    }
}