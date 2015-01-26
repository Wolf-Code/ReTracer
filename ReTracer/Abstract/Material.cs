using ReTracer.Rendering;
using ReTracer.Rendering.Objects;

namespace ReTracer.Abstract
{
    public abstract class Material
    {
        public PixelColor Color { set; get; }
        public PixelColor Emission { set; get; }

        public abstract float BRDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );
        public abstract float CosTheta( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );
        public abstract float PDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );
        public abstract Vector3 NewDirection( Vector3 RayIn, Intersection Intersection );
        public abstract PixelColor ColorAddition( PixelColor LightInput );

        public bool IsLightSource
        {
            get { return !Emission.IsBlack; }
        }

        protected Material( )
        {
            this.Color = new PixelColor( 1f );
            this.Emission = new PixelColor( 0f );
        }
    }
}