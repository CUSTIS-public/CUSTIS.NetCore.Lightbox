namespace CUSTIS.NetCore.Outbox.Processing
{
    /// <summary> Коллекция стрелочников  </summary>
    internal interface ISwitchmanCollection
    {
        /// <summary> Получить стрелочника по типу сообщения </summary>
        SwitchmanInfo Get(string messageType);
    }
}