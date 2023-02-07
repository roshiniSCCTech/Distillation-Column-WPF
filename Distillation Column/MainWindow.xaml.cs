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
using RenderData;
using Microsoft.Win32;
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
        JObject JProjects;
        List<Projects> projects = new List<Projects>();

        List<StackData> stackDatas = new List<StackData>();
        List<CircularAccessData> circularAccessDatas = new List<CircularAccessData>();
        List<InstrumentNozzleData> instrumentNozzleDatas= new List<InstrumentNozzleData>();
        List<AccessDoorData> accessDoorDatas= new List<AccessDoorData>();
        RectangularPlatformData rectPlatformData = new RectangularPlatformData();
        List<PlatformData> platformDatas = new List<PlatformData>();
        List<FlangeData> flangeDatas = new List<FlangeData>();
        ChairData chairDatas = new ChairData();
        StiffnerRingData ringDatas = new StiffnerRingData();
        Dictionary<string, bool?> checkComponents = new Dictionary<string, bool?>();


        string jsonFileName = "Data.json";

        public MainWindow()
        {
            InitializeComponent();


            // data for projects
            string jProjectsString = File.ReadAllText("projects.json");
            JProjects = JObject.Parse(jProjectsString);
            projects = JsonConvert.DeserializeObject<List<Projects>>(JProjects["projects"].ToString());
            ProjectsDropdown.ItemsSource = projects;
            ProjectsDropdown.DisplayMemberPath = "name";
            ProjectsDropdown.SelectedIndex = 0;
            ProjectName.Text = ProjectsDropdown.Text;

            InitialiseData();
        }

        private void InitialiseData()
        {
            getJSONData();

            StackGrid.ItemsSource = stackDatas;
            CircularAccessGrid.ItemsSource = circularAccessDatas;
            InstrumentNozleGrid.ItemsSource = instrumentNozzleDatas;
            AccessDoorGrid.ItemsSource = accessDoorDatas;
            PlatformGrid.ItemsSource = platformDatas;
            flangeGrid.ItemsSource = flangeDatas;      

            BindRecPlatformData();
            BindChairData();
            BindStiffnerRingData();
        }

        private void BindRecPlatformData()
        {
            Height.DataContext = rectPlatformData;
            Width.DataContext = rectPlatformData;
            Plate_Width.DataContext = rectPlatformData;
            Orientation_Angle.DataContext = rectPlatformData;
            Rung_Spacing.DataContext = rectPlatformData;
            Obstruction_Distance.DataContext = rectPlatformData;
            Platform_Start_Angle.DataContext = rectPlatformData;
            Platform_End_Angle.DataContext = rectPlatformData;
        }

        private void createModel(object sender, RoutedEventArgs e)
        {
            setJSONData();
            IntializeCheckboxes();

            _global = new Globals(JData);

            _tModel = new TeklaModelling(_global.Origin.X, _global.Origin.Y, _global.Origin.Z);

            new ComponentHandler(_global, _tModel, checkComponents);


        }

        private void getJSONData()
        {
            string jDataString = File.ReadAllText(jsonFileName);
            JData = JObject.Parse(jDataString);
            
            // data for stack 
            stackDatas = JsonConvert.DeserializeObject<List<StackData>>(JData["stack"].ToString());

            //data for Circular Access Door
            circularAccessDatas = JsonConvert.DeserializeObject<List<CircularAccessData>>(JData["CircularAccessDoor"].ToString());

            //data for Instrument Nozzle
            instrumentNozzleDatas = JsonConvert.DeserializeObject<List<InstrumentNozzleData>>(JData["instrumental_nozzle"].ToString());

            //data for Access Door 
            accessDoorDatas = JsonConvert.DeserializeObject<List<AccessDoorData>>(JData["access_door"].ToString());

            // data for rectangular platform
            rectPlatformData = JsonConvert.DeserializeObject<RectangularPlatformData>(JData["RectangularPlatform"].ToString());

            // data for platform
            platformDatas = JsonConvert.DeserializeObject<List<PlatformData>>(JData["Platform"].ToString());

            //chair data
            chairDatas = JsonConvert.DeserializeObject<ChairData>(JData["chair"].ToString());    
            
            //stiffner ring data
            ringDatas = JsonConvert.DeserializeObject<StiffnerRingData>(JData["stiffner_ring"].ToString());  
            
            //JData["Flange"].ToList().ForEach(tok => { tok = JsonConvert.SerializeObject(tok); });
            flangeDatas = JsonConvert.DeserializeObject<List<FlangeData>>(JData["Flange"].ToString());

        }

        private void setJSONData()
        {
            //set Json Data for Stack
            JData["stack"] = JArray.Parse(JsonConvert.SerializeObject(stackDatas));
           
            //set Json data for Circular Access Door
            JData["CircularAccessDoor"] = JArray.Parse(JsonConvert.SerializeObject(circularAccessDatas));
            
            //set Json data for Instrument Nozzle
            JData["instrumental_nozzle"] = JArray.Parse(JsonConvert.SerializeObject(instrumentNozzleDatas));
           
            //set Json data for Access Door
            JData["access_door"] = JArray.Parse(JsonConvert.SerializeObject(accessDoorDatas));
            
            // set data for rectangular platform
            JData["RectangularPlatform"] = JObject.Parse(JsonConvert.SerializeObject(rectPlatformData));

            // set data for  platform
            platformDatas.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            JData["Platform"] = JArray.Parse(JsonConvert.SerializeObject(platformDatas));

            // set data for  chair
            JData["chair"] = JObject.Parse(JsonConvert.SerializeObject(chairDatas));

            // set data for  stiffner ring
            JData["stiffner_ring"] = JObject.Parse(JsonConvert.SerializeObject(ringDatas));

            // set data for  flange
            JData["Flange"] = JArray.Parse(JsonConvert.SerializeObject(flangeDatas));

            File.WriteAllText(jsonFileName, JData.ToString());

        }

        private void addRowTopForStack(object sender, RoutedEventArgs e)
        {
            stackDatas.ForEach(segment => { segment.key++; });
            stackDatas.Insert(0, new StackData()
            {
                key = 0,
                seg_height = 0.0,
                inside_dia_bottom = 0.0,
                inside_dia_top = 0.0,
                shell_thickness = 0.0,
            });
            StackGrid.Items.Refresh();
        }

        private void deleteRowTopForStack(object sender, RoutedEventArgs e)
        {
            stackDatas.RemoveAt(0);
            stackDatas.ForEach(segment => { segment.key--; });
            StackGrid.Items.Refresh();
        }

        private void addRowBottomForStack(object sender, RoutedEventArgs e)
        {
            stackDatas.Add(new StackData()
            {
                key = stackDatas.Count,
                seg_height = 0.0,
                inside_dia_bottom = 0.0,
                inside_dia_top = 0.0,
                shell_thickness = 0.0,
            });
            StackGrid.Items.Refresh();
        }

        private void deleteRowBottomForStack(object sender, RoutedEventArgs e)
        {
            stackDatas.RemoveAt(stackDatas.Count - 1);
            StackGrid.Items.Refresh();
        }

        private void addRowforCircularAccessDoor(object sender, RoutedEventArgs e)
        {
            circularAccessDatas.Add(new CircularAccessData()
            {
                
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
                
                elevation = 0.0,
                orientation_angle = 0.0,
            });
            InstrumentNozleGrid.Items.Refresh();

        }      

        private void addRowforAccessDoor(object sender, RoutedEventArgs e)
        {
           accessDoorDatas.Add(new AccessDoorData()
            {
                
                elevation = 0.0,
                orientation_angle = 0.0,
                height = 0.0,
                width = 0.0,
                breadth= 0.0,
                
            });
            AccessDoorGrid.Items.Refresh();
        }

        private void addRowForPlatform(object sender, RoutedEventArgs e)
        {
            platformDatas.Add(new PlatformData()
            {
                Elevation = 0,
                Orientation_Angle = 0,
                Platform_Width = 0,
                Platform_Length = 0,
                Platform_Start_Angle = 0,
                Platfrom_End_Angle = 0,
                Distance_From_Stack = 0,
                Gap_Between_Grating_Plate = 0,
                Grating_Thickness = 0,
                Rungs_spacing = 0,
                Extended_Length = 0,
                Extended_Start_Angle = 0,
                Extended_End_Angle = 0,
                Obstruction_Distance = 0,
            });
            PlatformGrid.Items.Refresh();
        }

        public void BindChairData()
        {
            ChairWidth.DataContext = chairDatas;
            Top_Ring_Thickness.DataContext = chairDatas;
            Bottom_Ring_Thickness.DataContext = chairDatas;
            Stiffner_Count.DataContext = chairDatas;
            Stiffner_Plate_Thickness.DataContext = chairDatas;
            Inside_Distance.DataContext = chairDatas;
            Distance_between_Plates.DataContext = chairDatas;
            ChairHeight.DataContext = chairDatas;
        }

        public void BindStiffnerRingData()
        {
            StartHeight.DataContext = ringDatas;
            EndHeight.DataContext = ringDatas;
            StiffnerCount.DataContext = ringDatas;
        }

        public void IntializeCheckboxes()
        {
            checkComponents.Add("stack", Check_Stack.IsChecked);
            checkComponents.Add("chair", Check_Chair.IsChecked);
            checkComponents.Add("access_door", Check_Access_Door.IsChecked);
            checkComponents.Add("flange", Check_Distillation_Flange.IsChecked);
            checkComponents.Add("stiffner_ring", Check_Stiffner_Ring.IsChecked);
            checkComponents.Add("platform", Check_Platform.IsChecked);
            checkComponents.Add("handrail", Check_HandRail.IsChecked);
            checkComponents.Add("rectangular_platform", Check_Rectangular_Platform.IsChecked);
            checkComponents.Add("cap", Check_Cap_and_Outlets.IsChecked);
            checkComponents.Add("instrument_nozzle", Check_Instrument_Nozzle.IsChecked);
            checkComponents.Add("ladder", Check_Ladder.IsChecked);
            checkComponents.Add("circular_access_door", Check_Circular_Access_Door.IsChecked);
        }

        private void addRowFlange(object sender, RoutedEventArgs e)
        {
            flangeDatas.Add(new FlangeData()
            {
               
                elevation = 0.0,
                number_of_bolts = 0,
                inside_distance = 0,
                top_ring_thickness = 0,
                bottom_ring_thickness = 0,
                ring_width = 0,
            });
            flangeGrid.Items.Refresh();
        }

        private void GetNewData(object sender, SelectionChangedEventArgs e)
        {
            Projects p = (Projects)ProjectsDropdown.SelectedItem;
            ProjectName.Text = p.name;
            jsonFileName = p.file;

            InitialiseData();
        }

        private void ImportProject(object sender, RoutedEventArgs e)
        {
            // get new file

            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Json file (*.json)|*.json";
            openFileDlg.ShowDialog();

            File.WriteAllText(openFileDlg.SafeFileName, File.ReadAllText(openFileDlg.FileName));

            // add file to projects list in projects.json

            projects.Add(new Projects()
            {
                name = ImportProjectName.Text,
                file = openFileDlg.SafeFileName,
            });

            JProjects["projects"] = JArray.Parse(JsonConvert.SerializeObject(projects));
            File.WriteAllText("projects.json", JProjects.ToString());

            // update projects drop down

            ProjectsDropdown.Items.Refresh();

        }

        private void AddProject(object sender, RoutedEventArgs e)
        {
            // create new file

            File.WriteAllText(NewProjectName.Text + "Data.json", File.ReadAllText("templateData.json"));

            // add file to projects list in projects.json

            projects.Add(new Projects()
            {
                name = NewProjectName.Text,
                file = NewProjectName.Text + "Data.json",
            });

            JProjects["projects"] = JArray.Parse(JsonConvert.SerializeObject(projects));
            File.WriteAllText("projects.json", JProjects.ToString());

            // update projects drop down

            ProjectsDropdown.Items.Refresh();

        }
    }
}
