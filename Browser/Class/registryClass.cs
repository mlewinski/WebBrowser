using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Browser.Class
{
    class registryClass
    {
        public RegistryKey rk;
        public registryClass()
        {
            rk = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("CSharp").OpenSubKey("Browser");
        }
        public void initRegistrySetting()
        {
            if (rk.GetValue("isConfigured") == "1")
            {
                return;
            }
            else
            {
                rk.CreateSubKey("Accessibility");
                RegistryKey rs = rk.OpenSubKey("Accessibillity");
            }
        }
    }
}
