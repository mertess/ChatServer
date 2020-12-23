namespace ServerBusinessLogic.ReceiveModels.NotificationModels
{
    public class NotificationReceiveModel
    {
        public int? Id { get; set; }

        public string Message { get; set; }

        public int FromUserId { get; set; }

        public int ToUserId { get; set; }

        public bool IsAccepted { get; set; }
    }
}
