using System;
using System.Drawing;

namespace ReTracer.EventArgs
{
    public class RenderFinishedEventArgs : System.EventArgs
    {
        public Bitmap Image;
        public TimeSpan RenderTime;
    }
}
