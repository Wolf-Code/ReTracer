using System.ComponentModel;

namespace ReTracer.Rendering
{
    public class Vector2
    {
        public float X { set; get; }
        public float Y { set; get; }

        [Browsable( false )]
        public int IntX
        {
            get { return ( int ) X; }
        }

        [Browsable( false )]
        public int IntY
        {
            get { return ( int ) Y; }
        }

        public Vector2( int Width, int Height )
        {
            this.X = Width;
            this.Y = Height;
        }
    }
}