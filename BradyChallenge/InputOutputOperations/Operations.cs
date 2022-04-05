using BradyChallenge.Model.Inputs;
using BradyChallenge.Model.Output;
using BradyChallenge.Utilities;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace BradyChallenge.InputOutputOperations
{
    public class Operations : IOperations
    {
        #region Fields
        const string OUTPUT_FILE_NAME = "01-Basic-Result.xml";
        static readonly string ReferenceFile = ConfigurationManager.AppSettings["ReferencePath"];
        static readonly string OuputFolder = ConfigurationManager.AppSettings["OutputPath"];
        static GenerationOutput ResultData = new GenerationOutput();
        static double ValueFactor;
        static double EmissionFactor;
        static IXmlOperations XmlOperationsObject = new XmlOperations();
        #endregion

        #region PublicMethods
        /// <summary>
        /// Perform all operations when the input file is present in the input folder.
        /// </summary>
        /// <param name="e">event to notify the presence of input file</param>
        public void OperationsToPerform(FileSystemEventArgs e)
        {
            //FetchReferenceData
            ReferenceData referenceData = (ReferenceData)XmlOperationsObject.FromXml(XmlOperationsObject.ExtractInputData(ReferenceFile), typeof(ReferenceData));

            //Fetch Input Data
            GenerationReport generationReport = (GenerationReport)XmlOperationsObject.FromXml(XmlOperationsObject.ExtractInputData(e.FullPath), typeof(GenerationReport));

            #region Daily Generation Value
            //Calculate Totals for wind generator
            foreach (var generator in generationReport.Wind)
            {
                #region Switch ValueFactor
                switch (generator.Location)
                {
                    case "Offshore":
                        ValueFactor = referenceData.Factors.ValueFactor.Low; // Value factor Low for offshore
                        break;
                    case "Onshore":
                        ValueFactor = referenceData.Factors.ValueFactor.High; // Value factor High for onshore
                        break;
                    default:
                        Console.WriteLine("Invalid Location given. Check the input xml.");
                        return;
                }
                #endregion
                double dailyGenerationValue = CalculateDailyGenerationValue(generator);
                AddGeneratorToGenerationOutput(ResultData, generator, dailyGenerationValue);
            }
            #endregion

            // For gas Daily Generation Value and Daily Emissions  need to be calculated.
            foreach (var generator in generationReport.Gas)
            {
                #region Daily Generation Value
                ValueFactor = referenceData.Factors.ValueFactor.Medium; // Value factor Medium for gas                
                double dailyGenerationValue = CalculateDailyGenerationValue(generator);
                AddGeneratorToGenerationOutput(ResultData, generator, dailyGenerationValue);// Total heat generated is added. 
                #endregion

                #region Daily Emissions
                EmissionFactor = referenceData.Factors.EmissionsFactor.Medium; // Emission factor medium for gas
                double emissionRating = generator.EmissionsRating;
                AddHighestDailyEmissionToGenerationOutput(generator, emissionRating);
                #endregion

                //check for which day this daily emission is. 
                //If highest daily emission for this day is less than the daily emission
                //update the current value to daily emission for the day.
            }

            // For Coal Daily Generation Value, Daily Emissions and Actual Heat Rate  need to be calculated.
            foreach (var generator in generationReport.Coal)
            {
                #region Daily Generation Value
                ValueFactor = referenceData.Factors.ValueFactor.Medium; // Value factor Medium for Coal                
                double dailyGenerationValue = CalculateDailyGenerationValue(generator);
                AddGeneratorToGenerationOutput(ResultData, generator, dailyGenerationValue);// Total heat generated is added. 
                #endregion

                #region Daily Emissions
                EmissionFactor = referenceData.Factors.EmissionsFactor.High; // Emission factor high for Coal
                double emissionRating = generator.EmissionsRating;
                AddHighestDailyEmissionToGenerationOutput(generator, emissionRating);

                #endregion

                #region Actual Heat Rate

                //Actual Heat Rate = TotalHeatInput / ActualNetGeneration
                double actualHeatRate = generator.TotalHeatInput / generator.ActualNetGeneration;
                AddActualHeatRateToGenerationOutput(generator, actualHeatRate);
                #endregion
            }

            //Write result data to xml file
            WriteResult();
        }
        #endregion

        #region PrivateMethods

        /// <summary>
        /// Method to write the result object to xml file.
        /// </summary>
        private static void WriteResult()
        {
            string resultXml = XmlOperationsObject.ToXml(ResultData, typeof(GenerationOutput));
            string filepath = OuputFolder + "\\" + OUTPUT_FILE_NAME;
            File.WriteAllText(filepath, resultXml, Encoding.UTF8);
        }

        /// <summary>
        /// Calculate daily generation value using the formula Daily Generation Value = Energy x Price x ValueFactor
        /// </summary>
        /// <param name="generator"> The generator for which the data needs to be calculated</param>
        /// <returns>daily generation value</returns>
        private static double CalculateDailyGenerationValue(Model.Inputs.Generator generator)
        {
            double dailyGenerationValue = 0.0;
            foreach (var generation in generator.Generation)
            {
                //Daily Generation Value = Energy x Price x ValueFactor
                dailyGenerationValue += generation.Energy * generation.Price * ValueFactor;
            }

            return dailyGenerationValue;
        }

        /// <summary>
        /// Add the Generator data to output object
        /// </summary>
        /// <param name="generationOutput"> output object</param>
        /// <param name="generator">generator object</param>
        /// <param name="dailyGenerationValue"> daily generation value</param>
        private static void AddGeneratorToGenerationOutput(GenerationOutput generationOutput, Model.Inputs.Generator generator, double dailyGenerationValue)
        {
            Model.Output.Generator Generator = new Model.Output.Generator();
            Generator.Name = generator.Name;
            Generator.Total = Math.Round(dailyGenerationValue, 9);

            generationOutput.Totals.Add(Generator);
        }

        /// <summary>
        /// Add Highest Daily Emission to Output object
        /// </summary>
        /// <param name="generator">generator object</param>
        /// <param name="emissionRating"> emission rating</param>
        private static void AddHighestDailyEmissionToGenerationOutput(Model.Inputs.Generator generator, double emissionRating)
        {
            foreach (var generation in generator.Generation)
            {
                Day1 day = new Day1();
                day.Date = generation.Date;
                day.Name = generator.Name;
                // Daily Emissions = Energy x EmissionRating x EmissionFactor
                day.Emission = Math.Round(generation.Energy * emissionRating * EmissionFactor, 9);
                if (ResultData.MaxEmissionGenerators.Exists(x => x.Date == day.Date && x.Emission < day.Emission))
                {
                    ResultData.MaxEmissionGenerators.Remove(ResultData.MaxEmissionGenerators.Find(x => x.Date == day.Date && x.Emission < day.Emission));
                    ResultData.MaxEmissionGenerators.Add(day);
                }
                else if (!ResultData.MaxEmissionGenerators.Exists(x => x.Date == day.Date))
                {
                    ResultData.MaxEmissionGenerators.Add(day);
                }
            }
        }

        /// <summary>
        /// Add Actual Heat Rate to Output object
        /// </summary>
        /// <param name="generator"> generator object</param>
        /// <param name="actualHeatRate"> actual heat rate</param>
        private static void AddActualHeatRateToGenerationOutput(CoalGenerator generator, double actualHeatRate)
        {
            ActualHeatRate heatRate = new ActualHeatRate();
            heatRate.Name = generator.Name;
            heatRate.HeatRate = actualHeatRate;
            ResultData.ActualHeatRates.Add(heatRate);

        }

        #endregion
    }
}
