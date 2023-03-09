using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures;
using Part = Tekla.Structures.Model.Part;
using ModelObject = Tekla.Structures.Model.ModelObject;
using HelperLibrary;

namespace DistillationColumn
{
    enum AssemblyType
    {
        Parallel,
        Circular,
        StartEnd,
    }
    class HandrailAssemblyDrawing
    {

        Globals _global;
        TeklaModelling _tModel;
        Tekla.Structures.Geometry3d.CoordinateSystem ModelObjectCoordSys;
        AssemblyDrawing assemblyDrawing;
        public Part handrailMainPart;
        public List<ModelObject> handRailModelObjects = new List<ModelObject>();
        DrawingHandler dwgHandler = new DrawingHandler();
        Assembly handrailAssembly = new Assembly();
        ArrayList Parts;
        ArrayList handrails = new ArrayList();
        double platFromElevation = 0.0;
        List<double> elevation = new List<double>();
        double depthUp = 25;
        double depthDown = 50;
        double x = 0.0;
        double y = 0.0;
        const double verticalDistance = 500;
        T3D.Point origin = new T3D.Point(0, 0);
        const double extraX = 300;
        string handrailType;
        ArrayList handRailMember = new ArrayList();
        List<ModelObject> mainPartList = new List<ModelObject>();

        public HandrailAssemblyDrawing(Globals global, TeklaModelling tModel, string jStr)
        {

            _global = global;
            _tModel = tModel;
            // StackAsssemblyList = new List<Assembly>();
            AddHandrailAssembly(_global.handrailCollection);
            MessageBox.Show("Please perform numbering. After numbering click ok!!");
            Build(handrails);
        }


        //public void AddHandrailAssembly(List<ArrayList> assembly)
        //{
        //    try
        //    {

        //        foreach (var val in assembly)
        //        {
        //            if (val.Count != 0)
        //            {
        //                foreach (var item in val)
        //                {
        //                    handrailAssembly = (item as Part).GetAssembly();
        //                    (item as Part).PartNumber.Prefix = "SGR";
        //                    (item as Part).Modify();

        //                }
        //                handrailAssembly.Add(val);
        //                handrailAssembly.AssemblyNumber.Prefix = "SGR";
        //                handrailAssembly.Modify();
        //                handrails.Add(handrailAssembly);

        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        // MessageBox.Show(ex.ToString());
        //    }
        //}

