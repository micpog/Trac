using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.Models.Section
{
    public class Section
    {
        public string Header { get; set; }
        public IList<ISectionRow> SectionRows { get; set; } = new List<ISectionRow>();
    }
}
