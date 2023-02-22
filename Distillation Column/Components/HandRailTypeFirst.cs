using HelperLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures;
using Render;
using Tekla.Structures.ModelInternal;
using Tekla.Structures.Datatype;
using static Tekla.Structures.ModelInternal.Operation;
using System.Collections;
using Tekla.Structures.Model.UI;

namespace DistillationColumn
{
    internal class HandRailTypeFirst
    {
        Globals _global;
        TeklaModelling _tModel;


        double ladderOrientation;
        double platformStartAngle;
        double platformEndAngle;
        double elevation;
        double platformLength;
        double distanceFromStack;
        double gratingOuterRadius;
        double gratingThickness;
        double extensionStartAngle;
        double extensionEndAngle;
        double extensionLength;
        double obstructionDistance;
        double startAngle;
        double endAngle;
        double radius;
        double length;
        double ladderWidth = 470;
        double theta;
        string MidRailProfile = "L50*50*6";
        string TopRailProfile = "L50*50*6";
        string VerticalPostProfile = "L50*50*6";
        List<double> arcLengthList = new List<double>();
        List<List<double>> handRailData;

        List<TSM.ContourPoint> _pointsList;

        public HandRailTypeFirst(Globals global, TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;
            handRailData = new List<List<double>>();
            SetHandrailrData();
            _pointsList = new List<TSM.ContourPoint>();


            //HandRailAtStartAndEnd();

            foreach (List<double> grating in handRailData)
            {
                platformStartAngle = grating[0];
                platformEndAngle = grating[1];
                elevation = grating[2];
                platformLength = grating[3];
                distanceFromStack = grating[5];
                gratingThickness = (float)grating[4];
                ladderOrientation = grating[6];
                extensionStartAngle = grating[7];
                extensionEndAngle = grating[8];
                extensionLength = grating[9];
                obstructionDistance = grating[10];
                HandrailAtLadderLocation();

                if (platformStartAngle != 0 && platformEndAngle != 360)
                {
                    if (platformStartAngle != ladderOrientation)
                    {
                        double l = platformLength;
                        if (platformStartAngle == extensionStartAngle)
                        {
                            l += extensionLength;
                        }
                        HandRailAtStartAndEnd(platformStartAngle, l, "start");
                    }
                    if (platformEndAngle != ladderOrientation)
                    {
                        double l = platformLength;
                        if (platformEndAngle == extensionEndAngle)
                        {
                            l += extensionLength;
                        }
                        HandRailAtStartAndEnd(platformEndAngle, l, "end");
                    }
                }

                if (platformStartAngle == 0 && platformEndAngle == 360)
                {
                    radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
                    gratingOuterRadius = (radius + 25 + 10) + distanceFromStack + length;
                    if (platformStartAngle == ladderOrientation)
                    {
                        theta = 180 / Math.PI * (Math.Atan((ladderWidth + 320) / (gratingOuterRadius * 2)));
                        platformEndAngle = platformEndAngle - theta;
                    }

                    if (platformEndAngle == ladderOrientation)
                    {
                        theta = 180 / Math.PI * (Math.Atan((ladderWidth + 320) / (gratingOuterRadius * 2)));
                        platformStartAngle = platformStartAngle + theta;
                    }

                }


                if (extensionEndAngle - extensionStartAngle != 0)
                {


                    // first half of platform

                    startAngle = platformStartAngle;
                    endAngle = extensionStartAngle;
                    length = platformLength;
                    radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
                    gratingOuterRadius = radius + distanceFromStack + length;



                    if (startAngle != endAngle)
                    {
                        ShiftAngle();
                        CreateHandrail();
                    }

                    // extension

                    startAngle = extensionStartAngle;
                    endAngle = extensionEndAngle;
                    length = platformLength + extensionLength;

                    if (startAngle != endAngle)
                    {
                        ShiftAngle();
                        CreateHandrail();

                    }


                    // second half of platform

                    startAngle = extensionEndAngle;
                    endAngle = platformEndAngle;
                    length = platformLength;

                    if (startAngle != endAngle)
                    {
                        ShiftAngle();
                        CreateHandrail();

                    }

                }
                else
                {
                    startAngle = platformStartAngle;
                    endAngle = platformEndAngle;
                    length = platformLength;
                    radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
                    gratingOuterRadius = radius + distanceFromStack + length;



                    if (startAngle != endAngle)
                    {
                        ShiftAngle();
                        CreateHandrail();
                    }
                }


            }
            _tModel.Model.CommitChanges();
        }

        public void ShiftAngle()
        {
            theta = 180 / Math.PI * (Math.Atan((ladderWidth + 320) / (gratingOuterRadius * 2)));
            if (ladderOrientation == startAngle)
            {

                startAngle = startAngle + theta;

            }
            if (ladderOrientation == endAngle)
            {
                endAngle = endAngle - theta;
            }
        }

