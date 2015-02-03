using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ReTracer.EventArgs;
using ReTracer.Rendering;
using ReTracer.Settings;
using ThreadState = System.Threading.ThreadState;

namespace ReTracer.Abstract
{
    public abstract class Renderer
    {
        public event EventHandler<RenderStartEventArgs> OnStart; 
        public event EventHandler<RenderProgressEventArgs> OnProgress;
        public event EventHandler<RenderFinishedEventArgs> OnFinish;

        public enum RenderType
        {
            Off,
            Enabled,
            Continuous
        }

        protected PixelColor [ ] Pixels;
        protected uint [ ] PixelSamples;
        protected Scene CurrentScene { private set; get; }
        protected RenderSettings CurrentSettings { private set; get; }
        private readonly Stopwatch Watch = new Stopwatch( );
        private uint PixelsRendered;
        private Thread RenderThread;
        public RenderType Rendering { private set; get; }

        protected Renderer( )
        {
            this.Rendering = RenderType.Off;
        }

        private bool OnRenderStart( Scene RenderScene, RenderSettings Settings, RenderType Type )
        {
            if ( Rendering != RenderType.Off )
                return false;

            if ( RenderThread != null && RenderThread.IsAlive )
                return false;

            this.Rendering = Type;
            this.CurrentScene = RenderScene;
            this.CurrentSettings = Settings;

            int W = RenderScene.Camera.Resolution.IntX;
            int H = RenderScene.Camera.Resolution.IntY;

            this.Pixels = new PixelColor[ W * H ];
            this.PixelSamples = new uint[ this.Pixels.Length ];
            this.PixelsRendered = 0;

            return true;
        }

        public void Render( Scene RenderScene, RenderSettings Settings )
        {
            if ( !OnRenderStart( RenderScene, Settings, RenderType.Enabled ) )
                return;

            RenderThread = new Thread( this.StartRender );
            RenderThread.Start( );
        }

        public void ContinuousRender( Scene RenderScene, RenderSettings Settings )
        {
            Settings.Samples = uint.MaxValue;
            if ( !OnRenderStart( RenderScene, Settings, RenderType.Continuous ) )
                return;

            RenderThread = new Thread( this.ContinuousRender );
            RenderThread.Start( );
        }

        [HandleProcessCorruptedStateExceptions]
        public Bitmap RenderPreview( Scene S )
        {
            RenderSettings Settings = new RenderSettings
            {
                AreaDivider = 1,
                MaxBounces = 3,
                Samples = 1,
                SamplesPerProgress = 1
            };

            Vector2 Res = S.Camera.Resolution;
            S.Camera.Resolution = new Vector2( 100, 100 );

            if ( !OnRenderStart( S, Settings, RenderType.Enabled ) )
            {
                S.Camera.Resolution = Res;
                return new Bitmap( 1, 1 );
            }

            this.StartRender( );
            Bitmap B = this.GetCurrentRender( ).Item1;
            S.Camera.Resolution = Res;

            return B;
        }

        public void Cancel( )
        {
            if ( this.Rendering == RenderType.Off )
                return;

            RenderThread.Abort( );
            this.Rendering = RenderType.Off;
        }

        public void Reset( )
        {
            RenderThread.Abort( );
            while ( RenderThread.IsAlive )
                Thread.Sleep( 1 );

            Rendering = RenderType.Off;
            this.ContinuousRender( this.CurrentScene, this.CurrentSettings );
        }

        public Bitmap RequestImage( )
        {
            if ( RenderThread != null )
                RenderThread.Suspend( );

            Bitmap B = this.GetCurrentRender( ).Item1;

            if ( RenderThread != null )
                RenderThread.Resume( );

            return B;
        }

        protected void ContinuousRender( )
        {
            int W = CurrentScene.Camera.Resolution.IntX;
            int H = CurrentScene.Camera.Resolution.IntY;

            int AreaWidth = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.X / CurrentSettings.AreaDivider );
            int AreaHeight = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.Y / CurrentSettings.AreaDivider );

            if ( OnStart != null )
                OnStart.Invoke( this, new RenderStartEventArgs( ) );

