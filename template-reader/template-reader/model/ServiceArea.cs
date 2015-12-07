using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template_reader.model
{
    public class DataValue
    {
        public string ProgramArea { get; set; }
        public string IndicatorId { get; set; }
        public double IndicatorValue { get; set; }
        public string AgeGroup { get; internal set; }
        public string Sex { get; internal set; }
    }

    public class ProgramDataElements
    {
        public ProgramDataElements()
        {
            ProgramArea = string.Empty;
            ServiceAreas = new ServiceAreaDataset();
            Indicators = new List<ProgramIndicator>();
        }

        public ServiceAreaDataset ServiceAreas { get; set; }
        public string ProgramArea { get; set; }
        public List<ProgramIndicator> Indicators { get; set; }
    }

    public class ServiceAreaDataset
    {
        public ServiceAreaDataset()
        {
            ProgramArea = string.Empty;
            AgeDisaggregations = new List<string>();
            DefaultHandler = string.Empty;
            Gender = string.Empty;
        }

        public string ProgramArea { get; set; }
        public List<string> AgeDisaggregations { get; set; }
        public string DefaultHandler { get; set; }
        public string Gender { get; set; }
    }

    public class ProgramIndicator
    {
        public ProgramIndicator()
        {
            IndicatorId = string.Empty;
            Indicator = string.Empty;
        }

        public string IndicatorId { get; set; }
        public string Indicator { get; set; }
    }

    public class ProgramAreaIndicators
    {
        public ProgramAreaIndicators()
        {
            ProgramArea = string.Empty;
            Indicators = new List<ProgramIndicator>();
        }

        public string ProgramArea { get; set; }
        public List<ProgramIndicator> Indicators { get; set; }
    }

    public struct FirstAgeGroupOccurence
    {
        public FirstAgeGroupOccurence(int rowId, int colmn1, int colmn2)
        {
            RowId = rowId;
            ColumnId1 = colmn1;
            ColumnId2 = colmn2;
        }
        public int RowId;
        public int ColumnId1;
        public int ColumnId2;
    }
}
