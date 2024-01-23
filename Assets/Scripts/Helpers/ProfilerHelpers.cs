using System;
using UnityEngine.Profiling;

namespace Helpers
{
    public sealed class ProfileRange : IDisposable
    {
        private ProfileRange()
        {
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }

        public static ProfileRange Named(string name)
        {
            Profiler.BeginSample(name);
            return new ProfileRange();
        }
    }
}