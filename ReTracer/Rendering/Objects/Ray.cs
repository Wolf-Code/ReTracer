using ReTracer.Abstract;

namespace ReTracer.Rendering.Objects
{
    public class Ray
    {
        public Vector3 Start;
        public Vector3 Direction;
        public uint Depth;

        public Intersection CheckIntersection( Scene S )
        {
            Intersection Res = new Intersection( );
            foreach ( GraphicsObject Obj in S.Objects )
            {
                Intersection Temp = Obj.CheckIntersection( this );
                if ( !Res.Hit || ( Temp.Hit && Temp.Distance < Res.Distance ) )
                    Res = Temp;
            }

            return Res;
        }

        public override string ToString( )
        {
            return this.Start + " --=-- " + this.Direction;
        }
    }
}
