using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DistillationColumn
{
    class SegmentData
    {
        public int key { get; set; }
        public double seg_height { get; set; }
        public double inside_dia_bottom { get; set; }
        public double inside_dia_top { get; set; }
        public double shell_thickness { get; set; }
    }
}
