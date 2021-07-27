using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Orbs.LootSystem
{
    public static class EnemyTable
    {
        public static List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();

        public static void ReadEnemyTable()
        {
            XDocument enemytable = XDocument.Load(@"LootSystem\Tables\enemytable.xml");

            enemyTemplates = (
                    from row in enemytable.Descendants("Enemy")
                    where (string)row.Parent.Attribute("type") == "creep"
                    select new EnemyTemplate()
                    {
                        EnemyType = EnemyType.Creep,
                        uid = (string)row.Attribute("uid"),
                        attackAnimation = (string)row.Attribute("attackAnimation"),
                        name = (string)row.Attribute("name"),
                        minDmg = (int)row.Attribute("minDmg"),
                        maxDmg = (int)row.Attribute("maxDmg"),
                        attackSpeed = (int)row.Attribute("attackSpeed"),
                        movementSpeed = (int)row.Attribute("movementSpeed"),
                        maxHealth = (int)row.Attribute("maxHealth"),
                        textureHeight = (int)row.Attribute("textureHeight"),
                        textureWidth = (int)row.Attribute("textureWidth"),
                        texture = (string)row.Attribute("texture"),
                        ElementType = (ElementType) Enum.Parse(typeof(ElementType), (string)row.Attribute("element"))
                    }).ToList();

            List<EnemyTemplate> elites = new List<EnemyTemplate>();
            elites = (
                    from row in enemytable.Descendants("Enemy")
                    where (string)row.Parent.Attribute("type") == "elite"
                    select new EnemyTemplate()
                    {
                        EnemyType = EnemyType.Elite,
                        uid = (string)row.Attribute("uid"),
                        attackAnimation = (string)row.Attribute("attackAnimation"),
                        name = (string)row.Attribute("name"),
                        minDmg = (int)row.Attribute("minDmg"),
                        maxDmg = (int)row.Attribute("maxDmg"),
                        attackSpeed = (int)row.Attribute("attackSpeed"),
                        movementSpeed = (int)row.Attribute("movementSpeed"),
                        maxHealth = (int)row.Attribute("maxHealth"),
                        textureHeight = (int)row.Attribute("textureHeight"),
                        textureWidth = (int)row.Attribute("textureWidth"),
                        texture = (string)row.Attribute("texture")
                    }).ToList();

            List<EnemyTemplate> bosses = new List<EnemyTemplate>();
            bosses = (
                    from row in enemytable.Descendants("Enemy")
                    where (string)row.Parent.Attribute("type") == "boss"
                    select new EnemyTemplate()
                    {
                        EnemyType = EnemyType.Boss,
                        uid = (string)row.Attribute("uid"),
                        attackAnimation = (string)row.Attribute("attackAnimation"),
                        name = (string)row.Attribute("name"),
                        minDmg = (int)row.Attribute("minDmg"),
                        maxDmg = (int)row.Attribute("maxDmg"),
                        attackSpeed = (int)row.Attribute("attackSpeed"),
                        movementSpeed = (int)row.Attribute("movementSpeed"),
                        maxHealth = (int)row.Attribute("maxHealth"),
                        textureHeight = (int)row.Attribute("textureHeight"),
                        textureWidth = (int)row.Attribute("textureWidth"),
                        texture = (string)row.Attribute("texture")
                    }).ToList();

            enemyTemplates.AddRange(elites);
            enemyTemplates.AddRange(bosses);
        }
    }
}
