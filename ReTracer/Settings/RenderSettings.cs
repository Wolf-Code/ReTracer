
namespace ReTracer.Settings
{
    public class RenderSettings
    {
        public uint AreaDivider { set; get; }
        public uint MaxBounces { set; get; }
        private uint m_Samples = 1;

        public uint SamplesPerProgress { set; get; }

        public uint Samples
        {
            set
            {
                if ( value < 1 )
                    value = 1;

                m_Samples = value;
            }
            get { return m_Samples; }
        }

        public RenderSettings( )
        {
            this.AreaDivider = 1;
            this.MaxBounces = 1000;
            this.SamplesPerProgress = 5;
            this.Samples = 50;
        }
    }
}
