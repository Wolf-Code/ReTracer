
namespace ReTracer.Rendering
{
    public class Angle
    {
        public float Pitch;
        public float Yaw;
        public float Roll;
        public bool Radians;

        public Matrix4x4 Rotation
        {
            get
            {
                return Matrix4x4.CreateRotationX( Pitch, Radians ) *
                       Matrix4x4.CreateRotationY( Yaw, Radians ) *
                       Matrix4x4.CreateRotationZ( Roll, Radians );
            }
        }

        public Vector3 Right
        {
            get { return Vector3.UnitX * Rotation; }
        }

        public Vector3 Up
        {
            get { return Vector3.UnitY * Rotation; }
        }

        public Vector3 Forward
        {
            get { return Vector3.UnitZ * Rotation; }
        }

        public Angle( float Pitch, float Yaw, float Roll )
        {
            this.Pitch = Pitch;
            this.Yaw = Yaw;
            this.Roll = Roll;
        }
    }
}
