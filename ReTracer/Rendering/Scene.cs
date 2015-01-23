
using System.Collections.Generic;
using System.Linq;
using ReTracer.Abstract;

namespace ReTracer.Rendering
{
    public class Scene
    {
        public Camera Camera { set; get; }
        public List<GraphicsObject> Objects { set; get; }

        public GraphicsObject RandomLight
        {
            get
            {
                GraphicsObject [ ] Lights = Objects.Where( O => O.Material.IsLightSource ).ToArray( );
                return Lights[ ThreadRandom.Next( 0, Lights.Length - 1 ) ];
            }
        }

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
