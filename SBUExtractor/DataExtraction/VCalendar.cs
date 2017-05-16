using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class VCalendar:ExtractByBorders
    {
        // used for extracting Calendar and tasks data
        public VCalendar()
        {
            StartKey = "BEGIN:VCALENDAR";
            EndKey = "END:VCALENDAR";
            Label = "CalendarAndToDo";
            Suffix = ".vcs";
        }
    }
}
