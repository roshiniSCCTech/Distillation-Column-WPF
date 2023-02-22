using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using Tekla.Structures.Datatype;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.ModelInternal;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures.Drawing.UI;
using Tekla.Structures;
using Newtonsoft.Json.Linq;
using System.IO;
using HelperLibrary;

namespace DistillationColumn

{

    public static class DrawingUtils
    {
        
        static DrawingHandler DrawingHandler = new DrawingHandler();
        static readonly Tekla.Structures.Geometry3d.Vector UpDirection = new Tekla.Structures.Geometry3d.Vector(0.0, 0.0, 1.0);
        // static T3D.Point origin = new T3D.Point(0, 0);
        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicCoordinateSystemForFrontView(
            Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();
            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));
            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection).GetNormal();
            result.AxisY = UpDirection.GetNormal();
            return result;
        }

        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicCoordinateSystemForTopView(
          Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();
            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));
            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector.Cross(UpDirection);
            result.AxisY = tempVector;
            return result;
        }
        public static View Add3DView(string Name, GADrawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem displayCoordinateSystem = new Tekla.Structures.Geometry3d.CoordinateSystem();

            Tekla.Structures.Geometry3d.Matrix RotationAroundX = Tekla.Structures.Geometry3d.MatrixFactory.Rotate(20.0 * Math.PI * 2.0 / 360.0, CoordinateSystem.AxisX);
            Tekla.Structures.Geometry3d.Matrix RotationAroundZ = Tekla.Structures.Geometry3d.MatrixFactory.Rotate(30.0 * Math.PI * 2.0 / 360.0, CoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Matrix Rotation = RotationAroundX * RotationAroundZ;

            displayCoordinateSystem.AxisX = new Tekla.Structures.Geometry3d.Vector(Rotation.Transform(new Tekla.Structures.Geometry3d.Point(CoordinateSystem.AxisX)));
            displayCoordinateSystem.AxisY = new Tekla.Structures.Geometry3d.Vector(Rotation.Transform(new Tekla.Structures.Geometry3d.Point(CoordinateSystem.AxisY)));

            Tekla.Structures.Drawing.View FrontView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(),
                                                                                        CoordinateSystem,
                                                                                        displayCoordinateSystem,
                                                                                        Parts);

            FrontView.Attributes.LoadAttributes(DrawingSettings.GAFrontViewAttributes);
            FrontView.Name = Name;
            FrontView.Attributes.FixedViewPlacing = false;
            FrontView.Attributes.Scale = 1.0 / 75;
            FrontView.Insert();
            FrontView.Modify();
            return FrontView;
        }
        public static Tekla.Structures.Geometry3d.CoordinateSystem GetBasicViewsCoordinateSystemForEndView(Tekla.Structures.Geometry3d.CoordinateSystem objectCoordinateSystem)
        {
            Tekla.Structures.Geometry3d.CoordinateSystem result = new Tekla.Structures.Geometry3d.CoordinateSystem();

            result.Origin = new Tekla.Structures.Geometry3d.Point(objectCoordinateSystem.Origin);
            result.AxisX = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisX) * -1.0;
            result.AxisY = new Tekla.Structures.Geometry3d.Vector(objectCoordinateSystem.AxisY);

            Tekla.Structures.Geometry3d.Vector tempVector = (result.AxisX.Cross(UpDirection));

            if (tempVector == new Tekla.Structures.Geometry3d.Vector())
                tempVector = (objectCoordinateSystem.AxisY.Cross(UpDirection));

            result.AxisX = tempVector;
            result.AxisY = UpDirection;

            return result;
        }
        public static void GetCoordinateSystemAndNameOfSelectedObject(ModelObjectEnumerator SelectedModelOjects, out Tekla.Structures.Geometry3d.CoordinateSystem ModelObjectCoordSys, out string ModelObjectName)
        {
            if (SelectedModelOjects.Current is Tekla.Structures.Model.Part)
            {
                ModelObjectCoordSys = (SelectedModelOjects.Current as Tekla.Structures.Model.Part).GetCoordinateSystem();
                ModelObjectName = (SelectedModelOjects.Current as Tekla.Structures.Model.Part).GetPartMark();
            }
            else if (SelectedModelOjects.Current is Tekla.Structures.Model.Assembly)
            {
                ModelObjectCoordSys = (SelectedModelOjects.Current as Tekla.Structures.Model.Assembly).GetCoordinateSystem();
                ModelObjectName = (SelectedModelOjects.Current as Tekla.Structures.Model.Assembly).AssemblyNumber.Prefix +
                    (SelectedModelOjects.Current as Tekla.Structures.Model.Assembly).AssemblyNumber.StartNumber;
            }
            else if (SelectedModelOjects.Current is Tekla.Structures.Model.BaseComponent)
            {
                ModelObjectCoordSys = (SelectedModelOjects.Current as Tekla.Structures.Model.BaseComponent).GetCoordinateSystem();
                ModelObjectName = (SelectedModelOjects.Current as Tekla.Structures.Model.BaseComponent).Name;
            }
            else
            {
                ModelObjectCoordSys = new Tekla.Structures.Geometry3d.CoordinateSystem();
                ModelObjectName = "";
            }
        }
        public static ArrayList GetAssemblyParts(Assembly assembly)
        {
            ArrayList Parts = new ArrayList();
            IEnumerator AssemblyChildren = (assembly).GetSecondaries().GetEnumerator();
            Parts.Add((assembly).GetMainPart().Identifier);
            while (AssemblyChildren.MoveNext())
                Parts.Add((AssemblyChildren.Current as Tekla.Structures.Model.ModelObject).Identifier);
            return Parts;
        }
        public static View AddAssemblyFrontView(string Name, Tekla.Structures.Drawing.Drawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            View MyView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(), CoordinateSystem, CoordinateSystem, Parts);
            MyView.Insert();
            MyView.Name = Name;
            MyView.Attributes.LoadAttributes(DrawingSettings.AssemblyFrontViewAttributes);
            MyView.Modify();
            return MyView;
        }
        public static View AddAssemblySectionView(View view, T3D.Point startPoint, T3D.Point endPoint, double depthUp, double depthDown, string property = "IOCL_ASSE1")
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);
            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            viewAttributes.LoadAttributes(property);
            View sectionView = null;
            SectionMark sectionMark = new SectionMark(view, startPoint, endPoint);
            SectionMark.SectionMarkAttributes sectionMarkAttributes = new SectionMark.SectionMarkAttributes();
            View.CreateSectionView(view, endPoint, startPoint, insertionPoint, depthDown, depthUp, viewAttributes, sectionMarkAttributes, out sectionView, out sectionMark);
            return sectionView;
        }

        public static View AddAssemblySideView(View view, T3D.Point startPoint, T3D.Point endPoint)
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);
            double depthUp = 1;
            double depthDown = 1;
            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            viewAttributes.LoadAttributes(DrawingSettings.AssemblySideViewAttributes);
            viewAttributes.Scale = 1.0 / 10;
            View sectionView = null;

            SectionMark sectionMark = new SectionMark(view, startPoint, endPoint);
            SectionMark.SectionMarkAttributes sectionMarkAttributes = new SectionMark.SectionMarkAttributes();
            View.CreateSectionView(view, startPoint, endPoint, insertionPoint, depthUp, depthDown, viewAttributes, sectionMarkAttributes, out sectionView, out sectionMark);
            return sectionView;

        }
        public static View AddGaFrontView(string Name, GADrawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            View MyView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(), CoordinateSystem, CoordinateSystem, Parts);
            MyView.Attributes.LoadAttributes(DrawingSettings.GAFrontViewAttributes);
            MyView.Name = Name;
            MyView.Attributes.FixedViewPlacing = false;
            MyView.Attributes.Scale = 1.0 / 75;
            MyView.Insert();
            MyView.Modify();
            return MyView;
        }
        public static View AddGASectionView(View view, T3D.Point startPoint, T3D.Point endPoint, double depthUp, double depthDown)
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);

            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            viewAttributes.LoadAttributes(DrawingSettings.GASectionViewAttributes);
            viewAttributes.Scale = 1.0 / 50;
            View sectionView = null;
            SectionMark sectionMark = new SectionMark(view, startPoint, endPoint);
            SectionMark.SectionMarkAttributes sectionMarkAttributes = new SectionMark.SectionMarkAttributes();
            View.CreateSectionView(view, endPoint, startPoint, insertionPoint, depthDown, depthUp, viewAttributes, sectionMarkAttributes, out sectionView, out sectionMark);

            return sectionView;

        }
        public static View AddGASideView(View view, T3D.Point startPoint, T3D.Point endPoint)
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);
            double depthUp = 0.001;
            double depthDown = 0.001;
            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            viewAttributes.LoadAttributes(DrawingSettings.GASideViewAttributes);
            viewAttributes.Scale = 1.0 / 10;
            View sectionView = null;
            SectionMark sectionMark = new SectionMark(view, startPoint, endPoint);
            SectionMark.SectionMarkAttributes sectionMarkAttributes = new SectionMark.SectionMarkAttributes();
            View.CreateSectionView(view, startPoint, endPoint, insertionPoint, depthUp, depthDown, viewAttributes, sectionMarkAttributes, out sectionView, out sectionMark);
            //sectionView.Attributes.LoadAttributes("Side");
            return sectionView;
        }
        public static View AddMSDFrontView(string Name, GADrawing MyDrawing, ArrayList Parts, Tekla.Structures.Geometry3d.CoordinateSystem CoordinateSystem)
        {
            View MyView = new Tekla.Structures.Drawing.View(MyDrawing.GetSheet(), CoordinateSystem, CoordinateSystem, Parts);
            MyView.Attributes.LoadAttributes(DrawingSettings.GAFrontViewAttributes);
            MyView.Name = Name;
            MyView.Attributes.FixedViewPlacing = false;
            MyView.Attributes.Scale = 1.0 / 75;
            MyView.Insert();
            MyView.Modify();
            return MyView;
        }
        public static View AddMSDSectionView(View view, T3D.Point startPoint, T3D.Point endPoint, double depthUp, double depthDown)
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);

            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            viewAttributes.LoadAttributes(DrawingSettings.MarkingSchemeDrawingSection);
            viewAttributes.Scale = 1.0 / 50;
            View sectionView = null;
            SectionMark sectionMark = new SectionMark(view, startPoint, endPoint);
            SectionMark.SectionMarkAttributes sectionMarkAttributes = new SectionMark.SectionMarkAttributes();
            View.CreateSectionView(view, endPoint, startPoint, insertionPoint, depthDown, depthUp, viewAttributes, sectionMarkAttributes, out sectionView, out sectionMark);

            return sectionView;

        }

        public static View AddDetailView(View view, T3D.Point centrePoint, T3D.Point boundryPoint, T3D.Point labelPoint)
        {
            T3D.Point insertionPoint = new T3D.Point(2000, 2000);
            View.ViewAttributes viewAttributes = new View.ViewAttributes();
            DetailMark.DetailMarkAttributes detailMarkAttributes = new DetailMark.DetailMarkAttributes();
            DetailMark detailMark = null;
            View detailView = null;
            View.CreateDetailView(view, centrePoint, boundryPoint, labelPoint, insertionPoint, viewAttributes, detailMarkAttributes, out detailView, out detailMark);

            return view;
        }
        public static void CreateDimension(View view, T3D.Point point1, T3D.Point point2)
        {
            if (view != null)
            {
                Tekla.Structures.Drawing.PointList pts = new Tekla.Structures.Drawing.PointList();
                pts.Add(point1);
                pts.Add(point2);
                double distance = Math.Sqrt(new T3D.Vector(pts[1] - pts[0]).Dot(new T3D.Vector(pts[1] - pts[0])));

                double viewScale = 1.0;
                if (view is View)
                    viewScale = (view as View).Attributes.Scale;

                distance = distance / viewScale;
                T3D.Vector vertical = new T3D.Vector(new T3D.Point(0, 0));
                StraightDimensionSet.StraightDimensionSetAttributes straightDimensionAttributes = new StraightDimensionSet.StraightDimensionSetAttributes(DrawingSettings.DimensionAttributes);
                StraightDimension straightDimension = new StraightDimension(view, point1, point2, vertical, distance, straightDimensionAttributes);
                straightDimension.Insert();
                straightDimension.Attributes.LoadAttributes(DrawingSettings.DimensionAttributes);
                straightDimension.Modify();
            }
        }
        public static void CreateRadiusDimension(View view, double radius, int num, T3D.Point origin)
        {
            if (view != null)
            {
                double angle1 = 0.0;
                double angle2 = 0.0;
                double angle3 = 0.0;
                if (num == 1)
                {
                    angle1 = 45;
                    angle2 = 0;
                    angle3 = 90;
                }
                else if (num == 2)
                {
                    angle1 = 45;
                    angle2 = 90;
                    angle3 = 135;
                }
                else
                {
                    angle1 = 135;
                    angle2 = 180;
                    angle3 = 215;
                }
                T3D.Point firstPoint = ShiftRadiallyPoint(origin, radius, angle1);
                T3D.Point secondPoint = ShiftRadiallyPoint(origin, radius, angle2);
                T3D.Point thirdPoint = ShiftRadiallyPoint(origin, radius, angle3);

                RadiusDimension radiusDimension = new RadiusDimension(view, thirdPoint, firstPoint, secondPoint, 2);
                radiusDimension.Insert();
            }
        }

        public static void CreateRadialDimensionsforPlatform(View view, double angle1, double angle2, double angle3, double radius, T3D.Point origin)
        {
            if (view != null)
            {
                T3D.Point firstPoint = ShiftRadiallyPoint(origin, radius, angle1);
                T3D.Point secondPoint = ShiftRadiallyPoint(origin, radius, angle2);
                T3D.Point thirdPoint = ShiftRadiallyPoint(origin, radius, angle3);

                RadiusDimension radiusDimension = new RadiusDimension(view, thirdPoint, firstPoint, secondPoint, 2);
                radiusDimension.Insert();
            }
        }

        public static void CreateAngleDimension(View view, T3D.Point point1, T3D.Point point2, T3D.Point point3)
        {
            if (view != null)
            {
                Tekla.Structures.Drawing.PointList pointList = new Tekla.Structures.Drawing.PointList();
                double distance = 25;
                AngleDimension angleDimension = new AngleDimension(view, point1, point2, point3, distance);
                angleDimension.Attributes.Type = AngleTypes.AngleAtVertex;
                angleDimension.Insert();
            }
        }

        public static void CreateLevelMark(ViewBase view, T3D.Point basePoint, T3D.Point insertionPoint)
        {
            if (view != null)
            {
                DrawingHandler MyDrawingHandler = new DrawingHandler();
                var drawing1 = MyDrawingHandler.GetActiveDrawing();
                var sheet1 = drawing1.GetSheet();
                var allviews = sheet1.GetAllViews();
                foreach (var item in allviews)
                {
                    view = item as ViewBase;
                    LevelMark levelMark = new LevelMark(view, insertionPoint, basePoint);
                    levelMark.Attributes.LoadAttributes(DrawingSettings.LevelMarkAttributes);
                    bool res = levelMark.Insert();
                }
            }
        }

        public static T3D.Point ShiftRadiallyPoint(T3D.Point pt, double dist, double angle, int plane = 2)
        {
            T3D.Point shiftedPt;
            switch (plane)
            {
                case 0:
                    //shiftedPt = new ContourPoint(new T3D.Point(pt.X + dist * Math.Cos(angle * Math.PI / 180), pt.Y + dist * Math.Sin(angle * Math.PI / 180), pt.Z), null);
                    throw new Exception("Not implemented");
                //break;
                case 1:
                    //shiftedPt = new ContourPoint(new T3D.Point(pt.X + dist * Math.Cos(angle * Math.PI / 180), pt.Y + dist * Math.Sin(angle * Math.PI / 180), pt.Z), null);
                    throw new Exception("Not implemented");
                //break;
                default:
                    shiftedPt = new T3D.Point(pt.X + dist * Math.Cos(angle * Math.PI / 180), pt.Y + dist * Math.Sin(angle * Math.PI / 180), pt.Z);
                    break;
            }
            return shiftedPt;
        }
        public static T3D.Point ShiftTangentiallyPoint(T3D.Point pt, double dist, double angle, int side, int plane = 2)
        {
            T3D.Point shiftedPt;
            switch (plane)
            {
                case 0:
                    //shiftedPt = new ContourPoint(new T3D.Point(pt.X + dist * Math.Cos(angle * Math.PI / 180), pt.Y + dist * Math.Sin(angle * Math.PI / 180), pt.Z), null);
                    throw new Exception("Not implemented");
                //break;
                case 1:
                    //shiftedPt = new ContourPoint(new T3D.Point(pt.X + dist * Math.Cos(angle * Math.PI / 180), pt.Y + dist * Math.Sin(angle * Math.PI / 180), pt.Z), null);
                    throw new Exception("Not implemented");
                //break;
                default:
                    switch (side)
                    {
                        case 1:
                            shiftedPt = new T3D.Point(pt.X, pt.Y, pt.Z + dist);
                            break;
                        case 2:
                            shiftedPt = new T3D.Point(pt.X - dist * Math.Cos((90 - angle) * Math.PI / 180), pt.Y + dist * Math.Sin((90 - angle) * Math.PI / 180), pt.Z);
                            break;
                        case 3:
                            shiftedPt = new T3D.Point(pt.X, pt.Y, pt.Z - dist);
                            break;
                        default:
                            shiftedPt = new T3D.Point(pt.X + dist * Math.Cos((90 - angle) * Math.PI / 180), pt.Y - dist * Math.Sin((90 - angle) * Math.PI / 180), pt.Z);
                            break;
                    }
                    break;
            }
            return shiftedPt;
        }

        public static void GetPartPoints(Tekla.Structures.Model.Model MyModel, ViewBase PartView, Tekla.Structures.Drawing.ModelObject modelObject, out T3D.Point PartMiddleStart, out T3D.Point PartMiddleEnd, out T3D.Point PartCenterPoint)
        {
            Tekla.Structures.Model.ModelObject modelPart = GetModelObjectFromDrawingModelObject(MyModel, modelObject);
            GetModelObjectStartAndEndPoint(modelPart, (View)PartView, out PartMiddleStart, out PartMiddleEnd);
            PartCenterPoint = GetInsertionPoint(PartMiddleStart, PartMiddleEnd);
        }

        public static Tekla.Structures.Model.ModelObject GetModelObjectFromDrawingModelObject(Tekla.Structures.Model.Model MyModel, Tekla.Structures.Drawing.ModelObject PartOfMark)
        {
            Tekla.Structures.Model.ModelObject modelObject = MyModel.SelectModelObject(PartOfMark.ModelIdentifier);

            Tekla.Structures.Model.Part modelPart = (Tekla.Structures.Model.Part)modelObject;

            return modelPart;
        }

        public static void GetModelObjectStartAndEndPoint(Tekla.Structures.Model.ModelObject modelObject, View PartView, out T3D.Point PartStartPoint, out T3D.Point PartEndPoint)
        {
            Tekla.Structures.Model.Part modelPart = (Tekla.Structures.Model.Part)modelObject;

            PartStartPoint = modelPart.GetSolid().MinimumPoint;
            PartEndPoint = modelPart.GetSolid().MaximumPoint;
            //  PartView.
            T3D.Matrix convMatrix = T3D.MatrixFactory.ToCoordinateSystem(PartView.DisplayCoordinateSystem);
            // T3D.Matrix convMatrix = T3D.MatrixFactory.ToCoordinateSystem(PartView.ViewCoordinateSystem);
            PartStartPoint = convMatrix.Transform(PartStartPoint);
            PartEndPoint = convMatrix.Transform(PartEndPoint);
        }

        public static T3D.Point GetInsertionPoint(T3D.Point PartStartPoint, T3D.Point PartEndPoint)
        {
            T3D.Point MinPoint = PartStartPoint;
            T3D.Point MaxPoint = PartEndPoint;
            T3D.Point InsertionPoint = new T3D.Point((MaxPoint.X + MinPoint.X) * 0.5, (MaxPoint.Y + MinPoint.Y) * 0.5, (MaxPoint.Z + MinPoint.Z) * 0.5);
            InsertionPoint.Z = 0;
            return InsertionPoint;
        }

        //public static void InsertProfileAnnotations(View view, string partName, AnnotationsAttributes annotationsAttributes)
        //{
        //    if (view != null)
        //    {
        //        DrawingHandler MyDrawingHandler = new DrawingHandler();

        //        Tekla.Structures.Drawing.Drawing CurrentDrawing = MyDrawingHandler.GetActiveDrawing();
        //        DrawingObjectEnumerator allParts = view.GetAllObjects(typeof(Tekla.Structures.Drawing.Part));
        //        T3D.Point point = new T3D.Point();
        //        int count = 0;
        //        // DrawingObjectEnumerator allParts = CurrentDrawing.GetSheet().GetAllObjects(typeof(Tekla.Structures.Drawing.Part));
        //        while (allParts.MoveNext())
        //        {

        //            // if (allParts.Current is Tekla.Structures.Drawing.Part)
        //            //{
        //            Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;

        //            if (modelObject == null) continue;
        //            T3D.Point PartMiddleStart = null, PartMiddleEnd = null, PartCenterPoint = null;
        //            GetPartPoints(CAssembly.myModel, view, modelObject, out PartMiddleStart, out PartMiddleEnd, out PartCenterPoint);
        //            Mark Mark = new Mark(modelObject);

        //            Mark.Attributes.Content.Clear();
        //            // Mark.Attributes.LoadAttributes("standard");
        //            var dwgPart = allParts.Current as Tekla.Structures.Drawing.Part;
        //            if (dwgPart == null) continue;

        //            var mdlPart = new Tekla.Structures.Model.Model().SelectModelObject(dwgPart.ModelIdentifier) as Tekla.Structures.Model.Part;

        //            if (mdlPart == null) continue;
        //            if (mdlPart.Name.ToString() == partName)
        //            {
        //                string add = mdlPart.Profile.ToString();
        //                if (annotationsAttributes.Profile == true)
        //                {
        //                    Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.Profile()));

        //                }
        //                if (annotationsAttributes.Material == true)
        //                {
        //                    Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.Material()));
        //                }
        //                if (annotationsAttributes.Part_Position == true)
        //                {
        //                    Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.PartPosition()));
        //                }
        //                //Mark.Placing = new AlongLinePlacing(PartMiddleStart, PartMiddleEnd);
        //                point.X = PartCenterPoint.X + 1000;
        //                point.Y = PartCenterPoint.Y;
        //                //Mark.InsertionPoint = point;
        //                Mark.InsertionPoint = PartCenterPoint;
        //                Mark.Attributes.Frame.Type = FrameTypes.Line;
        //                Mark.Insert();
        //                // Mark.Attributes.Content.Clear();
        //                if (annotationsAttributes.Single == true)
        //                {
        //                    break;
        //                }
        //            }

        //            //}
        //            count++;
        //        }
        //    }

        //}
        //public static void InsertPartPositionInMSD(View view, AnnotationsAttributes annotationsAttributes)
        //{
        //    DrawingHandler MyDrawingHandler = new DrawingHandler();

        //    Tekla.Structures.Drawing.Drawing CurrentDrawing = MyDrawingHandler.GetActiveDrawing();
        //    DrawingObjectEnumerator allParts = view.GetModelObjects();
        //    T3D.Point point = new T3D.Point();

        //    //DrawingObjectEnumerator allParts = CurrentDrawing.GetSheet().GetAllObjects(typeof(Tekla.Structures.Drawing.Part));
        //    while (allParts.MoveNext())
        //    {

        //        if (allParts.Current is Tekla.Structures.Drawing.Part)
        //        {
        //            Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;

        //            if (modelObject == null) continue;
        //            T3D.Point PartMiddleStart = null, PartMiddleEnd = null, PartCenterPoint = null;
        //            DrawingUtils.GetPartPoints(CAssembly.myModel, view, modelObject, out PartMiddleStart, out PartMiddleEnd, out PartCenterPoint);
        //            Mark Mark = new Mark(modelObject);

        //            Mark.Attributes.Content.Clear();
        //            // Mark.Attributes.LoadAttributes("standard");
        //            var dwgPart = allParts.Current as Tekla.Structures.Drawing.Part;
        //            if (dwgPart == null) continue;

        //            var mdlPart = new Tekla.Structures.Model.Model().SelectModelObject(dwgPart.ModelIdentifier) as Tekla.Structures.Model.Part;

        //            if (mdlPart == null) continue;

        //            if (annotationsAttributes.Part_Position == true)
        //            {
        //                Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.PartMarkPropertyElementTypes.PartPosition()));
        //            }
        //            //Mark.Placing = new AlongLinePlacing(PartMiddleStart, PartMiddleEnd);
        //            point.X = PartCenterPoint.X + 1000;
        //            point.Y = PartCenterPoint.Y;
        //            //Mark.InsertionPoint = point;
        //            Mark.InsertionPoint = PartCenterPoint;
        //            Mark.Attributes.Frame.Type = FrameTypes.Line;
        //            Mark.Insert();
        //            // Mark.Attributes.Content.Clear();
        //            if (annotationsAttributes.Single == true)
        //            {
        //                break;
        //            }
        //        }

        //    }




        //}
        //public static void InsertBoltAnnotations(View view, AnnotationsAttributes annotationsAttributes)
        //{
        //    if (view != null)
        //    {
        //        DrawingHandler MyDrawingHandler = new DrawingHandler();

        //        Tekla.Structures.Drawing.Drawing CurrentDrawing = MyDrawingHandler.GetActiveDrawing();
        //        DrawingObjectEnumerator allParts = view.GetModelObjects();

        //        while (allParts.MoveNext())
        //        {
        //            Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;

        //            if (modelObject == null) continue;

        //            Mark Mark = new Mark(modelObject);
        //            Mark.Attributes.LoadAttributes(DrawingSettings.BoltAttributes);
        //            if (allParts.Current is Tekla.Structures.Drawing.Bolt)
        //            {
        //                var bolt = allParts.Current as Tekla.Structures.Drawing.Bolt;

        //                Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.NumberOfBolts()));
        //                Mark.Attributes.Content.Add(new PropertyElement(PropertyElement.PropertyElementType.BoltMarkPropertyElementTypes.Size()));
        //                Mark.InsertionPoint = new T3D.Point(100, 100);
        //                Mark.Insert();
        //                if (annotationsAttributes.Single == true)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
        public static void InsertStraightDimensionSet(ViewBase view, Tekla.Structures.Drawing.PointList pointList, char vector = 'h')
        {
            if (view != null)
            {
                DrawingHandler dh = new DrawingHandler();
                if (vector == 'h')
                {
                    StraightDimensionSet.StraightDimensionSetAttributes straightDimensionAttributes = new StraightDimensionSet.StraightDimensionSetAttributes();
                    straightDimensionAttributes.LoadAttributes(DrawingSettings.DimensionAttributes);
                    StraightDimensionSet horDimension = new StraightDimensionSetHandler().CreateDimensionSet(view, pointList, new Tekla.Structures.Geometry3d.Vector(0, 100, 0), 100, straightDimensionAttributes);
                }
                else
                {
                    StraightDimensionSet.StraightDimensionSetAttributes straightDimensionAttributes = new StraightDimensionSet.StraightDimensionSetAttributes();
                    straightDimensionAttributes.LoadAttributes(DrawingSettings.DimensionAttributes);
                    StraightDimensionSet verDimension = new StraightDimensionSetHandler().CreateDimensionSet(view, pointList, new Tekla.Structures.Geometry3d.Vector(100, 0, 0), 100, straightDimensionAttributes);
                }
                dh.GetActiveDrawing().CommitChanges();
            }
        }

        //public static void InsertInstrumentTags(View view, string tag_name, double orientation, double d)
        //{
        //    if (view != null)
        //    {
        //        DrawingHandler MyDrawingHandler = new DrawingHandler();
        //        T3D.Point origin = new T3D.Point(0, 0);
        //        Tekla.Structures.Drawing.Drawing CurrentDrawing = MyDrawingHandler.GetActiveDrawing();
        //        DrawingObjectEnumerator allParts = view.GetModelObjects();
        //        //DrawingObjectEnumerator allParts = CurrentDrawing.GetSheet().GetAllObjects(typeof(Tekla.Structures.Drawing.Part));

        //        while (allParts.MoveNext())
        //        {

        //            if (allParts.Current is Tekla.Structures.Drawing.Part)
        //            {
        //                Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;

        //                if (modelObject == null) continue;
        //                T3D.Point PartMiddleStart = null, PartMiddleEnd = null, PartCenterPoint = null;

        //                DrawingUtils.GetPartPoints(CAssembly.myModel, view, modelObject, out PartMiddleStart, out PartMiddleEnd, out PartCenterPoint);
        //                // T3D.Point insertionPoint = new T3D.Point(PartCenterPoint.X + distance, PartCenterPoint.Y);
        //                Mark Mark = new Mark(modelObject);

        //                Mark.Attributes.Content.Clear();
        //                var dwgPart = allParts.Current as Tekla.Structures.Drawing.Part;
        //                if (dwgPart == null) continue;
        //                var mdlPart = new Tekla.Structures.Model.Model().SelectModelObject(dwgPart.ModelIdentifier) as Tekla.Structures.Model.Part;
        //                if (mdlPart == null) continue;
        //                if (mdlPart.Name.ToString() == tag_name)
        //                {

        //                    Mark.Attributes.Content.Add(new Tekla.Structures.Drawing.TextElement(tag_name.Substring(AppStrings.Instrument_Nozzle.Length).ToString()));
        //                    Mark.InsertionPoint = ShiftRadiallyPoint(origin, d, orientation);
        //                    Mark.Attributes.Frame.Type = FrameTypes.Circle;
        //                    Mark.Insert();
        //                    Mark.Attributes.Content.Clear();
        //                    //if (annotationsAttributes.Single == true)
        //                    //{
        //                    //  break;
        //                    //}

        //                    break;
        //                }

        //            }
        //        }
        //    }
        //}

        //public static void InsertText(ViewBase view, T3D.Point insertion_Point, string text)
        //{
        //    if (view != null)
        //    {
        //        DrawingHandler MyDrawingHandler = new DrawingHandler();
        //        var drawing1 = MyDrawingHandler.GetActiveDrawing();
        //        var sheet1 = drawing1.GetSheet();
        //        var allviews = sheet1.GetAllViews();
        //        foreach (var item in allviews)
        //        {
        //            view = item as ViewBase;
        //            Text text1 = new Text(view, insertion_Point, "PLT. " + text + " THK.");
        //            text1.Attributes.Angle = 90;
        //            text1.Insert();
        //        }
        //    }
        //}

        //public static double GetTaperedSectionRadius(double bottomStackRadius, double stackRadius, double stackTotalHeight, double elevation, double stackSegHeight)
        //{
        //    double distBetTopBottomRad = bottomStackRadius - stackRadius;
        //    double base1 = distBetTopBottomRad / 2;
        //    double height1 = stackSegHeight;
        //    double height2 = Math.Abs(stackTotalHeight - elevation);
        //    double base2 = (base1 * height2) / height1;
        //    double mainDist = stackRadius + (2 * base2);
        //    // double mainWidthForTopRing = mainDist + upperRingWidth;    // Radius paticular elevation
        //    // double stackRadiusWithThicknessUp = mainDist + stackThinkness;

        //    return mainDist;
        //}

        //public static Tekla.Structures.Geometry3d.CoordinateSystem getSectionElevation(DrawingObjectEnumerator allParts)
        //{
        //    CoordinateSystem coordinateSystem = new CoordinateSystem();
        //    while (allParts.MoveNext())
        //    {
        //        if (allParts.Current is Tekla.Structures.Drawing.Part)
        //        {
        //            Tekla.Structures.Drawing.ModelObject modelObject = (Tekla.Structures.Drawing.ModelObject)allParts.Current;
        //            if (modelObject == null) continue;
        //            var dwgPart = allParts.Current as Tekla.Structures.Drawing.Part;
        //            if (dwgPart == null) continue;
        //            var mdlPart = new Tekla.Structures.Model.Model().SelectModelObject(dwgPart.ModelIdentifier) as Tekla.Structures.Model.Part;
        //            if (mdlPart == null) continue;
        //            coordinateSystem = mdlPart.GetCoordinateSystem();
        //            break;
        //        }
        //    }
        //    return coordinateSystem;
        //}

        //public static void InsertTemplate(ViewBase view, double angle)
        //{
        //    if (view != null)
        //    {
        //        DrawingHandler MyDrawingHandler = new DrawingHandler();
        //        var drawing1 = MyDrawingHandler.GetActiveDrawing();
        //        var sheet1 = drawing1.GetSheet();
        //        var allviews = sheet1.GetAllViews();
        //        ModelInfo modelInfo = CAssembly.myModel.GetInfo();
        //        string path = modelInfo.ModelPath;
        //        if (angle == 0)
        //            path = CheckFileExist(path + "\\North- Right.dwg");
        //        else if (angle == 90)
        //            path = CheckFileExist(path + "\\North- Up.dwg");
        //        else if (angle == 180)
        //            path = CheckFileExist(path + "\\North- Left.dwg");
        //        else
        //            path = CheckFileExist(path + "\\North- Down.dwg");
        //        if (path != null)
        //        {
        //            foreach (var item in allviews)
        //            {
        //                view = item as ViewBase;
        //                DwgObject dwgObject = new DwgObject(sheet1, new T3D.Point(5, 820), path);
        //                dwgObject.Attributes.XScale = 0.5;
        //                dwgObject.Attributes.YScale = 0.5;
        //                dwgObject.Insert();
        //            }
        //        }
        //    }
        //}

        public static void CheckRotation(View view, double angle)
        {
            if (view != null)
            {
                if (angle >= 45 && angle < (90 + 45))
                    view.RotateViewOnDrawingPlane(-90);
                else if (angle >= (90 + 45) && angle < (180 + 45))
                    view.RotateViewOnDrawingPlane(180);
                else if (angle >= (180 + 45) && angle < (270 + 45))
                    view.RotateViewOnDrawingPlane(90);
            }
        }

        //public static string CheckFileExist(string path)
        //{
        //    string checkPath = null;
        //    if (File.Exists(path))
        //        checkPath = path;
        //    return checkPath;
        //}
    }
}

