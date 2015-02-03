using System;
using System.Drawing.Imaging;
using System.IO;
using ReTracer;
using ReTracer.Abstract;
using ReTracer.EventArgs;
using ReTracer.Rendering;
using ReTracer.Rendering.Materials;
using ReTracer.Rendering.Objects;
using ReTracer.Settings;
using SFML.Graphics;
using SFML.Window;
using Keyboard = Flatty.Input.Keyboard;
using Mouse = Flatty.Input.Mouse;

namespace Tracer_SFMLTest
{
    internal class Window : Flatty.Window
    {
        private Renderer Renderer;
        private Texture Img;
        private Scene S;
        private RenderSettings Settings;
        private readonly RectangleShape Sp;
        private float Distance;
        private int Samples;
        private TimeSpan FrameTime;

        public Window( ) : base( "Tracer", 512, 512 )
        {
            this.Renderer = new CPURenderer( );
            this.Renderer.OnProgress += RendererOnOnProgress;
            this.RWindow.SetFramerateLimit( 30 );

            S = new Scene( ( int ) this.Resolution.X, ( int ) this.Resolution.Y );
            S.Camera.AddRotation( 30f, -45, 0f );
            S.Camera.Position = new Vector3( 10, 10, -7 );
            S.Camera.FOV = 90;
            S.Camera.ApertureSize = 1f;
            S.Camera.FocalLength = 11f;
            S.Camera.DepthOfFieldRays = 1;
            Sphere Light = new Sphere( new Vector3( 0, 35, 0 ), 5 )
            {
                Material = { Emission = new PixelColor( 500 ) }
            };
            S.AddObject( Light );

            const int Spheres = 10;
            const int Radius = 6;
            const float Begin = Spheres / 2f * ( Radius * 2 );

            for ( int Q = 0; Q < Spheres; Q++ )
            {
                Sphere Sph = new Sphere( new Vector3( -Begin + Q * Radius * 2, Radius, 0 ), Radius )
                {
                    Material = new Diffuse
                    {
                        Color =
                            new PixelColor( ThreadRandom.NextFloat( ), ThreadRandom.NextFloat( ),
                                ThreadRandom.NextFloat( ) )
                    }
                };

                S.AddObject( Sph );
            }

            Plane Floor = new Plane( Vector3.UnitY, 0 ) { Material = { Color = new PixelColor( 1f ) } };
            S.AddObject( Floor );

            Plane Ceiling = new Plane( Vector3.UnitY * -1, -60 );
            S.AddObject( Ceiling );

            Plane BackWall = new Plane( Vector3.UnitZ * -1, -60 );
            S.AddObject( BackWall );

            Plane Front = new Plane( Vector3.UnitZ * 1, -60 );
            S.AddObject( Front );

            Plane Left = new Plane( Vector3.UnitX * 1, -60 ) { Material = { Color = new PixelColor( 0.5f, 0.5f, 1f ) } };
            S.AddObject( Left );

            Plane Right = new Plane( Vector3.UnitX * -1, -60 )
            {
                Material = new Diffuse { Color = new PixelColor( 1f, 0.6f, 0.6f ) }
            };
            S.AddObject( Right );

            Img = new Texture( ( uint ) S.Camera.Resolution.IntX, ( uint ) S.Camera.Resolution.IntY );
            Sp = new RectangleShape( this.Resolution ) { Texture = new Texture( Img ) };

            Settings = new RenderSettings
            {
                AreaDivider = 1,
                MaxBounces = 1000,
                Samples = 3,
                SamplesPerProgress = 1
            };
            Renderer.Render( S, Settings );

            Mouse.MouseMoved += Mouse_MouseMoved;
            Mouse.ButtonPressed += Mouse_ButtonPressed;
            Keyboard.KeyPressed += ( Sender, Args ) =>
            {
                if ( Args.Code != SFML.Window.Keyboard.Key.Return ) return;
                string Name = DateTime.Now.ToShortDateString( ) + "_" +
                              DateTime.Now.ToLongTimeString( ).Replace( ':', '_' ) + ".png";
                Renderer.RequestImage( ).Save( Name );

                Console.WriteLine( "Saved screenshot {0}", Name );
            };

            this.FrameTime = new TimeSpan( );
        }

