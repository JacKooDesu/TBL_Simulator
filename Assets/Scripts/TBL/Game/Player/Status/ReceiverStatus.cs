using System;
namespace TBL.Game
{
    public class ReceiverStatus : ValueTypeStatus<ReceiveEnum>
    {
        public override PlayerStatusType Type() => PlayerStatusType.Reciver;
        public ReceiverStatus(ReceiveEnum value) : base(value) { }
        public void AddReciverStatus(ReceiveEnum r) =>
            Update(value | r);
    }
}