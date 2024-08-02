using BepInEx.Logging;
using BepInEx;

namespace Junkbox
{
    [BepInPlugin("madfrogz.junkbox", "MadFrogz.Junkbox", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new MusicPatch().Enable();
        }
    }
}
