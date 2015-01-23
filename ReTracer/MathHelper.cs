using System;

namespace ReTracer
{
    public static class MathHelper
    {
        public const float Theta = 0.001f;
        public const float OneOverPI = 0.31830988618379067153776752674503f;
        public const float OneOverTwoPI = 0.15915494309189533576888376337251f;

        private static float GetRadians( float Angle, bool InRadians = true )
        {
            return InRadians ? Angle : DegreesToRadians( Angle );
        }

        public static float RadiansToDegrees( float Radians )
        {
            return ( float )( Radians * ( 180.0 / Math.PI ) );
        }

        public static float DegreesToRadians( float Degrees )
        {
            return ( float )( Degrees * ( Math.PI / 180.0 ) );
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
