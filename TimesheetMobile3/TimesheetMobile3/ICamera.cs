using System;
using System.Collections.Generic;
using System.Text;

namespace TimesheetMobile3
{
    public interface ICamera
    {
        void TakePicture(string employeeName);

        Action PictureTaken { get; set; }
    }
}
