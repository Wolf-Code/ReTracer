
using System.ComponentModel;
using ReTracer.Rendering.Objects;

namespace ReTracer.Rendering
{
    public class Camera
    {
        private float Fov;
        private Vector2 Resolution1;

        public Vector2 Resolution
        {
            set
            {
                Resolution1 = value;
                AspectRatio = ( value.X / value.Y );
            }
            get { return Resolution1; }
        }

        public float AspectRatio { private set; get; }

        public Vector3 Position { set; get; }
        public Angle Angle { set; get; }

        [Browsable( false )]
        public float FOV
        {
            set
            {
                Fov = value;
                FOVDivided = 0.5f / MathHelper.Tan( FOV / 2, false );
            }
            get { return Fov; }
        }

        public float FOVDivided { private set; get; }

        public Camera( int W, int H )
        {
            Resolution = new Vector2( W, H );
            this.Angle = new Angle( 0, 0, 0 );
            this.Position = new Vector3( 0, 0, 0 );
            this.FOV = 90;
        }

        public Ray GetRay( float X, float Y )
        {
            Ray R = new Ray { Start = this.Position };
            Vector3 Dir = this.Angle.Forward * FOVDivided +
                          this.Angle.Right * ( X / this.Resolution.X - 0.5f ) *
                          this.AspectRatio -
                          this.Angle.Up * ( Y / this.Resolution.Y - 0.5f );

            Dir.Normalize( );
            R.Direction = Dir;

            return R;
        }

        public void AddRotation( float Pitch, float Yaw, float Roll, bool Radians = true )
        {
            this.Angle.Pitch += MathHelper.GetRadians( Pitch, Radians );
            this.Angle.Yaw += MathHelper.GetRadians( Yaw, Radians );
            this.Angle.Roll += MathHelper.GetRadians( Roll, Radians );
        }

        public void Move( float Forward, float Right, float Up )
        {
            this.Position += this.Angle.Forward * Forward +
                             this.Angle.Right * Right +
                             this.Angle.Up * Up;
        }
    }
}
