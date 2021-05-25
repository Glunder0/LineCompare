using System;

namespace LineCompare
{
    public class ProgressTracker : IProgress<float>
    {
        public float Progress { get; private set; }
        
        public void Report(float value)
        {
            Progress = value;
        }
    }
}