using HelperLibrary;

namespace DistillationColumn
{
  public interface IDrawingView
  {
    void Generate(string jsonStr, Globals _global, TeklaModelling _tModel);
   
  }
}
