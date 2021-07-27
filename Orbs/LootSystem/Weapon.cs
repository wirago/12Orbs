namespace Orbs.LootSystem
{
    public class Weapon : Item
    {
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public string SoundEffect { get; set; }
        public enum weaponClass { maim, bleed, blind };
        public weaponClass WeaponClass { get; set; }
    }
}
