using iiMenu.Classes;

namespace iiMenu.Mods.CustomMaps.Maps
{
    public abstract class CustomMap
    {
        public abstract long MapID { get; }
        public abstract ButtonInfo[] Buttons { get; }
    }
}
