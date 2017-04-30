using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rectangles.Models;
using Rectangles.Views;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace Rectangles.Controllers
{
    class BoxCreate
    {
        private MainWindow window;
        private List<BoxDrawing> boxes;
        private Random random;
        private int selected;
        private List<string> logs;

        public BoxCreate(MainWindow mw)
        {
            window = mw;
            boxes = new List<BoxDrawing>();
            random = new Random();
            logs = new List<string>();
            changeBgColor();
            addBox();
            addBox();

            // switching boxes sets up the shade of each box
            selected = 1;
            switchBox();
        }

        public void addBox()
        {
            BoxDrawing box = boxGenerate();
            if (box == null)
            {
                logs.Add("Box is null");
                return;
            }

            boxes.Add(box);
        }

        public void removeBox()
        {
            BoxDrawing box = getSelectedBox();
            if (box == null)
            {
                logs.Add("Box is null");
                return;
            }

            Rectangle rect = box.getObject();
            if (rect == null)
            {
                logs.Add("Box does not have a rectangle in it");
                return;
            }

            boxes.Remove(box);
            window.canvas.Children.Remove(rect);
            switchBox();
        }

        public void changeBgColor()
        {
            Brush brush = randomBrush();
            if (brush == null)
            {
                logs.Add("Brush is null");
                return;
            }
            window.canvas.Background = brush;
        }

        public void changeBoxColor()
        {
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Selected Box is null");
                return;
            }

            Brush brush = randomBrush();
            if (brush == null)
            {
                logs.Add("Brush is null");
                return;
            }

            selectedBox.setColor(brush);
        }

        public void displayBoxes()
        {
            if (boxes == null)
            {
                logs.Add("Boxes is null");
                return;
            }
            foreach (BoxDrawing box in boxes)
            {
                Rectangle rect = box.getObject();
                if (rect == null)
                {
                    logs.Add("Box does not have a rectangle in it");
                    continue;
                }
                double top = box.top;
                double left = box.left;
                if (Double.IsNaN(top) || Double.IsNaN(left))
                {
                    logs.Add("Box does not have a top or left coordinate");
                    continue;
                }
                if (window.canvas.Children.IndexOf(rect) == -1)
                    window.canvas.Children.Add(rect);
                Canvas.SetTop(rect, top);
                Canvas.SetLeft(rect, left);
            }

            bool adj = adjacency();
            bool con = containment();
            bool inter = intersection();

            // test for adjacency and containment first
            // since intersection tests for these things, too
            if (adj)
                window.results.Content = "ADJACENCY";
            else if (con)
                window.results.Content = "CONTAINMENT";
            else if (inter)
                window.results.Content = "INTERSECTION";
            else
                window.results.Content = "";
        }

        public void switchBox()
        {
            // old selected box
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Old Selected Box is null");
                return;
            }

            // dim out old selected box
            selectedBox.setShade(0.5);
            
            // switch selection
            if (selected == 0)
                selected = 1;
            else
                selected = 0;

            // new selected box
            selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("New Selected Box is null");
                return;
            }

            // light up new selected box
            selectedBox.setShade(0.75);
        }

        public void moveUp()
        {
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Selected Box is null");
                return;
            }

            double top = selectedBox.top;

            if (Double.IsNaN(top))
            {
                logs.Add("Could not move up");
                return;
            }

            // check if we've hit the top of the screen
            if (top <= 0)
                return;

            selectedBox.top--;
            boxes[selected] = selectedBox;
        }

        public void moveLeft()
        {
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Selected Box is null");
                return;
            }

            double left = selectedBox.left;

            if (Double.IsNaN(left))
            {
                logs.Add("Could not move left");
                return;
            }

            // check if we've hit the left side of the screen
            if (left <= 0)
                return;

            selectedBox.left--;
            boxes[selected] = selectedBox;
        }

        public void moveRight()
        {
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Selected Box is null");
                return;
            }

            double left = selectedBox.left;
            double right = selectedBox.getRight();
            double width = window.canvas.Width;

            if (Double.IsNaN(left) || Double.IsNaN(right) || Double.IsNaN(width))
            {
                logs.Add("Could not move right");
                return;
            }

            // check if we've hit the right side of the screen
            if (right >= width)
                return;

            selectedBox.left++;
            boxes[selected] = selectedBox;
        }

        public void moveDown()
        {
            BoxDrawing selectedBox = getSelectedBox();
            if (selectedBox == null)
            {
                logs.Add("Selected Box is null");
                return;
            }

            double top = selectedBox.top;
            double bottom = selectedBox.getBottom();
            double height = window.canvas.Height;

            if (Double.IsNaN(top) || Double.IsNaN(bottom) || Double.IsNaN(height))
            {
                logs.Add("Could not move down");
                return;
            }

            // check if we've hit the bottom of the screen
            if (bottom >= height)
                return;

            selectedBox.top++;
            boxes[selected] = selectedBox;
        }

        public void printLogs()
        {
            string logs = getLogs();
            LogView logView = new LogView();
            if (!string.IsNullOrWhiteSpace(logs))
                logView.logPrint.Text = logs;
            else
                logView.logPrint.Text = "No Logs Found";
            logView.Show();
        }

        private BoxDrawing getSelectedBox()
        {
            if (boxes == null || selected > boxes.Count - 1 || selected < 0)
            {
                logs.Add("Selected Box is out of bounds or Box list is null");
                return null;
            }

            BoxDrawing selectedBox = boxes[selected];
            if (selectedBox == null)
                logs.Add("Selected Box is null");
            return selectedBox;
        }

        private BoxDrawing getStaticBox()
        {
            int other = 0;
            if (selected == 0)
                other = 1;

            if (boxes == null || other > boxes.Count - 1)
            {
                logs.Add("Other Box is out of bounds or Box list is null");
                return null;
            }

            BoxDrawing staticBox = boxes[other];
            if (staticBox == null)
                logs.Add("Other Box is null");
            return staticBox;
        }

        private bool adjacency()
        {
            bool result = false;
            if (boxes.Count < 2)
            {
                logs.Add("There aren't enough boxes to compute adjacency");
                return false;
            }

            BoxDrawing selectedBox = getSelectedBox();
            BoxDrawing staticBox = getStaticBox();

            if (selectedBox == null || staticBox == null)
            {
                logs.Add("Could not get boxes for computing adjacency");
                return false;
            }

            double hDiff1 = selectedBox.top - staticBox.getBottom();
            double hDiff2 = staticBox.top - selectedBox.getBottom();
            double wDiff1 = selectedBox.left - staticBox.getRight();
            double wDiff2 = staticBox.left - selectedBox.getRight();

            if (Double.IsNaN(hDiff1) || Double.IsNaN(hDiff2) || Double.IsNaN(wDiff1) || Double.IsNaN(wDiff2))
            {
                logs.Add("Could not compute adjacency");
                return false;
            }

            if ((hDiff1 > -1 && hDiff1 < 2 && hDiff2 < 0 && wDiff1 < 1 && wDiff2 < 1) ||
                (hDiff2 > -1 && hDiff2 < 2 && hDiff1 < 0 && wDiff1 < 1 && wDiff2 < 1) ||
                (wDiff1 > -1 && wDiff1 < 2 && wDiff2 < 0 && hDiff1 < 1 && hDiff2 < 1) ||
                (wDiff2 > -1 && wDiff2 < 2 && wDiff1 < 0 && hDiff1 < 1 && hDiff2 < 1))
                result = true;

            return result;
        }

        private bool containment()
        {
            bool result = false;
            if (boxes.Count < 2)
            {
                logs.Add("Not enough objects.");
                return false;
            }

            BoxDrawing selectedBox = getSelectedBox();
            BoxDrawing staticBox = getStaticBox();

            if (selectedBox == null || staticBox == null)
            {
                logs.Add("Could not retrieve boxes.");
                return false;
            }

            double tDiff = selectedBox.top - staticBox.top;
            double bDiff = staticBox.getBottom() - selectedBox.getBottom();
            double lDiff = selectedBox.left - staticBox.left;
            double rDiff = staticBox.getRight() - selectedBox.getRight();

            if (Double.IsNaN(tDiff) || Double.IsNaN(bDiff) || Double.IsNaN(lDiff) || Double.IsNaN(rDiff))
            {
                logs.Add("Could not calculate containment");
                return false;
            }

            if ((tDiff > -1 && bDiff > -1 && lDiff > -1 && rDiff > -1) ||
                (tDiff < 1 && bDiff < 1 && lDiff < 1 && rDiff < 1))
                result = true;

            return result;
        }

        // check intersection, adjacency and containment
        private bool intersection()
        {
            bool result = false;
            if (boxes.Count < 2)
            {
                logs.Add("Not enough objects.");
                return false;
            }

            BoxDrawing selectedBox = getSelectedBox();
            BoxDrawing staticBox = getStaticBox();

            if (selectedBox == null || staticBox == null)
            {
                logs.Add("Could not retrieve boxes.");
                return false;
            }

            double t1 = selectedBox.top;
            double t2 = staticBox.top;
            double l1 = selectedBox.left;
            double l2 = staticBox.left;
            double r1 = selectedBox.getRight();
            double r2 = staticBox.getRight();
            double b1 = selectedBox.getBottom();
            double b2 = staticBox.getBottom();

            if (Double.IsNaN(t1) || Double.IsNaN(t2) || Double.IsNaN(l1) || Double.IsNaN(l2) ||
                Double.IsNaN(r1) || Double.IsNaN(r2) || Double.IsNaN(b1) || Double.IsNaN(b2))
            {
                logs.Add("Could not calculate intersection");
                return false;
            }

            if (!(l2 > r1 || r2 < l1 || t2 > b1 || b2 < t1))
                result = true;

            return result;
        }

        private BoxDrawing boxGenerate()
        {
            Brush brush = randomBrush();
            double top = Math.Round(randomTop());
            double left = Math.Round(randomLeft());
            double height = Math.Round(randomHeight(top));
            double width = Math.Round(randomWidth(left));
            BoxDrawing box = new BoxDrawing(brush, top, left, height, width);
            if (box == null)
                logs.Add("Could not create a new object");
            return box;
        }

        private Brush randomBrush()
        {
            Brush result = Brushes.Transparent;
            Type brushesType = typeof(Brushes);
            PropertyInfo[] properties = brushesType.GetProperties();
            int next = random.Next(properties.Length);
            result = (Brush)properties[next].GetValue(null, null);
            if (result == null)
                logs.Add("Could not get random brush color");
            return result;
        }

        private double randomTop()
        {
            double result = random.NextDouble() * (window.canvas.Height - 10);
            if (Double.IsNaN(result))
                logs.Add("Could not get random top for box");
            return result;
        }

        private double randomLeft()
        {
            double result = random.NextDouble() * (window.canvas.Width - 10);
            if (Double.IsNaN(result))
                logs.Add("Could not get random left for box");
            return result;
        }

        private double randomHeight(double top)
        {
            double result = random.NextDouble() * (window.canvas.Height - top);
            result = result < 10 ? 10 : result;
            if (Double.IsNaN(result))
                logs.Add("Could not get random height for box");
            return result;
        }

        private double randomWidth(double left)
        {
            double result = random.NextDouble() * (window.canvas.Width - left);
            result = result < 10 ? 10 : result;
            if (Double.IsNaN(result))
                logs.Add("Could not get random width for box");
            return result;
        }

        private string getLogs()
        {
            string result = "";
            if (logs == null || logs.Count == 0)
                result = "No Logs Found";
            else
                foreach (string log in logs)
                    result += log + "\n";
            return result;
        }
    }
}
