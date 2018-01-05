using reWZ.WZProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Tools
{
    public static class DataTool
    {
        public static int GetInt(WZObject node)
        {
            return node.ValueOrDie<int>();
        }

        public static string GetString (WZObject node)
        {
            return node.ValueOrDie<string>();
        }

        public static float GetFloat(WZObject node)
        {
            return node.ValueOrDie<float>();
        }
    }
}
