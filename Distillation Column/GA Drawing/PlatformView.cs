using HelperLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;

namespace DistillationColumn
{
    class PlatformView : CDrawingView
    {
        //Member declaration
        #region
        View platformSectionView;
        T3D.Point origin = new T3D.Point(0, 0);
        double platFromElevation = 0.0;
        double platFromLength = 0.0;
        double platFromWidth = 0.0;
        double distanceFromStack = 0.0;
        double platFromStartAngle = 0.0;
        double extendLength = 0.0;
        double extendStartAngle = 0.0;
        double extendEndAngle = 0.0;
        double plateThickness = 0.0;
        double stackRadiusWithThickness = 0.0;
        double angle = 0.0;
        double angle1 = 0.0;
        double plateHeight = 690;
        double depthUp = 0.0;
        double depthDown = 0.0;
        double bracketExtraAngle = 0.0;
        double grating_outerRadius = 0.0;
        double bracketTotalAngle = 0.0;
        double totalLadderAngle = 16.0;
        double platFormEndAngle = 0.0;
        double totalAngle = 0.0;
        double ladderOrientationAngle = 0.0;
        T3D.Point startPoint;
        T3D.Point endPoint;
        public static Tekla.Structures.Drawing.PointList PointList = new Tekla.Structures.Drawing.PointList();
        List<JToken> platformTokenList = null;
        double checkLadder_end = 0.0;
        double d = 0.0;
        double x = 0.0;
        double y = 0.0;
        double radius = 0.0;
        double segThinkness = 0.0;
        View platformSideView = null;
        double boltCountOfX = 0.0;
        double boltCountOfY = 0.0;
        double distX = 0.0;
        double distY = 0.0;
        double bracketToFirstBoltDist = 0.0;
        double bracketToPlateDist = 0.0;
        int rungDistance = 0;
        double secondBracketAngle;
        #endregion
        public PlatformView(GADrawing drawingInst)
        {
            m_drawingInst = drawingInst;
        }
        public PlatformView(Globals global, TeklaModelling tModel, string jsonStr)
        {
             Generate(jsonStr,global,tModel);
        }



        public override void Generate(string jsonStr, Globals _global, TeklaModelling _tModel)
        {
            string json = jsonStr;
            JObject jsonData = JObject.Parse(json);
            platformTokenList = jsonData["Platform"].ToList();
            foreach (JToken item in platformTokenList)
            {
                platFromElevation = (double)(item["Elevation"])+_global.Origin.Z ;
                platFromWidth = (double)(item["Platform_Width"]);
                platFromLength = (double)(item["Platform_Length"]);
                distanceFromStack = (double)(item["Distance_From_Stack"]);
                platFromStartAngle = (double)(item["Platform_Start_Angle"]);
                platFormEndAngle = (double)(item["Platfrom_End_Angle"]);
                extendLength = (double)(item["Extended_Length"]);
                extendStartAngle = (double)(item["Extended_Start_Angle"]);
                extendEndAngle = (double)(item["Extended_End_Angle"]);
                plateThickness = (double)(item["Grating_Thickness"]);
                x = (double)(jsonData["origin"]["x"]);
                y = (double)(jsonData["origin"]["y"]);
                origin = new T3D.Point(x, y);
                ladderOrientationAngle = (double)(item["Orientation_Angle"]);
                

                stackRadiusWithThickness = _tModel.GetRadiusAtElevation(platFromElevation-_global.Origin.Z, _global.StackSegList, true);
                grating_outerRadius = stackRadiusWithThickness + distanceFromStack + platFromLength;

                bracketExtraAngle = 200 / (grating_outerRadius) * (180 / Math.PI);
                bracketTotalAngle = 1000 / (grating_outerRadius) *(180 / Math.PI);
                totalAngle = platFromStartAngle + platFormEndAngle;
                totalLadderAngle = (790/ grating_outerRadius) * (180 / Math.PI);
                d = stackRadiusWithThickness;

                

                //Platform Plan View
                T3D.Point startPoint = new T3D.Point(-(stackRadiusWithThickness + distanceFromStack + platFromLength + extendLength + 200-x), (platFromElevation ));
                T3D.Point endPoint = new T3D.Point((stackRadiusWithThickness + distanceFromStack + platFromLength +extendLength + 200+x), (platFromElevation ));
                depthUp = plateThickness+100;
                depthDown = 280;
                if (m_completeStackView != null)
                    platformSectionView = DrawingUtils.AddGASectionView(m_completeStackView, startPoint, endPoint, depthUp, depthDown);
                //End

                //Side View at Bracket
                
                if (ladderOrientationAngle < (platFromStartAngle+bracketExtraAngle+bracketTotalAngle+(totalLadderAngle/2)) )
                {

                    secondBracketAngle = ladderOrientationAngle+(totalLadderAngle/2)+bracketExtraAngle+bracketTotalAngle;
                    
                }
                else
                {
                    if (platFromStartAngle == ladderOrientationAngle)
                        platFromStartAngle = ladderOrientationAngle + (totalLadderAngle * 0.5);

                    secondBracketAngle = platFromStartAngle + bracketExtraAngle + bracketTotalAngle;
                    
                }
                

                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness, secondBracketAngle);
                if (secondBracketAngle > extendStartAngle && secondBracketAngle < extendEndAngle)
                {
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength + extendLength + 70, secondBracketAngle);
                }
                else
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength + 70, secondBracketAngle);

