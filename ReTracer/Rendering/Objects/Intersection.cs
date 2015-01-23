using ReTracer.Abstract;

namespace ReTracer.Rendering.Objects
{
    public class Intersection
    {
        public bool Hit;
        public float Distance;
        public Vector3 Position;
        public Vector3 Normal;
        public GraphicsObject Object;

        public Material Material
        {
            get { return this.Object.Material; }
        }

        public Vector3 NewStart
        {
            get { return this.Position + this.Normal * MathHelper.Theta; }
        }
    }
}