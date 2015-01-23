using System.Linq;

namespace ReTracer.Rendering.Objects
{
    public class Ray
    {
        public Vector3 Start;
        public Vector3 Direction;
        public uint Depth;

        public Intersection CheckIntersection( Scene S )
        {
            return S.Objects
                .Select( O => O.CheckIntersection( this ) )
                .Where( O => O.Hit )
                .OrderBy( O => O.Distance )
                .FirstOrDefault( )
                   ?? new Intersection( );
        }
    }
}
