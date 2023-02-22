using HelperLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using System.Net;

namespace DistillationColumn.GA_Drawing
{
    class FlangeView : CDrawingView
    {
        public double topRingThickness;
        public double bottomRingThickness;
        public double ringWidth;
        public double ringRadius;
        public double insideDistance;
        public double elevation;
        public double numberOfBolts;
        public double shellThickness;
        public double x;
        public double y;
        T3D.Point origin;
        View flangeSectionView;
        View flangeSideView;
        Globals _global;
        TeklaModelling _tModel;
        List<JToken> flangeList;
        public FlangeView(Globals global, TeklaModelling tModel, string jsonStr)
        {
            _global = global;
            _tModel = tModel;
            flangeList = new List<JToken>();
            Generate(jsonStr, _global, _tModel);
        }

        public override void Generate(string jsonStr, Globals _global, TeklaModelling _tModel)
        {
            string json = jsonStr;
            JObject jsonData = JObject.Parse(json);
            flangeList = jsonData["Flange"].ToList();
            x = (double)(jsonData["origin"]["x"]);
            y = (double)(jsonData["origin"]["y"]);
            origin = new T3D.Point(_global.Origin.X, _global.Origin.Y);
            foreach (JToken flange in flangeList)
            {
                elevation = (double)flange["elevation"] + _global.Origin.Z;
                topRingThickness = (double)flange["top_ring_thickness"];
                bottomRingThickness = (double)flange["bottom_ring_thickness"];
                ringWidth = (double)flange["ring_width"];
                insideDistance = (double)flange["inside_distance"];
                numberOfBolts = (double)flange["number_of_bolts"];
                ringRadius = _tModel.GetRadiusAtElevation(elevation - _global.Origin.Z, _global.StackSegList, true);
                int n = _tModel.GetSegmentAtElevation(elevation, _global.StackSegList);
                shellThickness = _global.StackSegList[n][2];
                //flange plan view

                T3D.Point startPoint = new T3D.Point(-((ringRadius + ringWidth) - x), elevation + topRingThickness);
                T3D.Point endPoint = new T3D.Point((ringRadius + ringWidth + x), elevation + topRingThickness);
                double depthUp = 0;
                double depthDown = (bottomRingThickness + topRingThickness);

                if (m_completeStackView != null)
                {
                    flangeSectionView = DrawingUtils.AddGASectionView(m_completeStackView, startPoint, endPoint, depthUp, depthDown);

                }


                ////flange side view
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, ringRadius - insideDistance - shellThickness, 0);
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, ringRadius + ringWidth, 0);
                flangeSideView = DrawingUtils.AddGASideView(flangeSectionView, startPoint, endPoint);
                InsertDimensions(_global, _tModel);
            }
        }
        public override void InsertAnnotations(Globals _global, TeklaModelling _tModel)
        {


        }
        public override void InsertDimensions(Globals _global, TeklaModelling _tModel)
        {
            //Radial Dimesion
            DrawingUtils.CreateRadiusDimension(flangeSectionView, ringRadius, 1, origin);
            DrawingUtils.CreateRadiusDimension(flangeSectionView, ringRadius + (ringWidth / 2), 2, origin);
            DrawingUtils.CreateRadiusDimension(flangeSectionView, ringRadius - shellThickness - insideDistance, 3, origin);
            // end =====================

            //top Ring thickness
            T3D.Point Point1 = new T3D.Point(x + ringRadius - insideDistance - shellThickness - 10, 0);
            T3D.Point Point2 = new T3D.Point(x + ringRadius - insideDistance - shellThickness - 10, -topRingThickness);
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            //bottom Ring thickness
            Point1 = new T3D.Point(x + ringRadius - insideDistance - shellThickness - 10, -topRingThickness);
            Point2 = new T3D.Point(x + ringRadius - insideDistance - shellThickness - 10, -(topRingThickness + bottomRingThickness));
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            //ring width
            Point1 = new T3D.Point(x + ringRadius, -(topRingThickness + bottomRingThickness));
            Point2 = new T3D.Point(x + ringRadius + ringWidth, -(topRingThickness + bottomRingThickness));
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  ===============

            //inside distance
            Point1 = new T3D.Point(x + (ringRadius - shellThickness - insideDistance), -(topRingThickness + bottomRingThickness));
            Point2 = new T3D.Point(x + ringRadius - shellThickness, -(topRingThickness + bottomRingThickness));
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            //shell thickness
            Point1 = new T3D.Point(x + (ringRadius - shellThickness), -(topRingThickness + bottomRingThickness));
            Point2 = new T3D.Point(x + ringRadius, -(topRingThickness + bottomRingThickness));
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            //shell thickness
            Point1 = new T3D.Point(x + (ringRadius - shellThickness), -(topRingThickness + bottomRingThickness));
            Point2 = new T3D.Point(x + ringRadius, -(topRingThickness + bottomRingThickness));
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            //bolt distance
            Point1 = new T3D.Point(x + ringRadius, 0);
            Point2 = new T3D.Point(x + (ringRadius + ringWidth / 2), 0);
            DrawingUtils.CreateDimension(flangeSideView, Point1, Point2);
            //End  =====================

            // Horizontal dimension at diameter in Plan view ===========
            Point1 = new T3D.Point(-(ringRadius + ringWidth - x), y);
            Point2 = new T3D.Point(ringRadius + ringWidth + x, y);
            DrawingUtils.CreateDimension(flangeSectionView, Point1, Point2);
            // End ============

            // Horizontal dimension at stack front view ========

            Point1 = new T3D.Point(-(ringRadius - x), elevation - 50);
            Point2 = new T3D.Point((ringRadius + x), elevation - 50);
            DrawingUtils.CreateDimension(m_completeStackView, Point1, Point2);

            // end ============================
        }
    }
}
