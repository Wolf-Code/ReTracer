using System;

namespace ReTracer
{
    public static class MathHelper
    {
        public static float PI { private set; get; }

        static MathHelper( )
        {
            PI = ( float ) Math.PI;
        }

        private static float GetRadians( float Angle, bool Radians = true )
        {
            return Radians ? Angle : DegreesToRadians( Angle );
        }

        public static float RadiansToDegrees( float Radians )
        {
            return Radians * ( 180f / PI );
        }

        public static float DegreesToRadians( float Degrees )
        {
            return Degrees * ( PI / 180f );
        }

        public static float Cos( float Angle, bool Radians = true )
        {
            return ( float ) Math.Cos( GetRadians( Angle, Radians ) );
        }

        public static float Sin( float Angle, bool Radians = true )
        {
            return ( float ) Math.Cos( GetRadians( Angle, Radians ) );
        }

        public static float ATan( float Angle, bool Radians = true )
        {
            return ( float ) Math.Atan( GetRadians( Angle, Radians ) );
        }
    }
}
