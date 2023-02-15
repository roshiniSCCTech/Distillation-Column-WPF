using HelperLibrary;
using Newtonsoft.Json.Linq;
using Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.CatalogInternal;
using Tekla.Structures.Model;
using Tekla.Structures.ModelInternal;

namespace DistillationColumn
{
    internal class Duct
    {
        Globals _global;
        TeklaModelling _tModel;

        List<List<double>> ductlist;
        List<ContourPoint> pointlist;


        Beam Lbeam = new Beam();
        Beam bolt = new Beam();

        double elevation, duct_Orientation_Angle, duct_Width, duct_Height, doubler_plate_Width, doubler_Plate_Thickness, neckplate_Thickness;
        double flange_PlateDistanceFrom_Stack;
        double flange_PlateWidth;
        double flange_PlateThickness;
        double spacing_Horizontal_StiffnerPlate;
        double spacing_VerticalStiffnerPlate;
        double stiffner_PlateThickness;
        double stackRadiusBot;
        double stackRadiusTop;
        double stackRadiusMid;

        ContourPoint p1 = new ContourPoint();
        ContourPoint p2 = new ContourPoint();
        ContourPoint p3 = new ContourPoint();
        ContourPoint p4 = new ContourPoint();
        ContourPoint p5 = new ContourPoint();
        ContourPoint p6 = new ContourPoint();
        public Duct(Globals global, TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;

            ductlist = new List<List<double>>();
            pointlist = new List<ContourPoint>();

            SetDuctData();
            //CreateDuct();
            DuctTest();
        }
        public void SetDuctData()
        {
            List<JToken> _ductlist = _global.JData["Duct"].ToList();
            foreach (JToken duct in _ductlist)
            {
                duct_Orientation_Angle = (float)duct["duct_orientation_angle"];
                elevation = (float)duct["elevation"];
                duct_Width = (float)duct["duct_width"]; ;
                duct_Height = (float)duct["duct_height"];
                doubler_plate_Width = (float)duct["doubler_plate_width"];
                doubler_Plate_Thickness = (float)duct["doubler_plate_thickness"];
                neckplate_Thickness = (float)duct["neck_plate_thickness"];
                flange_PlateDistanceFrom_Stack = (float)duct["flange_plate_distance_from_stack"];
                flange_PlateWidth = (float)duct["flange_plate_width"];
                flange_PlateThickness = (float)duct["flange_plate_thickness"];
                spacing_Horizontal_StiffnerPlate = (float)duct["spacing_horizontal_stiffner_plate"];
                spacing_VerticalStiffnerPlate = (float)duct["Spacing_vertical_stiffner_plate"];
                stiffner_PlateThickness = (float)duct["stiffner_plate_thickness"];
                ductlist.Add(new List<double> { duct_Orientation_Angle, elevation, duct_Width, duct_Height, doubler_plate_Width, doubler_Plate_Thickness, neckplate_Thickness, flange_PlateDistanceFrom_Stack, flange_PlateWidth, flange_PlateThickness, spacing_Horizontal_StiffnerPlate, spacing_VerticalStiffnerPlate, stiffner_PlateThickness });
            }

        }

