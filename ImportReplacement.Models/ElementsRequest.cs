using System.Collections.Generic;

namespace ImportReplacement.Models
{
    public class ElementsRequest
    {
        public IEnumerable<ConsumerElement> ConsumerElements { get; set; }
        public long CommandId { get; set; }

    }
}
