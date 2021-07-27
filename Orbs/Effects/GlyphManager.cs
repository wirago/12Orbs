using Orbs.LootSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbs.Effects
{
    public static class GlyphManager
    {
        public static void ApplyWaterGlyphs()
        {

        }

        public static void RemoveExpiredGlyphs()
        {
            World.WorldObjects.placedGlyphs.RemoveAll(glyph => glyph.expired);
        }
    }
}
