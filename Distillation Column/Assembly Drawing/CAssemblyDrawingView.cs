using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;

namespace DistillationColumn
{
  abstract class CAssemblyDrawingView : IAssemblyDrawingView
  {
    public abstract void Generate(string jsonStr);
 
   
  }
}
