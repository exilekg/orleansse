using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackRock.OrleansStockExchange.Grains
{
    internal class UserGrainState
    {
        public List<Guid> Securities { get; set; } = new List<Guid>();
    }
}
