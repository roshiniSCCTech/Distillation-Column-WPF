using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures;
using DistillationColumn;
using HelperLibrary;

namespace DistillationColumn
{
    class StackModuleAssembly
    {
        #region
        public AssemblyDrawing assemblyDrawing = null;
        DrawingHandler dwgHandler = new DrawingHandler();
        public static View assembly_front_view;
        Tekla.Structures.Geometry3d.CoordinateSystem ModelObjectCoordSys;
        ArrayList Parts;
        List<ArrayList> PartsList = new List<ArrayList>();
        string viewType = "";
        public List<View> parentViewList = new List<View>();
        public List<View> sideViewList = new List<View>();
        const string Plan = "Plan";
        const string Side = "Side";
        const string InnerSide = "InnerSide";
        // StackAssemblyDimensions dimensions = new StackAssemblyDimensions();
        Globals _global;
        TeklaModelling _tModel;
        public StackModuleAssembly(Globals global,TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;
        }
        #endregion
        public void Build(Assembly assemb)
        {
            // dimensions.Build(jsonStr);
            _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            ModelObjectEnumerator selectedModelObjects = _tModel.Model.GetModelObjectSelector().GetAllObjects();
            ModelObjectCoordSys = new T3D.CoordinateSystem();
            DrawingUtils.GetCoordinateSystemAndNameOfSelectedObject(selectedModelObjects, out ModelObjectCoordSys, out string ModelObjectName);

            
              assemblyDrawing = new AssemblyDrawing(assemb.Identifier, "Leo.A");
                assemblyDrawing.Name = "Stack Module Assembly" ;
                Tekla.Structures.Drawing.Size A1SizePortrait = new Tekla.Structures.Drawing.Size(800, 843);
                assemblyDrawing.Layout.SheetSize = A1SizePortrait;
                assemblyDrawing.Insert();
                Parts = DrawingUtils.GetAssemblyParts(assemb);
                PartsList.Add(Parts);
                
            
            //InsertFrontView();
        }

        //public void InsertFrontView()
        //{
        //    DrawingEnumerator drawingEnumerator = dwgHandler.GetDrawings();
        //    while (drawingEnumerator.MoveNext())
        //    {
        //        Tekla.Structures.Drawing.Drawing drawing = drawingEnumerator.Current as AssemblyDrawing;
        //        if (drawing != null && drawing.Name.StartsWith("Stack Module Assembly"))
        //        {
        //            dwgHandler.SetActiveDrawing(drawing);
        //            DrawingObjectEnumerator allViews = drawing.GetSheet().GetAllViews();
        //            while (allViews.MoveNext())
        //            {
        //                if (allViews.Current != null)
        //                {
        //                    allViews.Current.Delete();
        //                    drawing.CommitChanges();
        //                }
        //            }
        //            string pr = drawing.Name.Substring(drawing.Name.Length - 1);
        //            int drawingNum = int.Parse(pr);
        //            assembly_front_view = DrawingUtils.AddAssemblyFrontView("FrontView", drawing, PartsList[drawingNum], DrawingUtils.GetBasicCoordinateSystemForFrontView(ModelObjectCoordSys));
        //            InsertSectionViews(assembly_front_view, drawingNum);
        //            drawing.PlaceViews();
        //            dwgHandler.CloseActiveDrawing(true);
        //        }
        //    }
        //}

        //public void InsertSectionViews(View parentView, int drawingNum)
        //{
        //    if (parentView != null)
        //    {
        //        foreach (ArrayList viewDataList in StackAssemblyStructure.assemblyStructure)
        //        {
        //            if (drawingNum == (int)viewDataList[0])
        //            {
        //                viewType = (string)viewDataList[1];
        //                if (viewType == Plan)
        //                    InsertPlanViews(parentView, viewDataList);
        //                else if (viewType == Side)
        //                    InsertSideViews(parentViewList, viewDataList);
        //                else if (viewType == InnerSide)
        //                    InsertSide2Views(viewDataList);
        //            }
        //        }
        //    }
        //}

        //public void InsertPlanViews(View parentView, ArrayList viewDataList)
        //{
        //    View view = DrawingUtils.AddAssemblySectionView(parentView, (T3D.Point)(viewDataList[2]), (T3D.Point)(viewDataList[3]), Convert.ToDouble(viewDataList[4]), Convert.ToDouble(viewDataList[5]), DrawingSettings.AssemblyPlanViewAttributes);
        //    parentViewList.Add(view);
        //    //      if (viewDataList.Count ==7)
        //    //dimensions.InsertDimensions((ViewName)(viewDataList[6]),view);
        //}
        //public void InsertSideViews(List<View> parentViewList, ArrayList viewDataList)
        //{
        //    View sideView = DrawingUtils.AddAssemblySideView(parentViewList[parentViewList.Count - 1], (T3D.Point)viewDataList[2], (T3D.Point)viewDataList[3]);
        //    sideViewList.Add(sideView);
        //    DrawingUtils.CheckRotation(sideView, (double)(viewDataList[4]));
        //}
        //public void InsertSide2Views(ArrayList viewDataList)
        //{
        //    View sideView = DrawingUtils.AddAssemblySideView(sideViewList[sideViewList.Count - 1], (T3D.Point)viewDataList[2], (T3D.Point)viewDataList[3]);
        //    DrawingUtils.CheckRotation(sideView, (double)(viewDataList[4]));
        //}

    }
}

