namespace Orbs.LootSystem
{
    public class Spell : Item
    {
        public int MinDmg { get; set; }
        public int MaxDmg { get; set; }
        public int Manacost { get; set; }
        public enum spellType { Fire, Arcane, Nature, Water, Earth };
        public spellType SpellType { get; set; }
        public string SoundEffect { get; set; }
        public string SpellTarget { get; set; }
        public int Range { get; set; }
    }
}
