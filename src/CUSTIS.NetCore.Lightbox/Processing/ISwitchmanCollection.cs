namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <summary> Коллекция стрелочников  </summary>
    internal interface ISwitchmanCollection
    {
        /// <summary> Получить стрелочника по типу сообщения </summary>
        SwitchmanInfo Get(string messageType);
    }
}