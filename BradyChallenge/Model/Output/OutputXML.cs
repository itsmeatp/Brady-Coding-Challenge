using System.Collections.Generic;

namespace BradyChallenge.Model.Output
{
    /// <summary>
    /// Class to represent the output data
    /// </summary>
    public class GenerationOutput
    {
        public List<Generator> Totals { get; set; }
        public List<Day1> MaxEmissionGenerators { get; set; }
        public List<ActualHeatRate> ActualHeatRates { get; set; }
        public GenerationOutput()
        {
            Totals = new List<Generator>();
            MaxEmissionGenerators = new List<Day1>();
            ActualHeatRates = new List<ActualHeatRate>();
        }
    }
    
    public class Generator
    {
        public string Name { get; set; }
        public double Total { get; set; }
    }

    public class Day1
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public double Emission { get; set; }
    }
    
    public class ActualHeatRate
    {
        public string Name { get; set; }
        public double HeatRate { get; set; }
    }
}
