using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using DistillationColumn;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using HelperLibrary;

namespace DistillationColumn
{
    class ChairView : CDrawingView
    {
        #region
        double stack_radius = 0.0;
        double bottom_radius = 0.0;
        double elevation_chair = 0.0;
        double baseRingThickness = 0.0; // Do not delete this variable;
        double upperRingThickness = 0.0;
        double chair_plate_thinkness = 0.0; //  Do not delete this variable;
        double BaseRingWidth = 0.0;
        double BaseRingWidthFront = 0.0;
        double chair_plateLength = 0.0;
        //double rad = 0.0;
        double stack_thinkness = 0.0;
        View chairSectionView;
        View chairSideView;
        View tailingLugSideView;
        T3D.Point startPoint;
        T3D.Point endPoint;
        T3D.Point origin = new T3D.Point(0, 0);
        List<double> elevation = new List<double>();
        double tailingLugAngle = 45;
        double numOfStiffnerPlates = 0.0;
        double distUpBolt = 0.0;
        double insideStackDistance = 0.0;
        //double seg_Height = 0.0;
        //double stackTotalHeight = 0.0;
        double tailingLugFrontWidth = 0.0;
        double x = 0.0;
        double y = 0.0;
        double tailingLugInsideCircleDia = 0.0;
        #endregion

        Globals _global;
        TeklaModelling _tModel;
        //public ChairView(GADrawing drawingInst)
        //{
        //    m_drawingInst = drawingInst;
        //}

        public ChairView(Globals global, TeklaModelling tModel, string jsonStr)
        {
            _global = global;
            _tModel = tModel;
            Generate(jsonStr,_global,_tModel);
        }
        public override void Generate(string jsonStr, Globals _global, TeklaModelling _tModel)
        {

            // Chair json 
            JObject jsonData = JObject.Parse(jsonStr);
            baseRingThickness = (double)(jsonData["chair"]["bottom_ring_thickness"]);
            upperRingThickness = (double)(jsonData["chair"]["top_ring_thickness"]);   //25;//Upper ring width
            chair_plate_thinkness = (double)(jsonData["chair"]["stiffner_plate_thickness"]) ;  //20;
            elevation_chair = _global.Origin.Z;//345;        //elevation of base chair
            BaseRingWidth = (double)(jsonData["chair"]["width"]);
            BaseRingWidthFront = (double)(jsonData["chair"]["width"]);
            chair_plateLength = (double)(jsonData["chair"]["height"])+baseRingThickness+upperRingThickness;
            //tailingLugAngle = (double)(jsonData["chairInput"]["tailing_lug_orientation_angle"]["value"]);
            numOfStiffnerPlates = 4*((double)(jsonData["chair"]["number_of_plates"]));
            
            insideStackDistance = (double)(jsonData["chair"]["inside_distance"]);
            x = (double)(jsonData["origin"]["x"]) ;
            y = (double)(jsonData["origin"]["y"]);
           
            origin = new T3D.Point(_global.Origin.X,_global.Origin.Y );
           
            bottom_radius = _tModel.GetRadiusAtElevation(baseRingThickness,_global.StackSegList,true);
            //int n = _tModel.GetSegmentAtElevation(chair_plateLength + baseRingThickness,_global.StackSegList);
            stack_radius = _tModel.GetRadiusAtElevation(chair_plateLength + baseRingThickness, _global.StackSegList,true);
            stack_thinkness = _global.StackSegList[0][2];
          
           
            T3D.Point startPoint = new T3D.Point(-(bottom_radius + BaseRingWidth -x), (elevation_chair + chair_plateLength));
            T3D.Point endPoint = new T3D.Point((bottom_radius + BaseRingWidth + x), (elevation_chair + chair_plateLength));
            double depthUp = 0;
            double depthDown = chair_plateLength;

            if (m_completeStackView != null)
            {
                chairSectionView = DrawingUtils.AddGASectionView(m_completeStackView, startPoint, endPoint, depthUp, depthDown);
                
            }

            //Side View of Chair at bolt degree
          
            startPoint = DrawingUtils.ShiftRadiallyPoint(origin, (bottom_radius  + BaseRingWidth) / 2, 0);
            endPoint = DrawingUtils.ShiftRadiallyPoint(origin, 2 * bottom_radius, 0);
            chairSideView = DrawingUtils.AddGASideView(chairSectionView, startPoint, endPoint);
           

            InsertDimensions(_global,_tModel);
        }
        public override void InsertAnnotations(Globals _global, TeklaModelling _tModel)
        {


        }
        public override void InsertDimensions(Globals _global, TeklaModelling _tModel)
        {
            double topRadius = _tModel.GetRadiusAtElevation(baseRingThickness + chair_plateLength, _global.StackSegList, true);
            double topRingWidth = BaseRingWidth;
            if (bottom_radius> topRadius)
            {
                
                topRingWidth = (bottom_radius - topRadius) + BaseRingWidth;
            }
            //Radial Dimesion
            DrawingUtils.CreateRadiusDimension(chairSectionView, topRadius, 1, origin);
            ///DrawingUtils.CreateRadiusDimension(chairSectionView, distUpBolt + stack_radius + stack_thinkness, 2, origin);
            DrawingUtils.CreateRadiusDimension(chairSectionView, bottom_radius - insideStackDistance, 3, origin);
            // end =====================

            // dimension of chair plate in side view
            T3D.Point Point1 = new T3D.Point(x+bottom_radius-insideStackDistance - stack_thinkness, - upperRingThickness);
            T3D.Point Point2 = new T3D.Point(x+bottom_radius-insideStackDistance - stack_thinkness, (- chair_plateLength + baseRingThickness));
            DrawingUtils.CreateDimension(chairSideView, Point1, Point2);
            //End

            //upper Ring thickness
            Point1 = new T3D.Point(x + bottom_radius - insideStackDistance-stack_thinkness, 0);
            Point2 = new T3D.Point(x + bottom_radius - insideStackDistance-stack_thinkness, - upperRingThickness);
            DrawingUtils.CreateDimension(chairSideView, Point1, Point2);
            //End

            //Bse Ring thickness
            Point1 = new T3D.Point(x + bottom_radius - insideStackDistance - stack_thinkness, - chair_plateLength);
            Point2 = new T3D.Point(x + bottom_radius - insideStackDistance - stack_thinkness, (- chair_plateLength + baseRingThickness));
            DrawingUtils.CreateDimension(chairSideView, Point1, Point2);
            //End

            // Horizontal dimension at diameter in Plan view ===========
            startPoint = new T3D.Point(-(bottom_radius + BaseRingWidth   -x), y);
            endPoint = new T3D.Point(bottom_radius + BaseRingWidth  + x, y);
            DrawingUtils.CreateDimension(chairSectionView, startPoint, endPoint);
            // End ============
            // Horizontal dimension at stack front view ========

            startPoint = new T3D.Point(-(stack_radius - x), (elevation_chair - 90));
            endPoint = new T3D.Point((stack_radius + x), (elevation_chair - 90));
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);

