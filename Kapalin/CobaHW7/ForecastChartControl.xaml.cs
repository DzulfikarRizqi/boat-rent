using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class ForecastChartControl : UserControl
    {
        public ForecastChartControl()
        {
            InitializeComponent();
        }

        public List<ChartPoint> ChartPoints
        {
            get { return (List<ChartPoint>)GetValue(ChartPointsProperty); }
            set { SetValue(ChartPointsProperty, value); }
        }

        public static readonly DependencyProperty ChartPointsProperty =
            DependencyProperty.Register("ChartPoints", typeof(List<ChartPoint>), typeof(ForecastChartControl),
                new PropertyMetadata(null, OnChartPointsChanged));

        public string ChartColor
        {
            get { return (string)GetValue(ChartColorProperty); }
            set { SetValue(ChartColorProperty, value); }
        }

        public static readonly DependencyProperty ChartColorProperty =
            DependencyProperty.Register("ChartColor", typeof(string), typeof(ForecastChartControl),
                new PropertyMetadata("#FF9800", OnChartColorChanged));

        private static void OnChartPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ForecastChartControl)d).RedrawChart();
        }

        private static void OnChartColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ForecastChartControl)d).RedrawChart();
        }

        private void RedrawChart()
        {
            ChartCanvas.Children.Clear();

            if (ChartPoints == null || ChartPoints.Count < 2)
                return;

            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
                return;

            double padding = 40;
            double chartWidth = canvasWidth - (padding * 2);
            double chartHeight = canvasHeight - (padding * 2);

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            foreach (var point in ChartPoints)
            {
                minValue = Math.Min(minValue, point.Value);
                maxValue = Math.Max(maxValue, point.Value);
            }

            double valueRange = maxValue - minValue;
            if (valueRange == 0) valueRange = 1;
            minValue -= valueRange * 0.1;
            maxValue += valueRange * 0.1;
            valueRange = maxValue - minValue;

            List<Point> polylinePoints = new List<Point>();
            List<Point> polygonPoints = new List<Point>();

            for (int i = 0; i < ChartPoints.Count; i++)
            {
                double x = padding + (i / (double)(ChartPoints.Count - 1)) * chartWidth;
                double normalizedValue = (ChartPoints[i].Value - minValue) / valueRange;
                double y = canvasHeight - padding - (normalizedValue * chartHeight);

                polylinePoints.Add(new Point(x, y));
                polygonPoints.Add(new Point(x, y));
            }

            polygonPoints.Add(new Point(padding + chartWidth, canvasHeight - padding));
            polygonPoints.Add(new Point(padding, canvasHeight - padding));

            Color brushColor = (Color)ColorConverter.ConvertFromString(ChartColor);
            LinearGradientBrush gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(100, brushColor.R, brushColor.G, brushColor.B), 0.0),
                    new GradientStop(Color.FromArgb(30, brushColor.R, brushColor.G, brushColor.B), 1.0)
                }
            };

            Polygon polygon = new Polygon
            {
                Points = new PointCollection(polygonPoints),
                Fill = gradientBrush
            };
            ChartCanvas.Children.Add(polygon);

            Polyline polyline = new Polyline
            {
                Points = new PointCollection(polylinePoints),
                Stroke = new SolidColorBrush(brushColor),
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };
            ChartCanvas.Children.Add(polyline);

            foreach (var point in polylinePoints)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = new SolidColorBrush(brushColor)
                };
                Canvas.SetLeft(ellipse, point.X - 3);
                Canvas.SetTop(ellipse, point.Y - 3);
                ChartCanvas.Children.Add(ellipse);
            }

            TextBlock minLabel = new TextBlock
            {
                Text = Math.Round(minValue).ToString(),
                Foreground = new SolidColorBrush(Colors.Gray),
                FontSize = 11
            };
            Canvas.SetLeft(minLabel, 5);
            Canvas.SetTop(minLabel, canvasHeight - padding - 5);
            ChartCanvas.Children.Add(minLabel);

            TextBlock maxLabel = new TextBlock
            {
                Text = Math.Round(maxValue).ToString(),
                Foreground = new SolidColorBrush(Colors.Gray),
                FontSize = 11
            };
            Canvas.SetLeft(maxLabel, 5);
            Canvas.SetTop(maxLabel, padding - 10);
            ChartCanvas.Children.Add(maxLabel);

            for (int i = 0; i < ChartPoints.Count; i++)
            {
                double x = padding + (i / (double)(ChartPoints.Count - 1)) * chartWidth;
                TextBlock timeLabel = new TextBlock
                {
                    Text = ChartPoints[i].TimeLabel,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(timeLabel, x - 20);
                Canvas.SetTop(timeLabel, canvasHeight - 25);
                ChartCanvas.Children.Add(timeLabel);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawChart();
        }
    }
}
