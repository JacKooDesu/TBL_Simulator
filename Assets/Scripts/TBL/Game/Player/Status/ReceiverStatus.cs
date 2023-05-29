using System;
namespace TBL.Game
{
    public class ReceiverStatus : ValueTypeStatus<ReceiveEnum>
    {
        public ReceiverStatus(ReceiveEnum value, PlayerStatusType type = PlayerStatusType.Reciver) : base(value, type) { }
    }
}