
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
using HelperLibrary;

namespace DistillationColumn
{
    class BracketAssemblyDrawing
    {
        Globals _global;
        TeklaModelling _tModel;
        Tekla.Structures.Geometry3d.CoordinateSystem ModelObjectCoordSys;
        AssemblyDrawing assemblyDrawing;
        Assembly bracketAssembly = new Assembly();
        DrawingHandler dwgHandler = new DrawingHandler();
        ArrayList Parts;
        List<ArrayList> PartsList = new List<ArrayList>();


        public BracketAssemblyDrawing(Globals global, TeklaModelling tModel, string jStr)
        {

            _global = global;
            _tModel = tModel;
            // StackAsssemblyList = new List<Assembly>();
            AddBracketAssembly(_global._bracketPartList);
            MessageBox.Show("Please perform numbering. After numbering click ok!!");
            Build(bracketAssembly);
        }

        public void AddBracketAssembly(ArrayList Assembly)
        {
            try
            {

                foreach (var val in Assembly)
                {
                    bracketAssembly = (val as Tekla.Structures.Model.Part).GetAssembly();
                    (val as Tekla.Structures.Model.Part).PartNumber.Prefix = "SGR";
                    (val as Tekla.Structures.Model.Part).Modify();

                }
                bracketAssembly.Add(Assembly);
                bracketAssembly.AssemblyNumber.Prefix = "SGR";
                bracketAssembly.Modify();

            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
        }

        public void Build(Assembly assemb)
        {
            int i = 0;

            assemblyDrawing = new AssemblyDrawing(assemb.Identifier, "Bracket.A");
            assemblyDrawing.Name = "BRACKET";
            Tekla.Structures.Drawing.Size A1SizePortrait = new Tekla.Structures.Drawing.Size(841, 594);
            assemblyDrawing.Layout.SheetSize = A1SizePortrait;
            assemblyDrawing.Insert();
            Parts = DrawingUtils.GetAssemblyParts(assemb);
            //PartsList.Add(Parts);
            i++;


            DrawingEnumerator drawingEnumerator = dwgHandler.GetDrawings();

            while (drawingEnumerator.MoveNext())
            {
                Tekla.Structures.Drawing.Drawing drawing = drawingEnumerator.Current as AssemblyDrawing;
                if (drawing != null && drawing.Name.StartsWith("BRACKETS"))
                {
                    dwgHandler.SetActiveDrawing(drawing);
                    DrawingObjectEnumerator allViews = drawing.GetSheet().GetAllViews();
                    while (allViews.MoveNext())
                    {
                        View BracketView = (allViews.Current as View);
                        BracketView.Attributes.LoadAttributes("Bracket_View");
                        BracketView.Modify();
                        if (BracketView.ViewType == View.ViewTypes.FrontView)
                        {
                            View sectoinView = DrawingUtils.AddAssemblySectionView(BracketView, new T3D.Point(0, 225), new T3D.Point(0, -150), 100, 100);
                            sectoinView.Attributes.LoadAttributes(DrawingSettings.BracketAssemblySectionView);
                            sectoinView.Modify();
                        }
                        drawing.PlaceViews();
                        dwgHandler.CloseActiveDrawing(true);
                    }
                }
            }

        }
    }
}
