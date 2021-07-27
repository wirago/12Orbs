using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbs.Effects
{
    //As extension for Penumbra.Light 
    public class LightSourceExtension
    {
        public string id;
        public Vector2 origin;

        public LightSourceExtension(string id, Vector2 origin)
        {
            this.id = id;
            this.origin = origin;
        }
    }
}
