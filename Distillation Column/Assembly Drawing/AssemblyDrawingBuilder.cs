using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using HelperLibrary;
using DistillationColumn;

namespace DistillationColumn
{
  class AssemblyDrawingBuilder 
  {

        Globals _global;
        TeklaModelling _tModel;
        
    public AssemblyDrawingBuilder(Globals global,TeklaModelling tModel,string jStr)
    {
        _global = global;
        _tModel = tModel;
         Build(jStr);
    }
    public void Build(string jsonStr)
    {
      //List<ArrayList> listOfArrayLists = new List<ArrayList>();
      //foreach (ArrayList assembyPartsList in listofAssemblyPartsList)
      //{
      //  listOfArrayLists.Add(assembyPartsList);
      //}
      //AddStackAssembly();
      //AddFramesAssembly(GratingPlatformLatest.PlatFormFramesCollection);
      //HandrailTypeThree.pushFrameInAssembly();
      //AddGratingAssembly(GratingPlatformLatest.gratingCollection);
      //AddBracketAssembly(GratingPlatformLatest.PlatFormBracketsCollection);
      //AddLadderAssembly(ladderCollection);
      //AddHandrailAssembly(handrailCollection);
      //try { AddsafetyGateAssembly(SafetyGateCollection); } catch (Exception e) { };
      //MessageBox.Show("Please perform numbering. After numbering click ok!!");
      //new StackModuleAssemblyDrawings(_global, _tModel, jsonStr);
      //StackModuleAssemblyDrawings(_global,_tModel,jsonStr);
      //BuilDrawing(new GratingAssemblyDrawing(), jsonStr);
      //BuilDrawing(new BracketAssemblyDrawing(), jsonStr);
       new HandrailAssemblyDrawing(_global, _tModel, jsonStr);
      //BuilDrawing(new LadderAssemblyDrawing(), jsonStr);
      //BuilDrawing(new FrameAssemblyDrawing(), jsonStr);
      //ClearData();
      MessageBox.Show("Assembly Drawing finished!!");
    }

   
    //public static void ClearData()
    //{
    //  ClearListOfList(GratingPlatformLatest.gratingCollection);
    //  ClearListOfList(GratingPlatformLatest.PlatFormBracketsCollection);
    //  ClearListOfList(GratingPlatformLatest.PlatFormFramesCollection);
    //  ClearListOfList(handrailCollection);
    //  ClearListOfList(SafetyGateCollection);
    //  ClearListOfList(ladderCollection);
    //}

        //public static void BuilDrawing(CAssembly assembly, string jsonstr)
        //{
        //  try
        //  {
        //    assembly.Build(jsonstr);
        //  }
        //  catch (Exception ex)
        //  {
        //   // MessageBox.Show(ex.ToString());
        //  }
        //}
  }
}
