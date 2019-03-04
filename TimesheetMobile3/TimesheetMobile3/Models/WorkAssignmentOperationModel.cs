using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace TimesheetMobile3.Models
{
    public class WorkAssignmentOperationModel
    {
        public string Operation { get; set; }
        public string AssignmentTitle { get; set; }
        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
