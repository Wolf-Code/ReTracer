using ReTracer.Abstract;
using ReTracer.Rendering.Objects;

namespace ReTracer.Rendering.Materials
{
    public class Diffuse : Material
    {
        public override float BRDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            return MathHelper.OneOverPI;
        }

        public override float CosTheta( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            return Intersection.Normal.Dot( RayOut );
        }

        public override float PDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            return MathHelper.OneOverTwoPI;
        }
    }
}
