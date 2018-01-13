using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Tools
{
    public class Config
    {
        #region Window
        #endregion

        #region Network
        #endregion

        #region WZ

        public static string wzFolder = developerMode ? AppDomain.CurrentDomain.BaseDirectory : @"E:\v83\"; 

        #endregion

        #region Testing

        public static bool developerMode = true;

        #endregion
    }
}
