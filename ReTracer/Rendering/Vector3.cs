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

        public static Vector3 UnitX { private set; get; }
        public static Vector3 UnitY { private set; get; }
        public static Vector3 UnitZ { private set; get; }

        static Vector3( )
        {
            UnitX = new Vector3( 1, 0, 0 );
            UnitY = new Vector3( 0, 1, 0 );
            UnitZ = new Vector3( 0, 0, 1 );
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

        #region Static

        /// <summary>
        /// Returns a <see cref="Vector3"/> with random values between -1 and 1 for its coordinates.
        /// </summary>
        /// <returns>A random vector3.</returns>
        public static Vector3 Random( )
        {
            return new Vector3(
                ThreadRandom.NextFloat( ) * 2 - 1,
                ThreadRandom.NextFloat( ) * 2 - 1,
                ThreadRandom.NextFloat( ) * 2 - 1 );
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> with a random direction, but in the same hemisphere as the given direction.
        /// </summary>
        /// <param name="Direction"></param>
        /// <returns></returns>
        public static Vector3 RandomInSameHemisphere( Vector3 Direction )
        {
            Vector3 Rand = Random( ).Normalized( );
            if ( Direction.Dot( Rand ) < 0 )
                Rand *= -1;

            return Rand;
        }

        public static Vector3 Reflect( Vector3 Vector, Vector3 Normal )
        {
            return Vector - 2.0f * Vector.Dot( Normal ) * Normal;
        }

        #endregion

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

        public static Vector3 operator *( float Mul, Vector3 Vector )
        {
            return Vector * Mul;
        }

        #endregion

        public override string ToString( )
        {
            return string.Format( "{0}, {1}, {2}", this.X, this.Y, this.Z );
        }
    }
}
