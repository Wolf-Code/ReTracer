
using ReTracer.Rendering;
using ReTracer.Rendering.Materials;
using ReTracer.Rendering.Objects;

namespace ReTracer.Abstract
{
    public abstract class GraphicsObject
    {
        public Vector3 Position { set; get; }
        public Material Material { set; get; }

        protected GraphicsObject( )
        {
            this.Material = new Diffuse { Color = new PixelColor( 1f ), Emission = new PixelColor( 0f ) };
        }

        public abstract Intersection CheckIntersection( Ray R );

        public abstract Vector3 SamplePosition( );
    }
}