            while ( true )
            {
                Watch.Restart( );

                this.RenderImage( W, H, AreaWidth, AreaHeight );

                Watch.Stop( );
            }
        }

        protected void StartRender( )
        {
            int W = CurrentScene.Camera.Resolution.IntX;
            int H = CurrentScene.Camera.Resolution.IntY;

            int AreaWidth = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.X / CurrentSettings.AreaDivider );
            int AreaHeight = ( int ) Math.Ceiling( CurrentScene.Camera.Resolution.Y / CurrentSettings.AreaDivider );

            if ( OnStart != null )
                OnStart.Invoke( this, new RenderStartEventArgs( ) );

            Watch.Restart( );
            
            this.RenderImage( W, H, AreaWidth, AreaHeight );

            Watch.Stop( );

            Finished( );
        }

        private void RenderImage( int W, int H, int AreaWidth, int AreaHeight )
        {
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
        }

        protected int ConvertPixelCoordinatesToArrayIndex( int RealX, int RealY )
        {
            return RealY * CurrentScene.Camera.Resolution.IntX + RealX;
        }

        protected abstract void RenderRegion( uint Samples, int StartX, int StartY, int Width, int Height );

        private Tuple<Bitmap, byte [ ]> GetCurrentRender( )
        {
            Bitmap I = new Bitmap( CurrentScene.Camera.Resolution.IntX, CurrentScene.Camera.Resolution.IntY,
                PixelFormat.Format32bppArgb );

            BitmapData Data = I.LockBits( new Rectangle( 0, 0, I.Width, I.Height ), ImageLockMode.WriteOnly,
                I.PixelFormat );

            int BPP = Image.GetPixelFormatSize( I.PixelFormat ) / 8;
            byte [ ] Bytes = new byte[ Pixels.Length * BPP ];
            //Console.WriteLine("Start");
            unsafe
            {
                byte* Scan0 = ( byte* ) Data.Scan0;
                Parallel.For( 0, Pixels.Length, Var =>
                {
                    if ( PixelSamples[ Var ] == 0 )
                        return;

                    PixelColor C = ( Pixels[ Var ] / PixelSamples[ Var ] );
                    byte* Pixel = Scan0 + Var * BPP;
                        
                    Pixel[ 0 ] = C.ByteB;
                    Pixel[ 1 ] = C.ByteG;
                    Pixel[ 2 ] = C.ByteR;
                    Pixel[ 3 ] = 255;
                } );
            }
            //Console.WriteLine("End");
            I.UnlockBits( Data );

            return new Tuple<Bitmap, byte [ ]>( I, Bytes );
        }

        protected void ReportProgress( int StartX, int StartY, int Width, int Height )
        {
            TimeSpan FrameTime = Watch.Elapsed;
            Watch.Reset( );

            PixelsRendered += ( uint ) ( ( Width * Height ) * this.CurrentSettings.SamplesPerProgress );

            if ( this.OnProgress != null )
            {
                Tuple<Bitmap, byte [ ]> Render = this.GetCurrentRender( );

                this.OnProgress.Invoke( this, new RenderProgressEventArgs
                {
                    RenderTime = FrameTime,
                    Render = Render.Item1,
                    Bytes = Render.Item2,
                    Progress =
                        ( float ) PixelsRendered /
                        ( this.CurrentScene.Camera.Resolution.IntX * this.CurrentScene.Camera.Resolution.IntY *
                          this.CurrentSettings.Samples ),
                    Samples =
                        ( uint )
                            ( ( float ) PixelsRendered /
                              ( this.CurrentScene.Camera.Resolution.IntX * this.CurrentScene.Camera.Resolution.IntY ) )
                } );
            }

            Watch.Start( );
        }

        private void Finished( )
        {
            this.Rendering = RenderType.Off;
            if ( OnFinish == null ) return;

            Tuple<Bitmap, byte [ ]> Render = this.GetCurrentRender( );
            OnFinish.Invoke( this, new RenderFinishedEventArgs
            {
                Render = Render.Item1,
                RenderTime = Watch.Elapsed
            } );
        }
    }
}
