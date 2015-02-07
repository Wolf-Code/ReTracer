using System;
using System.Text;

namespace ReTracer.Rendering
{
    public class Matrix4x4
    {
        public float [ , ] Data { private set; get; }

        public static Matrix4x4 Identity { private set; get; }
        private const int Dimensions = 4;

        public float this[ int x, int y ]
        {
            get { return this.Data[ x, y ]; }
        }

        public Vector3 Right
        {
            get { return new Vector3( this[ 0, 0 ], this[ 0, 1 ], this[ 0, 2 ] ); }
        }

        public Vector3 Up
        {
            get { return new Vector3( this[ 1, 0 ], this[ 1, 1 ], this[ 1, 2 ] ); }
        }

        public Vector3 Forward
        {
            get { return new Vector3( this[ 2, 0 ], this[ 2, 1 ], this[ 2, 2 ] ); }
        }

        static Matrix4x4( )
        {
            float [ , ] Temp = new float[ Dimensions, Dimensions ];
            for ( int Q = 0; Q < Dimensions; Q++ )
                Temp[ Q, Q ] = 1;

            Identity = new Matrix4x4( Temp );
        }

        public Matrix4x4( params float [ ] Data )
        {
            if ( Data.Length % Dimensions != 0 )
                throw new Exception( string.Format( "No valid {0}x{1} array.", Dimensions, Dimensions ) );

            float [ , ] Temp = new float[ Dimensions, Dimensions ];
            for ( int Q = 0; Q < Data.Length; Q++ )
            {
                int X = Q % Dimensions;
                int Y = ( Q - X ) / Dimensions;

                Temp[ X, Y ] = Data[ Q ];
            }

            this.Data = Temp;
        }

        public Matrix4x4( float [ , ] Data )
        {
            if ( Data.GetLength( 0 ) != Data.GetLength( 1 ) && Data.GetLength( 0 ) != Dimensions )
                throw new Exception( string.Format( "No valid {0}x{1} array.", Dimensions, Dimensions ) );

            this.Data = Data;
        }

        public static Matrix4x4 CreateTranslation( Vector3 Translation )
        {
            return new Matrix4x4(
                1, 0, 0, Translation.X,
                0, 1, 0, Translation.Y,
                0, 0, 1, Translation.Z,
                0, 0, 0, 1 );
        }

        public static Matrix4x4 CreateScale( Vector3 Scale )
        {
            return new Matrix4x4(
                Scale.X, 0, 0, 0,
                0, Scale.Y, 0, 0,
                0, 0, Scale.Z, 0 );
        }

        public static Matrix4x4 CreateRotationX( float Angle, bool Radians = true )
        {
            float Sin = MathHelper.Sin( Angle, Radians );
            float Cos = MathHelper.Cos( Angle, Radians );
            return new Matrix4x4(
                1, 0, 0, 0,
                0, Cos, -Sin, 0,
                0, Sin, Cos, 0,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 CreateRotationY( float Angle, bool Radians = true )
        {
            float Sin = MathHelper.Sin( Angle, Radians );
            float Cos = MathHelper.Cos( Angle, Radians );

            return new Matrix4x4(
                Cos, 0, Sin, 0,
                0, 1, 0, 0,
                -Sin, 0, Cos, 0,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 CreateRotationZ( float Angle, bool Radians = true )
        {
            float Sin = MathHelper.Sin( Angle, Radians );
            float Cos = MathHelper.Cos( Angle, Radians );

            return new Matrix4x4(
                Cos, -Sin, 0, 0,
                Sin, Cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
                );
        }

        public static Matrix4x4 CreateRotation( float Pitch, float Yaw, float Roll, bool Radians = true )
        {
            return CreateRotationX( Pitch, Radians ) *
                   CreateRotationY( Yaw, Radians ) *
                   CreateRotationZ( Roll, Radians );
        }

        public static Matrix4x4 CreateOrtho(float Width, float Height, float Znear, float Zfar)
        {
            return new Matrix4x4(
                1f / Width, 0, 0, 0,
                0, 1f / Height, 0, 0,
                0, 0, -( 2f / ( Zfar - Znear ) ), ( Zfar + Znear ) / ( Zfar - Znear ),
                0, 0, 0, 1 );
        }

        public static Matrix4x4 CreateProjection( float FovX, float FovY, float Znear, float Zfar )
        {
            return new Matrix4x4(
                MathHelper.ATan( FovX / 2f ), 0, 0, 0,
                0, MathHelper.ATan( FovY / 2f ), 0, 0,
                0, 0, -( 2f / ( Zfar - Znear ) ), ( Zfar + Znear ) / ( Zfar - Znear ),
                0, 0, 0, 1 );
        }

        #region Operators

        public static Matrix4x4 operator *( Matrix4x4 A, Matrix4x4 B )
        {
            float [ , ] Temp = new float[ Dimensions, Dimensions ];

            for ( int i = 0; i < Dimensions; i++ )
            {
                for ( int j = 0; j < Dimensions; j++ )
                {
                    Temp[ i, j ] = 0;
                    for ( int k = 0; k < Dimensions; k++ )
                        Temp[ i, j ] += A[ i, k ] * B[ k, j ];
                }
            }

            return new Matrix4x4( Temp );
        }

        public static Vector3 operator*( Vector3 V, Matrix4x4 M )
        {
            float [ ] VectorMatrix = { V.X, V.Y, V.Z, 1 };

            float [ ] Temp = new float[ 4 ];
            for ( int i = 0; i < Dimensions; i++ )
            {
                Temp[ i ] = 0;
                for ( int j = 0; j < Dimensions; j++ )
                    Temp[ i ] += VectorMatrix[ j ] * M[ j, i ];
            }

            return new Vector3( Temp[ 0 ], Temp[ 1 ], Temp[ 2 ] );
        }

        #endregion

        public override string ToString( )
        {
            StringBuilder Builder = new StringBuilder( );
            for ( int Y = 0; Y < Dimensions; Y++ )
            {
                for ( int X = 0; X < Dimensions; X++ )
                    Builder.AppendFormat( "{0}\t", this[ X, Y ] );

                Builder.Append( '\n' );
            }

            return Builder.ToString( );
        }
    }
}