        public void AddHandrailAssembly(List<ArrayList> assembly)
        {
            int i = 0;
            try
            {
                foreach (ArrayList value in assembly)
                {
                    if (value.Count != 0)
                    {
                        //Assembly handrailAssembly = new Assembly();

                        bool Flag = false;
                        foreach (var val in value)
                        {
                            handrailAssembly = (val as Part).GetAssembly();
                            (val as Part).PartNumber.Prefix = "SHR";
                            (val as Part).Modify();
                            //   if ((val as Part).Name == "SecondPost_Type3" && Flag == false)
                            //if ((val as Part).Name == "TopRail_Type3")
                            //{
                            //    handrailMainPart = (val as Part);
                            //    handRailModelObjects.Add(handrailMainPart);
                            //     //Flag = true;
                            //}
                            //if ((val as Part).Name == "TopRail_Type3")
                            //{
                            //    handrailMainPart = (val as Part);
                            //    handRailModelObjects.Add(handrailMainPart);
                            //     // Flag = true;
                            //}
                            //;
                            //singlePartList.Add(val);
                        }
                        handrailAssembly.Add(value);
                        handrailAssembly.AssemblyNumber.Prefix = "SHR";
                        handrailAssembly.Modify();
                        //if (handrailMainPart != null)
                        //    handrailAssembly.SetMainPart(handrailMainPart);
                        handrailAssembly.Modify();
                        handrails.Add(handrailAssembly);
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + i.ToString());
            }
        }
        //public void Build()
        //{


        //    // JObject jsonData = JObject.Parse(jsonStr);
        //    // List<JToken> platformTokenList = jsonData["Platform"].ToList();
        //    //// handrailType = (string)jsonData["Handrail"]["name"];
        //    // foreach (JToken item in platformTokenList.Reverse<JToken>())
        //    // {
        //    //     platFromElevation = (double)(item["Elevation"])+_global.Origin.Z;
        //    //     elevation.Add(platFromElevation);
        //    // }
        //    //handRailMember.Add("Length");
        //    //origin = new T3D.Point(x, y);

        //    _tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
        //    ModelObjectEnumerator selectedModelObjects = _tModel.Model.GetModelObjectSelector().GetAllObjects();
        //    ModelObjectCoordSys = new T3D.CoordinateSystem();
        //    DrawingUtils.GetCoordinateSystemAndNameOfSelectedObject(selectedModelObjects, out ModelObjectCoordSys, out string ModelObjectName);
        //    int i = 0;
        //    foreach (Assembly assemb in handrails)
        //    {
        //        assemblyDrawing = new AssemblyDrawing(assemb.Identifier, "HandRail.A");
        //        assemblyDrawing.Name = "HANDRAIL";
        //        Tekla.Structures.Drawing.Size A1SizePortrait = new Tekla.Structures.Drawing.Size(800, 843);
        //        assemblyDrawing.Layout.SheetSize = A1SizePortrait;
        //        assemblyDrawing.Insert();
        //        Parts = DrawingUtils.GetAssemblyParts(assemb);

        //    }

        //    i++;

        //    //MapDrawingNumberWithMainPart();

        //    //InsertSectionView();
        //}

        public void Build(ArrayList assemb)
        {
            int i = 0;
            foreach (Assembly assembly in assemb)
            {
                //_tModel.Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
                //ModelObjectEnumerator selectedModelObjects = _tModel.Model.GetModelObjectSelector().GetAllObjects();
                //ModelObjectCoordSys = new T3D.CoordinateSystem();
                //DrawingUtils.GetCoordinateSystemAndNameOfSelectedObject(selectedModelObjects, out ModelObjectCoordSys, out string ModelObjectName);
                assemblyDrawing = new AssemblyDrawing(assembly.Identifier);
                assemblyDrawing.Name = "HANDRAIL"+i.ToString();
                Tekla.Structures.Drawing.Size A1SizePortrait = new Tekla.Structures.Drawing.Size(800, 843);
                assemblyDrawing.Layout.SheetSize = A1SizePortrait;
                assemblyDrawing.Insert();
                i++;
            }
        }

        public void InsertSectionView()
        {
            DrawingEnumerator drawingEnumerator = dwgHandler.GetDrawings();
            //List<double> topRailLength = GetLengthOfMainPart();
            double topRailLength;
            while (drawingEnumerator.MoveNext())
            {
                Tekla.Structures.Drawing.Drawing drawing = drawingEnumerator.Current as AssemblyDrawing;
                dwgHandler.UpdateDrawing(drawing);
                if (drawing != null && drawing.Name.StartsWith("HANDRAIL"))
                {
                    dwgHandler.SetActiveDrawing(drawing);
                    DrawingObjectEnumerator allViews = drawing.GetSheet().GetAllViews();
                    while (allViews.MoveNext())
                    {
                        View handRailFrontView = (allViews.Current as View);
                        topRailLength = (GetLengthOfMainPartFromView(SelectMainPartFromDrawing(handRailFrontView.GetModelObjects())));
                        string pr = drawing.Name.Substring(drawing.Name.Length - 1);
                        //int drawingnum = Int32.Parse(pr);
                        try
                        {
                            if (type == AssemblyType.Parallel)
                            {
                                double verDist = 250;
                                View section1 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X + extraX, origin.Y + verDist), new T3D.Point(origin.X - topRailLength - extraX, origin.Y + verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView); //top rail
                                View section2 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X + extraX, -verDist), new T3D.Point(origin.X - topRailLength - extraX, -verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView); // mid rail
                                if (handrailType == "Type 3")
                                {
                                    View section3 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - 50, -3 * verDist), new T3D.Point(origin.X + 50, -3 * verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView);
                                }
                            }
                            else if (type == AssemblyType.Circular)
                            {
                                View section1 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - extraX, origin.Y), new T3D.Point(origin.X + topRailLength + extraX, origin.Y), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView); //top rail
                                View section2 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - extraX, -verticalDistance), new T3D.Point(origin.X + topRailLength + extraX, -verticalDistance), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView); // mid rail
                                if (handrailType == "Type 3")
                                {
                                    View section3 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - 50, -2 * verticalDistance), new T3D.Point(origin.X + 50, -2 * verticalDistance), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView);
                                }
                            }
                            else
                            {
                                double verDist = 250;
                                View section1 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - 10, origin.Y + verDist), new T3D.Point(origin.X + topRailLength + 2 * extraX, origin.Y + verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView);
                                View section2 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X - 10, -verDist), new T3D.Point(origin.X + topRailLength + 2 * extraX, -verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView);
                                if (handrailType == "Type 3")
                                {
                                    View section3 = DrawingUtils.AddAssemblySectionView(handRailFrontView, new T3D.Point(origin.X + 240, -3 * verDist), new T3D.Point(origin.X + 310, -3 * verDist), depthUp, depthDown, DrawingSettings.HandrailAssemblySectionView);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                        drawing.PlaceViews();
                        dwgHandler.CloseActiveDrawing(true);
                    }
                }
            }
        }

        public void MapDrawingNumberWithMainPart()
        {
            DrawingEnumerator drawingEnumerator = dwgHandler.GetDrawings();
            List<int> drawingIndex = new List<int>();
            string drawingName = "";
            while (drawingEnumerator.MoveNext())
            {
                Tekla.Structures.Drawing.Drawing drawing = drawingEnumerator.Current as AssemblyDrawing;
                if (drawing != null && drawing.Name.StartsWith("HANDRAIL"))
                {
                    drawingName = drawing.Name.Substring(8);
                    drawingIndex.Add(int.Parse(drawingName));
                }
            }
            drawingIndex.Sort();
        }
        public List<double> GetLengthOfMainPart(List<ModelObject> mainPartList)
        {
            List<double> tempList = new List<double>();
            foreach (ModelObject modelObject in mainPartList)
            {
                Hashtable table = new Hashtable(handRailMember.Count);
                if (modelObject.GetDoubleReportProperties(handRailMember, ref table))
                {
                    foreach (DictionaryEntry value in table)
                    {
                        tempList.Add(Convert.ToDouble(value.Value));
                    }
                }
            }
            return tempList;
        }
        AssemblyType type;
        public double GetLengthOfMainPartFromView(ModelObject modelObject)
        {
            double length = 0.0;

            Hashtable table = new Hashtable(handRailMember.Count);
            if (modelObject.GetDoubleReportProperties(handRailMember, ref table))
            {
                foreach (DictionaryEntry value in table)
                    length = (Convert.ToDouble(value.Value));
            }
            return length;
        }

        public Part SelectMainPartFromDrawing(DrawingObjectEnumerator allParts)
        {
            Part mdlPart = null;
            while (allParts.MoveNext())
            {
                if (allParts.Current is Tekla.Structures.Drawing.Part)
                {
                    Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;
                    if (modelObject == null) continue;
                    var dwgPart = allParts.Current as Tekla.Structures.Drawing.Part;
                    if (dwgPart == null) continue;
                    mdlPart = new Tekla.Structures.Model.Model().SelectModelObject(dwgPart.ModelIdentifier) as Tekla.Structures.Model.Part;
                    if (mdlPart == null) continue;
                    //  if (mdlPart.Name == AppStrings.handrailPipeTopRail || mdlPart.Name == AppStrings.topRail || mdlPart.Name == AppStrings.handrailPipeTopRailStartEnd)
                    if (mdlPart.Name == "SecondPost_Type3")
                    {
                        //if (mdlPart.Name == AppStrings.topRail)
                        //    type = AssemblyType.Parallel;
                        //else if (mdlPart.Name == AppStrings.handrailPipeTopRail)
                        //    type = AssemblyType.Circular;
                        //else
                        // type = AssemblyType.StartEnd;
                        type = AssemblyType.Circular;
                        return mdlPart;
                    }
                }
            }
            return mdlPart;
        }
    }
}
