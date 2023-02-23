using HelperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using Newtonsoft.Json.Linq;
using Render;

namespace DistillationColumn
{
    internal class Flange
    {
        public double topRingThickness ;
        public double bottomRingThickness;
        public double ringWidth ;
        public double ringRadius;    
        public double insideDistance ;
        public double elevation;
        public double numberOfBolts;
        public double shellThickness;
        List<Part> _ringList;
        public Globals _global;
        public TeklaModelling _tModel;
        List<List<double>> _flangelist;

        public Flange(Globals global, TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;
            _ringList = new List<Part>();
            _flangelist = new List<List<double>>();
            SetFlangeData();
            BuildFlange();
            
        }

        void SetFlangeData()
        {
            List<JToken> flangelist = _global.JData["Flange"].ToList();
            foreach (JToken flange in flangelist)
            {
                elevation = (double)flange["elevation"];
                topRingThickness= (double)flange["top_ring_thickness"];
                 bottomRingThickness = (double)flange["bottom_ring_thickness"];
                 ringWidth=(double)flange["ring_width"]; 
                 insideDistance = (double)flange["inside_distance"] ;
                 numberOfBolts = (double)flange["number_of_bolts"];


               _flangelist.Add(new List<double> { elevation,topRingThickness,bottomRingThickness,ringWidth,insideDistance,numberOfBolts });
            }
        }

        void BuildFlange()
        {
            foreach (List<double> flange in _flangelist)
            {
                elevation = flange[0];
                topRingThickness = flange[1];
                bottomRingThickness = flange[2];
                ringWidth = flange[3];
                insideDistance = flange[4];
                numberOfBolts = flange[5];

                CreateFlange();
            }
        }

     

        public void CreateFlange()
        {

            int n = _tModel.GetSegmentAtElevation(elevation, _global.StackSegList);
            shellThickness = _global.StackSegList[n][2];
            ContourPoint sPoint = new ContourPoint(_tModel.ShiftVertically(_global.Origin, elevation), null);
            ringRadius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList);
            ringRadius += shellThickness;

            for (int i = 0; i < 4; i++)
            {

                ContourPoint ePoint = new ContourPoint(_tModel.ShiftHorizontallyRad(sPoint, ringRadius, i + 1), null);
                CustomPart CPart = new CustomPart();
                CPart.Name = "custom_flange";
                CPart.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;
                CPart.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.MIDDLE;
                CPart.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.MIDDLE;
                CPart.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.TOP;



                CPart.SetInputPositions(sPoint, ePoint);


                CPart.SetAttribute("ringWidth", ringWidth);
                CPart.SetAttribute("Radius", ringRadius);
                CPart.SetAttribute("topRingThickness", topRingThickness);
                CPart.SetAttribute("bottomRingThickness", bottomRingThickness);
                CPart.SetAttribute("insideDistance", insideDistance);
                CPart.SetAttribute("shellThickness", shellThickness);
                CPart.SetAttribute("numberOfBolts", numberOfBolts);
                CPart.SetAttribute("bolt_standard_screwdin", "UNDEINED_BOLT");
                CPart.SetAttribute("Cut_Length", -420);
                CPart.SetAttribute("bolt_diameter", 20);



                CPart.Insert();
               // _tModel.Model.CommitChanges();

            }
        }
      
    }
}
