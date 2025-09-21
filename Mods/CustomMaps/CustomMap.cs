using iiMenu.Classes.Menu;

namespace iiMenu.Mods.CustomMaps
{
    public abstract class CustomMap
    {
        public abstract long MapID { get; }
        public abstract ButtonInfo[] Buttons { get; }
    }
}
