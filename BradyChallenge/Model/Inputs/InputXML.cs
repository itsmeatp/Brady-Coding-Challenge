using System.Collections.Generic;

namespace BradyChallenge.Model.Inputs
{
    /// <summary>
    /// Class to represent the Input data
    /// </summary>
    public class GenerationReport
    {
        public List<WindGenerator> Wind { get; set; }
        public List<GasGenerator> Gas { get; set; }
        public List<CoalGenerator> Coal { get; set; }
    }
    public class Generator
    {
        public string Name { get; set; }
        public List<Day> Generation { get; set; }
        
    }
    public class WindGenerator : Generator
    {
        public string Location { get; set; }
    }
    public class GasGenerator : Generator
    {
        public double EmissionsRating { get; set; }
    }
    public class CoalGenerator : Generator
    {
        public double TotalHeatInput { get; set; }
        public double ActualNetGeneration { get; set; }
        public double EmissionsRating { get; set; }
    }
    public class Day
    {
        public string Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
    }
}
