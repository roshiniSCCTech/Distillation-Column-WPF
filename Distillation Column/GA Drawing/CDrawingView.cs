using Tekla.Structures.Drawing;

namespace DistillationColumn
{
  public abstract class CDrawingView : IDrawingView
  {
    protected GADrawing m_drawingInst;
    protected GADrawing sectionInst;

    protected static double m_viewScale = 1.0 / 200;
    protected static View m_completeStackView = null;
    public abstract void Generate(string jsonStr);
    //public  abstract void InsertAnnotations();
    //public abstract void InsertDimensions();
   
  }
}
