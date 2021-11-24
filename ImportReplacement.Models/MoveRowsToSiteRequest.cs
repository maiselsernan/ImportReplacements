using System.Collections.Generic;

namespace ImportReplacement.Models
{
    public class MoveRowsToSiteRequest
    {
        public IEnumerable<long> CommandIds { get; set; }
        public int? SiteId { get; set; }

    }
}