                platformSideView = DrawingUtils.AddGASideView(platformSectionView, startPoint, endPoint);
                DrawingUtils.CheckRotation(platformSideView, secondBracketAngle);
                //End

                InsertDimensions(_global, _tModel);
            }
        }
        public override void InsertAnnotations(Globals _global, TeklaModelling _tModel)
        {
            // There is no annotation required in platform view
        }
        public override void InsertDimensions(Globals _global, TeklaModelling _tModel)
        {
            // angle dimension between two support beams
            // 
            if (ladderOrientationAngle < (platFromStartAngle + bracketExtraAngle + bracketTotalAngle + (totalLadderAngle / 2)))
            {
                if (platFromStartAngle == totalLadderAngle)
                    platFromStartAngle += totalLadderAngle / 2;

                else if (platFromStartAngle == ladderOrientationAngle)
                    platFromStartAngle = ladderOrientationAngle + totalLadderAngle / 2;

                angle1 = ladderOrientationAngle + (totalLadderAngle / 2) + bracketExtraAngle;
                angle = platFromStartAngle + bracketExtraAngle;
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, d, (angle1));
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, d, (angle1 + bracketTotalAngle));
                DrawingUtils.CreateAngleDimension(platformSectionView, origin, startPoint, endPoint);
            }
            else
            {
                if (platFromStartAngle == totalLadderAngle)
                    platFromStartAngle += totalLadderAngle / 2;

                else if (platFromStartAngle == ladderOrientationAngle)
                    platFromStartAngle = ladderOrientationAngle + totalLadderAngle / 2;


                angle = platFromStartAngle + bracketExtraAngle;
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, d, (angle));
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, d, (angle + bracketTotalAngle));
                DrawingUtils.CreateAngleDimension(platformSectionView, origin, startPoint, endPoint);

            }
                
            //End================

            // angle dimension between Ladder and Platform support beam at start
            checkLadder_end = 360 - totalAngle;
            if (checkLadder_end <= totalLadderAngle)
            {
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength, (angle));
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength, (ladderOrientationAngle));
                DrawingUtils.CreateAngleDimension(platformSectionView, origin, startPoint, endPoint);
            }
            else
            {
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength, (angle));
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadiusWithThickness + distanceFromStack + platFromLength, (ladderOrientationAngle));
                DrawingUtils.CreateAngleDimension(platformSectionView, origin, startPoint, endPoint);
            }
            //End ===============================


            //Radial dimesions
            DrawingUtils.CreateRadiusDimension(platformSectionView, stackRadiusWithThickness, 2, origin);
            if(extendStartAngle == extendEndAngle)
            {
                if (platFromStartAngle > platFormEndAngle)
                {
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, 0, platFormEndAngle, stackRadiusWithThickness + distanceFromStack, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, 0, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength, origin);
                }
                else
                {
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, (platFromStartAngle + platFormEndAngle) / 4, platFormEndAngle, stackRadiusWithThickness + distanceFromStack, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, (platFromStartAngle + platFormEndAngle) / 4, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength, origin);
                }
            }
            else
            {
                if (platFromStartAngle > platFormEndAngle)
                {
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, 0, platFormEndAngle, stackRadiusWithThickness + distanceFromStack, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, 0, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, 0, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength+extendLength, origin);
                }
                else
                {
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, (platFromStartAngle + platFormEndAngle) / 4, platFormEndAngle, stackRadiusWithThickness + distanceFromStack, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, (platFromStartAngle + platFormEndAngle) / 4, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength, origin);
                    DrawingUtils.CreateRadialDimensionsforPlatform(platformSectionView, platFromStartAngle, (platFromStartAngle + platFormEndAngle) / 4, platFormEndAngle, stackRadiusWithThickness + distanceFromStack + platFromLength+extendLength, origin);
                }
            }

            //End===============

            // Dimension from origin to rung of ladder
            //if (LadderMain.mainRungDistance.Count != 0)
            //{
            //  T3D.Point pp = DrawingUtils.ShiftRadiallyPoint(origin, LadderMain.mainRungDistance[rungDistance], ladderOrientationAngle);
            //  DrawingUtils.CreateDimension(platformSectionView, origin, pp);
            //  rungDistance++;
            //}
            //end=======

            // Bracket Dimensions (Side View)
            if (secondBracketAngle > extendStartAngle && secondBracketAngle < extendEndAngle)
                platFromLength += extendLength;
            else
                platFromLength =platFromLength;

            if (platFromLength <= 1500)
            {
                boltCountOfX = 2;
                boltCountOfY = 1;
                distX = 70;
                distY = 60;
                bracketToPlateDist = 5.0;
                bracketToFirstBoltDist = 40;
            }
            else if (platFromLength > 1500 && platFromLength <= 2000)
            {
                boltCountOfX = 2;
                boltCountOfY = 1;
                distX = 70;
                distY = 100;
                bracketToPlateDist = 10.0;
                bracketToFirstBoltDist = 40;
            }
            else if (platFromLength > 2000 && platFromLength <= 2500)
            {
                boltCountOfX = 2;
                boltCountOfY = 3;
                distX = 80;
                distY = 60;
                bracketToPlateDist = 10.0;
                bracketToFirstBoltDist = 55;
            }
            else if (platFromLength > 2500 && platFromLength <= 3500)
            {
                boltCountOfX = 2;
                boltCountOfY = 5;
                distX = 80;
                distY = 70;
                bracketToPlateDist = 10.0;
                bracketToFirstBoltDist = 65;
            }

            T3D.Point point1;
            T3D.Point point2;
            T3D.Point point3;
            T3D.Point point4;

            // Bracket to plate dimensions
            PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness, 0));
            PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness, ( - bracketToPlateDist)));
            DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList, 'v');
            PointList.Clear();
            //End

            // Bracket to first bolt dimenion
            PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness,  - bracketToPlateDist));
            PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness, ( - bracketToPlateDist - bracketToFirstBoltDist)));
            DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList, 'v');
            PointList.Clear();
            //End

            //Horizontal Dimensions (Bracket Length, 70 , and 200 )
            PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness, 0, 0));
            PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness + distanceFromStack, 0));
            PointList.Add(point3 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + platFromLength, 0));
            PointList.Add(point4 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + platFromLength + 70, 0));
            DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList);
            PointList.Clear();
            //End

            //Bracket length
            PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness + distanceFromStack - 40, -50, 0));
            PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness + distanceFromStack+ platFromLength + 70, -50,0));
            DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList);
            PointList.Clear();

            //TOE plate bolt Dimension
            PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + 20, -bracketToPlateDist-bracketToFirstBoltDist, 0));
            PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + 20,  point1.Y-distY, 0));
            DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList, 'v');
            PointList.Clear();
            //End

            // Vertical dimensions between Bolts
            for (int i = 0; i < boltCountOfY; i++)
            {
                PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness, (-bracketToPlateDist - bracketToFirstBoltDist - (distY * i))));
                PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness, (point1.Y - (distY))));
                DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList, 'v');
                PointList.Clear();
            }
            //End=====

            // Horizontal dimensions of bolts
            for (int i = 0; i < boltCountOfX; i++)
            {
                PointList.Add(point1 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + (distX * i), -250));
                PointList.Add(point2 = new T3D.Point(stackRadiusWithThickness + distanceFromStack + (distX * i) + distX, -250));
                DrawingUtils.InsertStraightDimensionSet(platformSideView, PointList);
                PointList.Clear();
            }
            //End
        }
    }
}
