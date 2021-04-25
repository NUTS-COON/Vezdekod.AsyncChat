namespace AsyncApi.Models
{
    public class KafkaMessage
    {
        public string Topic { get; set; }
        
        public string Text { get; set; }
        
        public string Sender { get; set; }
    }
}