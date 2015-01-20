
using ReTracer.Rendering;
using ReTracer.Rendering.Objects;

namespace ReTracer.Abstract
{
    public abstract class GraphicsObject
    {
        public Vector3 Position { set; get; }

        public abstract Intersection CheckIntersection( Ray R );
    }
}