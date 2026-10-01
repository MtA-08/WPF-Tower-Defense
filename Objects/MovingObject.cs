using System.Windows.Media;

namespace Tower_Defense.Objects
{
    public class MovingObject : BaseObject
    {
        public double Speed { get; set; }
		public int HitboxRadius { get; set; }

        public MovingObject() { }
		public MovingObject(double posX, double posY, string svgName, double speed, int hitboxRadius) : base(posX, posY, svgName)
        {
            Speed = speed;  
            HitboxRadius = hitboxRadius;
        }

	}
}
