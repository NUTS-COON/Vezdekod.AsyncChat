namespace Contract.Request
{
    public class MessageSendRequest
    {
        public string To { get; set; }
        
        public string Text { get; set; }
        
        public string Sender { get; set; }
    }
}