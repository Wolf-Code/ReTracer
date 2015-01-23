using System;
using System.Drawing;

namespace ReTracer.EventArgs
{
    public class RenderFinishedEventArgs : System.EventArgs
    {
        public Bitmap Render;
        public TimeSpan RenderTime;
    }
}
