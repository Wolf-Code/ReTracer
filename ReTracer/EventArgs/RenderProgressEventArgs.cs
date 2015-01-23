using System;
using System.Drawing;

namespace ReTracer.EventArgs
{
    public class RenderProgressEventArgs : System.EventArgs
    {
        public TimeSpan RenderTime;
        public Bitmap Render;
    }
}
