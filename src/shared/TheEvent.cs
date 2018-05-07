namespace shared
{
    public class TheEvent
    {
        public string WhatsUp { get; set; }

        public override string ToString()
        {
            return $"Whatsup: {WhatsUp}";
        }
    }
}