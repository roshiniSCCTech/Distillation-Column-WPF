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
        public PartList(Globals global, TeklaModelling tModel)
        {
            _global = global;
            _tModel = tModel;
            setPartList();
            ChairAssembly(_global._chairPartList);
            MessageBox.Show("Dot");
            StackModuleAssembly stack = new StackModuleAssembly(_global,_tModel);
            stack.Build(gratingAssembly);

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
                    _global._chairPartList.Add(polybeam);
                }

                //flange rings
                if (polybeam.Name == "FlangeTopRing" || polybeam.Name == "FlangeBottomRing")
                {
                    _global._flangePartList.Add(polybeam);
                }

                //handrail midrail and toprail
                if (polybeam.Name == "TopRail_Type3" || polybeam.Name == "MidRail_Type3" || polybeam.Name == "StartBent_Type3" || polybeam.Name == "EndBent_Type3")
                {
                    _global._handrailPartList.Add(polybeam);
                }

            }

            //selecting all contourplates in model
            Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE);


            foreach (ContourPlate plate in Obj)
            {
                //chair stiffner plates
                if (plate.Name == "StiffnerPlates")
                {
                    _global._chairPartList.Add(plate);
                }

            }

            Obj = _tModel.Model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            foreach (Beam beam in Obj)
            {
                if (beam.Name == "FirstPost_Type3" || beam.Name == "SecondPost_Type3" || beam.Name == "ThirdPost_Type3")
                {
                    _global._handrailPartList.Add(beam);
                }
            }
        }


        public void ChairAssembly(ArrayList assembly)
        {
            try
            {

                gratingAssembly = new Assembly();
                foreach (var val in assembly)
                {
                    gratingAssembly = (val as Part).GetAssembly();
                    (val as Part).PartNumber.Prefix = "SGR";
                    (val as Part).Modify();

                }
                gratingAssembly.Add(assembly);
                gratingAssembly.AssemblyNumber.Prefix = "SGR";
                gratingAssembly.Modify();
                //GratingDrawingAsssemblyList.Add(gratingAssembly);


            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
        }



    }




}

