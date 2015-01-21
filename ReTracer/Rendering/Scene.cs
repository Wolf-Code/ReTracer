
using System.Collections.Generic;
using ReTracer.Abstract;

namespace ReTracer.Rendering
{
    public class Scene
    {
        public Camera Camera { set; get; }
        public List<GraphicsObject> Objects { set; get; }

        public Scene( int X, int Y )
        {
            this.Camera = new Camera( X, Y );
            Objects = new List<GraphicsObject>( );
        }

        public Scene( Vector2 Resolution ) : this( ( int ) Resolution.X, ( int ) Resolution.Y )
        {

        }

        public void SetCamera( Vector3 Position, Angle Angle )
        {
            this.Camera.Position = Position;
            this.Camera.Angle = Angle;
        }
    }
}
