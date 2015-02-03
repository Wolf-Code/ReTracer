using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCL.Net;
using OpenCL.Net.Extensions;
using ReTracer.Abstract;

namespace Tracer_SFMLTest
{
    public class OpenCLRenderer : Renderer
    {
        private Context Cntxt;
        private Kernel Krnl;
        private Device m_Device;

        public OpenCLRenderer( )
        {
            ErrorCode error;
            Platform [ ] platforms = Cl.GetPlatformIDs( out error );
            List<Device> devicesList = new List<Device>( );

            CheckErr( error, "GetPlatformIDs" );

            foreach ( Platform platform in platforms )
            {
                string platformName = Cl.GetPlatformInfo( platform, PlatformInfo.Name, out error ).ToString( );
                Console.WriteLine( "Platform: " + platformName );
                CheckErr( error, "GetPlatformInfo" );
                //We will be looking only for GPU devices
                foreach ( Device device in Cl.GetDeviceIDs( platform, DeviceType.Gpu, out error ) )
                {
                    CheckErr( error, "GetDeviceIDs" );
                    Console.WriteLine( "Device: " + device + "; " +
                                       Cl.GetDeviceInfo( device, DeviceInfo.Name, out error ) );
                    devicesList.Add( device );
                }
            }

            if ( devicesList.Count <= 0 )
            {
                Console.WriteLine( "No devices found." );
                return;
            }

            m_Device = devicesList[ 0 ];

            if ( Cl.GetDeviceInfo( m_Device, DeviceInfo.ImageSupport,
                out error ).CastTo<Bool>( ) == Bool.False )
            {
                Console.WriteLine( "No image support." );
                return;
            }
            Cntxt = Cl.CreateContext( null, 1, new [ ] { m_Device }, ContextNotify,
                IntPtr.Zero, out error ); //Second parameter is amount of devices
            CheckErr( error, "CreateContext" );

            this.SetupKernel( );
        }

        private void SetupKernel( )
        {
            ErrorCode error;
            //Load and compile kernel source code.
            string programPath = System.Environment.CurrentDirectory + "/Kernels/Tracer.cl";
            //The path to the source file may vary

            if ( !System.IO.File.Exists( programPath ) )
            {
                Console.WriteLine( "Program doesn't exist at path " + programPath );
                return;
            }

            string programSource = System.IO.File.ReadAllText( programPath );

            using (
                OpenCL.Net.Program program = Cl.CreateProgramWithSource( Cntxt, 1, new [ ] { programSource }, null,
                    out error ) )
            {
                CheckErr( error, "Cl.CreateProgramWithSource" );
                //Compile kernel source
                error = Cl.BuildProgram( program, 1, new [ ] { m_Device }, string.Empty, null, IntPtr.Zero );
                CheckErr( error, "Cl.BuildProgram" );
                //Check for any compilation errors
                if ( Cl.GetProgramBuildInfo( program, m_Device, ProgramBuildInfo.Status, out error )
                        .CastTo<BuildStatus>( ) != BuildStatus.Success )
                {
                    CheckErr( error, "Cl.GetProgramBuildInfo" );
                    Console.WriteLine( "Cl.GetProgramBuildInfo != Success" );
                    Console.WriteLine( Cl.GetProgramBuildInfo( program, m_Device, ProgramBuildInfo.Log, out error ) );
                    return;
                }
                //Create the required kernel (entry function)
                Krnl = Cl.CreateKernel( program, "test", out error );
                CheckErr( error, "Cl.CreateKernel" );
            }
        }

        private static void CheckErr( ErrorCode err, string name )
        {
            if ( err == ErrorCode.Success )
                return;

            Console.WriteLine( "ERROR: " + name + " (" + err.ToString( ) + ")" );
        }

        private static void ContextNotify( string errInfo, byte [ ] data, IntPtr cb, IntPtr userData )
        {
            Console.WriteLine( "OpenCL Notification: " + errInfo );
        }

        protected override void RenderRegion( uint Samples, int StartX, int StartY, int Width, int Height )
        {

        }
    }
}
