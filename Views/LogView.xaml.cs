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
using System.Windows.Shapes;

namespace Rectangles.Views
{
    public partial class LogView : Window
    {
        public LogView()
        {
            InitializeComponent();
            this.Title = "Rectangles Debugger Log";
        }
    }
}
