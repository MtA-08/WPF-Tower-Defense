using System.Windows.Media;

namespace Tower_Defense.Objects
{
    public class StaticObject : BaseObject
    {
        public StaticObject() { }
        public StaticObject(double posX, double posY, string svgName) : base(posX, posY, svgName)
        { 
        }
	}
}
