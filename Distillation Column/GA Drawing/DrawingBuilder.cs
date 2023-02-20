using System;
using System.Windows;
using System.Windows.Documents;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using System.Collections.Generic;
using HelperLibrary;

namespace DistillationColumn
{
    class DrawingBuilder
    {
        Globals _global;
        TeklaModelling _tModel;
        TransformationPlane currentPlane;
        public DrawingBuilder(Globals global, TeklaModelling tModel,string jsonStr)
        {
            _global = global;
            _tModel = tModel;
            currentPlane = _tModel.Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();// GLOBAL TRANSFORMATION PLANE
            BuildViews(jsonStr);
        }

        DrawingHandler dwgHandler = new DrawingHandler();
        readonly Tekla.Structures.Geometry3d.Vector UpDirection = new Tekla.Structures.Geometry3d.Vector(0.0, 0.0, 1.0);
        List<CDrawingView> viewList = new List<CDrawingView>();
        public void BuildViews(string jsonStr)
        {
           
            

            //Tekla.Structures.Drawing.Size A1Size = new Tekla.Structures.Drawing.Size(841, 594);
            Tekla.Structures.Drawing.Size A1SizePortrait = new Tekla.Structures.Drawing.Size(594, 843);
            GADrawing drawingInst = new GADrawing("LCH", A1SizePortrait);
            drawingInst.Name = "General Arrangement Drawing";
            try
            {
                _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
                CDrawingView compleStackView = new CompleteStackView(drawingInst);
                compleStackView.Generate(jsonStr);
                dwgHandler.SetActiveDrawing(drawingInst, true);
                //compleStackView.InsertAnnotations();
                //compleStackView.InsertDimensions();
            }
            catch (Exception exception)
            {
                _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                //MessageBox.Show(exception.ToString());
            }
            //BuildDrawing(new ChairView(drawingInst), jsonStr);
            //BuildDrawing(new InstrumentVIews(drawingInst), jsonStr);
            //BuildDrawing(new PlatformView(drawingInst), jsonStr);
            //BuildDrawing(new SpliceView(drawingInst), jsonStr);
            //BuildDrawing(new FloorSteelView(drawingInst), jsonStr);
            //BuildDrawing(new DuctOpeningView(drawingInst), jsonStr);
            //BuildDrawing(new PaintersTrolleyView(drawingInst), jsonStr);

            _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane); //return original plane
            drawingInst.PlaceViews();
            dwgHandler.CloseActiveDrawing(true);
            MessageBox.Show("GaDrawing is finished.Please check in Document Manager");
        }

        public void BuildDrawing(CDrawingView drawingView, string jsonStr)
        {
            try
            {
                drawingView.Generate(jsonStr);
            }
            catch (Exception exception)
            {
                _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);
                // MessageBox.Show(exception.ToString());
            }
        }
    }
}
