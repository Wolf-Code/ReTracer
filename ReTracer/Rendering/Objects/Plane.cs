using System;
using ReTracer.Abstract;

namespace ReTracer.Rendering.Objects
{
    public class Plane : GraphicsObject
    {
        public Vector3 Normal { set; get; }
        public float Offset { set; get; }

        public Plane( Vector3 Normal, float Offset )
        {
            this.Normal = Normal;
            this.Offset = Offset;
        }

        public override Intersection CheckIntersection( Ray R )
        {
            Intersection Res = new Intersection( );

            float Div = this.Normal.Dot( R.Direction );
            if ( Math.Abs( Div ) < MathHelper.Theta )
                return Res;

            float Distance = -( this.Normal.Dot( R.Start ) - this.Offset ) / Div;
            if ( Distance < 0 )
                return Res;

            Res.Hit = true;
            Res.Distance = Distance;
            Res.Normal = this.Normal;
            Res.Position = R.Start + R.Direction * Distance;
            Res.Object = this;

            return Res;
        }

        public override Vector3 SamplePosition( )
        {
            // TODO: Actual random position
            return this.Normal * this.Offset +
                   new Vector3( ThreadRandom.Next( -20, 20 ), ThreadRandom.Next( -20, 20 ), 0 );
        }
    }
}
