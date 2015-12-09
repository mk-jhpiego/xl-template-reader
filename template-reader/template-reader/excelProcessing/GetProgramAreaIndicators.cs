using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.model;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace template_reader.excelProcessing
{
    public class GetProgramAreaIndicators
    {
        public void UpdateProgramAreaIndicators()
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application() { Visible = false };
                UpdateIndicatorDefinitionsByProgramArea(excelApp);
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
        }

        private static List<ServiceAreaDataset> GetAvailableWorksheetByName(List<ServiceAreaDataset> svcs, string worksheetName)
        {
            return svcs.FindAll(t => t.ProgramArea == worksheetName);
        }

        private void UpdateIndicatorDefinitionsByProgramArea(Microsoft.Office.Interop.Excel.Application excelApp)
        {
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

            //we get the list of available modules
            var svcAreas = File.ReadAllLines("staticdata//requiredTemplateHeaders.json");
            var svcs = (from svc in svcAreas
                        select Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceAreaDataset>(svc)).ToList();

            var allProgramAreaIndicators = new List<ProgramAreaIndicators>();
            var builder = new StringBuilder();
            foreach (Worksheet sht in shts)
            {
                var matchingWorksheets = GetAvailableWorksheetByName(svcs, sht.Name.Trim());
                if (matchingWorksheets.Count == 0)
                    continue;

                var first = matchingWorksheets.First();
                if (first.DefaultHandler == "custom")
                {
                    //for now, we skip PMTCT and Family Planning
                    continue;
                }

                var xlRange = sht.UsedRange;
                var arrayObj = xlRange.Value;

                var arr = (Array)xlRange.Value;

                var lbound = arr.GetLowerBound(0);
                var ubound = arr.GetUpperBound(0);
                var arrLength = arr.Length;
                var colCount = arrLength / ubound;

                var programIndicators = new ProgramAreaIndicators() { ProgramArea = sht.Name.Trim() };
                allProgramAreaIndicators.Add(programIndicators);

                var indicatorsByProgramArea = new List<string>();
                for (var i = 1; i <= ubound; i++)
                {
                    if (i == 1) continue;

                    var indicatorCode = string.Empty;
                    var indicatorName = string.Empty;
                    indicatorCode = Convert.ToString(arr.GetValue(i, 1));
                    indicatorName = Convert.ToString(arr.GetValue(i, 2));
                    if (string.IsNullOrWhiteSpace(indicatorCode) || string.IsNullOrWhiteSpace(indicatorName))
                        continue;

                    programIndicators.Indicators.Add(new ProgramIndicator() { Indicator = indicatorName, IndicatorId = indicatorCode });
                }
                builder.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(programIndicators));
            }

            if (File.Exists("ProgramAreaIndicators.txt"))
                File.Delete("ProgramAreaIndicators.txt");
            File.AppendAllText("ProgramAreaIndicators.txt", builder.ToString());

            MessageBox.Show("Done");
        }

        public List<ProgramDataElements> GetAllProgramDataElements()
        {
            //we get the available program areas
            var svcAreas = File.ReadAllLines("staticdata//requiredTemplateHeaders.json");
            var allServiceAreas = (from svc in svcAreas
                                   select Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceAreaDataset>(svc)).ToList();

            //we get the avilable indicators by program area
            var progAreas = File.ReadAllLines("staticdata//programAreaIndicators.json");
            var programAreaInidcators = (from progArea in progAreas
                                         select Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramAreaIndicators>(progArea)).ToList();

            return (from pi in programAreaInidcators
                    let sarea = allServiceAreas.Where(t => t.ProgramArea == pi.ProgramArea).FirstOrDefault()
                    where sarea != null
                    select new ProgramDataElements()
                    {
                        ProgramArea = pi.ProgramArea,
                        Indicators = pi.Indicators,
                        ServiceAreas = sarea
                    }).ToList();
        }

    }
}
