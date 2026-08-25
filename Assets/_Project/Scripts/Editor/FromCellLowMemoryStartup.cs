#if UNITY_EDITOR
using Unity.Burst;
using UnityEditor;

namespace FromCell.Editor
{
    /// <summary>
    /// Startup is crashing on 16 GB machines (OutOfMemory in editor UI + Burst).
    /// Disable Burst compilation in the Editor only; player/build Burst is unchanged.
    /// </summary>
    [InitializeOnLoad]
    static class FromCellLowMemoryStartup
    {
        static FromCellLowMemoryStartup()
        {
            try
            {
                BurstCompiler.Options.EnableBurstCompilation = false;
            }
            catch
            {
                // Burst API may be unavailable during a partial domain reload.
            }
        }
    }
}
#endif
