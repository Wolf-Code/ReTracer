
namespace ReTracer.Settings
{
    public class RenderSettings
    {
        public uint AreaDivider;
        private uint m_Samples = 1;

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
    }
}