        public void CreateDuct()
        {
            foreach (List<double> duct in ductlist)
            {
                duct_Orientation_Angle = duct[0] * Math.PI / 180;
                elevation = duct[1];
                duct_Width = duct[2];
                duct_Height = duct[3];
                doubler_plate_Width = duct[4];
                doubler_Plate_Thickness = duct[5];
                neckplate_Thickness = duct[6];
                flange_PlateDistanceFrom_Stack = duct[7];
                flange_PlateWidth = duct[8];
                flange_PlateThickness = duct[9];
                spacing_Horizontal_StiffnerPlate = duct[10];
                spacing_VerticalStiffnerPlate = duct[11];
                stiffner_PlateThickness = duct[12];
            }
            stackRadiusMid = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
            stackRadiusBot = _tModel.GetRadiusAtElevation(elevation - (duct_Height / 2), _global.StackSegList, true);
            stackRadiusTop = _tModel.GetRadiusAtElevation(elevation + (duct_Height / 2), _global.StackSegList, true);

            CreateDoublerPlate(duct_Orientation_Angle, duct_Height, duct_Width, doubler_plate_Width, doubler_Plate_Thickness, flange_PlateDistanceFrom_Stack);
            NeckPlate(duct_Orientation_Angle, duct_Height, duct_Width, neckplate_Thickness, flange_PlateWidth, flange_PlateDistanceFrom_Stack, flange_PlateThickness);
            CreateVerticalStiffnerPlate(elevation, duct_Orientation_Angle, duct_Height, doubler_plate_Width, flange_PlateDistanceFrom_Stack, stiffner_PlateThickness, doubler_Plate_Thickness);
            CreateHorizontalStiffnerPlate(elevation, duct_Orientation_Angle, duct_Width, duct_Height, doubler_plate_Width, flange_PlateDistanceFrom_Stack, stiffner_PlateThickness, doubler_Plate_Thickness);
        }
        public void CreateDoublerPlate(double orientationAngle, double ductHeight, double ductWidth, double doublerplateWidth, double doublerPlateThickness, double flangePlateDistanceFromStack)
        {

            ContourPoint origin = new ContourPoint(_global.Origin, null);
            ContourPoint centrePoint = _tModel.ShiftVertically(origin, elevation + (ductHeight / 2) + (doublerplateWidth));
            ContourPoint ductCentrePoint = _tModel.ShiftHorizontallyRad(centrePoint, stackRadiusTop, 1, orientationAngle);

            double theta = Math.Asin((ductWidth / 2) / stackRadiusTop); // Radians

            // Left Vertical Doubler PLate
            p1 = _tModel.ShiftAlongCircumferenceRad(ductCentrePoint, theta, 1);
            p2 = _tModel.ShiftAlongCircumferenceRad(p1, doublerplateWidth / 2, 2);
            p2.Chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT);
            p3 = _tModel.ShiftAlongCircumferenceRad(p1, doublerplateWidth, 2);

            pointlist.Add(p1);
            pointlist.Add(p2);
            pointlist.Add(p3);

            _global.Position.Depth = Position.DepthEnum.BEHIND;
            _global.Position.Rotation = Position.RotationEnum.TOP;
            _global.Position.Plane = Position.PlaneEnum.RIGHT;

            _tModel.CreatePolyBeam(pointlist, "PL" + doublerPlateThickness + "*" + (ductHeight + 2 * doublerplateWidth), "IS2062", "3", _global.Position);
            pointlist.Clear();

            // Right Vertical Doubler PLate
            p4 = _tModel.ShiftAlongCircumferenceRad(ductCentrePoint, -theta, 1);
            p5 = _tModel.ShiftAlongCircumferenceRad(p4, -(doublerplateWidth / 2), 2);
            p5.Chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT);
            p6 = _tModel.ShiftAlongCircumferenceRad(p4, -doublerplateWidth, 2);

            pointlist.Add(p4);
            pointlist.Add(p5);
            pointlist.Add(p6);

            _global.Position.Plane = Position.PlaneEnum.LEFT;

            _tModel.CreatePolyBeam(pointlist, "PL" + doublerPlateThickness + "*" + (ductHeight + 2 * doublerplateWidth), "IS2062", "3", _global.Position);
            pointlist.Clear();

