using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadianceSkin
{
    public class Setting
    {

        public int skinID = 0;
        public bool Animation = false;
        
        
    }
    public class LocalSetting
    {
        public bool blend = false;
        public bool shotCharge = false;
        public bool customBack = false;
        public bool removeOthers = false;   
        public bool removeCloud=false;
        public string shotColor = "#FFFFFFFF";
        public string backColor = "#FFFFFFFF";
        public bool snow=false;
        public bool orblight = true;
        [NonSerialized]
        public bool wind = false;   
        
    }

}
