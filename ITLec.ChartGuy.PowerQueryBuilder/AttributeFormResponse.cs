using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.ChartGuy.PowerQueryBuilder
{
 public    class AttributeFormResponse
    {
        //public string AttributeLogicName { get; set; }
        //public string CurrentAttributeDisplayName { get; set; }

      public   PowerQueryAttribute CurrentPowerQueryAttribute { get; set; }

        public List<PowerQueryAttribute> NewFields = new List<PowerQueryAttribute>();
    }
}
