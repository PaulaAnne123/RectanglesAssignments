using System;
using System.Collections.Generic;
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
using Rectangles.Controllers;

namespace Rectangles.Views
{
    public partial class MainWindow : Window
    {
        BoxCreate boxCreate;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            this.Title = "Rectangles";
            boxCreate = new BoxCreate(this);
            boxCreate.displayBoxes();
            CanvasBorder.BorderThickness = new Thickness(1);
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    boxCreate.moveUp();
                    break;
                case Key.Left:
                case Key.A:
                    boxCreate.moveLeft();
                    break;
                case Key.Right:
                case Key.D:
                    boxCreate.moveRight();
                    break;
                case Key.Down:
                case Key.S:
                    boxCreate.moveDown();
                    break;
                case Key.Delete:
                    boxCreate.addBox();
                    boxCreate.removeBox();
                    break;
                case Key.Space:
                    boxCreate.switchBox();
                    break;
                case Key.C:
                    boxCreate.changeBoxColor();
                    break;
                case Key.B:
                    boxCreate.changeBgColor();
                    break;
                case Key.L:
                    boxCreate.printLogs();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
            boxCreate.displayBoxes();
        }
    }
}
