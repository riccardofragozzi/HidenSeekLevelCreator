using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HidenSeekLevelCreator
{
    /// <summary>
    /// Logica di interazione per OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private Point currOffset = new Point(-1, -1);
        private Point firstVertex = new Point(-1, -1);
        public OutputWindow()
        {
            InitializeComponent();
            setInstruction();
        }

        private void setInstruction()
        {
            if(currOffset.X < 0)
            {
                instructionLbl.Content = "Scegli il punto offset in alto a sinistra del muro";
            }
            else
            {
                instructionLbl.Content = "Scegli tutti i vertici in sequenza";
            }
        }

        public void appendPoint(Point point)
        {
            if(firstVertex.X>=0 && point.Equals(firstVertex))
            {
                MessageBox.Show("Poligono terminato.");
                newPolygonBtn_Click(null, null);
                return;
            }
            if (currOffset.X < 0) 
            {
                currOffset = point;
                txtOut.Text += string.Format("\t<wall position=\"{0}\">", point.X * 17 + "," + point.Y * 17) + Environment.NewLine;
                setInstruction();
                return;
            }
            if (firstVertex.X < 0)
            {
                firstVertex = point;
            }

            txtOut.Text += string.Format("\t\t<vertex position=\"{0}\"/>", (point.X - currOffset.X) * 17 + "," + (point.Y - currOffset.Y) * 17) + Environment.NewLine;
            setInstruction();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            switch(MessageBox.Show("Sicuro di voler eliminare tutto?", "", MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.Yes:
                    txtOut.Text = "";
                    currOffset = new Point(-1, -1);
                    firstVertex = new Point(-1, -1);
                    setInstruction();
                    break;

                default:
                    break;
            }
        }

        private void newPolygonBtn_Click(object sender, RoutedEventArgs e)
        {
            currOffset = new Point(-1, -1);
            firstVertex = new Point(-1, -1);
            setInstruction();
            txtOut.Text += "\t</wall>" + Environment.NewLine + Environment.NewLine;
        }

        private void copyOutputBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOut.Text);
        }
    }
}
