using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Tower_Defense.Objects;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.IO;

namespace Tower_Defense
{
    public class GameVisual : FrameworkElement
    {
        public readonly VisualCollection visuals;
		public Tower Tower { get;  set; }

        public GameVisual()
        {
            visuals = new VisualCollection(this);
            ClipToBounds = true;
		}

		public static DrawingImage LoadSvg(string filePath)
		{
			var settings = new WpfDrawingSettings
			{
				IncludeRuntime = false
			};

			using (var reader = new FileSvgReader(settings))
			{
				DrawingGroup drawing = reader.Read(filePath);

				var image = new DrawingImage(drawing);

				if (image.CanFreeze)
					image.Freeze();

				return image;
			}
		}

		public DrawingVisual AddEnemy(double x, double y, double radius, string svgName)
        {
			var image = LoadSvg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "svg/enemies", svgName));


			var width = 60;
			var height = width * image.Height / image.Width;

			var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
				dc.DrawImage(image, new Rect(-width / 2, -height / 2, width, height));
			}

            visual.Offset = new Vector(x, y);

            visuals.Add(visual);

            return visual;
        }

		public DrawingVisual AddProjectile(double x, double y,double bodyRadius, string svgName)
		{
			var image = LoadSvg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "svg/projectiles", svgName));


			var width = bodyRadius;
			var height = width * image.Height / image.Width;

			var visual = new DrawingVisual();
			using (DrawingContext dc = visual.RenderOpen())
			{
				dc.DrawImage(image, new Rect(-width / 2, -height / 2, width, height));
			}

			visual.Offset = new Vector(x, y);

			visuals.Add(visual);

			return visual;
		}

		public DrawingVisual AddTower(double x, double y, double rangeRadius, string svgName)
		{
			var image = LoadSvg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "svg/towers", svgName));

			var width = 80;
			var height = width * image.Height / image.Width;

			var strokePen = new Pen(Brushes.Black, 0.25);
			var visual = new DrawingVisual();
			var rect =  new Rect(-62.5, -62.5, 125, 125);
			using (var dc = visual.RenderOpen())
			{
				dc.DrawImage(image, new Rect(-width / 2, -height / 2, width, height));

				dc.DrawEllipse(null, strokePen, new Point(0, 0), rangeRadius, rangeRadius);
			}

			visual.Offset = new Vector(x, y);

			visuals.Add(visual);

			return visual;
		}

		public void RemoveVisual(DrawingVisual visual)
        {
            visuals.Remove(visual);
        }

        protected override int VisualChildrenCount { get { return visuals.Count; } }

		protected override Visual GetVisualChild(int index)
		{
            if (index < 0 || index >= visuals.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

			return visuals[index];
		}
	}
}
