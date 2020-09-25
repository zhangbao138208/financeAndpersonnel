using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Wage
{
    public class WageReport
    {
        public string Title { get; set; }
        public Legend Legend { get; set; }
        public List<WageSeries> WageSeries { get; set; }
    }
    public class WageSeries
    {
        public string Name { get; set; }
        public string Type => "line";
        // public string Stack => "总量";
        public bool Smooth => true;
        public Areastyle AreaStyle { get; set; }
        public ItemStyle ItemStyle { get; set; }
        public List<float> Data { get; set; }
    }

    public class Areastyle
    {
        public AreastyleNormal Normal { get; set; }
    }
    public class AreastyleNormal
    {
        public string Color { get; set; }
    }
    
    public class ItemStyle
    {
        public ItemStyleNormal Normal { get; set; }
    }
    public class ItemStyleNormal
    {
        public ItemStyleLabel Label { get; set; }
    }
    public class ItemStyleLabel
    {
        public bool Show => true;
    }

    public class Legend
    {
        public List<string> Data { get; set; }
    }

}
