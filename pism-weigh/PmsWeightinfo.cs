using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pism_weigh
{
    class PmsWeightinfo
    {
    public String uuid { get; set; }

    public String cargoPlate { get; set; }

        public Double roughWeight { get; set; }

        public Double tare { get; set; }

        public Double netWeight { get; set; }

        public String psimType { get; set; }

        public Boolean cargoComeOut { get; set; }

        public int printCount { get; set; }

        public String printUser { get; set; }

        public DateTime createDate { get; set; }
    }
}