        public void SetHandrailrData()
        {

            List<JToken> _gratinglist = _global.JData["Platform"].ToList();
            foreach (JToken grating in _gratinglist)
            {
                platformStartAngle = (float)grating["Platform_Start_Angle"];
                platformEndAngle = (float)grating["Platfrom_End_Angle"];
                elevation = (float)grating["Elevation"];
                platformLength = (float)grating["Platform_Length"];
                gratingThickness = (float)grating["Grating_Thickness"];
                distanceFromStack = (float)grating["Distance_From_Stack"];
                ladderOrientation = (float)grating["Orientation_Angle"];
                extensionStartAngle = (float)grating["Extended_Start_Angle"];
                extensionEndAngle = (float)grating["Extended_End_Angle"];
                extensionLength = (float)grating["Extended_Length"];
                obstructionDistance = (float)grating["Obstruction_Distance"];
                handRailData.Add(new List<double> { platformStartAngle, platformEndAngle, elevation, platformLength, gratingThickness, distanceFromStack, ladderOrientation, extensionStartAngle, extensionEndAngle, extensionLength, obstructionDistance });
            }


        }

        public void CreateHandrail()
        {
            gratingOuterRadius = radius + distanceFromStack + length;

            double totalArcLength;
            double totalAngle;
            double tempEndAngle = endAngle;
            //when ladder is in between start angle and end angle
            if ((startAngle) < ladderOrientation && (endAngle) > ladderOrientation)
            {
                theta = 180 / Math.PI * (Math.Atan((ladderWidth + 320) / (gratingOuterRadius * 2)));
                endAngle = ladderOrientation - theta;
                totalAngle = endAngle - startAngle;
                totalArcLength = Math.Abs(2 * Math.PI * gratingOuterRadius * (totalAngle / 360));
                calculateArcLengthOfCircularHandrail(totalArcLength);
                ManageLastDistance();
                createHandrail1();
                arcLengthList.Clear();
                startAngle = ladderOrientation + theta;
                endAngle = tempEndAngle;
            }
            totalAngle = endAngle - startAngle;
            totalArcLength = Math.Abs(2 * Math.PI * gratingOuterRadius * (totalAngle / 360));
            calculateArcLengthOfCircularHandrail(totalArcLength);
            ManageLastDistance();
            createHandrail1();
            arcLengthList.Clear();
        }

        void calculateArcLengthOfCircularHandrail(double totalArcLength)
        {
            double tempArcLength = 0.0;
            while (totalArcLength > 0)
            {
                tempArcLength = totalArcLength - 2000;
                if (tempArcLength < 600)
                {
                    arcLengthList.Add(totalArcLength);
                    break;
                }
                else
                {
                    arcLengthList.Add(2000);
                    totalArcLength = totalArcLength - 2000;
                }
            }

        }

        public void ManageLastDistance()
        {
            int n = arcLengthList.Count - 1;
            if (n == 0)
            {
                n = 1;
            }
            for (int i = 0; i < n; i++)
            {
                //if only one distance available
                if (arcLengthList.Count == 1)
                {
                    if (arcLengthList[i] >= 600 && arcLengthList[i] <= 2000)
                    {
                        arcLengthList.Add(arcLengthList[i]);
                        arcLengthList.RemoveAt(i);
                    }

                    else if (arcLengthList[i] > 2000)
                    {
                        arcLengthList.Add((arcLengthList[i]) / 2);
                        arcLengthList.Add((arcLengthList[i]) / 2);
                        arcLengthList.RemoveAt(i);
                    }

                    //To create only single handrail minimum 1100 distance is required 
                    else if (arcLengthList[0] < 600)
                    {
                        arcLengthList.Clear();
                        break;
                    }
                }

                //check the distance of last handrail and according to that either modify second last or keep as it is
                else if (i + 1 == arcLengthList.Count - 1)
                {
                    if (arcLengthList[i + 1] >= 600 && arcLengthList[i + 1] <= 2000)
                    {
                        arcLengthList.Add(arcLengthList[i + 1]);
                        arcLengthList.RemoveAt(i + 1);
                    }


                    else if (arcLengthList[i + 1] > 2000)
                    {
                        double sum = (arcLengthList[i + 1]) / 2;
                        arcLengthList.RemoveAt(i + 1);
                        arcLengthList.Add(sum);
                        arcLengthList.Add(sum);

                    }

                    else if (arcLengthList[i + 1] >= 500 && arcLengthList[i + 1] < 600)
                    {
                        double sum = (arcLengthList[i + 1] + arcLengthList[i]) / 2;
                        arcLengthList.RemoveAt(i);
                        arcLengthList.RemoveAt(i);
                        arcLengthList.Add(sum);
                        arcLengthList.Add(sum);
                    }

                }
            }

        }

