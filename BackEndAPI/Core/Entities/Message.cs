namespace BackEndAPI.Core.Entities
{
    public class Message : BaseEntity<string>
    {
        public string SendUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string Text { get; set; }
    }
}
