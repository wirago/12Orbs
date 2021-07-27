using System;

namespace Orbs.LootSystem
{
    public class Armor : Item
    {
        public int ArmorValue { get; set; }
        public enum armorSlot {Head, Neck, Shoulders, Chest, Waist, Legs, Hands, Feet};
        public armorSlot ArmorSlot { get; set; }

        public static armorSlot ParseFromXMLTable(string slot)
        {
            switch(slot)
            {
                case "Head":
                    return armorSlot.Head;

                case "Neck":
                    return armorSlot.Neck;

                case "Shoulders":
                    return armorSlot.Shoulders;

                case "Chest":
                    return armorSlot.Chest;

                case "Waist":
                    return armorSlot.Waist;

                case "Legs":
                    return armorSlot.Legs;

                case "Hands":
                    return armorSlot.Hands;

                case "Feet":
                    return armorSlot.Feet;

                default:
                    throw new InvalidOperationException("Invalid Slot from XML: " + slot);
            }

        }
    }
}
