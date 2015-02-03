using System.Collections.Generic;
using System.Linq;
using ReTracer.Abstract;

namespace ReTracer.Rendering
{
    public class Scene
    {
        public Camera Camera { set; get; }
        protected List<GraphicsObject> m_Objects { set; get; }
        public GraphicsObject [ ] Objects;
        private GraphicsObject [ ] Lights;

        public GraphicsObject RandomLight
        {
            get
            {
                return Lights[ ThreadRandom.Next( 0, Lights.Length - 1 ) ];
            }
        }

        public Scene( int X, int Y )
        {
            this.Camera = new Camera( X, Y );
            m_Objects = new List<GraphicsObject>( );
        }

        public Scene( Vector2 Resolution ) : this( ( int ) Resolution.X, ( int ) Resolution.Y )
        {

        }

        private void BuildLightArray( )
        {
            Lights = Objects.Where( O => O.Material.IsLightSource ).ToArray( );
        }

        public void AddObject( GraphicsObject Object )
        {
            this.m_Objects.Add( Object );
            this.Objects = m_Objects.ToArray( );
            this.BuildLightArray( );
        }

        public void SetCamera( Vector3 Position, Angle Angle )
        {
            this.Camera.Position = Position;
            this.Camera.Angle = Angle;
        }
    }
}
