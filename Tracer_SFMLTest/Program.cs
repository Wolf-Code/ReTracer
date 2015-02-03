using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer_SFMLTest
{
    internal class Program
    {
        private static void Main( string [ ] args )
        {
            using ( Window W = new Window( ) )
            {
                W.Run( );
            }
        }
    }
}