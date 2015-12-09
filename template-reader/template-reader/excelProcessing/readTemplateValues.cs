//using Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.model;
//using excel = Excel;

namespace template_reader.excelProcessing
{
    public class GetValuesFromReport: IQueryHelper<List<DataValue>>
    {
        public string fileName { get; set; }

        public IDisplayProgress progressDisplayHelper { get; set; }

        List<ProgramDataElements> _loadAllProgramDataElements;

        public List<DataValue> Execute()
        {
            return DoDataImport();
        }

        public List<DataValue> DoDataImport()
        {
            List<DataValue> toReturn = null;
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

        private LocationDetail GetReportLocationDetails(Workbook workbook)
        {
            var locationDetail = new LocationDetail();
            //check if not Hmiscode, month and year are specifierd. If not, we quit
            Worksheet coverWorksheet = null;
            const string coverWorksheetName = "Cover1";
            try
            {
                coverWorksheet = (Worksheet)workbook.Sheets[coverWorksheetName];
            }
            catch
            {
                ShowMissingWorksheet(coverWorksheetName);
                return null;
            }

            //we index the list of field headers and just get the i,j + 2 cell to haethe vakueas entered by  the user
            var coverRange = coverWorksheet.UsedRange;
            var rows = coverRange.Rows.Count;
            var colmns = coverRange.Columns.Count;
            var expectedCoverFields = new List<string>() {
                "Name of Health Facility",
"Province","District","Constituency","Ward",
"Date Report Compiled","Month Reported on","Year Reported on"
};
            var coverFieldsLower = (from label in expectedCoverFields
                                    select label.ToLower()).ToList();
            var fieldIndex = new Dictionary<string, int>();

            if (colmns < 2)
                throw new ArgumentOutOfRangeException("Expected more than 2 columns for the cover page");

            var healthFacilityLabel = "Name of Health Facility".ToLowerInvariant();
            var reportMonth = "Month Reported on".ToLowerInvariant();
            var reportYear = "Year Reported on".ToLowerInvariant();

            var yearfound = false;
            for (var i = 1; i <= rows; i++)
            {
                var fieldName = Convert.ToString(getCellValue(coverRange, i, 2));
                var fieldValue = Convert.ToString(getCellValue(coverRange, i, 4));

                if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldValue))
                {
                    continue;
                }

                var lowerFieldName = fieldName.ToLowerInvariant().Trim();
                if (!coverFieldsLower.Contains(lowerFieldName))
                    continue;

                var lowerFieldValue = fieldValue.ToLowerInvariant().Trim();

                if (lowerFieldName == healthFacilityLabel)
                    locationDetail.FacilityName = fieldValue.Trim();
                else if (lowerFieldName == reportMonth)
                    locationDetail.ReportMonth = fieldValue.Trim();
                else if (lowerFieldName == reportYear)
                {
                    yearfound = true;
                    int year = -1;
                    if(!int.TryParse(fieldValue.Trim(), out year))
                    {
                        ShowErrorAndAbort(fieldValue, "Year Reported on", coverWorksheetName, i, 4, true);
                        //throw new ArgumentException("Error converting value " + fieldValue + " as a number");
                    }
                    locationDetail.ReportYear = Convert.ToInt32(fieldValue.Trim());

                    //if (DateTime.TryParseExact(fieldValue, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    //{
                    //    var asDate = Convert.ToDateTime(fieldValue);
                    //    locationDetail.ReportYear = asDate.Year;
                    //}
                    //else
                    //{
                    //    ShowErrorAndAbort(fieldValue, dateReportCompiled, coverWorksheetName, i, 4, true);
                    //    return null;
                    //}
                }
            }

            if (string.IsNullOrWhiteSpace(locationDetail.FacilityName))
            {
                throw new ArgumentException("Missing entry or value for 'Name of Health Facility' in worksheet " + coverWorksheetName);
            }
            if (!yearfound)
            {
                throw new ArgumentException("Missing entry or value for 'Year Reported on' in worksheet " + coverWorksheetName);
            }

