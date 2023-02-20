using System.Collections;
using System.Windows.Documents;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using System.Collections.Generic;
using Tekla.Structures;
using TechnipSteelStack;
using TechnipSteelStack.Utilities;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Windows;

namespace TechnipSteelStack.Drawing
{
  public class CompleteStackView : CDrawingView
  {
    // Global member declarations
    #region
    double x = 0.0;
    List<string> profileList = new List<string>();
    T3D.Point startPoint = new T3D.Point();
    T3D.Point endPoint = new T3D.Point();
    double platFromElevation = 0.0;
    List<double> elevation = new List<double>();
    List<double> spliceElevation = new List<double>();
    List<double> accessDoorElevation = new List<double>();
    List<double> accessOri = new List<double>();
    List<double> inspectionDoorElevation = new List<double>();
    List<double> inspectionOri = new List<double>();
    List<double> instrumentNozzleElevation = new List<double>();
    List<double> nozzleOri = new List<double>();
    List<double> ductElevation = new List<double>();
    double refactor_thinkness = 0.0;
    double chairElevation = 0.0;
    double stackTipElevation = 0.0;
    double ringEnd_elevation = 0.0;
    double ring_start_elevation = 0.0;
    List<double> refactoryStartEleList = new List<double>();
    List<double> refactoryEndEleList = new List<double>();
    List<double> refactorythickList = new List<double>();
    double northOrientationAngle = 0.0;
    #endregion   
    //End
    double getRadiusAtElevation(double elevation)
    {
      double radius = 0.0;
      for (int i = 0; i < CAssembly.heightandRadList.Count; i++)
      {
        if (CAssembly.heightandRadList[i][0] > elevation)
        {
          radius = CAssembly.heightandRadList[i][1] / 2;
          break;
        }
      }
      return radius;
    }

    public CompleteStackView(GADrawing drawingInst)
    {
      m_drawingInst = drawingInst;
    }
    public override void Generate(string jsonStr)
    {
      try
      {
        JObject jsonData1 = JObject.Parse(jsonStr);
        List<JToken> platformTokenList = jsonData1["platform"]["platform"].ToList();
        foreach (JToken item in platformTokenList.Reverse<JToken>())
        {
          platFromElevation = (double)(item["elevation"]) * 1000 + CAssembly.m_stackLocZ;
          elevation.Add(platFromElevation);
        }
        List<JToken> spliceTokenList = jsonData1["splice2"].ToList();
        foreach (JToken item in spliceTokenList.Reverse<JToken>())
        {
          double elevation = (double)(item["elevation"]) * 1000 + CAssembly.m_stackLocZ;
          spliceElevation.Add(elevation);
        }
        List<JToken> accessDoorTokenList = jsonData1["accessDoor"].ToList();
        foreach (JToken item in accessDoorTokenList.Reverse<JToken>())
        {
          double elevation = (double)(item["access_door_elevation"]) * 1000 + CAssembly.m_stackLocZ;
          accessDoorElevation.Add(elevation);
          double angle = (double)(item["orientation_angle"]);
          accessOri.Add(angle);
        }
        List<JToken> inspectionDoorTokenList = jsonData1["inspectionDoor"].ToList();
        foreach (JToken item in inspectionDoorTokenList.Reverse<JToken>())
        {
          double elevation = (double)(item["inspection_door_elevation"]) * 1000 + CAssembly.m_stackLocZ;
          inspectionDoorElevation.Add(elevation);
          double angle = (double)(item["orientation_angle"]);
          inspectionOri.Add(angle);
        }
        List<JToken> instrumentNozzleList = jsonData1["instrument_nozzle"].ToList();
        foreach (JToken item in instrumentNozzleList.Reverse<JToken>())
        {
          double elevation = (double)(item["height"]) * 1000 + CAssembly.m_stackLocZ;
          instrumentNozzleElevation.Add(elevation);
          double angle = (double)(item["orientation_angle"]);
          nozzleOri.Add(angle);
        }
        List<JToken> refactoryList = jsonData1["Refractory_lining_input"].ToList();
        foreach (JToken item in refactoryList.Reverse<JToken>())
        {
          refactor_thinkness = (double)(item["thickness"]) * 1000;
          ringEnd_elevation = (double)(item["ele_end"]) * 1000 + CAssembly.m_stackLocZ;
          ring_start_elevation = (double)(item["ele_start"]) * 1000 + CAssembly.m_stackLocZ;
          refactoryStartEleList.Add(ring_start_elevation);
          refactoryEndEleList.Add(ringEnd_elevation);
          refactorythickList.Add(refactor_thinkness);
        }
        List<JToken> ductElevationList = jsonData1["duct"].ToList();
        foreach (JToken item in ductElevationList.Reverse<JToken>())
        {
          double elevation = (double)(item["duct_elevation"]) * 1000 + CAssembly.m_stackLocZ;
          ductElevation.Add(elevation);
        }
        northOrientationAngle = (double)(jsonData1["orientationPlan"]["orientation_plan_angle"]["value"]);
      }
      catch (Exception e)
      {
        //MessageBox.Show(e);
      }
      ModelObjectEnumerator selectedModelObjects = CAssembly.myModel.GetModelObjectSelector().GetAllObjects();
      Tekla.Structures.Geometry3d.CoordinateSystem ModelObjectCoordSys = new T3D.CoordinateSystem();
      string ModelObjectName = "CompleteStack";
      DrawingUtils.GetCoordinateSystemAndNameOfSelectedObject(selectedModelObjects, out ModelObjectCoordSys, out ModelObjectName);
      ArrayList Parts = new ArrayList();

      while (selectedModelObjects.MoveNext())
      {
        if (selectedModelObjects.Current is Tekla.Structures.Model.Part)
        {
          Parts.Add(selectedModelObjects.Current.Identifier);
        }
      }
      T3D.CoordinateSystem coordinateSystem = new T3D.CoordinateSystem(new T3D.Point(0, 0, 0), new T3D.Vector(1, 0, 0), new T3D.Vector(0, 0, 1));
      m_completeStackView = DrawingUtils.AddGaFrontView("Front View of" + ModelObjectName, m_drawingInst, Parts, DrawingUtils.GetBasicCoordinateSystemForFrontView(coordinateSystem));

      m_drawingInst.Insert();
    }

