using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
 public    class AttributeFormResponse
    {
        public string AttributeLogicName { get; set; }
        public string CurrentAttributeDisplayName { get; set; }

        public List<string> NewFields = new List<string>();
    }
}
