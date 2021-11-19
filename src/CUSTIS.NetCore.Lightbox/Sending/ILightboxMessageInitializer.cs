using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Initializes lightbox messages </summary>
    internal interface ILightboxMessageInitializer
    {
        /// <summary> Initialize lightbox message according to data in <paramref name="context"/> </summary>
        ILightboxMessage FillMessage(ILightboxMessage message, PutContext context);
    }
}