            // end ============================

            //inside distance dimension

            startPoint= new T3D.Point(x+(bottom_radius - stack_thinkness - insideStackDistance), - chair_plateLength - 50);
            endPoint = new T3D.Point(x+(bottom_radius-stack_thinkness),  - chair_plateLength - 50);
            DrawingUtils.CreateDimension(chairSideView, startPoint, endPoint);

            //end ============================

            //stack thickness
            startPoint = new T3D.Point(endPoint.X, - chair_plateLength - 50);
            endPoint = new T3D.Point(startPoint.X+stack_thinkness, - chair_plateLength - 50);
            DrawingUtils.CreateDimension(chairSideView, startPoint, endPoint);

            //end ============================
            //top ring width
            startPoint = new T3D.Point(x+topRadius, 50);
            endPoint = new T3D.Point(startPoint.X + topRingWidth, 50);
            DrawingUtils.CreateDimension(chairSideView, startPoint, endPoint);
            //end ============================

            //bottom ring width
            startPoint = new T3D.Point(x + bottom_radius, - chair_plateLength-50);
            endPoint = new T3D.Point(startPoint.X + BaseRingWidth, - chair_plateLength-50);
            DrawingUtils.CreateDimension(chairSideView, startPoint, endPoint);

            //For platform to platform Dimensions Here is A data from Json of Platform ==============================================

            startPoint = new T3D.Point(-(stack_radius + BaseRingWidth + BaseRingWidthFront - x), (elevation_chair + chair_plateLength));

            foreach (double i in elevation)
            {
                endPoint = new T3D.Point(startPoint.X, i);
                DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
                startPoint = new T3D.Point(-(stack_radius + BaseRingWidth + BaseRingWidthFront - x), i);
            }
            // End Of Platform dimensions ======================================================================

            
        }
    }
}
