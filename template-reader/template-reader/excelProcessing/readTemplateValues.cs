//using Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.model;
//using excel = Excel;

namespace template_reader
{
    public class ReadTemplateValues
    {
        public string fileName { get; set; }
        List<ProgramDataElements> _loadAllProgramDataElements;

        public void GetValuesReloaded()
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application() { Visible = false };
                UpdateIndicatorDefinitionsByProgramArea(excelApp);
            }
            catch(Exception ex)
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

        public List<ProgramDataElements> LoadAllProgramDataElements()
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

        private DataSet ImportData(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            //we get the facility codes
            //check if not Hmiscode, month and year are specifierd. If not, we quit

            //we load the indicator definitions and data categories
            if (_loadAllProgramDataElements == null)
                _loadAllProgramDataElements = LoadAllProgramDataElements();

            //for all available program areas, we read the values into an array
            var workbook = excelApp.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            foreach (var dataElement in _loadAllProgramDataElements)
            {
                var xlrange = ((Worksheet)workbook.Sheets[dataElement.ProgramArea]).UsedRange;
                //we scan the first 3 rows for the row with the age groups specified for this program area
                int rowCount = xlrange.Rows.Count;
                int colCount = xlrange.Columns.Count;

                //we have the column indexes of the first age category options, and other occurrences of the same
                var firstAgeGroupCell = GetFirstAgeGroupCell(dataElement, xlrange);

                //var rowIndex = firstAgeGroupCell.RowId;
                //var colmnIndex = firstAgeGroupCell.ColumnId1;

                //Now we find the row indexes of the program indicators
                var firstIndcatorRowIndex = -1;
                for(var rowIndex = 1; rowIndex <= rowCount; rowIndex++)
                {
                    var value = Convert.ToString(xlrange[rowIndex, 1].Value);                    
                    if (string.IsNullOrWhiteSpace(value)) continue;

                    if (dataElement.Indicators.Exists(t => t.IndicatorId == value))
                    {
                        firstIndcatorRowIndex = rowIndex;
                        break;
                    }
                }

                //now we know that AgeGroups start from e.g. colmn 7 of row 4
                //we also know that indicators start from Row X of column 1
                //we can now start reading these values into an array for each indicator vs age group
                var tr = 90;
                //we start reading the values from cell [firstIndcatorRowIndex, firstAgeGroupCell.Colmn1]
                var testBuilder = new StringBuilder();
                for (var i = firstIndcatorRowIndex; i <= dataElement.Indicators.Count; i++)
                {
                    for (var j = firstAgeGroupCell.ColumnId1; j < dataElement.ServiceAreas.AgeDisaggregations.Count; j++)
                    {
                        var value = Convert.ToString(xlrange[i, j].Value);
                        testBuilder.AppendFormat("{0}\t", value);
                    }
                    testBuilder.AppendLine();
                }

                File.AppendAllText("valuesRead.csv", testBuilder.ToString());


                MessageBox.Show("Done");


                //xlRange = worksheet.UsedRange;

                //var cols = xlRange.Columns.Count;
                //var rows = xlRange.Rows.Count;

                //Object arr = xlRange.Value;
                //var i = 0;
                //foreach (var s in (Array)arr)
                //{
                //    //we show the objects
                //    var asString = Convert.ToString(s);
                //    if (string.IsNullOrWhiteSpace(asString))
                //        continue;
                //}
            }
            //convert to dataset

            //and return the value
            return new DataSet();
        }

        private static FirstAgeGroupOccurence GetFirstAgeGroupCell(ProgramDataElements dataElement, Range xlrange)
        {
            int colCount = xlrange.Columns.Count;
            int row = -1, colmn = -1, colmn2 = -1;

            var matchfound = false;
            for (var rowId = 1; rowId <= 3; rowId++)
            {
                for (var colmnId = 1; colmnId <= colCount; colmnId++)
                {
                    var value = Convert.ToString(xlrange[rowId, colmnId].Value);
                    if (string.IsNullOrWhiteSpace(value) || value.Length > 7) continue;

                    if (dataElement.ServiceAreas.AgeDisaggregations.Contains(value))
                    {
                        //we've found our row
                        row = rowId;
                        colmn = colmnId;
                        matchfound = true;

                        if (dataElement.ServiceAreas.Gender == "both")
                        {
                            //we continue and find the next occurrence of this value
                            colmn2 = findNextOccurence(dataElement, xlrange, colCount, rowId, colmnId + 1, value);
                        }

                        break;
                    }
                }
                if (matchfound) break;
            }

            return new FirstAgeGroupOccurence(row, colmn, colmn2);
        }

        static int findNextOccurence(ProgramDataElements dataElement, Range xlrange, int colCount, int rowId, int startColmnIndex, string valueToFind)
        {
            int colmnIndex = -1;
            for (var colmnId = startColmnIndex; colmnId <= colCount; colmnId++)
            {
                var value = Convert.ToString(xlrange[rowId, colmnId].Value);
                if (value != valueToFind)
                    continue;
                colmnIndex = colmnId;
                break;
            }
            return colmnIndex;
        }

        public DataSet DoDataImport()
        {
            DataSet toReturn = null;
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application() { Visible = false };
                toReturn = ImportData(excelApp);
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
            return toReturn;
        }

        public void UpdateIndicatorDefinitionsByProgramArea(Microsoft.Office.Interop.Excel.Application excelApp)
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
                throw new FileNotFoundException("Couldn't find the file "+ relativePath);
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
                var matchingWorksheets = GetAvailableWorksheetByName(svcs, sht.Name);
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

                var programIndicators = new ProgramAreaIndicators() {ProgramArea = sht.Name };
                allProgramAreaIndicators.Add(programIndicators);

                var indicatorsByProgramArea = new List<string>();                
                for (var i = 1; i <= ubound; i++)
                {
                    if (i == 1) continue;

                    var indicatorCode = string.Empty;
                    var indicatorName = string.Empty;
                    indicatorCode = Convert.ToString(arr.GetValue(i, 1));
                    indicatorName = Convert.ToString(arr.GetValue(i, 2));
                    if (string.IsNullOrWhiteSpace(indicatorCode)|| string.IsNullOrWhiteSpace(indicatorName))
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

        private static List<ServiceAreaDataset> GetAvailableWorksheetByName(List<ServiceAreaDataset> svcs, string worksheetName)
        {
            return svcs.FindAll(t => t.ProgramArea == worksheetName);
        }

        //public DataSet getValues()
        //{
        //    //https://exceldatareader.codeplex.com/
        //    var stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
        //    excel.IExcelDataReader excelReader = null;

        //    if (fileName.EndsWith("xls"))
        //    {
        //        //1. Reading from a binary Excel file ('97-2003 format; *.xls)
        //        excelReader = excel.ExcelReaderFactory.CreateBinaryReader(stream);
        //    }
        //    else if (fileName.EndsWith("xlsx"))
        //    {
        //        //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
        //        excelReader = excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
        //    }
            
        //    //3. DataSet - The result of each spreadsheet will be created in the result.Tables
        //    var result = excelReader.AsDataSet();

        //    //4. DataSet - Create column names from first row
        //    excelReader.IsFirstRowAsColumnNames = true;

        //    //5. Data Reader methods
        //    //while (excelReader.Read())
        //    //{
        //    //    excelReader.GetInt32(0);
        //    //}

        //    //6. Free resources (IExcelDataReader is IDisposable)
        //    excelReader.Close();
        //    return result;
        //}
    }
}
