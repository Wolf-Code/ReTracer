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

        #endregion
    }
}