            if (string.IsNullOrWhiteSpace(locationDetail.ReportMonth))
            {
                throw new ArgumentException("Missing entry or value for 'Month Reported on' in worksheet " + coverWorksheetName);
            }
            else
            {
                var lower = locationDetail.ReportMonth.ToLowerInvariant();
                if (!monthsLongName.Select(t => t.ToLowerInvariant()).Contains(lower))
                {
                    //we check the short form
                    if (lower == "sept")
                    {
                        lower = "sep".ToLowerInvariant();
                    }
                    else if (lower.Length != 3 || !monthsShortName.Select(t => t.ToLowerInvariant()).Contains(lower))
                    {
                        throw new ArgumentException("Invalid Value '"+ locationDetail.ReportMonth + "' specified for 'Month Reported on' in worksheet " + coverWorksheetName);
                    }
                    var monthIndx = monthsShortName.FindIndex(t => t.ToLowerInvariant() == lower);
                    locationDetail.ReportMonth = monthsLongName[monthIndx];
                }
                else
                {
                    var monthIndx = monthsLongName.FindIndex(t => t.ToLowerInvariant() == lower);
                    locationDetail.ReportMonth = monthsLongName[monthIndx];
                }
            }
            return locationDetail;
        }

        static List<string> monthsLongName = new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        static List<string> monthsShortName = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public void PerformProgressStep(string message = "")
        {
            progressDisplayHelper.PerformProgressStep(message);
        }

        public void MarkStartOfMultipleSteps(int stepsToExpect)
        {
            progressDisplayHelper.MarkStartOfMultipleSteps(stepsToExpect);
        }

        public void ResetSubProgressIndicator(int stepsToExpect)
        {
            progressDisplayHelper.ResetSubProgressIndicator(stepsToExpect);
        }

        public void PerformSubProgressStep()
        {
            progressDisplayHelper.PerformSubProgressStep();
        }

        void LogCsvOutput(string text)
        {
            //File.AppendAllText("valuesRead.csv", text);
        }

        object initProgDatElmts = new object();
        private List<DataValue> ImportData(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            PerformProgressStep("Please wait, initialising");
            //we load the indicator definitions and data categories
            if (_loadAllProgramDataElements == null)
            {
                lock (initProgDatElmts)
                {
                    if (_loadAllProgramDataElements == null)
                        _loadAllProgramDataElements = new GetProgramAreaIndicators().GetAllProgramDataElements();
                }
            }

            PerformProgressStep("Please wait, Opening Excel document");

            //for all available program areas, we read the values into an array
            var workbook = excelApp.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            //we get the facility codes
            var locationDetail = GetReportLocationDetails(workbook);

            //we get the other data
            var worksheetCount = workbook.Sheets.Count;
            var worksheetNames = new Dictionary<string, string>();
            for (var indx = 1; indx <= worksheetCount; indx++)
            {
                var worksheetName = ((Worksheet)(workbook.Sheets[indx])).Name;
                worksheetNames.Add(worksheetName.Trim(), worksheetName);
            }

            var datavalues = new List<model.DataValue>();

            MarkStartOfMultipleSteps(_loadAllProgramDataElements.Count + 2);
            foreach (var dataElement in _loadAllProgramDataElements)
            {
                PerformProgressStep("Please wait, Processing worksheet: " + dataElement.ProgramArea);
                ResetSubProgressIndicator(dataElement.Indicators.Count + 1);

                var xlrange = ((Worksheet)workbook.Sheets[worksheetNames[dataElement.ProgramArea]]).UsedRange;
                //we scan the first 3 rows for the row with the age groups specified for this program area
                var rowCount = xlrange.Rows.Count;
                var colCount = xlrange.Columns.Count;

                //we have the column indexes of the first age category options, and other occurrences of the same
                var firstAgeGroupCell = GetFirstAgeGroupCell(dataElement, xlrange);

                //Now we find the row indexes of the program indicators
                //ProgramIndicator currentRowMatchingIndicator = null;
                var firstIndcatorRowIndex = -1;
                for (var rowIndex = 1; rowIndex <= rowCount; rowIndex++)
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

                //we start reading the values from cell [firstIndcatorRowIndex, firstAgeGroupCell.Colmn1]
                LogCsvOutput("Processing: " + dataElement.ProgramArea);


                var testBuilder = new StringBuilder();
                testBuilder.AppendLine();
                testBuilder.AppendLine(dataElement.ProgramArea);

                var countdown = dataElement.Indicators.Count;
                var i = firstIndcatorRowIndex;

                PerformSubProgressStep();
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
                        while (counter < dataElement.ServiceAreas.AgeDisaggregations.Count)
                        {
                            var dataValue = GetDataValue(
                                xlRange: xlrange,
                                dataElement: dataElement,
                                indicatorid: indicatorid,
                                rowId: i,
                                colmnId: j,
                                counter: counter,
                                sex: dataElement.ServiceAreas.Gender == "both" ? "Male" : "",
                                builder: testBuilder
                                );
                            if (dataValue != null)
                                datavalues.Add(dataValue);
                            j++;
                            counter++;
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
                            var dataValue = GetDataValue(
                                xlRange: xlrange,
                                dataElement: dataElement, indicatorid: indicatorid,
                                rowId: i, colmnId: j,
                                counter: counter,
                                sex: "Female",
                                builder: testBuilder
                                );
                            if (dataValue != null)
                                datavalues.Add(dataValue);
                            j++;
                            counter++;
                        }
                    }

                    PerformSubProgressStep();
                    testBuilder.AppendLine();
                    countdown--;
                    i++;
                } while (countdown > 0);

