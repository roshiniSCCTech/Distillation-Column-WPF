using HelperLibrary;
using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
//using DistillationColumn.Components;

namespace DistillationColumn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Globals _global;
        TeklaModelling _tModel;
        JObject JData;
        List<SegmentData> segmentDatas = new List<SegmentData>();
        List<CircularAccessData> circularAccessDatas = new List<CircularAccessData>();
        List<InstrumentNozzleData> instrumentNozzleDatas= new List<InstrumentNozzleData>();
        List<AccessDoorData> accessDoorDatas= new List<AccessDoorData>();


        string jsonFileName = "Data.json";

        public MainWindow()
        {
            InitializeComponent();

            getJSONData();

            segmentGrid.ItemsSource = segmentDatas;
            CircularAccessGrid.ItemsSource= circularAccessDatas;
            InstrumentNozleGrid.ItemsSource= instrumentNozzleDatas;
            AccessDoorGrid.ItemsSource= accessDoorDatas;

           

        }

        private void createModel(object sender, RoutedEventArgs e)
        {
            setJSONData();

            _global = new Globals(JData);

            _tModel = new TeklaModelling(_global.Origin.X, _global.Origin.Y, _global.Origin.Z);

            new ComponentHandler(_global, _tModel);


        }

        private void getJSONData()
        {
            string jDataString = File.ReadAllText(jsonFileName);
            JData = JObject.Parse(jDataString);

            // data for stack 
            JData["stack"].ToList().ForEach(tok => { tok = JsonConvert.SerializeObject(tok);});
            segmentDatas = JsonConvert.DeserializeObject<List<SegmentData>>(JData["stack"].ToString());

            //data for Circular Access Door
            JData["CircularAccessDoor"].ToList().ForEach(tok => { tok = JsonConvert.SerializeObject(tok); });
            circularAccessDatas = JsonConvert.DeserializeObject<List<CircularAccessData>>(JData["CircularAccessDoor"].ToString());

            //data for Instrument Nozzle
            JData["instrumental_nozzle"].ToList().ForEach(tok => { tok = JsonConvert.SerializeObject(tok); });
            instrumentNozzleDatas = JsonConvert.DeserializeObject<List<InstrumentNozzleData>>(JData["instrumental_nozzle"].ToString());

            //data for Access Door 
            JData["access_door"].ToList().ForEach(tok => { tok = JsonConvert.SerializeObject(tok); });
            accessDoorDatas = JsonConvert.DeserializeObject<List<AccessDoorData>>(JData["access_door"].ToString());
        }

        private void setJSONData()
        {
            //set Json Data for Stack
            JData["stack"] = JArray.Parse(JsonConvert.SerializeObject(segmentDatas));
            File.WriteAllText(jsonFileName, JData.ToString());

            //set Json data for Circular Access Door
            JData["CircularAccessDoor"] = JArray.Parse(JsonConvert.SerializeObject(circularAccessDatas));
            File.WriteAllText(jsonFileName, JData.ToString());

            //set Json data for Instrument Nozzle
            JData["instrumental_nozzle"] = JArray.Parse(JsonConvert.SerializeObject(instrumentNozzleDatas));
            File.WriteAllText(jsonFileName, JData.ToString());

            //set Json data for Access Door
            JData["access_door"] = JArray.Parse(JsonConvert.SerializeObject(accessDoorDatas));
            File.WriteAllText(jsonFileName, JData.ToString());

        }

        private void addRowTop(object sender, RoutedEventArgs e)
        {
            segmentDatas.ForEach(segment => { segment.key++; });
            segmentDatas.Insert(0, new SegmentData()
            {
                key = 0,
                seg_height = 0.0,
                inside_dia_bottom = 0.0,
                inside_dia_top = 0.0,
                shell_thickness = 0.0,
            });
            segmentGrid.Items.Refresh();
        }

        private void deleteRowTop(object sender, RoutedEventArgs e)
        {
            segmentDatas.RemoveAt(0);
            segmentDatas.ForEach(segment => { segment.key--; });
            segmentGrid.Items.Refresh();
        }

        private void addRowBottom(object sender, RoutedEventArgs e)
        {
            segmentDatas.Add(new SegmentData()
            {
                key = segmentDatas.Count,
                seg_height = 0.0,
                inside_dia_bottom = 0.0,
                inside_dia_top = 0.0,
                shell_thickness = 0.0,
            });
            segmentGrid.Items.Refresh();
        }

        private void deleteRowBottom(object sender, RoutedEventArgs e)
        {
            segmentDatas.RemoveAt(segmentDatas.Count - 1);
            segmentGrid.Items.Refresh();
        }

        private void addRowforCircularAccessDoor(object sender, RoutedEventArgs e)
        {
            circularAccessDatas.Add(new CircularAccessData()
            {
                key = circularAccessDatas.Count,
                elevation=0.0,
                orientation_angle=0.0,
                neck_plate_Thickness = 0.0,
                plate_Diameter = 0.0,
                neck_plate_width = 0.0,
                lining_plate_width = 0.0,
                number_of_bolts = 0.0,
            });
            CircularAccessGrid.Items.Refresh();
        }   
        private void addRowforInstrumentNozzle(object sender, RoutedEventArgs e)
        {
            instrumentNozzleDatas.Add(new InstrumentNozzleData()
            {
                key = instrumentNozzleDatas.Count,
                elevation = 0.0,
                orientation_angle = 0.0,
            });
            InstrumentNozleGrid.Items.Refresh();

        }      

        private void addRowforAccessDoor(object sender, RoutedEventArgs e)
        {
           accessDoorDatas.Add(new AccessDoorData()
            {
                key = accessDoorDatas.Count,
                elevation = 0.0,
                orientation_angle = 0.0,
                height = 0.0,
                width = 0.0,
                breadth= 0.0,
                
            });
            AccessDoorGrid.Items.Refresh();
        }

    }
}
