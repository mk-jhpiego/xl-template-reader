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
            //var worksheets = (Worksheets)workbook.Sheets;
            var worksheetCount = workbook.Sheets.Count;
            var worksheetNames = new Dictionary<string, string>();
            for(var indx = 1; indx<= worksheetCount; indx++)
            {
                var worksheetName = ((Worksheet)(workbook.Sheets[indx])).Name;
                worksheetNames.Add(worksheetName.Trim(), worksheetName);
            }

            var datavalues = new List<model.DataValue>();

            foreach (var dataElement in _loadAllProgramDataElements)
            {
                var xlrange = ((Worksheet)workbook.Sheets[worksheetNames[dataElement.ProgramArea]]).UsedRange;
                //we scan the first 3 rows for the row with the age groups specified for this program area
                var rowCount = xlrange.Rows.Count;
                var colCount = xlrange.Columns.Count;

                //we have the column indexes of the first age category options, and other occurrences of the same
                var firstAgeGroupCell = GetFirstAgeGroupCell(dataElement, xlrange);

                //Now we find the row indexes of the program indicators
                ProgramIndicator currentRowMatchingIndicator = null;
                var firstIndcatorRowIndex = -1;
                for(var rowIndex = 1; rowIndex <= rowCount; rowIndex++)
                {
                    var value = getCellValue(xlrange, rowIndex, 1);                   
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    var matchingDataElementByIndicatorId = dataElement.Indicators.FirstOrDefault(t => t.IndicatorId == value);
                    if (matchingDataElementByIndicatorId != null)
                    {
                        firstIndcatorRowIndex = rowIndex;
                        //currentRowMatchingIndicator = matchingDataElementByIndicatorId;
                        break;
                    }
                }

                //now we know that AgeGroups start from e.g. colmn 7 of row 4
                //we also know that indicators start from Row X of column 1
                //we can now start reading these values into an array for each indicator vs age group
                var tr = 90;
                //we start reading the values from cell [firstIndcatorRowIndex, firstAgeGroupCell.Colmn1]
                File.AppendAllText("valuesRead.csv", "Processing: " + dataElement.ProgramArea);

                var testBuilder = new StringBuilder();
                testBuilder.AppendLine();
                testBuilder.AppendLine(dataElement.ProgramArea);

                var countdown = dataElement.Indicators.Count;
                var i = firstIndcatorRowIndex;

                do
                {
                    var indicatorid = getCellValue(xlrange, i, 1);
                    if (string.IsNullOrWhiteSpace(indicatorid))
                    {
                        throw new ArgumentNullException(string.Format("Expected a value in Cell ( A{0}) for sheet {1}", i, dataElement.ProgramArea));
                    }

                    var j = firstAgeGroupCell.ColumnId1;
                    var counter = 0;

                    //or we can get the corresponding data element and see what indicators it reports under
                    while (counter < dataElement.ServiceAreas.AgeDisaggregations.Count)
                    {
                        var value = getCellValue(xlrange, i, j);
                        double asDouble;
                        try
                        {
                            asDouble = value.ToDouble();
                            if (asDouble == -2146826273 || asDouble == -2146826281)
                            {
                                ShowErrorAndAbort(value, indicatorid, dataElement.ProgramArea, i, j);
                                return null;
                            }
                        }
                        catch
                        {
                            ShowErrorAndAbort(value, indicatorid, dataElement.ProgramArea, i, j);
                            return null;
                        }

                        if (asDouble != model.Constants.NOVALUE)
                        {
                            if (value == null)
                            {
                                ShowValueNullErrorAndAbort(indicatorid, dataElement.ProgramArea, i, j);
                            }

                            datavalues.Add(new DataValue()
                            {
                                IndicatorValue = asDouble,
                                IndicatorId = indicatorid,
                                ProgramArea = dataElement.ProgramArea,
                                AgeGroup = dataElement.ServiceAreas.AgeDisaggregations[counter],
                                Sex = dataElement.ServiceAreas.Gender == "both" ? "Male" : ""
                            });
                            testBuilder.AppendFormat("{0}\t", value);
                        }
                        else
                        {
                            testBuilder.AppendFormat("{0}\t", "x");
                        }

                        j++;

                        counter++;
                    }

                    if (dataElement.ServiceAreas.Gender == "both")
                    {
                        j = firstAgeGroupCell.ColumnId2;

                        counter = 0;
                        while (counter < dataElement.ServiceAreas.AgeDisaggregations.Count)
                        {
                            var value = getCellValue(xlrange, i, j);
                            double asDouble;
                            try
                            {
                                asDouble = value.ToDouble();
                                if (asDouble == -2146826273 || asDouble == -2146826281)
                                {
                                    ShowErrorAndAbort(value, indicatorid, dataElement.ProgramArea, i, j);
                                    return null;
                                }
                            }
                            catch
                            {
                                ShowErrorAndAbort(value, indicatorid, dataElement.ProgramArea, i, j);
                                return null;
                            }

                            if (asDouble != model.Constants.NOVALUE)
                            {
                                if (value == null)
                                {
                                    ShowValueNullErrorAndAbort(indicatorid, dataElement.ProgramArea, i, j);
                                }

                                datavalues.Add(new DataValue()
                                {
                                    IndicatorValue = asDouble,
                                    IndicatorId = indicatorid,
                                    ProgramArea = dataElement.ProgramArea,
                                    AgeGroup = dataElement.ServiceAreas.AgeDisaggregations[counter],
                                    Sex = "Female"
                                });

                                testBuilder.AppendFormat("{0}\t", value);
                            }
                            else
                            {
                                testBuilder.AppendFormat("{0}\t", "x");
                            }
                            j++;
                            counter++;
                        }
                    }

                    testBuilder.AppendLine();
                    countdown--;
                    i++;
                } while (countdown > 0);

                File.AppendAllText("valuesRead.csv", testBuilder.ToString());

                Console.WriteLine("Done - "+ dataElement.ProgramArea);
            }
            //convert to dataset
            var ds = new DataSet();
            var table = new System.Data.DataTable() { TableName = "DataValue" };
            table.Columns.Add("ProgramArea", typeof(string));
            table.Columns.Add("IndicatorId", typeof(string));
            table.Columns.Add("AgeGroup", typeof(string));
            table.Columns.Add("Sex", typeof(string));
            table.Columns.Add("IndicatorValue", typeof(double));

            ds.Tables.Add(table);

            foreach(var datavalue in datavalues)
            {
                table.Rows.Add(
                    datavalue.ProgramArea,
                    datavalue.IndicatorId,
                    datavalue.AgeGroup,
                    datavalue.Sex,
                    datavalue.IndicatorValue
                    );
            }
            table.AcceptChanges();

            //and return the value
            MessageBox.Show("Done");
            return ds;
        }

        static string GetColumnName(int index)
        {
            return (index > 26 ? "A" : "") + (index == 0 ? 'A' : Convert.ToChar('A' + index % 26 - 1)).ToString();
        }

        private void ShowErrorAndAbort(string value, string indicatorid, string programArea, int i, int j)
        {
            MessageBox.Show(string.Format("Could not convert value '{0}' in worksheet '{1}' and Cell ({3}{2}) as a number", value, programArea, i, GetColumnName(j)));
        }

        private void ShowValueNullErrorAndAbort(string indicatorid, string programArea, int i, int j)
        {
            MessageBox.Show(string.Format("Could not determine the value in worksheet '{0}' and Cell ({2}{1}). Check that the cells are not merged", programArea, i, GetColumnName(j)));
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
                    var value = getCellValue(xlrange, rowId, colmnId);
                    if (string.IsNullOrWhiteSpace(value) || value.Length > 20) continue;

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

        static string getCellValue(Range xlrange, int rowId, int colmnId)
        {
            var cellvalue = Convert.ToString(xlrange[rowId, colmnId].Value);
            return cellvalue == null ? string.Empty : cellvalue.ToString().Trim();
        }

        static int findNextOccurence(ProgramDataElements dataElement, Range xlrange, int colCount, int rowId, int startColmnIndex, string valueToFind)
        {
            //if(dataElement.ProgramArea =="PEP")
            int colmnIndex = -1;
            for (var colmnId = startColmnIndex; colmnId <= colCount; colmnId++)
            {
                var value = getCellValue(xlrange, rowId, colmnId);
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

                var programIndicators = new ProgramAreaIndicators() {ProgramArea = sht.Name.Trim() };
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
    }
}
