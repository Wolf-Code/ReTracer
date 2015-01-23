using ReTracer.Rendering;
using ReTracer.Rendering.Objects;

namespace ReTracer.Abstract
{
    public abstract class Material
    {
        public PixelColor Color;
        public PixelColor Emission;

        public abstract float BRDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );
        public abstract float CosTheta( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );
        public abstract float PDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection );

        public bool IsLightSource
        {
            get { return !Emission.IsBlack; }
        }
    }
}