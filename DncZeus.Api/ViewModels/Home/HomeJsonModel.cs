using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Home
{
    public class HomeJsonModel
    {
        public IList<CountCard> CountCards { get; set; }
    }
    public class CountCard
    {
        public string Title { get; set; }
        public int Count { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
        public string Color { get; set; }
    }
}
