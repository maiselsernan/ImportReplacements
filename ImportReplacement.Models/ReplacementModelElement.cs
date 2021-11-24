using System;
using System.Collections.Generic;
using System.Text;

namespace ImportReplacement.Models
{
    public class ConsumerElement
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsAssociated { get; set; }
    }
}
