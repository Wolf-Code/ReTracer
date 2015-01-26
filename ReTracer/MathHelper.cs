using System;

namespace ReTracer
{
    public static class MathHelper
    {
        public const float Theta = 0.001f;
        public const float PI = ( float ) Math.PI;
        public const float OneOverPI = ( float ) ( 1.0 / Math.PI );
        public const float OneOverTwoPI = ( float ) ( 1.0 / ( 2.0 * Math.PI ) );

        public static float GetRadians( float Angle, bool InRadians = true )
        {
            return InRadians ? Angle : DegreesToRadians( Angle );
        }

        public static float RadiansToDegrees( float Radians )
        {
            return Radians * ( 180.0f / PI );
        }

        public static float DegreesToRadians( float Degrees )
        {
            return Degrees * ( PI / 180.0f );
        }

        public static float Cos( float Angle, bool Radians = true )
        {
            return ( float ) Math.Cos( GetRadians( Angle, Radians ) );
        }

        public static float Sin( float Angle, bool Radians = true )
        {
            return ( float ) Math.Sin( GetRadians( Angle, Radians ) );
        }

        public static float Tan( float Angle, bool Radians = true )
        {
            return ( float ) Math.Tan( GetRadians( Angle, Radians ) );
        }

        public static float ATan( float Angle, bool Radians = true )
        {
            return ( float ) Math.Atan( GetRadians( Angle, Radians ) );
        }
    }
}
