using ServerBusinessLogic.Enums.Transmission;

namespace ServerBusinessLogic.TransmissionModels
{
    public class OperationResultInfo
    {
        public ListenerType ToListener { get; set; }
        public string ErrorInfo { get; set; }
        public OperationsResults OperationResult { get; set; }
        public object JsonData { get; set; }
    }
}
