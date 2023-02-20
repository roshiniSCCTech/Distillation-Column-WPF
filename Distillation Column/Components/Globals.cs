using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using Newtonsoft.Json.Linq;
using Tekla.Structures.Model;
using Newtonsoft.Json;

namespace DistillationColumn
{
    public class Globals
    {
        public string ProfileStr;
        public const string MaterialStr = "IS2062";
        public string ClassStr;
        public string NameStr;
        public TSM.Position Position;

        public readonly TSM.ContourPoint Origin;

        // 0 - bottom inner diameter, 1 - top inner diameter, 2 - thickness, 3 - height, 4 - height from base of stack to bottom of segment
        public readonly List<List<double>> StackSegList;
        public JObject JData;
        public List<Part> platformParts = new List<Part>();

        // list of stack segment parts
        public readonly List<TSM.Beam> SegmentPartList;


        public Globals(JObject jData)
        {
            Origin = new TSM.ContourPoint(new T3D.Point(Convert.ToDouble(jData["origin"]["x"].ToString()), Convert.ToDouble(jData["origin"]["y"].ToString()), Convert.ToDouble(jData["origin"]["z"].ToString())), null);
            ProfileStr = "";
            ClassStr = "";
            NameStr = "";
            Position = new TSM.Position();
            StackSegList = new List<List<double>>();
            SegmentPartList = new List<TSM.Beam>();

            JData = jData;

            SetStackData();
            CalculateElevation();
        }

        public void SetStackData()
        {
            List<JToken> stackList = JData["stack"].ToList();
            foreach (JToken stackSeg in stackList)
            {
                double bottomDiameter = (float)stackSeg["inside_dia_bottom"] ; // inside bottom diamter
                double topDiameter = (float)stackSeg["inside_dia_top"] ; // inside top diameter
                double thickness = (float)stackSeg["shell_thickness"];
                double height = (float)stackSeg["seg_height"];

                StackSegList.Add(new List<double> { bottomDiameter, topDiameter, thickness, height });
            }
            StackSegList.Reverse();
        }

        void CalculateElevation()
        {
            double elevation = 0;

            foreach (List<double> segment in StackSegList)
            {
                segment.Add(elevation);
                elevation += segment[3];
            }
        }
    }
}
