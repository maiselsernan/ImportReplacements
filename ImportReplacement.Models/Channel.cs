using System;

namespace ImportReplacement.Models
{
    public class Channel
    {
        public long ChannelID { get; set; }
        public long EndUnit { get; set; }
        public int ChannelIndex { get; set; }
        public DateTime LastReceived { get; set; }
        public int Site { get; set; }
        public bool HasConsumer { get; set; }
    }
}
