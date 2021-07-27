using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Xml.Serialization;

namespace Orbs.LootSystem
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rarity { get; set; }
        public int Value { get; set; }
        public enum type { Consumable, Material, Equipable, Weapon, Castable, QuestItem };
        public type Type { get; set; }
        public string UseType { get; set; }
        public string UseEffect { get; set; }
        public bool isQuestItem = false;


        [XmlIgnore]
        public Texture2D IconTexture { get; set; }

        public bool IsEquipped { get; set; }

        public Item()
        {
            Id = "";
        }

        public Item(Item droppedItem)
        {
            var props = typeof(Item).GetProperties().Where(p => !p.GetIndexParameters().Any());
            foreach (var prop in props)
            {
                prop.SetValue(this, prop.GetValue(droppedItem));
            }
        }
    }
}
