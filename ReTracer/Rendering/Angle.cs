
namespace ReTracer.Rendering
{
    public class Angle
    {
        public float Pitch
        {
            set
            {
                Pitch1 = value;
                this.ForceRefresh( );
            }
            get { return Pitch1; }
        }

        public float Yaw
        {
            set
            {
                Yaw1 = value;
                this.ForceRefresh( );
            }
            get { return Yaw1; }
        }

        public float Roll
        {
            set
            {
                Roll1 = value;
                this.ForceRefresh( );
            }
            get { return Roll1; }
        }

        public bool Radians
        {
            set
            {
                Radians1 = value;
                this.ForceRefresh( );
            }
            get { return Radians1; }
        }

        private Vector3 m_Right, m_Up, m_Forward;
        private Matrix4x4 m_Rotation;

        private bool m_RefreshRight, m_RefreshUp, m_RefreshForward, m_RefreshRotation;
        private float Pitch1;
        private float Roll1;
        private float Yaw1;
        private bool Radians1;

        public Matrix4x4 Rotation
        {
            get
            {
                if ( m_RefreshRotation )
                {
                    m_Rotation = Matrix4x4.CreateRotationX( Pitch, Radians ) *
                                 Matrix4x4.CreateRotationY( Yaw, Radians ) *
                                 Matrix4x4.CreateRotationZ( Roll, Radians );
                    m_RefreshRotation = false;
                }

                return m_Rotation;
            }
        }

        public Vector3 Right
        {
            get
            {
                if ( m_RefreshRight )
                {
                    m_Right = Vector3.UnitX * Rotation;
                    m_RefreshRight = false;
                }

                return m_Right;
            }
        }

        public Vector3 Up
        {
            get
            {
                if ( m_RefreshUp )
                {
                    m_Up = Vector3.UnitY * Rotation;
                    m_RefreshUp = false;
                }

                return m_Up;
            }
        }

        public Vector3 Forward
        {
            get
            {
                if ( m_RefreshForward )
                {
                    m_Forward = Vector3.UnitZ * Rotation;
                    m_RefreshForward = false;
                }

                return m_Forward;
            }
        }

        private void ForceRefresh( )
        {
            m_RefreshForward = true;
            m_RefreshRight = true;
            m_RefreshUp = true;
            m_RefreshRotation = true;
        }

        public Angle( float Pitch, float Yaw, float Roll )
        {
            this.Pitch = Pitch;
            this.Yaw = Yaw;
            this.Roll = Roll;
        }
    }
}
