using System;
using ReTracer.Abstract;
using ReTracer.Rendering.Objects;

namespace ReTracer.Rendering.Materials
{
    public class Specular : Material
    {
        public float Glossyness { set; get; }

        public override float BRDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            return 1f;
        }

        public override float CosTheta( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            Vector3 Ref = Vector3.Reflect( RayIn, Intersection.Normal );

            return Math.Abs( Ref.Dot( RayOut ) - 1f ) < MathHelper.Theta + Glossyness ? 1f : 0f;
        }

        public override float PDF( Vector3 RayIn, Vector3 RayOut, Intersection Intersection )
        {
            return 1f;
        }

        public override Vector3 NewDirection( Vector3 RayIn, Intersection Intersection )
        {
            Vector3 Ref = Vector3.Reflect( RayIn, Intersection.Normal );
            Vector3 Rand = Vector3.RandomInSameHemisphere( Ref );

            return ( Ref * ( 1f - this.Glossyness ) + Rand * this.Glossyness ).Normalized( );
        }

        public override PixelColor ColorAddition( PixelColor LightInput )
        {
            return PixelColor.Black;
        }
    }
}
