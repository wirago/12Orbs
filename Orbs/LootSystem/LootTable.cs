using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Orbs.LootSystem
{
    public static class LootTable
    {
        public static List<Consumable> consumables;
        public static List<Material> materials;
        public static List<Weapon> weapons;
        public static List<Armor> armor;
        public static List<Spell> spells;
        public static List<Material> questItems;
        public static List<Item> miscItems;
        public static List<Glyph> glyphs;

        private const string ICONFILEPATH = @"UserInterface\Icons\";

        public static void ReadLootTable()
        {
            XDocument loottable = XDocument.Load(@"LootSystem\Tables\loottable.xml");
            XDocument spelltable = XDocument.Load(@"LootSystem\Tables\spelltable.xml");

            consumables = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "consumable"
                    select new Consumable()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Consumable,
                        UseEffect = (string)item.Attribute("useEffect")
                    }).ToList();

            materials = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "material"
                    select new Material()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Material,
                        UseEffect = (string)item.Attribute("useEffect")
                    }).ToList();

            questItems = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "questItems"
                    select new Material()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.QuestItem,
                        UseType = (string)item.Attribute("useType"),
                        UseEffect = (string)item.Attribute("useEffect"),
                        isQuestItem = true
                    }).ToList();

            miscItems = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "misc"
                    select new Item()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Material,
                        UseType = (string)item.Attribute("useType"),
                        UseEffect = (string)item.Attribute("useEffect")
                    }).ToList();

            weapons = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "weapon"
                    select new Weapon()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        MinDmg = (int)item.Attribute("minDmg"),
                        MaxDmg = (int)item.Attribute("maxDmg"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Weapon,
                        UseEffect = (string)item.Attribute("useEffect"),
                        SoundEffect = (string)item.Attribute("sound"),
                        WeaponClass = (Weapon.weaponClass)Enum.Parse(typeof(Weapon.weaponClass), (string)item.Attribute("class"))
                    }).ToList();

            armor = (
                    from item in loottable.Descendants("Item")
                    where (string)item.Parent.Attribute("type") == "armor"
                    select new Armor()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        Rarity = (int)item.Attribute("rarity"),
                        Value = (int)item.Attribute("value"),
                        ArmorValue = (int)item.Attribute("armor"),
                        ArmorSlot = Armor.ParseFromXMLTable((string)item.Attribute("slot")),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Equipable,
                        UseEffect = (string)item.Attribute("useEffect")
                    }).ToList();

            spells = (
                    from item in spelltable.Descendants("Spell")
                    where (string)item.Parent.Attribute("type") == "fire"
                    select new Spell()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        MinDmg = (int)item.Attribute("minDmg"),
                        MaxDmg = (int)item.Attribute("maxDmg"),
                        Manacost = (int)item.Attribute("manacost"),
                        SpellType = Spell.spellType.Fire,
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Castable,
                        UseEffect = (string)item.Attribute("useEffect"),
                        SoundEffect = (string)item.Attribute("sound"),
                        SpellTarget = (string)item.Attribute("target"),
                        Range = (int)item.Attribute("range")
                    }).ToList();

            List<Spell> tempNature = (
                    from item in spelltable.Descendants("Spell")
                    where (string)item.Parent.Attribute("type") == "nature"
                    select new Spell()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        MinDmg = (int)item.Attribute("minDmg"),
                        MaxDmg = (int)item.Attribute("maxDmg"),
                        Manacost = (int)item.Attribute("manacost"),
                        SpellType = Spell.spellType.Nature,
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Castable,
                        UseEffect = (string)item.Attribute("useEffect"),
                        SoundEffect = (string)item.Attribute("sound"),
                        SpellTarget = (string)item.Attribute("target"),
                        Range = (int)item.Attribute("range")
                    }).ToList();

            List<Spell> tempArcane = (
                    from item in spelltable.Descendants("Spell")
                    where (string)item.Parent.Attribute("type") == "arcane"
                    select new Spell()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        MinDmg = (int)item.Attribute("minDmg"),
                        MaxDmg = (int)item.Attribute("maxDmg"),
                        Manacost = (int)item.Attribute("manacost"),
                        SpellType = Spell.spellType.Arcane,
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Castable,
                        UseEffect = (string)item.Attribute("useEffect"),
                        SoundEffect = (string)item.Attribute("sound"),
                        SpellTarget = (string)item.Attribute("target"),
                        Range = (int)item.Attribute("range")
                    }).ToList();

            spells.AddRange(tempNature);
            spells.AddRange(tempArcane);

            glyphs = (
                    from item in spelltable.Descendants("Spell")
                    where (string)item.Parent.Attribute("type") == "rune"
                    select new Glyph()
                    {
                        Id = (string)item.Attribute("id"),
                        Name = (string)item.Attribute("name"),
                        MinDmg = (int)item.Attribute("minDmg"),
                        MaxDmg = (int)item.Attribute("maxDmg"),
                        Manacost = (int)item.Attribute("manacost"),
                        IconTexture = Orbs.content.Load<Texture2D>(ICONFILEPATH + (string)item.Attribute("icon")),
                        Type = Item.type.Castable,
                        UseEffect = (string)item.Attribute("useEffect"),
                        SoundEffect = (string)item.Attribute("sound"),
                        GlyphTexture = Orbs.content.Load<Texture2D>(@"Textures\" + (string)item.Attribute("glyph"))
                    }).ToList();            
        }
    }
}