    public override void InsertAnnotations()
    {

      //Profile annotation for Stiffener ring in stack front view
      AnnotationsAttributes annotationsAttributes = new AnnotationsAttributes();
      annotationsAttributes.Profile = true;
      annotationsAttributes.Single = true;
    //  DrawingUtils.InsertProfileAnnotations(m_completeStackView, AppStrings.StiffenerRingName, annotationsAttributes);
      //end======

      //Profile and material annotation for floor steel pipe in stack front view
      AnnotationsAttributes annotationsAttributes1 = new AnnotationsAttributes();
      annotationsAttributes1.Profile = true;
      annotationsAttributes1.Material = true;
      //DrawingUtils.InsertProfileAnnotations(m_completeStackView, AppStrings.Floor_Steel_Pipe, annotationsAttributes1);
      //end======

      ////Instrument nozzle tags in front view
      //string[] instrumentTagNameArray = null;
      //JObject jsonData = JObject.Parse(json);
      //List<JToken> tokenList = jsonData["stack"].ToList();
      //List<float> insertionPoint = new List<float>();
      //foreach (JToken item in tokenList.Reverse<JToken>())
      //{
      //    float insideDiaBottom = (float)item["inside_dia_bottom"] * 1000;
      //    insertionPoint.Add(insideDiaBottom);
      //}
      //insertionPoint.Sort();
      //List<JToken> tokenListIN = jsonData["instrument_nozzle"].ToList();
      //instrumentTagNameArray = new string[tokenListIN.Count];
      //int y = 0;
      //foreach (JToken item in tokenListIN)
      //{
      //    instrumentTagNameArray[y] = (string)(item["tags"]);
      //    y++;
      //}
      //foreach (string tag in instrumentTagNameArray)
      //{
      //    // DrawingUtils.InsertText(m_completeStackView, AppStrings.Instrument_Nozzle + tag, insertionPoint.Last());
      //}


      //level marks at platform elvations
      foreach (double i in elevation)
      {
        endPoint = new T3D.Point(3600, i);
        startPoint = new T3D.Point(0, i); ;
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }
      //end

      //level marks at Splice elevation
      foreach (double i in spliceElevation)
      {
        endPoint = new T3D.Point(3600, i);
        startPoint = new T3D.Point(0, i); ;
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }
      //end

      //level marks at Access Door elevation
      for (int i = 0; i < accessDoorElevation.Count; i++)
      {
        double rad = getRadiusAtElevation(accessDoorElevation[i]);
        endPoint = new T3D.Point(2 * rad, accessDoorElevation[i]);
        startPoint = new T3D.Point(x + rad * Math.Cos(accessOri[i] * Math.PI / 180), accessDoorElevation[i]);
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }

      //level marks at Inspection Door elevation
      for (int i = 0; i < inspectionDoorElevation.Count; i++)
      {
        double rad = getRadiusAtElevation(inspectionDoorElevation[i]);
        endPoint = new T3D.Point(3600, inspectionDoorElevation[i]);
        startPoint = new T3D.Point(x + (rad * Math.Cos(inspectionOri[i])), inspectionDoorElevation[i]);
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }
      //end

      //level marks at Instrument nozzle elevation
      for (int i = 0; i < instrumentNozzleElevation.Count; i++)
      {
        double rad = getRadiusAtElevation(instrumentNozzleElevation[i]);
        endPoint = new T3D.Point(3600, instrumentNozzleElevation[i]);
        startPoint = new T3D.Point(x + (rad * Math.Cos(nozzleOri[i])), instrumentNozzleElevation[i]);
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }
      //end

      //level marks at duct elevation
      foreach (double i in ductElevation)
      {
        double rad = getRadiusAtElevation(i);
        endPoint = new T3D.Point(3600, i);
        startPoint = new T3D.Point(rad * Math.Cos(0), i);
        DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      }
      //end

      //level mark at stacktip elevation
      endPoint = new T3D.Point(3600, stackTipElevation);
      startPoint = new T3D.Point(0, stackTipElevation); ;
      //   DrawingUtils.createLevelMark(m_completeStackView, startPoint, endPoint);
      //end

      //level mark at chair elevation
      chairElevation = CAssembly.m_stackLocZ;
      endPoint = new T3D.Point(3600, chairElevation);
      startPoint = new T3D.Point(0, chairElevation); ;
      DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);

