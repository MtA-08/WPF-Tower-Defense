using System.Collections.Generic;
using System.Windows.Media;

namespace Tower_Defense.Objects
{
	public class BaseObject
	{
		public double PosX { get; set; }
		public double PosY { get; set; }
		public string SvgName{ get; set; }

		public BaseObject() { }
		public BaseObject(double posX, double posY, string svgName)
		{
			PosX = posX;
			PosY = posY;
			SvgName = svgName;
		}

		public struct Position
		{
			public double X { get; set; }
			public double Y { get; set; }

			public Position(double x, double y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
