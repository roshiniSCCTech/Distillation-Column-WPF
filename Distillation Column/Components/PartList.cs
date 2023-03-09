using HelperLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tekla.Structures.Model;

namespace DistillationColumn
{
    internal class PartList
    {
        Globals _global;
        TeklaModelling _tModel;
        Assembly gratingAssembly;
        ArrayList _chairList;
        ArrayList _flangeList;
        public PartList(Globals global, TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;
            _chairList = new ArrayList();
            _flangeList = new ArrayList();
            setPartList();
            //StackAssembly(_global._stackPartList);
            //MessageBox.Show("Dot");
            //StackModuleAssembly stack = new StackModuleAssembly(_global,_tModel);
            //stack.Build(gratingAssembly);

        }

        public void setPartList()
        {
            ModelObjectEnumerator Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.POLYBEAM);

            //selecting all polybeams in model
            foreach (PolyBeam polybeam in Obj)
            {
                //chair rings
                if (polybeam.Name == "TopRing" || polybeam.Name == "BottomRing")
                {
                    _chairList.Add(polybeam);
                }

                //flange rings
                if (polybeam.Name == "FlangeTopRing" || polybeam.Name == "FlangeBottomRing")
                {
                    _flangeList.Add(polybeam);
                }

                //handrail midrail and toprail
                //if (polybeam.Name == "TopRail_Type3" || polybeam.Name == "MidRail1_Type3" || polybeam.Name == "MidRail2_Type3" || polybeam.Name == "StartBent_Type3" || polybeam.Name == "EndBent_Type3")
                //{
                //    _global._handrailPartList.Add(polybeam);
                //}

            }

            //selecting all contourplates in model
            Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE);


            foreach (ContourPlate plate in Obj)
            {
                //chair stiffner plates
                if (plate.Name == "StiffnerPlates")
                {
                    _chairList.Add(plate);
                }
                

            }

            //Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            //foreach (Beam beam in Obj)
            //{
            //    if (beam.Name == "FirstPost_Type3" || beam.Name == "SecondPost_Type3" || beam.Name == "ThirdPost_Type3")
            //    {
            //        _global._handrailPartList.Add(beam);
            //        //_global.handrailCollection.Add(_global._handrailPartList);
            //        //_global._handrailPartList = new ArrayList();

            //    }

            //}

            for (int i = 0; i < Handrail.count; i++)
            {

                Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

                foreach (Beam beam in Obj)
                {
                    if (beam.Name == "FirstPost_Type3"+i || beam.Name == "SecondPost_Type3"+i || beam.Name == "ThirdPost_Type3"+i)
                    {
                        _global._handrailPartList.Add(beam);
                        
                    }

                }

                //handrail midrail and toprail
                Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.POLYBEAM);               
                foreach (PolyBeam polybeam in Obj)
                {
                    if (polybeam.Name == "TopRail_Type3"+i || polybeam.Name == "MidRail1_Type3"+i || polybeam.Name == "MidRail2_Type3"+i || polybeam.Name == "StartBent_Type3"+i || polybeam.Name == "EndBent_Type3"+i)
                    {
                        _global._handrailPartList.Add(polybeam);
                    }
                }


                _global.handrailCollection.Add(_global._handrailPartList);
                _global._handrailPartList = new ArrayList();
            }


            _global._stackPartList.Add(_chairList);
            _global._stackPartList.Add(_flangeList);

        }


        //public void StackAssembly(ArrayList assembly)
        //{
        //    try
        //    {

        //        gratingAssembly = new Assembly();
        //        foreach (var val in assembly)
        //        {
        //            gratingAssembly = (val as Part).GetAssembly();
        //            (val as Part).PartNumber.Prefix = "SGR";
        //            (val as Part).Modify();

        //        }
        //        gratingAssembly.Add(assembly);
        //        gratingAssembly.AssemblyNumber.Prefix = "SGR";
        //        gratingAssembly.Modify();
        //        //GratingDrawingAsssemblyList.Add(gratingAssembly);


        //    }
        //    catch (Exception ex)
        //    {
        //        // MessageBox.Show(ex.ToString());
        //    }
        //}



    }




}