            //Top Doubler Beam
            ductCentrePoint.Chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT);

            pointlist.Add(p1);
            pointlist.Add(ductCentrePoint);
            pointlist.Add(p4);

            _tModel.CreatePolyBeam(pointlist, "PL" + doublerPlateThickness + "*" + doublerplateWidth, "IS2062", "3", _global.Position);
            pointlist.Clear();

            //Bottom Doubler Beam
            p2 = _tModel.ShiftVertically(p1, -(ductHeight + doublerplateWidth));
            p5 = _tModel.ShiftVertically(p4, -(ductHeight + doublerplateWidth));
            p6 = _tModel.ShiftVertically(ductCentrePoint, -(ductHeight + doublerplateWidth));

            pointlist.Add(p2);
            pointlist.Add(p6);
            pointlist.Add(p5);


            _tModel.CreatePolyBeam(pointlist, "PL" + doublerPlateThickness + "*" + doublerplateWidth, "IS2062", "3", _global.Position);
            pointlist.Clear();

        }

        public void NeckPlate(double orientationAngle, double ductHeight, double ductWidth, double neckPlateThickness, double flangePlateWidth, double flangePlateDistanceFromStack, double flangePlateThickness)
        {

            ContourPoint origin = new ContourPoint(_global.Origin, null);
            ContourPoint centrePoint = _tModel.ShiftVertically(origin, elevation + (ductHeight / 2));

            double theta = Math.Asin((ductWidth / 2) / stackRadiusTop);
            double distance = flangePlateDistanceFromStack - (Math.Cos(theta) * stackRadiusTop);



            // Top Flange Plate
            p2 = _tModel.ShiftHorizontallyRad(centrePoint, flangePlateDistanceFromStack, 1, orientationAngle);
            p1 = _tModel.ShiftHorizontallyRad(p2, ductWidth / 2, 4);
            p3 = _tModel.ShiftHorizontallyRad(p2, ductWidth / 2, 2);

            _global.Position.Rotation = Position.RotationEnum.FRONT;

            Beam topFlangePlate = _tModel.CreateBeam(p1, p3, "PL" + neckPlateThickness + "*" + distance, "IS2062", "2", _global.Position);
            Lbeam.Position.Plane = Position.PlaneEnum.RIGHT;
            Lbeam.Position.Depth = Position.DepthEnum.FRONT;
            Beam topLBeam = CreateLBeams(p1, p3, flangePlateThickness, 0);

            bolt.Position.Rotation = Position.RotationEnum.TOP;

            CreateBolts(topFlangePlate, topLBeam, p1, p3, ductWidth, ductHeight, 0);

            //Right Flange Plate

            _global.Position.Rotation = Position.RotationEnum.TOP;

            ContourPoint p4 = _tModel.ShiftVertically(p1, -(ductHeight));

            ContourPoint rightTop = _tModel.ShiftVertically(p1, -neckPlateThickness);
            ContourPoint rightBot = _tModel.ShiftVertically(p4, neckPlateThickness);

            Beam rightFlangePlate = _tModel.CreateBeam(rightTop, rightBot, "PL" + neckPlateThickness + "*" + distance, "IS2062", "2", _global.Position);
            Lbeam.Position.Plane = Position.PlaneEnum.RIGHT;
            Beam rightLBeam = CreateLBeams(p1, p4, flangePlateThickness, 1);
            bolt.Position.Rotation = Position.RotationEnum.FRONT;
            CreateBolts(rightFlangePlate, rightLBeam, p1, p4, ductWidth, ductHeight, 1);

            //Left Flange Plate
            ContourPoint p5 = _tModel.ShiftVertically(p3, -(ductHeight));

            ContourPoint leftTop = _tModel.ShiftVertically(p3, -neckPlateThickness);
            ContourPoint leftBot = _tModel.ShiftVertically(p5, neckPlateThickness);

            _global.Position.Plane = Position.PlaneEnum.RIGHT;

            Beam leftFlangePlate = _tModel.CreateBeam(leftTop, leftBot, "PL" + neckPlateThickness + "*" + distance, "IS2062", "2", _global.Position);
            Lbeam.Position.Plane = Position.PlaneEnum.LEFT;
            Lbeam.Position.Rotation = Position.RotationEnum.FRONT;
            Beam leftLBeam = CreateLBeams(p3, p5, flangePlateThickness, 1);
            bolt.Position.Rotation = Position.RotationEnum.FRONT;
            CreateBolts(leftFlangePlate, leftLBeam, p3, p5, ductWidth, ductHeight, 2);

            //Bot Flange Plate
            _global.Position.Rotation = Position.RotationEnum.BACK;
            _global.Position.Plane = Position.PlaneEnum.RIGHT;
            _global.Position.Depth = Position.DepthEnum.FRONT;

            Beam botFlangePlate = _tModel.CreateBeam(p5, p4, "PL" + neckPlateThickness + "*" + distance, "IS2062", "2", _global.Position);
            Lbeam.Position.Plane = Position.PlaneEnum.RIGHT;
            Lbeam.Position.Rotation = Position.RotationEnum.BACK;
            Lbeam.Position.Depth = Position.DepthEnum.BEHIND;
            Beam botLBeam = CreateLBeams(p4, p5, flangePlateThickness, 0);
            bolt.Position.Rotation = Position.RotationEnum.TOP;
            CreateBolts(botFlangePlate, botLBeam, p4, p5, ductWidth, ductHeight, 3);

            //Top NeckPLate
            _global.Position.Rotation = Position.RotationEnum.TOP;
            _global.Position.Depth = Position.DepthEnum.FRONT;
            FlangePlate(p1, p3, flangePlateThickness, flangePlateWidth);
            //Bot NeckPlate
            _global.Position.Plane = Position.PlaneEnum.LEFT;
            _global.Position.Depth = Position.DepthEnum.BEHIND;
            FlangePlate(p5, p4, flangePlateThickness, flangePlateWidth);
            _global.Position.Depth = Position.DepthEnum.FRONT;
            //CreateLBeams(p4, p5, neckPlateThckness);
            //CreateLBeams(p3, p5, neckPlateThckness);
            //CreateLBeams(p4, p1, neckPlateThckness);
            //Left NeckPLate

            p1 = _tModel.ShiftVertically(p1, 150);
            p3 = _tModel.ShiftVertically(p3, 150);
            p4 = _tModel.ShiftVertically(p4, -150); // 150 from neckplate
            p5 = _tModel.ShiftVertically(p5, -150);

            _global.Position.Depth = Position.DepthEnum.FRONT;
            _global.Position.Rotation = Position.RotationEnum.FRONT;
            FlangePlate(p3, p5, flangePlateThickness, flangePlateWidth);
            //Right NeckPlate
            _global.Position.Plane = Position.PlaneEnum.RIGHT;
            FlangePlate(p1, p4, flangePlateThickness, flangePlateWidth);


        }

        public void FlangePlate(ContourPoint point1, ContourPoint point2, double flangePlateThickness, double flangePlateWidth)
        {
            _tModel.CreateBeam(point1, point2, "PL" + flangePlateThickness + "*" + flangePlateWidth, "IS2062", "6", _global.Position);
        }

        public Beam CreateLBeams(ContourPoint point1, ContourPoint point2, double neckPLateThickness, int i)
        {
            //Lbeam.Position.Depth = Position.DepthEnum.FRONT;
            point1 = _tModel.ShiftHorizontallyRad(point1, neckPLateThickness, 1);
            point2 = _tModel.ShiftHorizontallyRad(point2, neckPLateThickness, 1);
            if (i == 1)
            {
                point1 = _tModel.ShiftVertically(point1, 100);
                point2 = _tModel.ShiftVertically(point2, -100);
            }
            return _tModel.CreateBeam(point1, point2, "L100*100*" + neckplate_Thickness, "IS2062", "7", Lbeam.Position);
        }

        public void CreateVerticalStiffnerPlate(double elevation, double orientationAngle, double ductHeight, double doublerplateWidth, double flangePlateDistanceFromStack, double stiffnerPlateThickness, double doublerPlateThickness)
        {

            _global.Position.Depth = Position.DepthEnum.MIDDLE;

            ContourPoint origin = new ContourPoint(_global.Origin, null);
            ContourPoint centrePoint = _tModel.ShiftVertically(origin, elevation + (ductHeight / 2));
            p1 = _tModel.ShiftHorizontallyRad(centrePoint, stackRadiusTop + doublerPlateThickness, 1, orientationAngle);
            p2 = _tModel.ShiftHorizontallyRad(p1, flangePlateDistanceFromStack - stackRadiusTop - doublerPlateThickness, 1);
            p3 = _tModel.ShiftVertically(p1, doublerplateWidth);

            ContourPoint p4 = _tModel.ShiftHorizontallyRad(p3, 50, 1, orientationAngle);

            ContourPoint p5 = _tModel.ShiftVertically(p2, flange_PlateWidth);

            pointlist.Add(p1);
            pointlist.Add(p2);
            pointlist.Add(p5);
            pointlist.Add(p4);
            pointlist.Add(p3);

            _tModel.CreateContourPlate(pointlist, "PL" + stiffnerPlateThickness, "IS2062", "5", _global.Position);
            pointlist.Clear();

            //Bot Stiffner Plate
            centrePoint = _tModel.ShiftVertically(origin, elevation - (ductHeight / 2));
            p1 = _tModel.ShiftHorizontallyRad(centrePoint, stackRadiusBot + doublerPlateThickness, 1, orientationAngle);
            p2 = _tModel.ShiftHorizontallyRad(p1, flangePlateDistanceFromStack - stackRadiusBot - doublerPlateThickness, 1);
            p3 = _tModel.ShiftVertically(p1, -doublerplateWidth);

            p4 = _tModel.ShiftHorizontallyRad(p3, 50, 1, orientationAngle);

            p5 = _tModel.ShiftVertically(p2, -flange_PlateWidth);

            pointlist.Add(p1);
            pointlist.Add(p2);
            pointlist.Add(p5);
            pointlist.Add(p4);
            pointlist.Add(p3);

            _tModel.CreateContourPlate(pointlist, "PL" + stiffnerPlateThickness, "IS2062", "5", _global.Position);
            pointlist.Clear();

        }

        public void CreateHorizontalStiffnerPlate(double elevation, double orientationAngle, double ductWidth, double ductHeight, double doublerplateWidth, double flangePlateDistanceFromStack, double stiffnerPlateThickness, double doublerPlateThickness)
        {
            //Left side StiffnerPlate 
            ContourPoint origin = new ContourPoint(_global.Origin, null);
            ContourPoint centrePoint = _tModel.ShiftVertically(origin, elevation+(ductHeight/2)-200);

            double theta = Math.Asin((ductWidth / 2) / (stackRadiusMid + doublerPlateThickness));
            double phi = Math.Asin(((ductWidth / 2) + (doublerPlateThickness)) / (stackRadiusMid + doublerPlateThickness));

            p1 = _tModel.ShiftHorizontallyRad(centrePoint, doublerPlateThickness + stackRadiusBot, 1, orientationAngle);
            p1 = _tModel.ShiftAlongCircumferenceRad(p1, theta, 1);

            p2 = _tModel.ShiftHorizontallyRad(centrePoint, doublerPlateThickness + stackRadiusBot, 1, orientationAngle);
            p2 = _tModel.ShiftAlongCircumferenceRad(p2, phi, 1);
            p2 = _tModel.ShiftAlongCircumferenceRad(p2, doublerplateWidth - 25, 2); //25 from drawing


            ContourPoint p4 = _tModel.ShiftAlongCircumferenceRad(p1, (doublerplateWidth + doublerPlateThickness) / 2, 2);
            p4.Chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT);

            ContourPoint p5 = _tModel.ShiftHorizontallyRad(p2, 50, 1); //50 from drawing

            p3 = _tModel.ShiftHorizontallyRad(centrePoint, flangePlateDistanceFromStack, 1, orientationAngle);
            p3 = _tModel.ShiftHorizontallyRad(p3, ductWidth / 2, 2);

            ContourPoint p6 = _tModel.ShiftHorizontallyRad(p3, flange_PlateWidth, 2, orientationAngle);

            pointlist.Add(p1);
            pointlist.Add(p4);
            pointlist.Add(p2);
            pointlist.Add(p5);
            pointlist.Add(p6);
            pointlist.Add(p3);

            _tModel.CreateContourPlate(pointlist, "PL" + stiffnerPlateThickness, "IS2062", "7", _global.Position);
            pointlist.Clear();

            //Right side StiffnerPlate 

            p1 = _tModel.ShiftHorizontallyRad(centrePoint, doublerPlateThickness + stackRadiusBot, 1, orientationAngle);
            p1 = _tModel.ShiftAlongCircumferenceRad(p1, -theta, 1);

            p2 = _tModel.ShiftHorizontallyRad(centrePoint, doublerPlateThickness + stackRadiusBot, 1, orientationAngle);
            p2 = _tModel.ShiftAlongCircumferenceRad(p2, -phi, 1);
            p2 = _tModel.ShiftAlongCircumferenceRad(p2, -(doublerplateWidth - 25), 2);


            p4 = _tModel.ShiftAlongCircumferenceRad(p1, -(doublerplateWidth + doublerPlateThickness) / 2, 2);
            p4.Chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT);

            p5 = _tModel.ShiftHorizontallyRad(p2, 50, 1);

            p3 = _tModel.ShiftHorizontallyRad(centrePoint, flangePlateDistanceFromStack, 1, orientationAngle);
            p3 = _tModel.ShiftHorizontallyRad(p3, ductWidth / 2, 4);

            p6 = _tModel.ShiftHorizontallyRad(p3, flange_PlateWidth, 4, orientationAngle);

            pointlist.Add(p1);
            pointlist.Add(p4);
            pointlist.Add(p2);
            pointlist.Add(p5);
            pointlist.Add(p6);
            pointlist.Add(p3);

            _tModel.CreateContourPlate(pointlist, "PL" + stiffnerPlateThickness, "IS2062", "7", _global.Position);
            pointlist.Clear();
        }
        void CreateBolts(Part toBeBolted, Part toBoltTo, ContourPoint point1, ContourPoint point2, double ductWidth, double ductHeight ,int s)
        {
            BoltArray B = new BoltArray();
            B.PartToBeBolted = toBeBolted;
            B.PartToBoltTo = toBoltTo;
            B.FirstPosition = point1;
            B.SecondPosition = point2; 
            B.BoltSize = 20;
            B.Tolerance = 3.00;
            B.BoltStandard = "UNDEFINED_BOLT";
            B.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP;
            B.CutLength = 100; B.Length = 50;
            B.ExtraLength = 15;
            B.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_YES;
            B.Position.Rotation = bolt.Position.Rotation;
            B.Position.PlaneOffset = -60;
            B.Position.DepthOffset = 0; B.Bolt = true;
            B.Washer1 = true;
            B.Washer2 = true;
            B.Washer3 = true;
            B.Nut1 = true;
            B.Nut2 = true; B.Hole1 = true;
            B.Hole2 = true;
            B.Hole3 = true;
            B.Hole4 = true;
            B.Hole5 = true;
            if (s == 0)
            {
                int offset = Convert.ToInt32(spacing_Horizontal_StiffnerPlate / 2);
                int x = Convert.ToInt32((ductWidth - (2 * offset)) / spacing_Horizontal_StiffnerPlate);
                for (int i = 0; i < x; i++)
                {
                    B.AddBoltDistX(spacing_Horizontal_StiffnerPlate);
                }
                B.AddBoltDistY(0);
                B.StartPointOffset.Dx = offset;
                B.EndPointOffset.Dx = offset;
                B.StartPointOffset.Dz = 50;
                B.EndPointOffset.Dz = 50;
            }
            else if (s == 1)
            {
                int offset = Convert.ToInt32(spacing_VerticalStiffnerPlate / 2);
                int x = Convert.ToInt32((ductHeight - (2 * offset)) / spacing_VerticalStiffnerPlate);
                for (int i = 0; i <= x; i++)
                {
                    B.AddBoltDistX(spacing_VerticalStiffnerPlate);
                }
                B.AddBoltDistY(0);
                //B.StartPointOffset.Dx = offset;
                //B.EndPointOffset.Dx = -offset;
                B.StartPointOffset.Dz = 50;
                B.EndPointOffset.Dz = 50;
            }
            else if (s == 2)
            {
                int offset = Convert.ToInt32(spacing_VerticalStiffnerPlate / 2);
                int x = Convert.ToInt32((ductHeight - (2 * offset)) / spacing_VerticalStiffnerPlate);
                for (int i = 0; i <= x; i++)
                {
                    B.AddBoltDistX(spacing_VerticalStiffnerPlate);
                }
                B.AddBoltDistY(0);
                //B.StartPointOffset.Dx = offset;
                //B.EndPointOffset.Dx = offset;
                B.StartPointOffset.Dy = 125;
                B.EndPointOffset.Dy = 125;
                B.StartPointOffset.Dz = 50;
                B.EndPointOffset.Dz = 50;
            }
            else if (s == 3)
            {
                int offset = Convert.ToInt32(spacing_Horizontal_StiffnerPlate / 2);
                int x = Convert.ToInt32((ductWidth - (2 * offset)) / spacing_Horizontal_StiffnerPlate);
                for (int i = 0; i < x; i++)
                {
                    B.AddBoltDistX(spacing_Horizontal_StiffnerPlate);
                }
                B.AddBoltDistY(0);
                B.StartPointOffset.Dx = offset;
                B.EndPointOffset.Dx = offset;
                B.StartPointOffset.Dz = -50;
                B.EndPointOffset.Dz = -50;
            }

            if (!B.Insert())
                Console.WriteLine("BoltXYList Insert failed!");
            _tModel.Model.CommitChanges();
        }

        void DuctTest()
        {
            double radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
            ContourPoint origin = new ContourPoint(_tModel.ShiftVertically(_global.Origin, elevation), null);
            ContourPoint ePoint = new ContourPoint(_tModel.ShiftHorizontallyRad(origin,radius , 1,duct_Orientation_Angle*(Math.PI/180)), null);
            CustomPart CPart = new CustomPart();
            CPart.Name = "duct";
            CPart.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;
            CPart.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.MIDDLE;
            CPart.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.MIDDLE;
            CPart.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.TOP;
            CPart.SetInputPositions(origin, ePoint);
            CPart.SetAttribute("Doubler_Plate_Width", doubler_plate_Width);
            CPart.SetAttribute("Doubler_Plate_Thickness", doubler_Plate_Thickness);
            CPart.SetAttribute("Neck_Plate_Thickness" , neckplate_Thickness);
            CPart.SetAttribute("Flange_Plate_Width", flange_PlateWidth);
            CPart.SetAttribute("Flange_Plate_Thickness", flange_PlateThickness);
            CPart.SetAttribute("Stiffner_Plate_Thickness", stiffner_PlateThickness);
            CPart.SetAttribute("Radius", radius);
            CPart.SetAttribute("Duct_Width", duct_Width);
            CPart.SetAttribute("Duct_Height", duct_Height);
            CPart.SetAttribute("FP_Dist_Center", flange_PlateDistanceFrom_Stack);
            CPart.SetAttribute("SP_Horizontal_Spacing", spacing_Horizontal_StiffnerPlate);
            CPart.SetAttribute("SP_Vertical_Spacing", spacing_VerticalStiffnerPlate);
            CPart.Insert();
            _tModel.Model.CommitChanges();

            double distance1 = flange_PlateDistanceFrom_Stack - (radius + doubler_Plate_Thickness);
            double dist1 = elevation + (duct_Height / 2) - (neckplate_Thickness / 2);
            ContourPoint origin1 = new ContourPoint(_tModel.ShiftVertically(_global.Origin, dist1), null);
            origin1 = (_tModel.ShiftHorizontallyRad(origin1, radius + doubler_Plate_Thickness + (distance1), 1, duct_Orientation_Angle * (Math.PI / 180)));
            ContourPoint ePoint1 = new ContourPoint(_tModel.ShiftHorizontallyRad(origin1, radius + doubler_Plate_Thickness, 2), null);
            CustomPart CPart1 = new CustomPart();
            CPart1.Name = "DuctF";
            CPart1.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;
            CPart1.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
            CPart1.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.FRONT;
            CPart1.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.TOP;
            CPart1.SetInputPositions(origin1, ePoint1);
            CPart1.SetAttribute("P2", radius);
            CPart1.SetAttribute("P3", duct_Width);
            CPart1.SetAttribute("P10", flange_PlateDistanceFrom_Stack - radius - flange_PlateThickness);
            CPart1.SetAttribute("P11", spacing_Horizontal_StiffnerPlate);
            CPart1.SetAttribute("P19", flange_PlateWidth);
            CPart1.SetAttribute("P21", doubler_plate_Width);
            CPart1.Insert();
            _tModel.Model.CommitChanges();

            double distance2 = flange_PlateDistanceFrom_Stack - (radius + doubler_Plate_Thickness);
            double dist2 = elevation - (duct_Height / 2) + (neckplate_Thickness /2);
            ContourPoint origin2 = new ContourPoint(_tModel.ShiftVertically(_global.Origin, dist2), null);
            origin2 = (_tModel.ShiftHorizontallyRad(origin2, radius + doubler_Plate_Thickness + (distance2), 1, duct_Orientation_Angle * (Math.PI / 180)));
            ContourPoint ePoint2 = new ContourPoint(_tModel.ShiftHorizontallyRad(origin2, radius + doubler_Plate_Thickness, 4), null);
            CustomPart CPart2 = new CustomPart();
            CPart2.Name = "DuctF";
            CPart2.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;
            CPart2.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
            CPart2.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
            CPart2.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
            CPart2.SetInputPositions(origin2, ePoint2);
            CPart2.SetAttribute("P2", radius);
            CPart2.SetAttribute("P3", duct_Width);
            CPart2.SetAttribute("P10", flange_PlateDistanceFrom_Stack - radius - flange_PlateThickness);
            CPart2.SetAttribute("P11", spacing_Horizontal_StiffnerPlate);
            CPart2.SetAttribute("P19", flange_PlateWidth);
            CPart2.SetAttribute("P21", doubler_plate_Width);
            CPart2.Insert();
            _tModel.Model.CommitChanges();


            double theta = Math.Asin((duct_Width / 2) / radius);
            double distance = radius - (Math.Cos(theta) * radius);
            ContourPoint ePointStart = new ContourPoint(_tModel.ShiftHorizontallyRad(ePoint, duct_Width/2, 2, duct_Orientation_Angle * (Math.PI / 180)), null);
            ContourPoint ePointEnd = new ContourPoint(_tModel.ShiftHorizontallyRad(ePoint, duct_Width / 2, 4, duct_Orientation_Angle * (Math.PI / 180)), null);
            double r = (radius - distance);
            string profile = "RHS"+ r+"*"+ duct_Height + "*10";
            

            double ele1 = elevation + (duct_Height / 2) - (neckplate_Thickness / 2);
            double ele2 = elevation - (duct_Height / 2) + (neckplate_Thickness / 2);
            int no = _tModel.GetSegmentAtElevation(ele2, _global.StackSegList);
            int no1 = _tModel.GetSegmentAtElevation(ele1, _global.StackSegList);
            for (int i = no; i <= no1; i++)
            {
                Beam b = _tModel.CreateBeam(ePointStart, ePointEnd, profile, "IS2062", BooleanPart.BooleanOperativeClassName, _global.Position, "");
                _tModel.cutPart(b,_global.SegmentPartList[i]);
            }

        }
    }
}

