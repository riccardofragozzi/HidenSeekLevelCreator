using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HidenSeekLevelCreator
{

    public partial class MainWindow : Window
    {
        private static Color INACTIVE_COLOR = Color.FromRgb(25, 25, 25);
        private static Color BORDER_COLOR = Color.FromRgb(100, 100, 100);
        private static Color ACTIVE_COLOR = Color.FromRgb(204, 153, 0);

        private Dictionary<Point, Border> cells = new Dictionary<Point, Border>();
        private OutputWindow outputWindow = new OutputWindow() { Visibility = Visibility.Collapsed };
        private Point currPoint = new Point(0, 0);
        public MainWindow()
        {
            InitializeComponent();
            for(int y=0; y<1080/20; y++)
            {
                for(int x=0; x<1920/20; x++)
                {
                    Border cell = new Border()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Background = new SolidColorBrush(INACTIVE_COLOR),
                        BorderBrush = new SolidColorBrush(BORDER_COLOR),
                        BorderThickness = new Thickness(0.2),
                        CornerRadius = new CornerRadius(3),
                        Margin = new Thickness(x*17, y*17, 0, 0),
                        Width = 17,
                        Height = 17
                    };

                    cell.MouseMove += (s, e) =>
                    {
                        if (draw_mode)
                        {
                            if (e.LeftButton == MouseButtonState.Pressed && cell.Tag == null)
                            {
                                handleColorChange(cell);
                                cell.Tag = "handled";
                            }
                        }

                        if (!draw_mode)
                        {
                            double xOffset = e.GetPosition(cell).X > cell.ActualWidth / 2 ? cell.ActualWidth : 0;
                            double yOffset = e.GetPosition(cell).Y > cell.ActualHeight / 2 ? cell.ActualHeight : 0;
                            cursorBorder.Margin = new Thickness(cell.Margin.Left + xOffset - 5, cell.Margin.Top + yOffset - 5, 0, 0);
                            currPoint = new Point(Math.Round((cell.Margin.Left + xOffset) / 17.0, 0), Math.Round((cell.Margin.Top + yOffset) / 17.0));
                            coordinateLbl.Content = currPoint.X + "," + currPoint.Y;
                        }
                    };

                    cell.MouseLeave += (s, e) =>
                    {
                        if (draw_mode)
                        {
                            cell.Tag = null;
                        }
                    };

                    cell.MouseDown += (s, e) =>
                    {
                        if (draw_mode)
                        {
                            handleColorChange((Border)s);
                            cell.Tag = "handled";
                        }
                        if (!draw_mode)
                        {
                            outputWindow.appendPoint(currPoint);
                        }
                    };

                    mainGrid.Children.Add(cell);
                    cells[new Point(x, y)] = cell;
                }
            }

            LocationChanged += (s, e) =>
            {
                outputWindow.Top = Top;
                outputWindow.Left = Left + Width;
            };

            SizeChanged += (s, e) =>
            {
                outputWindow.Top = Top;
                outputWindow.Height = Height;
                outputWindow.Left = Left + Width;
            };

            Closing += (s, e) =>
            {
                outputWindow.Close();
            };

            cursorBorder.MouseDown += (s, e) =>
            {
                if (!draw_mode)
                {
                    outputWindow.appendPoint(currPoint);
                }
            };

            funcBtn_Click(null, null);
            funcBtn_Click(null, null);
        }

        private void handleColorChange(Border cell)
        {
            if (isActive(cell))
            {
                setActive(cell, false);
            }
            else
            {
                setActive(cell, true);
            }
        }

        private bool isActive(Border cell)
        {
            return ((SolidColorBrush)cell.Background).Color.Equals(ACTIVE_COLOR);
        }

        private void setActive(Border cell, bool active)
        {
            cell.Background = new SolidColorBrush(active ? ACTIVE_COLOR : INACTIVE_COLOR);
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            string filename = string.Empty;
            OpenFileDialog open = new OpenFileDialog() { Filter = "Configuration files|*.hsc" };
            if (open.ShowDialog().Value)
            {
                filename = open.FileName;
            }

            if (filename == string.Empty)
            {
                return;
            }

            int currKey = 0;
            foreach (byte ssbyte in System.IO.File.ReadAllBytes(filename))
            {
                setActive(cells[cells.Keys.ElementAt(currKey++)], ssbyte == 0xF);
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string filename = string.Empty;
            SaveFileDialog save = new SaveFileDialog() { Filter = "Configuration files|*.hsc" };
            if (save.ShowDialog().Value)
            {
                filename = save.FileName;
            }

            if(filename == string.Empty)
            {
                return;
            }

            List<byte> bytes = new List<byte>();
            foreach(Point cellPos in cells.Keys)
            {
                byte ssbyte = (byte)(isActive(cells[cellPos]) ? 0xF : 0xA);
                bytes.Add(ssbyte);
            }
            System.IO.File.WriteAllBytes(filename, bytes.ToArray());
        }
        private bool draw_mode=true;
        private void funcBtn_Click(object sender, RoutedEventArgs e)
        {
            draw_mode = !draw_mode;
            funcBtn.Content = draw_mode ? "Disegna" : "Seleziona";
            cursorBorder.Visibility = draw_mode ? Visibility.Collapsed : Visibility.Visible;
            coordinateLbl.Visibility = draw_mode ? Visibility.Collapsed : Visibility.Visible;
            outputWindow.Visibility = draw_mode ? Visibility.Collapsed : Visibility.Visible;
            outputWindow.Hide();
            outputWindow.Show();
        }
    }
}
