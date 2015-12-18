﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using template_reader.model;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace template_reader.excelProcessing
{
    public class GetProgramAreaIndicators
    {
        public string UpdateProgramAreaIndicators()
        {
            var res = string.Empty;
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application() { Visible = false };
                res = UpdateIndicatorDefinitionsByProgramArea(excelApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (excelApp != null)
                {
                    excelApp.Quit();
                }
            }
            return res;
        }

        private static List<ServiceAreaDataset> GetAvailableWorksheetByName(List<ServiceAreaDataset> svcs, string worksheetName)
        {
            return svcs.FindAll(t => t.ProgramArea == worksheetName);
        }

        List<string> maleFemaleIndicators = new List<string>() { "STI", "TB", "ART", "Family Planning", "Prevention - PWP", "Clinical Care" };

        List<string> singleGenderIndicators = new List<string>() { "PMTCT", "Prevention-MC" };

        private string UpdateIndicatorDefinitionsByProgramArea(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            var res = string.Empty;
            //http://stackoverflow.com/questions/16213255/how-to-read-cell-values-from-existing-excel-file
            var relativePath = "staticdata//sampleTemplate.xlsx";
            var filePath = string.Empty;

            if (File.Exists(relativePath))
            {
                filePath = Path.GetFullPath(relativePath);
            }
            else
            {
                throw new FileNotFoundException("Couldn't find the file " + relativePath);
            }

            var wb = excelApp.Workbooks.Open(filePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            var shts = wb.Sheets;
            var sheetsToSkip = new[] { "Narrative", "Appendix 1", "Cover1" };

            var programAreaDefinitions = new List<ProgramAreaDefinition>();
            foreach (Worksheet sht in shts)
            {
                var usedRange = sht.UsedRange;
                var rows = usedRange.Rows.Count;
                var colmns = usedRange.Columns.Count;

                var programAreaName = sht.Name.Trim();

                if (sheetsToSkip.Contains(programAreaName))
                    continue;

                var programAreaDefinition = new ProgramAreaDefinition() { ProgramArea = programAreaName };
                programAreaDefinitions.Add(programAreaDefinition);

                if(maleFemaleIndicators.Contains(programAreaName))
                    programAreaDefinition.Gender = "both";
                else if (singleGenderIndicators.Contains(programAreaName))
                {
                    programAreaDefinition.Gender = "none";
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("Error. Gender categorization of {0} not defined", programAreaName));
                }

                if (programAreaName == "Family Planning")
                {
                    programAreaDefinition.DefaultHandler = "Custom";
                }

                //we get the indicator codes and names. These are in the first and second columns starting from the second row
                for (var i = 2; i < rows; i++)
                {
                    var indicatorCode = GetValuesFromReport.getCellValue(usedRange, i, 1);
                    var indicatorName = GetValuesFromReport.getCellValue(usedRange, i, 2);
                    programAreaDefinition.Indicators.Add(new ProgramIndicator()
                    {
                        IndicatorId = indicatorCode,
                        Indicator = indicatorName
                    });
                }

                //we get the agegroup categories
                for (var j = 3; j < colmns; j++)
                {
                    var ageGroupLabel = GetValuesFromReport.getCellValue(usedRange, 2, j);
                    if (string.IsNullOrWhiteSpace(ageGroupLabel))
                        continue;

                    programAreaDefinition.AgeDisaggregations.Add(ageGroupLabel);
                }
            }

            //we save the data
            if (File.Exists("ProgramAreaDefinitions.json"))
                File.Delete("ProgramAreaDefinitions.json");

            var builder = new StringBuilder();
            programAreaDefinitions.ForEach(t => builder.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(t)));
            res = builder.ToString();

            File.AppendAllText("ProgramAreaDefinitions.json", res);

            MessageBox.Show("Done");
            return res;
        }

        public List<ProgramAreaDefinition> GetAllProgramDataElements()
        {
            //we get the avilable indicators by program area
            var progAreas = File.ReadAllLines("staticdata//ProgramAreaDefinitions.json");
            return (from progArea in progAreas
                                         select Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramAreaDefinition>(progArea)).ToList();
        }
    }
}
