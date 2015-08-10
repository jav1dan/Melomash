using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbadoz.Melomany.Engine.Json
{
    class Levels_Base
    {
        public List<LevelBase> levels { get; set; }
    }
    class LevelBase
    {
        public string ident { get; set; }
        public string name { get; set; }
        public string artists_count { get; set; }
    }
}

