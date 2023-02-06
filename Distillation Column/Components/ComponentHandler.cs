using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using HelperLibrary;

namespace DistillationColumn
{
    class ComponentHandler
    {
        Globals _global;
        TeklaModelling _tModel;
        public ComponentHandler(Globals global, TeklaModelling teklaModel, Dictionary<string, bool?> checkComponents)
        {
            _global = global;
            _tModel = teklaModel; new ImportCustomComponent(_global, _tModel);
            new Stack(_global, _tModel);
            if (checkComponents["chair"].Value)
            {
                new Chair(_global, _tModel);
            }
            if (checkComponents["stiffner_ring"].Value)
            {
                new StiffnerRings(_global, _tModel);
            }
            if (checkComponents["flange"].Value)
            {
                new Flange(_global, _tModel);
            }
            if (checkComponents["access_door"].Value)
            {
                new AccessDoor(_global, _tModel);
            }
            if (checkComponents["platform"].Value)
            {
                new Platform(_global, _tModel);
            }
            if (checkComponents["handrail"].Value)
            {
                new Handrail(_global, _tModel);
            }
            if (checkComponents["circular_access_door"].Value)
            {
                new CircularAccessDoor(_global, _tModel);
            }
            if (checkComponents["rectangular_platform"].Value)
            {
                new RectangularPlatform(_global, _tModel);
            }
            if (checkComponents["cap"].Value)
            {
                new CapAndOutlets(_global, _tModel);
            }
            if (checkComponents["ladder"].Value)
            {
                new Ladder(_global, _tModel);
            }
            if (checkComponents["instrument_nozzle"].Value)
            {
                new InstrumentNozzle(_global, _tModel);
            }
        }
    }
}
