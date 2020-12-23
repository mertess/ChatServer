using ServerBusinessLogic.Enums.Transmission;

namespace ServerBusinessLogic.TransmissionModels
{
    public class ClientOperationMessage
    {
        public ClientOperations Operation { get; set; }
        public string JsonData { get; set; }
    }
}
