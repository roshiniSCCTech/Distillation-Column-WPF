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
        string jsonFileName = "Data.json";

        public MainWindow()
        {
            InitializeComponent();

            getJSONData();

            segmentGrid.ItemsSource = segmentDatas;

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

        }

        private void setJSONData()
        {
            JData["stack"] = JArray.Parse(JsonConvert.SerializeObject(segmentDatas));
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

    }
}
