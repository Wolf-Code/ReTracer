using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ReTracer.Rendering
{
    public class PixelColor
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PixelColorStruct
        {
            public float R;
            public float G;
            public float B;
        }

        public float R { set; get; }
        public float G { set; get; }
        public float B { set; get; }

        public int IntR
        {
            get { return Math.Min( 255, ( int ) ( R * 255 ) ); }
        }

        public int IntG
        {
            get { return Math.Min( 255, ( int ) ( G * 255 ) ); }
        }

        public int IntB
        {
            get { return Math.Min( 255, ( int ) ( B * 255 ) ); }
        }

        public float HighestValue
        {
            get { return Math.Max( Math.Max( this.R, this.G ), this.B ); }
        }

        public static PixelColor Black { private set; get; }
        public static PixelColor White { private set; get; }

        public bool IsBlack
        {
            get { return !( this.R > 0 || this.G > 0 || this.B > 0 ); }
        }

        static PixelColor( )
        {
            Black = new PixelColor( 0f );
            White = new PixelColor( 1f );
        }

        public PixelColor( ) : this( 0, 0, 0 )
        {

        }

        public PixelColor( float Value ) : this( Value, Value, Value )
        {

        }

        public PixelColor( float R, float G, float B )
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public Color ToColor( )
        {
            return Color.FromArgb( 255,
                IntR,
                IntG,
                IntB );
        }

        #region Operators

        public static PixelColor operator /( PixelColor C, float Div )
        {
            return new PixelColor( C.R / Div, C.G / Div, C.B / Div );
        }

        public static PixelColor operator *( PixelColor C, float Mul )
        {
            return new PixelColor( C.R * Mul, C.G * Mul, C.B * Mul );
        }

        public static PixelColor operator +( PixelColor C1, PixelColor C2 )
        {
            return new PixelColor( C1.R + C2.R, C1.G + C2.G, C1.B + C2.B );
        }

        public static PixelColor operator *( PixelColor C1, PixelColor C2 )
        {
            return new PixelColor( C1.R * C2.R, C1.G * C2.G, C1.B * C2.B );
        }

        #endregion

        public override string ToString( )
        {
            return string.Format( "Red: {0}, Green: {1}, Blue: {2}", R, G, B );
        }
    }
}
