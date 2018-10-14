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

        Vertex lastVertex = null;

        bool drawMode = true;
        bool moveMode = false;
        Vertex movedVertex;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);

            if (drawMode)
            {
                var result = polygon.CheckClickTarget(p, out object vertex);

                if (result == ClickTarget.FirstVertex)
                {
                    DrawSide(lastVertex, ((Vertex)vertex));
                    drawMode = false;
                    return;
                }

                Ellipse dot = CreateDot(p, Constants.PointSize);
                DrawDot(dot);
                Vertex v = new Vertex(p, dot);
                polygon.Vertexes.Add(v);

                if (lastVertex != null)
                {
                    DrawSide(lastVertex, v);
                }

                lastVertex = v;
            }
            else
            {
                var result = polygon.CheckClickTarget(p, out object vertex);

                if (result == ClickTarget.Vertex || result == ClickTarget.FirstVertex)
                {
                    moveMode = true;
                    movedVertex = (Vertex)vertex;
                }
                if (polygon.CheckClickTarget(p, out object side) == ClickTarget.Side)
                {
                    Side clickedSide = (Side)side;
                    polygon.AddVertex(clickedSide);
                    DrawPolygon();
                }

            }
        }

        private void DrawSide(Vertex v1, Vertex v2)
        {
            Point p1 = v1.P, p2 = v2.P;
            Line line = CreateLine(p1, p2);
            DrawLine(line);
            Point middle = new Point(p1.X / 2 + p2.X / 2, p1.Y / 2 + p2.Y / 2);
            Ellipse dot = CreateDot(middle, Constants.MiddlePointSize);
            DrawDot(dot);

            polygon.Sides.Add(new Side(v1, middle, v2, line));
        }

        private void DrawLine(Line line)
        {
            Canvas.Children.Add(line);
        }

        private static Line CreateLine(Point p1, Point p2)
        {
            return new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y
            };
        }

        private void DrawDot(Ellipse dot)
        {
            Canvas.Children.Add(dot);
        }

        private static Ellipse CreateDot(Point p, int size)
        {
            Ellipse currentDot = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3
            };
            currentDot.Height = size;
            currentDot.Width = size;
            currentDot.Fill = new SolidColorBrush(Colors.Black);
            currentDot.Margin = new Thickness(p.X - size / 2, p.Y - size / 2, 0, 0);
            return currentDot;
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);

            if (!drawMode)
            {
                if (polygon.CheckClickTarget(p, out object vertex) == ClickTarget.Vertex ||
                    polygon.CheckClickTarget(p, out vertex) == ClickTarget.FirstVertex)
                {
                    polygon.RemoveVertex((Vertex)vertex);
                    DrawPolygon();
                }
            }
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);

            if (moveMode)
            {
                moveMode = false;
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }

        private void DrawPolygon()
        {
            Canvas.Children.Clear();
            foreach (var side in polygon.Sides)
            {
                DrawLine(side.Line);
                DrawDot(CreateDot(side.Middle, Constants.MiddlePointSize));
                DrawDot(side.V2.Dot);
            }
            Image image = new Image() { Source = Constants.NoConstraintImage, Width = 10, Height = 10 };
            Canvas.Children.Add(image);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(this);

            if (moveMode)
            {
                polygon.MoveVertex(movedVertex, p);
                DrawPolygon();
            }
        }
    }
}