        private void Mouse_ButtonPressed( object sender, SFML.Window.MouseButtonEventArgs e )
        {
            if ( e.Button != SFML.Window.Mouse.Button.Left ) return;

            Ray R =
                S.Camera.GetRay(
                    ( S.Camera.Resolution.X / this.Resolution.X ) * ( this.Resolution.X / this.RWindow.Size.X ) * e.X,
                    ( S.Camera.Resolution.Y / this.Resolution.Y ) * ( this.Resolution.Y / this.RWindow.Size.Y ) * e.Y );
            Console.WriteLine( ( S.Camera.Resolution.X / this.Resolution.X ) *
                               ( this.Resolution.X / this.RWindow.Size.X ) * e.X );
            Intersection I = R.CheckIntersection( S );

            Console.WriteLine( "Focal length: {0}", I.Distance );

            S.Camera.FocalLength = I.Distance;
        }

        private void Mouse_MouseMoved( object sender, Mouse.MouseMoveEventArgs e )
        {
            if ( Mouse.IsButtonDown( SFML.Window.Mouse.Button.Right ) )
                S.Camera.AddRotation( e.dY, e.dX, 0 );
            else
            {
                Ray R = S.Camera.GetRay(
                    ( S.Camera.Resolution.X / this.Resolution.X ) * ( this.Resolution.X / this.RWindow.Size.X ) * e.X,
                    ( S.Camera.Resolution.Y / this.Resolution.Y ) * ( this.Resolution.Y / this.RWindow.Size.Y ) * e.Y );
                Intersection I = R.CheckIntersection( S );

                this.Distance = I.Distance;
            }
        }

        private void RendererOnOnProgress( object Sender, RenderProgressEventArgs RenderProgressEventArgs )
        {
            using ( MemoryStream Strm = new MemoryStream( ) )
            {
                RenderProgressEventArgs.Render.Save( Strm, ImageFormat.Bmp );
                //Img.Dispose( );
                Img = new Texture( new Image( Strm ) );
                Sp.Texture = Img;
                Sp.TextureRect = new IntRect( 0, 0, ( int ) Img.Size.Y, ( int ) Img.Size.X );
            }

            this.Samples = ( int ) RenderProgressEventArgs.Samples;
            this.FrameTime = RenderProgressEventArgs.RenderTime;
            //Console.WriteLine( RenderProgressEventArgs.Progress * 100 + "%, " + RenderProgressEventArgs.RenderTime );
        }

        protected override void OnDraw( double dT )
        {
            Flatty.Graphics.Renderer.Draw( Sp );

            Text DebugText =
                Flatty.Graphics.TextRenderer.GetText(
                    "Distance: " + this.Distance + "\nSamples: " + this.Samples + "\nFrame Time: " + this.FrameTime,
                    "Resources\\Font.ttf", 12 );
            DebugText.Position = new Vector2f( 0,
                this.Resolution.Y - Flatty.Graphics.TextRenderer.MeasureText( DebugText ).Y );
            Flatty.Graphics.Renderer.Draw( DebugText );

            if ( S.Camera.CheckForChange( ) )
            {
                Renderer.Cancel( );
                Renderer.RenderPreview( S );
            }
            else
            {
                if ( Renderer.Rendering == Renderer.RenderType.Off )
                    Renderer.ContinuousRender( S, Settings );
            }
        }

        protected override void OnUpdate( double dT )
        {
            Vector3 Movement = new Vector3( );
            if ( Keyboard.IsKeyDown( SFML.Window.Keyboard.Key.W ) )
                Movement.X += 1f;

            if ( Keyboard.IsKeyDown( SFML.Window.Keyboard.Key.S ) )
                Movement.X -= 1f;

            if ( Keyboard.IsKeyDown( SFML.Window.Keyboard.Key.D ) )
                Movement.Y += 1f;

            if ( Keyboard.IsKeyDown( SFML.Window.Keyboard.Key.A ) )
                Movement.Y -= 1f;

            if ( Movement.LengthSquared > 0 )
                S.Camera.Move( Movement.X, Movement.Y, Movement.Z );
        }
    }
}