        void createHandrail1()
        {
            CustomPart handrail = new CustomPart();
            handrail.Name = "HandRail_Type_First";
            handrail.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;

            //origin for handrail
            TSM.ContourPoint point1 = _tModel.ShiftVertically(_global.Origin, elevation);
            TSM.ContourPoint point2 = _tModel.ShiftHorizontallyRad(point1, gratingOuterRadius, 1, startAngle * (Math.PI / 180));
            //second point for handrail



            for (int i = 0; i < arcLengthList.Count; i++)
            {
                if (i > 0)
                {
                    point2 = _tModel.ShiftAlongCircumferenceRad(point2, arcLengthList[i - 1], 2);
                }

                handrail.SetAttribute("Radius", gratingOuterRadius);
                handrail.SetAttribute("ArcLength", arcLengthList[i]);
                handrail.SetAttribute("VerticalPostProfile", VerticalPostProfile);
                handrail.SetAttribute("TopRailProfile", TopRailProfile);
                handrail.SetAttribute("MidRailProfile", MidRailProfile);
                handrail.SetAttribute("hideBeam", 0);
                handrail.Position.Plane = Position.PlaneEnum.MIDDLE;
                handrail.Position.Rotation = Position.RotationEnum.TOP;
                handrail.Position.Depth = Position.DepthEnum.MIDDLE;

                if ((startAngle == extensionStartAngle && startAngle != platformStartAngle) && i == 0)
                {
                    ContourPoint sPoint = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, radius + distanceFromStack + platformLength, 1, startAngle * Math.PI / 180), null);
                    sPoint = _tModel.ShiftVertically(sPoint, 500);
                    sPoint = _tModel.ShiftHorizontallyRad(sPoint, 6, 2);
                    _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                    _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                    _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                    CreateSideHandRailAtExtension(sPoint, extensionLength, "start", 4);
                }
                else if ((endAngle == extensionEndAngle && endAngle != platformEndAngle) && i == arcLengthList.Count - 1)
                {
                    ContourPoint sPoint = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, radius + distanceFromStack + platformLength, 1, endAngle * Math.PI / 180), null);
                    sPoint = _tModel.ShiftVertically(sPoint, 500);
                    sPoint = _tModel.ShiftHorizontallyRad(sPoint, 6, 4);
                    _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                    _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                    _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                    CreateSideHandRailAtExtension(sPoint, extensionLength, "end", 2);

                }

                handrail.SetInputPositions(point1, point2);
                handrail.Insert();

            }
            //_tModel.Model.CommitChanges();


        }

        public void CreateSideHandRailAtExtension(ContourPoint sPoint, double extensionlength, string location, int side)
        {
            ContourPoint ePoint = new ContourPoint();
            ContourPoint point1 = new ContourPoint();


            if (VerticalPostProfile == "L50*50*6")
            {
                if (location == "end" && MidRailProfile == "FB50*6")
                {
                    ePoint = new ContourPoint(_tModel.ShiftHorizontallyRad(sPoint, extensionlength, 1), null);
                }
                else
                {
                    ePoint = new ContourPoint(_tModel.ShiftHorizontallyRad(sPoint, extensionlength + 50, 1), null);
                }

            }
            else
            {
                ePoint = new ContourPoint(_tModel.ShiftHorizontallyRad(sPoint, extensionlength + 65, 1), null);
            }
            //midrail           
            Beam b1 = _tModel.CreateBeam(sPoint, ePoint, MidRailProfile, "IS2062", "10", _global.Position, "midrail");

            if (MidRailProfile != "FB50*6")
            {
                point1 = _tModel.ShiftHorizontallyRad(sPoint, 6, side);
                CreateCutBox(point1, side, "MidRailProfile", "MidRailProfile", b1, b1, false);
            }

            //toprail
            if (location == "start")
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
            }
            else
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
            }
            sPoint = _tModel.ShiftVertically(sPoint, 500);
            ePoint = _tModel.ShiftVertically(ePoint, 500);

            Beam b2 = _tModel.CreateBeam(sPoint, ePoint, TopRailProfile, "IS2062", "10", _global.Position, "toprail");
            point1 = _tModel.ShiftHorizontallyRad(sPoint, 6, side);
            CreateCutBox(point1, side, "TopRailProfile", "TopRailProfile", b2, b2, false);

            //vertical post
            sPoint = _tModel.ShiftHorizontallyRad(sPoint, 6, side);
            sPoint = _tModel.ShiftHorizontallyRad(_tModel.ShiftVertically(sPoint, -1000), extensionlength / 2, 1);
            ePoint = _tModel.ShiftVertically(sPoint, 985);
            if (location == "start")
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                _global.Position.RotationOffset = startAngle;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                Beam b3 = _tModel.CreateBeam(sPoint, ePoint, VerticalPostProfile, "IS2062", "10", _global.Position, "verticalPost");
                CreateBolt(b3, sPoint, "start", startAngle);
                point1 = _tModel.ShiftVertically(sPoint, 500);
                CreateCutBox(point1, side, "MidRailProfile", "VerticalPostProfile", b1, b3, false);
            }
            else
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                _global.Position.RotationOffset = endAngle;

                Beam b4 = _tModel.CreateBeam(sPoint, ePoint, VerticalPostProfile, "IS2062", "10", _global.Position, "verticalPost");
                _global.Position.RotationOffset = 0;
                CreateBolt(b4, sPoint, "end", endAngle);
                point1 = _tModel.ShiftVertically(sPoint, 500);
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                CreateCutBox(point1, side, "MidRailProfile", "VerticalPostProfile", b1, b4, false);
            }

            _global.Position.RotationOffset = 0;

        }

        public void CreateCutBox(ContourPoint point1, int side, string type1, string type2, Part part1, Part part2, bool isAtLadder)
        {
            double d = getDistance(type1);

            ContourPoint point2 = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, d - 6, side), null);
            if (isAtLadder)
            {
                point2 = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, d - 6, side, ladderOrientation * Math.PI / 180), null);
            }
            d = getDistance(type2);
            if (type2 != "VerticalPostProfile")
            {
                d = d - 6;
            }
            ContourPoint point3 = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, d, 1), null);
            ContourPoint point4 = new ContourPoint(_tModel.ShiftHorizontallyRad(point2, d, 1), null);
            if (isAtLadder)
            {
                point3 = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, d, 1, ladderOrientation * Math.PI / 180), null);
                point4 = new ContourPoint(_tModel.ShiftHorizontallyRad(point2, d, 1, ladderOrientation * Math.PI / 180), null);
            }

            _pointsList.Add(point1);
            _pointsList.Add(point2);
            _pointsList.Add(point4);
            _pointsList.Add(point3);

            if (MidRailProfile != "FB50*6" && type1 == "MidRailProfile")
            {
                ContourPlate cut = _tModel.CreateContourPlate(_pointsList, "PL6", "IS2062", BooleanPart.BooleanOperativeClassName, _global.Position, "");

                _tModel.cutPart(cut, part1);

            }
            if (type1 == "TopRailProfile")
            {
                _global.Position.Depth = Position.DepthEnum.FRONT;
                ContourPlate cut = _tModel.CreateContourPlate(_pointsList, "PL6", "IS2062", BooleanPart.BooleanOperativeClassName, _global.Position, "");

                _tModel.cutPart(cut, part1);
            }
            _pointsList.Clear();

            if (type2 == "VerticalPostProfile")
            {
                point1 = _tModel.ShiftVertically(point1, 485);
                point2 = _tModel.ShiftHorizontallyRad(point1, 15, side);
                point3 = _tModel.ShiftHorizontallyRad(point1, d, side);
                if (isAtLadder)
                {
                    point2 = _tModel.ShiftHorizontallyRad(point1, 15, side, ladderOrientation * Math.PI / 180);
                    point3 = _tModel.ShiftHorizontallyRad(point1, d, side, ladderOrientation * Math.PI / 180);
                }

                double t = (d - 15) * Math.Tan(Math.PI / 3);
                point4 = _tModel.ShiftVertically(point3, -t);
                _pointsList.Add(point2);
                _pointsList.Add(point3);
                _pointsList.Add(point4);
                if (side == 4)
                {
                    _global.Position.Depth = Position.DepthEnum.FRONT;
                }
                else if (side == 2)
                {
                    _global.Position.Depth = Position.DepthEnum.BEHIND;
                }

                ContourPlate cut = _tModel.CreateContourPlate(_pointsList, "PL6", "IS2062", BooleanPart.BooleanOperativeClassName, _global.Position, "");

                _tModel.cutPart(cut, part2);
                _pointsList.Clear();
            }
        }

        public double getDistance(string profileType)
        {
            double d = 0.0;
            if (profileType == "MidRailProfile")
            {
                if (MidRailProfile == "L50*50*6")
                {
                    d = 50;
                }
                else if (MidRailProfile == "L65*65*6")
                {
                    d = 65;
                }

            }
            else if (profileType == "TopRailProfile")
            {
                if (TopRailProfile == "L50*50*6")
                {
                    d = 50;
                }
                else if (TopRailProfile == "L65*65*6")
                {
                    d = 65;
                }

            }
            else if (profileType == "VerticalPostProfile")
            {
                if (VerticalPostProfile == "L50*50*6")
                {
                    d = 50;
                }
                else if (VerticalPostProfile == "L65*65*6")
                {
                    d = 65;
                }
            }




            return d;
        }

        public void CreateBolt(Part part, ContourPoint point1, string location, double angle)
        {
            if (VerticalPostProfile == "L50*50*6")
            {
                point1 = _tModel.ShiftHorizontallyRad(point1, 25, 1);
            }
            else
            {
                point1 = _tModel.ShiftHorizontallyRad(point1, (65 / 2), 1);
            }
            point1 = _tModel.ShiftVertically(point1, 80);

            BoltArray B = new BoltArray();
            B.PartToBeBolted = part;
            B.PartToBoltTo = part;

            B.FirstPosition = point1;
            B.SecondPosition = _tModel.ShiftVertically(point1, -50);

            B.BoltSize = 10;
            B.Tolerance = 3.00;
            B.BoltStandard = "8.8XOX";
            B.BoltType = BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE;
            B.CutLength = 100;

            B.Length = 100;
            B.ExtraLength = 15;
            B.ThreadInMaterial = BoltGroup.BoltThreadInMaterialEnum.THREAD_IN_MATERIAL_YES;

            B.Position.Depth = Position.DepthEnum.MIDDLE;
            B.Position.Plane = Position.PlaneEnum.LEFT;
            B.Position.Rotation = Position.RotationEnum.TOP;
            B.Position.RotationOffset = -angle;
            if (location == "end")
            {
                B.Position.Rotation = Position.RotationEnum.BELOW;
                //B.Position.RotationOffset = angle;
            }



            B.Bolt = true;
            B.Washer1 = true;
            B.Washer2 = true;
            B.Washer3 = true;
            B.Nut1 = true;
            B.Nut2 = true;

            B.Hole1 = true;
            B.Hole2 = true;
            B.Hole3 = true;
            B.Hole4 = true;
            B.Hole5 = true;



            B.AddBoltDistX(60);
            B.AddBoltDistY(0);


            if (!B.Insert())
                Console.WriteLine("Bolt Insert failed!");
            _tModel.Model.CommitChanges();


        }

        public void HandRailAtStartAndEnd(double angle, double l, string location)
        {
            ContourPoint origin = new ContourPoint(_global.Origin, null);
            ContourPoint point1 = new ContourPoint(_tModel.ShiftVertically(origin, elevation), null);
            double r = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
            r += distanceFromStack;
            ContourPoint point2 = new ContourPoint(_tModel.ShiftHorizontallyRad(point1, r, 1, angle * Math.PI / 180), null);

            CustomPart handrail = new CustomPart();
            handrail.Name = "Side_HandRail";
            handrail.Number = BaseComponent.CUSTOM_OBJECT_NUMBER;
            handrail.SetAttribute("Radius", r);
            handrail.SetAttribute("Length", l);
            handrail.SetAttribute("VerticalPostProfile", VerticalPostProfile);
            handrail.SetAttribute("TopRailProfile", TopRailProfile);
            handrail.SetAttribute("MidRailProfile", MidRailProfile);
            if (location == "end")
            {
                handrail.SetAttribute("end", 1);


            }
            else
            {
                handrail.SetAttribute("end", 0);
                handrail.SetAttribute("start", 1);
            }

            handrail.Position.Plane = Position.PlaneEnum.MIDDLE;
            handrail.Position.Rotation = Position.RotationEnum.TOP;
            handrail.Position.Depth = Position.DepthEnum.MIDDLE;
            handrail.SetInputPositions(point1, point2);
            bool b = handrail.Insert();
            if (location == "end")
            {
                handrail.SetAttribute("start", 0);
                handrail.Modify();
            }



        }

        void HandrailAtLadderLocation()
        {
            //radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
            //gratingOuterRadius = radius + distanceFromStack;
            //double theta = 180 / Math.PI * (Math.Atan((ladderWidth + 320) / (gratingOuterRadius * 2)));
            // left of ladder
            if (ladderOrientation - theta > platformStartAngle || ((ladderOrientation == 0) && (platformStartAngle == 0 && platformEndAngle == 360)))
            {
                // section near stack
                HandrailAtLadderLocationNearStack(4, theta, "end");

                // section near hoop
                HandrailAtLadderLocationNearHoop(4, theta, "end");

            }

            // right of ladder

            if (ladderOrientation + theta < platformEndAngle || ((ladderOrientation == 360) && (platformStartAngle == 0 && platformEndAngle == 360)))
            {
                // section near stack
                HandrailAtLadderLocationNearStack(2, theta, "start");

                // section near hoop
                HandrailAtLadderLocationNearHoop(2, theta, "start");
            }
        }

        void HandrailAtLadderLocationNearStack(int side, double angle, string location)
        {
            double handrailHeight = 1000;
            double radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);

            TSM.ContourPoint origin = new TSM.ContourPoint(_global.Origin, null);
            origin = _tModel.ShiftVertically(origin, elevation);

            // vertical post
            TSM.ContourPoint postBottomPoint = _tModel.ShiftHorizontallyRad(origin, radius + distanceFromStack, 1, ladderOrientation * Math.PI / 180);
            //postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, (ladderWidth / 2) + 100 + 60, side);



            if (location == "start")
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                postBottomPoint = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, (ladderWidth / 2) + 160, 2);
            }
            else
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                postBottomPoint = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, -((ladderWidth / 2) + 160), 2);
            }
            TSM.ContourPoint postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 15);
            _global.Position.RotationOffset = ladderOrientation;

            _global.ProfileStr = VerticalPostProfile;
            _global.ClassStr = "10";
            TSM.Beam post = _tModel.CreateBeam(postBottomPoint, postTopPoint, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
            CreateBolt(post, postBottomPoint, location, ladderOrientation);
            ContourPoint point3 = new ContourPoint(_tModel.ShiftVertically(postBottomPoint, 500), null);
            //bent 
            double dist = getDistance("VerticalPostProfile");

            _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
            _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.FRONT;
            _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.FRONT;
            if (location == "end")
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.TOP;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.FRONT;
                postBottomPoint = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, -6, 2);
            }
            else
            {
                postBottomPoint = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, 6, 2);

            }

            double m = getDistance("TopRailProfile");
            postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, dist, 1, ladderOrientation * Math.PI / 180);
            postBottomPoint = _tModel.ShiftVertically(postBottomPoint, 500);
            ContourPoint point1 = new ContourPoint(_tModel.ShiftHorizontallyRad(postBottomPoint, 200 + dist, 3, ladderOrientation * Math.PI / 180), null);
            postTopPoint = _tModel.ShiftVertically(postBottomPoint, 500);
            ContourPoint point2 = new ContourPoint(_tModel.ShiftHorizontallyRad(postTopPoint, 200 + dist, 3, ladderOrientation * Math.PI / 180), null);
            _pointsList.Add(postBottomPoint);
            _pointsList.Add(point1);
            _pointsList.Add(point2);
            _pointsList.Add(postTopPoint);
            _global.Position.RotationOffset = 0;

            TSM.PolyBeam p = _tModel.CreatePolyBeam(_pointsList, TopRailProfile, Globals.MaterialStr, _global.ClassStr, _global.Position);
            _pointsList.Clear();

            point1 = _tModel.ShiftHorizontallyRad(postBottomPoint, dist, 3, ladderOrientation * Math.PI / 180);
            int _side = 4;
            if (location == "end")
            {
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                _side = 2;
            }

            CreateCutBox(point3, _side, "TopRailProfile", "VerticalPostProfile", p, post, true);


        }

        void HandrailAtLadderLocationNearHoop(int side, double angles, string location)
        {
            double handrailHeight = 1000;
            double radius = _tModel.GetRadiusAtElevation(elevation, _global.StackSegList, true);
            double remainingDistance; // distance between first and last vertical post

            TSM.ContourPoint origin = new TSM.ContourPoint(_global.Origin, null);
            origin = _tModel.ShiftVertically(origin, elevation);

            TSM.ContourPoint horizontalRodPoint1;
            TSM.ContourPoint horizontalRodPoint2;
            TSM.ContourPoint point2;
            // vertical post 1
            TSM.ContourPoint postBottomPoint = _tModel.ShiftHorizontallyRad(origin, radius + obstructionDistance + 325 + 365 + 25, 1, ladderOrientation * Math.PI / 180);
            //postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, (ladderWidth / 2) + 100 + 25, side);

            point2 = new ContourPoint(_tModel.ShiftHorizontallyRad(origin, radius + distanceFromStack, 1, ladderOrientation * Math.PI / 180), null);


            _global.ProfileStr = VerticalPostProfile;
            _global.ClassStr = "10";
            if (location == "start")
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, (ladderWidth / 2) + 160, 2, ladderOrientation * Math.PI / 180);
                point2 = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, (ladderWidth / 2) + 160, 2);
            }
            else
            {
                _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, ((ladderWidth / 2) + 100 + 60), 4, ladderOrientation * Math.PI / 180);
                point2 = _tModel.ShiftAlongCircumferenceRad(postBottomPoint, (ladderWidth / 2) + 160, 2);
            }
            _global.Position.RotationOffset = ladderOrientation;
            TSM.ContourPoint postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 15);
            TSM.Beam verticalPost = _tModel.CreateBeam(postBottomPoint, postTopPoint, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
            ContourPoint point1 = new ContourPoint(_tModel.ShiftVertically(postBottomPoint, 500), null);

            CreateBolt(verticalPost, postBottomPoint, location, ladderOrientation);

            //horizontalRodPoint1 = _tModel.ShiftHorizontallyRad(postTopPoint, 25, 3, ladderOrientation * Math.PI / 180);
            horizontalRodPoint1 = _tModel.ShiftHorizontallyRad(_tModel.ShiftVertically(postTopPoint, 15), 6, side, ladderOrientation * Math.PI / 180);

            length = platformLength;
            if (_tModel.AngleAtCenter(postBottomPoint) >= extensionStartAngle * Math.PI / 180 && _tModel.AngleAtCenter(postBottomPoint) < extensionEndAngle * Math.PI / 180)
            {
                length += extensionLength;
            }
            double angle = Math.Asin(((ladderWidth + 320) / 2) / (radius + distanceFromStack + length));

            if (side == 2)
            {
                angle = (ladderOrientation * Math.PI / 180) + angle;
            }
            else
            {
                angle = (ladderOrientation * Math.PI / 180) - angle;
            }
            postBottomPoint = _tModel.ShiftHorizontallyRad(origin, radius + distanceFromStack + length, 1, angle);
            // postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, 25, side, ladderOrientation * Math.PI / 180);
            horizontalRodPoint2 = new ContourPoint(_tModel.ShiftHorizontallyRad(_tModel.ShiftVertically(postBottomPoint, handrailHeight), 6, side, ladderOrientation * Math.PI / 180), null);
            postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, 250, 3, ladderOrientation * Math.PI / 180);
            postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 15);

            // bent pipe 

            //BentPipeAtLadderLocation(postTopPoint, _tModel.ShiftVertically(postTopPoint, -500 + 25), 1, 250 - 25);



            //vertical posts

            remainingDistance = _tModel.DistanceBetweenPoints(horizontalRodPoint1, horizontalRodPoint2) - 25;

            /*while (remainingDistance > 200)
            {
                _tModel.CreateBeam(postBottomPoint, postTopPoint, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
                if (remainingDistance - 600 > 200)
                {
                    postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, 600, 3, ladderOrientation * Math.PI / 180);
                    postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 25);
                    remainingDistance -= 600;
                }
                else
                {
                    postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, remainingDistance / 2, 3, ladderOrientation * Math.PI / 180);
                    postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 25);
                    remainingDistance /= 2;
                }

            }*/

            // hotizontal pipes 
            _global.ProfileStr = TopRailProfile;
            _global.ClassStr = "10";
            _global.Position.RotationOffset = 0;
            _global.Position.Depth = Position.DepthEnum.BEHIND;
            TSM.Beam horizontalPipe = _tModel.CreateBeam(horizontalRodPoint1, horizontalRodPoint2, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
            //HandrailAtLadderLocationCut(verticalPost, horizontalPipe, 1);

            horizontalRodPoint1 = _tModel.ShiftVertically(horizontalRodPoint1, -500);
            horizontalRodPoint2 = _tModel.ShiftVertically(horizontalRodPoint2, -500);

            _global.ProfileStr = MidRailProfile;
            _global.ClassStr = "10";
            _global.Position.Depth = Position.DepthEnum.BEHIND;
            horizontalPipe = _tModel.CreateBeam(horizontalRodPoint1, horizontalRodPoint2, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);


            int _side = 4;
            if (location == "end")
            {
                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                _side = 2;
            }

            CreateCutBox(point1, _side, "MidRailProfile", "VerticalPostProfile", horizontalPipe, verticalPost, true);
            CreateCutBox(point1, _side, "MidRailProfile", "VerticalPostProfile", horizontalPipe, verticalPost, true);
            int count = 1;
            double distanceBetweenVerticalPost = remainingDistance / count;
            while (distanceBetweenVerticalPost > 600)
            {
                count++;
                distanceBetweenVerticalPost = remainingDistance / count;
            }

            for (int i = 0; i < count; i++)
            {
                _global.ProfileStr = VerticalPostProfile;
                _global.ClassStr = "10";
                if (location == "start")
                {
                    _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.RIGHT;
                    _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BACK;
                    _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;

                }
                else
                {
                    _global.Position.Plane = Tekla.Structures.Model.Position.PlaneEnum.LEFT;
                    _global.Position.Rotation = Tekla.Structures.Model.Position.RotationEnum.BELOW;
                    _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;

                }
                _global.Position.RotationOffset = ladderOrientation;

                _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                TSM.Beam Post = _tModel.CreateBeam(postBottomPoint, postTopPoint, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);

                _global.Position.RotationOffset = 0;

                _side = 4;
                if (location == "end")
                {
                    _global.Position.Depth = Tekla.Structures.Model.Position.DepthEnum.BEHIND;
                    _side = 2;
                }
                point1 = new ContourPoint(_tModel.ShiftVertically(postBottomPoint, 500), null);
                CreateCutBox(point1, _side, "MidRailProfile", "VerticalPostProfile", horizontalPipe, Post, true);



                CreateBolt(Post, postBottomPoint, location, ladderOrientation);
                postBottomPoint = _tModel.ShiftHorizontallyRad(postBottomPoint, distanceBetweenVerticalPost, 3, ladderOrientation * Math.PI / 180);
                postTopPoint = _tModel.ShiftVertically(postBottomPoint, handrailHeight - 15);
            }
            _global.Position.RotationOffset = 0;
            // hotizontal pipes 
            //_global.ProfileStr = TopRailProfile;
            //_global.ClassStr = "10";
            //_global.Position.Depth = Position.DepthEnum.BEHIND;
            //TSM.Beam horizontalPipe = _tModel.CreateBeam(horizontalRodPoint1, horizontalRodPoint2, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
            ////HandrailAtLadderLocationCut(verticalPost, horizontalPipe, 1);

            //horizontalRodPoint1 = _tModel.ShiftVertically(horizontalRodPoint1, -500 );
            //horizontalRodPoint2 = _tModel.ShiftVertically(horizontalRodPoint2, -500);

            //_global.ProfileStr = MidRailProfile;
            //_global.ClassStr = "10";
            //_global.Position.Depth = Position.DepthEnum.BEHIND;
            //_tModel.CreateBeam(horizontalRodPoint1, horizontalRodPoint2, _global.ProfileStr, Globals.MaterialStr, _global.ClassStr, _global.Position);
        }


        //public void HandRailAtStartAndEnd()
        //{
        //    ContourPoint origin = new ContourPoint(_global.Origin, null);
        //    List<ContourPoint> l = new List<ContourPoint>();
        //    List<Part> _allParts = new List<Part>();
        //    double r = _tModel.GetRadiusAtElevation(0, _global.StackSegList, true);
        //    r += 200;
        //    ContourPoint p1 = new ContourPoint(_tModel.ShiftHorizontallyRad(origin, r, 1), null);
        //    for (int i = 0; i < 1; i++)
        //    {
        //        ContourPoint p2 = new ContourPoint(_tModel.ShiftVertically(p1, 985), null);
        //        _global.Position.Rotation = Position.RotationEnum.BACK;

        //        _global.Position.Plane = Position.PlaneEnum.RIGHT;
        //        _global.Position.Depth = Position.DepthEnum.BEHIND;

        //        Beam b1 = _tModel.CreateBeam(p1, p2, "L50*50*6", "IS2062", "10", _global.Position, "b1");
        //        l.Add(p1);
        //        _allParts.Add(b1);
        //        CreateBolt(b1, p1, "start", 0);
        //        _global.Position.Rotation = Position.RotationEnum.BELOW;

        //        _global.Position.Plane = Position.PlaneEnum.LEFT;
        //        _global.Position.Depth = Position.DepthEnum.BEHIND;

        //        b1 = _tModel.CreateBeam(p1, p2, "L50*50*6", "IS2062", "10", _global.Position, "b1");
        //        CreateBolt(b1, p1, "start", 0);

        //        _allParts.Add(b1);
        //        ContourPoint pn = new ContourPoint(_tModel.ShiftHorizontallyRad(p1, 2000, 1), null);
        //        p2 = _tModel.ShiftVertically(pn, 985);
        //        b1 = _tModel.CreateBeam(pn, p2, "L50*50*6", "IS2062", "10", _global.Position, "b1");

        //    }
        //    p1 = new ContourPoint(_tModel.ShiftVertically(p1, 500), null);
        //    ContourPoint pd = new ContourPoint(_tModel.ShiftAlongCircumferenceRad(p1, -6, 2), null);
        //    p1 = _tModel.ShiftAlongCircumferenceRad(p1, 6, 2);
        //    for (int i = 0; i < 2; i++)
        //    {
        //        List<ContourPoint>_points=new List<ContourPoint>();
        //        ContourPoint mp = new ContourPoint(_tModel.ShiftHorizontallyRad(p1, 1000, 1, 0), null);
        //        ContourPoint p3 = new ContourPoint(_tModel.ShiftHorizontallyRad(mp, 1000, 1, 0), null);
        //        _points.Add(p1);
        //        _points.Add(mp);
        //        _points.Add(p3);


        //        _global.Position.Depth = Position.DepthEnum.BEHIND;
        //        _global.Position.Plane = Position.PlaneEnum.RIGHT;
        //        _global.Position.Rotation = Position.RotationEnum.BACK;
        //        _global.Position.RotationOffset = 0;
        //        PolyBeam p = _tModel.CreatePolyBeam(_points, "L50*50*6", "IS2062", "10", _global.Position, "b1");
        //        if (i == 0)
        //        {
        //            CreateCutBox(_tModel.ShiftVertically(l[0], 500), 4, "MidRailProfile", "VerticalPostProfile", p, _allParts[i]);

        //        }
        //        _points.Clear();
        //        mp = new ContourPoint(_tModel.ShiftHorizontallyRad(pd, 1000, 1, 0), null);
        //        p3 = new ContourPoint(_tModel.ShiftHorizontallyRad(mp, 1000, 1, 0), null);
        //        _points.Add(pd);
        //        _points.Add(mp);
        //        _points.Add(p3);

        //        _global.Position.Depth = Position.DepthEnum.BEHIND;
        //        _global.Position.Plane = Position.PlaneEnum.LEFT;
        //        _global.Position.Rotation = Position.RotationEnum.BELOW;
        //        p = _tModel.CreatePolyBeam(_points, "L50*50*6", "IS2062", "10", _global.Position, "b1");

        //        if (i == 0)
        //        {
        //            CreateCutBox(_tModel.ShiftVertically(l[0], 500), 2, "MidRailProfile", "VerticalPostProfile", p, _allParts[i+1]);

        //        }

        //        p1 = _tModel.ShiftVertically(p1, 500);
        //        p1 = _tModel.ShiftVertically(pd, 500);



        //    }


        //}





    }
}