                LogCsvOutput(testBuilder.ToString());
                //Console.WriteLine("Done - "+ dataElement.ProgramArea);
            }

            PerformProgressStep("Please wait, finalizing");
            datavalues.ForEach(t =>
            {
                t.ReportYear = locationDetail.ReportYear;
                t.ReportMonth = locationDetail.ReportMonth;
                t.FacilityName = locationDetail.FacilityName;
            });
            PerformProgressStep("Please wait, Preparing to display results");

            //convert to dataset
            return datavalues;
        }

        private DataValue GetDataValue(Range xlRange, ProgramDataElements dataElement, string indicatorid, int rowId, int colmnId, int counter, string sex, StringBuilder builder=null )
        {
            var i = rowId;
            var j = colmnId;
            var value = getCellValue(xlRange, i, j);
            double asDouble;
            DataValue dataValue = null;
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

                dataValue = new DataValue()
                {
                    IndicatorValue = asDouble,
                    IndicatorId = indicatorid,
                    ProgramArea = dataElement.ProgramArea,
                    AgeGroup = dataElement.ServiceAreas.AgeDisaggregations[counter],
                    Sex = sex
                };
                if (builder != null)
                {
                    LogCsvOutput(string.Format("{0}\t", value));
                    //builder.AppendFormat("{0}\t", value);
                }
            }
            else
            {
                if (builder != null)
                {
                    LogCsvOutput(string.Format("{0}\t", "x"));
                    //builder.AppendFormat("{0}\t", "x");
                }
            }
            return dataValue;
        }

        private void ShowMissingWorksheet(string coverWorksheetName)
        {
            MessageBox.Show("Error trying to access the worksheet "+ coverWorksheetName);
        }

        static string GetColumnName(int index)
        {
            return (index > 26 ? "A" : "") + (index == 0 ? 'A' : Convert.ToChar('A' + index % 26 - 1)).ToString();
        }

        private void ShowErrorAndAbort(string value, string indicatorid, string programArea, int i, int j, bool throwException = false)
        {
            if (throwException)
            {
                throw new ArgumentException(string.Format("Could not convert value '{0}' in worksheet '{1}' and Cell ({3}{2}) as a number", value, programArea, i, GetColumnName(j)));
            };
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
    }
}
