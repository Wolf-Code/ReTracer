using System;
using System.ComponentModel;

namespace ReTracer.Rendering
{
    public class Vector3
    {
        public float X { set; get; }
        public float Y { set; get; }
        public float Z { set; get; }

        [Browsable( false )]
        public float LengthSquared
        {
            get { return this.Dot( this ); }
        }

        [Browsable( false )]
        public float Length
        {
            get { return ( float ) Math.Sqrt( this.LengthSquared ); }
        }

        public Vector3( )
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Vector3( float X, float Y, float Z )
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public float Dot( Vector3 Other )
        {
            return this.X * Other.X + this.Y * Other.Y + this.Z * Other.Z;
        }

        public void Normalize( )
        {
            float L = this.Length;
            this.X /= L;
            this.Y /= L;
            this.Z /= L;
        }

        public Vector3 Normalized( )
        {
            float L = this.Length;

            return new Vector3(
                this.X / L,
                this.Y / L,
                this.Z / L );
        }

        #region Operators

        public static Vector3 operator -( Vector3 V1, Vector3 V2 )
        {
            return new Vector3(
                V1.X - V2.X,
                V1.Y - V2.Y,
                V1.Z - V2.Z );
        }

        public static Vector3 operator +(Vector3 V1, Vector3 V2)
        {
            return new Vector3(
                V1.X + V2.X,
                V1.Y + V2.Y,
                V1.Z + V2.Z);
        }

        public static Vector3 operator *( Vector3 Vector, float Mul )
        {
            return new Vector3(
                Vector.X * Mul,
                Vector.Y * Mul,
                Vector.Z * Mul );
        }

        #endregion
    }
}
