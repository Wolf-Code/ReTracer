using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ReTracer.EventArgs;
using ReTracer.Rendering;
using ReTracer.Settings;

namespace ReTracer.Abstract
{
    public abstract class Renderer
    {
        public event EventHandler<RenderStartEventArgs> OnStart; 
        public event EventHandler<RenderProgressEventArgs> OnProgress;
        public event EventHandler<RenderFinishedEventArgs> OnFinish;

        protected PixelColor [ ] Pixels;
        protected uint [ ] PixelSamples;
        protected Scene CurrentScene { private set; get; }
        protected RenderSettings CurrentSettings { private set; get; }
        private readonly Stopwatch Watch = new Stopwatch( );
        private uint PixelsRendered;
        private Thread RenderThread;
        private bool Rendering;

        public void Render( Scene RenderScene, RenderSettings Settings )
        {
            if ( Rendering )
                return;

            this.Rendering = true;
            this.CurrentScene = RenderScene;
            this.CurrentSettings = Settings;

            RenderThread = new Thread( this.StartRender );
            RenderThread.Start( );
        }

        public void Cancel( )
        {
            RenderThread.Abort( );
            this.Rendering = false;
        }

        protected void StartRender( )
        {
            int W = CurrentScene.Camera.Resolution.IntX;
            int H = CurrentScene.Camera.Resolution.IntY;
            this.Pixels = new PixelColor[ W * H ];

            Parallel.For( 0, this.Pixels.Length, Var =>
            {
                this.Pixels[ Var ] = new PixelColor( );
            } );
            this.PixelSamples = new uint[ this.Pixels.Length ];

            int AreaWidth = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.X / CurrentSettings.AreaDivider );
            int AreaHeight = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.Y / CurrentSettings.AreaDivider );

            if ( OnStart != null )
                OnStart.Invoke( this, new RenderStartEventArgs( ) );

            Watch.Restart( );
            for ( int X = 0; X < W; X += AreaWidth )
            {
                int Width = Math.Min( X + AreaWidth, W ) - X;

                for ( int Y = 0; Y < H; Y += AreaHeight )
                {
                    int Height = Math.Min( Y + AreaHeight, H ) - Y;
                    int MaxSamples = ( int ) this.CurrentSettings.SamplesPerProgress;
                    uint CurSamples = 0;

                    while ( CurSamples < this.CurrentSettings.Samples )
                    {
                        uint Samples =
                            ( uint ) Math.Min( MaxSamples, this.CurrentSettings.Samples - CurSamples );
                        this.RenderRegion( Samples, X, Y, Width, Height );
                        this.ReportProgress( X, Y, Width, Height );

                        CurSamples += Samples;
                    }
                }
            }

            Watch.Stop( );

            Finished( );
        }

        protected int ConvertPixelCoordinatesToArrayIndex( int RealX, int RealY )
        {
            return RealY * CurrentScene.Camera.Resolution.IntX + RealX;
        }

        protected abstract void RenderRegion( uint Samples, int StartX, int StartY, int Width, int Height );

        private Bitmap GetCurrentRender( )
        {
            Bitmap I = new Bitmap( CurrentScene.Camera.Resolution.IntX, CurrentScene.Camera.Resolution.IntY,
                PixelFormat.Format32bppArgb );

            BitmapData Data = I.LockBits( new Rectangle( 0, 0, I.Width, I.Height ), ImageLockMode.WriteOnly,
                I.PixelFormat );

            int BPP = Image.GetPixelFormatSize( I.PixelFormat ) / 8;
            byte [ ] Bytes = new byte[ Pixels.Length * BPP ];

            Parallel.For( 0, Pixels.Length, Var =>
            {
                if ( PixelSamples[ Var ] == 0 )
                    return;

                Color C = ( Pixels[ Var ] / PixelSamples[ Var ] ).ToColor( );

                Var *= BPP;

                Bytes[ Var ] = C.B;
                Bytes[ Var + 1 ] = C.G;
                Bytes[ Var + 2 ] = C.R;
                Bytes[ Var + 3 ] = 255;
            } );

            Marshal.Copy( Bytes, 0, Data.Scan0, Bytes.Length );
            I.UnlockBits( Data );

            return I;
        }

        protected void ReportProgress( int StartX, int StartY, int Width, int Height )
        {
            TimeSpan FrameTime = Watch.Elapsed;
            Watch.Reset( );

            PixelsRendered += ( uint ) ( ( Width * Height ) * this.CurrentSettings.SamplesPerProgress );

            if ( this.OnProgress != null )
            {
                this.OnProgress.Invoke( this, new RenderProgressEventArgs
                {
                    RenderTime = FrameTime,
                    Render = this.GetCurrentRender( ),
                    Progress =
                        ( float ) PixelsRendered /
                        ( this.CurrentScene.Camera.Resolution.IntX * this.CurrentScene.Camera.Resolution.IntY *
                          this.CurrentSettings.Samples )
                } );
            }

            Watch.Start( );
        }

        private void Finished( )
        {
            this.Rendering = false;
            if ( OnFinish == null ) return;

            OnFinish.Invoke( this, new RenderFinishedEventArgs
            {
                Render = this.GetCurrentRender(  ),
                RenderTime = Watch.Elapsed
            } );
        }
    }
}