      //end

      //Level mark of total height of stack
      int cell = CAssembly.heightandRadList.Count - 1;
      double lastShellHeight = CAssembly.heightandRadList[cell][4];
      endPoint = new T3D.Point(3600, CAssembly.heightandRadList[cell][0] + lastShellHeight);
      startPoint = new T3D.Point(0, CAssembly.heightandRadList[cell][0] + lastShellHeight);
      DrawingUtils.CreateLevelMark(m_completeStackView, startPoint, endPoint);
      //End

      // North Direction object insert on left corner of sheet
      DrawingUtils.InsertTemplate(m_completeStackView, northOrientationAngle);
      //End

    }
    public override void InsertDimensions()
    {
      int pos = 2500;
      double lastConicalShell = StackModel.drawing_Tapered_Seg_Points[StackModel.drawing_Tapered_Seg_Points.Count - 1].Z;
      // Conical segments dimensions
      for (int val = 0; val <= StackModel.drawing_Tapered_Seg_Points.Count - 2; val = val + 2)
      {
        startPoint = new T3D.Point(StackModel.drawing_Tapered_Seg_Points[val].X, StackModel.drawing_Tapered_Seg_Points[val].Z);
        endPoint = new T3D.Point(StackModel.drawing_Tapered_Seg_Points[val + 1].X, StackModel.drawing_Tapered_Seg_Points[val + 1].Z);
        // DrawingUtils.createDimension(m_completeStackView, startPoint, endPoint);
      }
      StackModel.drawing_Tapered_Seg_Points.Clear();
      //End

      //Plate Thickness wise dimensions
      double heightDrawing = 0.0;
      double drawingZ = heightDrawing = CAssembly.m_stackLocZ; //Starting elevation of first dimension
      startPoint = new T3D.Point(-pos, drawingZ);
      int i = 0;
      PointList pointList = new PointList();
      while (i != CAssembly.heightandRadList.Count - 1)
      {
        if (CAssembly.heightandRadList[i][3] == CAssembly.heightandRadList[i + 1][3])
        {
          heightDrawing = StackModel.dimElevations[i + 1];
          if (i == CAssembly.heightandRadList.Count - 2)
          {
            heightDrawing = StackModel.dimElevations[i + 1];
            startPoint = new T3D.Point(-pos, drawingZ, 0);
            endPoint = new T3D.Point(-pos, heightDrawing + CAssembly.heightandRadList[i + 1][4], 0);
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
            T3D.Point textPoint = new T3D.Point(-pos, (startPoint.Y + endPoint.Y) / 2);
            DrawingUtils.InsertText(m_completeStackView, textPoint, (CAssembly.heightandRadList[i][3]).ToString());
          }
        }
        else
        {
          heightDrawing = StackModel.dimElevations[i + 1];
          startPoint = new T3D.Point(-pos, drawingZ, 0);
          endPoint = new T3D.Point(-pos, heightDrawing, 0);
          pointList.Add(startPoint);
          pointList.Add(endPoint);
          DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
          drawingZ = heightDrawing;
          T3D.Point textPoint = new T3D.Point(-pos, (startPoint.Y + endPoint.Y) / 2);
          DrawingUtils.InsertText(m_completeStackView, textPoint, (CAssembly.heightandRadList[i][3]).ToString());
        }
        i++;
      }
      //End

      // From Bottom of the Stack to Top of the stack
      pos = 2700;
      int cell = CAssembly.heightandRadList.Count - 1;
      double lastShellHeight = CAssembly.heightandRadList[cell][4];
      startPoint = new T3D.Point(-pos, CAssembly.m_stackLocZ);
      endPoint = new T3D.Point(-pos, CAssembly.heightandRadList[cell][0] + lastShellHeight);
      DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
      //End

      //Splice to splice Dimension (including last splice to total height of stack)
      pos = 3300;
      startPoint = new T3D.Point(-(pos), CAssembly.m_stackLocZ);
      foreach (double i1 in StackModel.arraySplice)
      {
        endPoint = new T3D.Point(startPoint.X, i1, 0);
        DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
        startPoint = new T3D.Point(-(pos), i1, 0);
      }
      startPoint = new T3D.Point(-(pos), StackModel.arraySplice[StackModel.arraySplice.Length - 1], 0);
      endPoint = new T3D.Point(-pos, CAssembly.heightandRadList[cell][0] + lastShellHeight, 0);
      DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
      //End

      // Horizontal Dimension where radius is Changing
      for (int itr = 0; itr < CAssembly.heightandRadList.Count - 1; itr++)
      {
        if (CAssembly.heightandRadList[itr][1] != CAssembly.heightandRadList[itr][2])
        {
          double radius = (CAssembly.heightandRadList[itr + 1][1]) / 2;
          startPoint = new T3D.Point(-radius, StackModel.dimElevations[itr + 1]);
          endPoint = new T3D.Point(radius, StackModel.dimElevations[itr + 1]);
          DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
        }
      }
      //End

      //Vertical Dimensions upto conical shell
      pos = -3100;
      startPoint = new T3D.Point(pos, CAssembly.m_stackLocZ);
      endPoint = new T3D.Point(pos, CAssembly.m_stackLocZ + CAssembly.heightandRadList[0][4]);
      for (int itr = 0; itr < CAssembly.heightandRadList.Count - 1; itr++)
      {
        if (CAssembly.heightandRadList[itr][1] == CAssembly.heightandRadList[itr][2])
        {
          endPoint = new T3D.Point(pos, StackModel.dimElevations[itr + 1]);
          if (itr == CAssembly.heightandRadList.Count - 2)
          {
            startPoint = new T3D.Point(pos, CAssembly.heightandRadList[cell][0] + lastShellHeight);
            endPoint = new T3D.Point(pos, lastConicalShell);
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
          }
        }
        else
        {
          if (itr == 0)
          {
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
            startPoint = endPoint;
          }
          else
          {
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
            double shellHeight = CAssembly.heightandRadList[itr][4];
            startPoint = new T3D.Point(pos, endPoint.Y + shellHeight);
            DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
            startPoint = endPoint;
          }
        }
      }
      //End

      // vertical Dimensions for Refactory Lining according to thickness
      pos = -3600;
      for (int itr = 0; itr < refactoryEndEleList.Count; itr++)
      {
        startPoint = new T3D.Point(pos, refactoryStartEleList[itr]);
        endPoint = new T3D.Point(pos, refactoryEndEleList[itr]);
        DrawingUtils.CreateDimension(m_completeStackView, startPoint, endPoint);
        DrawingUtils.InsertText(m_completeStackView, new T3D.Point(pos, (startPoint.Y + endPoint.Y) / 2), refactorythickList[itr].ToString());
      }
      //End

      StackModel.dimElevations.Clear();
    }
  }
}
