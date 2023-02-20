using HelperLibrary;
using Tekla.Structures.Drawing;

namespace DistillationColumn
{
  abstract class CDrawingView : IDrawingView
  {
    protected GADrawing m_drawingInst;
    protected GADrawing sectionInst;

    protected static double m_viewScale = 1.0 / 200;
    protected static View m_completeStackView = null;
    public abstract void Generate(string jsonStr, Globals _global, TeklaModelling _tModel);
    public  abstract void InsertAnnotations(Globals _global, TeklaModelling _tModel);
    public abstract void InsertDimensions(Globals _global, TeklaModelling _tModel);
   
  }
}
