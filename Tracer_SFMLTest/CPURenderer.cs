using System.Threading.Tasks;
using ReTracer;
using ReTracer.Abstract;
using ReTracer.Rendering;
using ReTracer.Rendering.Materials;
using ReTracer.Rendering.Objects;

namespace Tracer_SFMLTest
{
    internal class CPURenderer : Renderer
    {
        protected override void RenderRegion( uint Samples, int StartX, int StartY, int Width, int Height )
        {
            Parallel.For( 0, Width * Height, Var =>
            {
                int LocalX = Var % Width;
                int LocalY = ( Var - LocalX ) / Width;
                int RealX = StartX + LocalX;
                int RealY = StartY + LocalY;

                int ID = this.ConvertPixelCoordinatesToArrayIndex( RealX, RealY );

                for ( int Q = 0; Q < Samples; Q++ )
                {
                    float RayX = RealX + ThreadRandom.NextFloat( );
                    float RayY = RealY + ThreadRandom.NextFloat( );
                    Ray R = this.CurrentScene.Camera.GetRay( RayX, RayY );

                    if ( this.CurrentScene.Camera.ApertureSize > 0 )
                    {
                        Vector3 Start = R.Start;
                        Vector3 FocalPoint = R.Start + R.Direction * this.CurrentScene.Camera.FocalLength;

                        PixelColor Col = new PixelColor( 0f );
                        for ( int I = 0; I < this.CurrentScene.Camera.DepthOfFieldRays; I++ )
                        {
                            R.Start = Start + this.CurrentScene.Camera.GetRandomPositionOnAperture( );
                            R.Direction = ( FocalPoint - R.Start ).Normalized( );

                            Col += Radiance( R );
                        }
                        Col /= this.CurrentScene.Camera.DepthOfFieldRays;

                        this.Pixels[ ID ] += Col;
                    }
                    else
                        this.Pixels[ ID ] += Radiance( R );
                }

                this.PixelSamples[ ID ] += Samples;
            } );
            /*
            PixelColor Max = new PixelColor( 0f );
            for ( int Q = 0; Q < Width * Height; Q++ )
            {
                PixelColor C = this.Pixels[ Q ] / this.PixelSamples[ Q ];
                if ( C > Max )
                    Max = C;
            }

            Console.WriteLine( "Max: {0}", Max );*/
        }

        private PixelColor ShadowRay( Vector3 Position )
        {
            GraphicsObject Light = this.CurrentScene.RandomLight;
            Vector3 SamplePosition = Light.SamplePosition( );

            Vector3 Dir = SamplePosition - Position;
            float Distance = Dir.Length;

            Ray R = new Ray
            {
                Start = Position,
                Direction = Dir / Distance
            };

            Intersection Check = R.CheckIntersection( this.CurrentScene );
            if ( Check.Object == Light )
                return Light.Material.Emission / ( Distance * Distance );

            return PixelColor.Black;
        }

        private PixelColor Radiance( Ray R )
        {
            PixelColor Value = PixelColor.Black;
            PixelColor ThroughPut = PixelColor.White;
            float PreviousChance = 1.0f;

            bool Primary = true;

            while ( R.Depth < this.CurrentSettings.MaxBounces )
            {
                Intersection Intersect = R.CheckIntersection( this.CurrentScene );
                if ( !Intersect.Hit )
                    break;

                if ( Intersect.Material.IsLightSource )
                    if ( Primary )
                        return Intersect.Material.Emission;
                    else
                        break;

                if ( Primary && !( Intersect.Material is Specular ) )
                    Primary = false;

                Vector3 NewDirection = Intersect.Material.NewDirection( R.Direction, Intersect );
                float CosTheta = Intersect.Material.CosTheta( R.Direction, NewDirection, Intersect );
                float BRDF = Intersect.Material.BRDF( R.Direction, NewDirection, Intersect );
                float PDF = Intersect.Material.PDF( R.Direction, NewDirection, Intersect );
                PixelColor LightSample = ShadowRay( Intersect.NewStart );

                ThroughPut *= Intersect.Material.Color * ( BRDF * CosTheta ) / PDF;

                Value += ( Intersect.Material.ColorAddition( LightSample ) * ThroughPut ) / PreviousChance;

                if ( ThreadRandom.NextFloat( ) > BRDF )
                    break;

                PreviousChance = BRDF;

                R = new Ray
                {
                    Depth = R.Depth + 1,
                    Direction = NewDirection,
                    Start = Intersect.NewStart
                };
            }

            return Value;
        }
    }
}