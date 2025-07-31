using PacMan.Entities;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public Map Map { get; set; }
        private Renderer renderer = null;
        private List<GameObject> objects = new List<GameObject>();
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            renderer = new Renderer(MainScreen);
            Map = new Map(MainScreen.Width, MainScreen.Height, 16, 16);

            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Render;
            timer.Start();
        }

        public void Render(object sender, EventArgs e) 
        {
            List<GameObject> objects = new List<GameObject>();
            for (int row = 0; row < Map.map.GetLength(0); row++)
            {
                for (int col = 0; col < Map.map.GetLength(1); col++)
                {
                    if (Map.map[row, col] == null)
                        continue;

                    objects.Add(Map.map[row, col].GameObject);
                }
            }

            MainScreen.Source = renderer.Render(Map, objects);
        }

        private void OverlayCanvas_InsertObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            string entity = AddObjectsBox.Text;
            Entity newEntity;

            switch (entity)
            {
                case "Wall":
                    newEntity = new Wall();
                    break;

                case "Cookie":
                    newEntity = new Cookie();
                    break;

                case "SuperCookie":
                    newEntity = new SuperCookie();
                    break;

                case "Monster": 
                    newEntity = new Monster();
                    break;

                case "SuperMonster":
                    newEntity = new SuperMonster();
                    break;

                case "MonsterSpawn": 
                    newEntity = new MonsterSpawn();
                    break;

                default:
                    MessageBox.Show("Please select an item!");
                    return;
            }

            newEntity.Coordinates = new Coordinates() { X = column, Y = row };
            newEntity.ImagePath = PathToEntityImage.Text;
            var newScreenElement = new ScreenElement(Map.TileSizeWidth, Map.TileSizeHeight, newEntity.ImagePath);
            newScreenElement.LoadImage();

            newEntity.GameObject = new GameObject(newScreenElement, newEntity);
            objects.Add(newEntity.GameObject);

            var actualPosition = Map.map[row, column];

            if (actualPosition == null)
                Map.map[row, column] = newEntity;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //var CookieElement = new ScreenElement(30, 30, "../../../images/cookie.png");
            //CookieElement.LoadImage();
            //var CookieCoordinates = new Coordinates() { X = 0, Y = 0 };
            //var PacmanElement = new ScreenElement(30, 30, "../../../images/pacman.png");
            //PacmanElement.LoadImage();
            //var PacmanCoordinates = new Coordinates() { X = 50, Y = 50};
            //var CookieObject = new GameObject(CookieElement, CookieCoordinates);
            //var PacmanObject = new GameObject(PacmanElement, PacmanCoordinates);

            //objects.Add(CookieObject);
            //objects.Add(PacmanObject);

        }

        private void MapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Collapsed;
            LoadButton.Visibility = Visibility.Collapsed;
            QuitButton.Visibility = Visibility.Collapsed;
            MapEditorButton.Visibility = Visibility.Collapsed;

            OverlayCanvas.Visibility = Visibility.Visible;
            AddObjectsBox.Visibility = Visibility.Visible;
            QuitMapEditorButton.Visibility = Visibility.Visible;
            SaveMapButton.Visibility = Visibility.Visible;
            LoadBackgroundButton.Visibility = Visibility.Visible;
            Label1.Visibility = Visibility.Visible;
            PathToEntityImage.Visibility = Visibility.Visible;
            Label2.Visibility = Visibility.Visible;
            PathToBackgroundImage.Visibility = Visibility.Visible;
            DefaultBackgroundButton.Visibility = Visibility.Visible;
        }

        private void QuitMapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayCanvas.Visibility = Visibility.Collapsed;
            AddObjectsBox.Visibility = Visibility.Collapsed;
            QuitMapEditorButton.Visibility = Visibility.Collapsed;
            SaveMapButton.Visibility = Visibility.Collapsed;
            Label1.Visibility= Visibility.Collapsed;
            PathToEntityImage.Visibility = Visibility.Collapsed;
            Label2.Visibility= Visibility.Collapsed;
            PathToBackgroundImage.Visibility = Visibility.Collapsed;
            LoadBackgroundButton.Visibility= Visibility.Collapsed;
            DefaultBackgroundButton.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Visible;
            LoadButton.Visibility = Visibility.Visible;
            QuitButton.Visibility = Visibility.Visible;
            MapEditorButton.Visibility = Visibility.Visible;
        }

        private void AddObjectsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(AddObjectsBox.SelectedIndex)
            {
                case 1: 
                    PathToEntityImage.Text = "../../../images/wall.png";
                    break;

                case 2: PathToEntityImage.Text = "../../../images/cookie.png";
                    break;

                case 3: PathToEntityImage.Text = "../../../images/flower.png"; //change to supercookie
                    break;

                case 4: PathToEntityImage.Text = "../../../images/monsters.png"; 
                    break;

                case 5: PathToEntityImage.Text = "../../../images/huntMode.png";
                    break;
                case 6: PathToEntityImage.Text = "../../../images/flower.png"; //change to spawn
                    break;
                default:
                    return;
            }
        }

        private void OverlayCanvas_DeleteObjectHandler(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(OverlayCanvas);

            int column = (int)(clickPoint.X / Map.TileSizeWidth);
            int row = (int)(clickPoint.Y / Map.TileSizeHeight);

            if (Map.map[row, column] != null)
                Map.map[row, column] = null;
        }

        private void LoadBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            string userPath = PathToBackgroundImage.Text;
            if (File.Exists(userPath))
            {
                Map.ChangeBackgoundSource(userPath);
            }
            else
                MessageBox.Show("File does not exist.");
        }

        private void DefaultBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Map.ChangeBackgoundSource("../../../images/background.png");
        }
    }
}