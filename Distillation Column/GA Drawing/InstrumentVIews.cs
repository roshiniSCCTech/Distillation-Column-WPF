using DistillationColumn;
using HelperLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace DistillationColumn
{
    class InstrumentVIews : CDrawingView
    {

        Globals _global;
        TeklaModelling _tModel;

        double elevation = 0.0;
        double orientation = 0.0;
        List<TSM.Part> stackSegList = new List<TSM.Part>();
        double stackRad = 0.0;
        double shellThickness = 0.0;
        double stackRadWithThickness = 0.0;
        double refractoryThickness = 0.0;

        double PipeLengh = 0.0;
        double number_of_bolts;


        int startStackIndex;
        int endStackIndex;
        T3D.Point startPoint;
        T3D.Point endPoint;
        T3D.Point origin = new T3D.Point(0, 0);
        View instrumentView;
        View nozzleSideView;
        double depthUp = 0.0;
        double depthDown = 0.0;
        string tagName = "";
        public InstrumentVIews(Globals global, TeklaModelling tModel, string jsonStr)
        {
            // m_drawingInst = drawingInst;
            _global = global;
            _tModel = tModel;
            Generate(jsonStr, _global, _tModel);
        }
        public override void Generate(string jsonStr, Globals _global, TeklaModelling _tModel)
        {

            string json = jsonStr;
            JObject jsonData = JObject.Parse(json);
            List<JToken> tokenListIN = jsonData["instrumental_nozzle"].ToList();
            double x = (double)(jsonData["origin"]["x"]); 
            double y = (double)(jsonData["origin"]["y"]);
            origin = new T3D.Point(x,y);
            foreach (JToken itemIN in tokenListIN.Reverse<JToken>())
            {
                //tagName = ((string)itemIN["tags"]);
                orientation = Convert.ToDouble((string)itemIN["orientation_angle"]);   //0.0;
                elevation = (Double)itemIN["elevation"] + _global.Origin.Z;//m_stackLocZ + 29000;
                PipeLengh = (double)itemIN["pipe_length"];                       //6 in
                number_of_bolts = (double)itemIN["number_of_bolts"];

                //List<JToken> tokenList = jsonData["Refractory_lining_input"].ToList();
                //foreach (JToken item in tokenList.Reverse<JToken>())
                //{
                //    refractoryThickness = (double)(item["thickness"]) * 1000;
                //}

                if (getFirstAndLastStackIndex(elevation, 0, out startStackIndex, out endStackIndex))
                {
                    stackRad = (_global.StackSegList[endStackIndex][1]) / 2;
                    shellThickness = _global.StackSegList[endStackIndex][3];

                }
                stackRad = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
                shellThickness = 0;
                stackRadWithThickness = stackRad + shellThickness;

                // Instrument nozzle views

                startPoint = new T3D.Point(-(stackRadWithThickness + PipeLengh), elevation + 50);
                endPoint = new T3D.Point((stackRadWithThickness + PipeLengh), elevation + 50);
                depthUp = 0;
                depthDown = 100;
                if (m_completeStackView != null)
                {
                    instrumentView = DrawingUtils.AddGASectionView(m_completeStackView, startPoint, endPoint, depthUp, depthDown);
                }

                // Instrument Tags 
                double flangePlateDia = 100;
                double d = stackRadWithThickness + flangePlateDia + 400;
                
                //DrawingUtils.InsertInstrumentTags(instrumentView, AppStrings.Instrument_Nozzle + tagName, orientation, d);
                //End..

                //Top View
                startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, orientation);
                endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness + PipeLengh, orientation);
                nozzleSideView = DrawingUtils.AddGASideView(instrumentView, startPoint, endPoint);
                DrawingUtils.CheckRotation(nozzleSideView, orientation);
                //End======================================

                double theta = Math.Asin((flangePlateDia / 2) / (stackRadWithThickness + PipeLengh));

               T3D.Point p1 = new T3D.Point(stackRadWithThickness, 0);
                T3D.Point p2 = new T3D.Point((stackRadWithThickness + PipeLengh ), 0 );
                DrawingUtils.CreateDimension(nozzleSideView, p1, p2);

                T3D.Point Point1 = new T3D.Point(stackRadWithThickness,0);
                T3D.Point Point2 = new T3D.Point(stackRadWithThickness ,- flangePlateDia);
                DrawingUtils.CreateDimension(nozzleSideView, Point1, Point2);


                //Angle Dimensions
                if (orientation > 0 && orientation < 90)
                {
                    startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, 0);
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, orientation);
                    DrawingUtils.CreateAngleDimension(instrumentView, origin, startPoint, endPoint);
                }
                else if (orientation > 90 && orientation < 180)
                {
                    startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, 90);
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, orientation);
                    DrawingUtils.CreateAngleDimension(instrumentView, origin, startPoint, endPoint);
                }
                else if (orientation > 180 && orientation < 270)
                {
                    startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, 180);
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, orientation);
                    DrawingUtils.CreateAngleDimension(instrumentView, origin, startPoint, endPoint);
                }
                else if (orientation > 270 && orientation < 360)
                {
                    startPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, 270);
                    endPoint = DrawingUtils.ShiftRadiallyPoint(origin, stackRadWithThickness, orientation);
                    DrawingUtils.CreateAngleDimension(instrumentView, origin, startPoint, endPoint);
                }
                //End
            }
        }

        public bool getFirstAndLastStackIndex(double elevation, double ductHeight, out int startStackIndex, out int endStackIndex)
        {
            startStackIndex = -1;
            endStackIndex = -1;
            double ductBottomHeight = elevation - ductHeight;
            double ductTopHeight = elevation;
            //double ductBottomHeight = elevation;

            for (int i = 0; i < _global.StackSegList.Count; i++)
            {
                if (_global.StackSegList[i][0] > (ductBottomHeight))
                {
                    startStackIndex = i - 1;
                    for (int j = startStackIndex; j < _global.StackSegList.Count; j++)
                    {
                        if (_global.StackSegList[j][0] > ductTopHeight)
                        {
                            endStackIndex = j - 1;
                            return true;
                        }
                    }
                    endStackIndex = _global.StackSegList.Count - 1;
                    return true;
                }
            }
            return false;
        }
        public override void InsertAnnotations(Globals _global, TeklaModelling _tModel)
        {

        }

        public override void InsertDimensions(Globals _global, TeklaModelling _tModel)
        {

        }
    }
}
