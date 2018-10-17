﻿using System;
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

namespace Polygon_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon polygon = new Polygon();
        WriteableBitmap bitmap;

        Vertex firstVertex = null;

        bool drawMode = true;
        bool moveMode = false;
        Vertex movedVertex;

        public MainWindow()
        {
            InitializeComponent();
            Canvas.Source = bitmap;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (drawMode)
            {
                Vertex clickedVertex = polygon.GetClickedVertex(p);

                if (clickedVertex != null && polygon.Vertices.Count >= 3 && clickedVertex == firstVertex)
                {
                    drawMode = false;
                    DrawPolygon();
                    return;
                }

                Vertex v = new Vertex(p);
                polygon.AddVertex(v);
                DrawPolygon();

                if (firstVertex == null)
                    firstVertex = v;
            }
            else
            {

                Vertex clickedVertex = polygon.GetClickedVertex(p);

                if (clickedVertex != null)
                {
                    moveMode = true;
                    movedVertex = clickedVertex;
                }
                else
                {
                    Vertex clickedSideFirstVertex = polygon.GetClickedSideFirstVertex(p);
                    if (clickedSideFirstVertex != null)
                    {
                        polygon.AddVertexAfter(clickedSideFirstVertex);
                        DrawPolygon();
                    }
                }
            }
        }

        private void DrawLine(MyLine line)
        {
            foreach (var point in line.Points)
            {
                bitmap.SetPixel((int)point.X, (int)point.Y, Colors.Black);
            }
        }

        private void DrawDot(Point p, int size)
        {
            bitmap.DrawEllipse((int)p.X - size / 2, (int)p.Y - size / 2, (int)p.X + size / 2, (int)p.Y + size / 2, Colors.Black);
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (!drawMode)
            {
                Vertex clickedVertex = polygon.GetClickedVertex(p);
                if (clickedVertex != null)
                {
                    polygon.RemoveVertex(clickedVertex);
                    DrawPolygon();
                }
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(Canvas);
            
            if (moveMode)
            {
                moveMode = false;
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }

        private void DrawPolygon()
        {
            bitmap.Clear();

            foreach(Vertex v in polygon.Vertices.Enumerate())
            {
                DrawDot(new Point(v.X, v.Y), Constants.PointSize);
                if (polygon.Vertices.Count >= 2)
                {
                    if (drawMode && v.Next == firstVertex)
                        return;
                    var line = new MyLine(new Point(v.X, v.Y), new Point(v.Next.X, v.Next.Y));
                    line.Bresenham();
                    DrawLine(line);
                }
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(Canvas);

            if (moveMode)
            {
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayout();

            Canvas.Width = SystemParameters.PrimaryScreenWidth;
            Canvas.Height = SystemParameters.PrimaryScreenHeight;

            bitmap = new WriteableBitmap((int)1, (int)1, 96, 96, PixelFormats.Bgra32, null);
            Canvas.Source = bitmap;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bitmap = bitmap.Resize((int)e.NewSize.Width, (int)e.NewSize.Height,
                WriteableBitmapExtensions.Interpolation.Bilinear);
            Canvas.Source = bitmap;
        }
    }
}
