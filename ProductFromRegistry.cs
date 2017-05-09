using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Microsoft.Win32;


namespace ProductFromRegistry
{
    [Cmdlet(VerbsCommon.Get,"SoftwareProductInstalled")]
    [OutputType(typeof(SoftwareProductInstalled))]
    public class GetProductRegistry : Cmdlet
    {
        [Parameter(Mandatory = true,  Position = 0, HelpMessage = "Get Installed Software  from Registry of local or remote computer")]
        public string[] Computername { get; set; }

        protected override void ProcessRecord()
        {
            var qry = GetSoft();
            qry.ToList().ForEach(WriteObject);
        }

        public  IEnumerable<SoftwareProductInstalled> GetSoft()

        {
           string DisplayVersion, InstallDate, ModDate;
            foreach (string computer in Computername)
            {
                using (RegistryKey r = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computer).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
                {

                    foreach (string subk in r.GetSubKeyNames())
                    {
                        using (RegistryKey rr = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computer).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall" + "\\" + subk))
                        {

                            if (rr.GetValue("DisplayName") != null)
                            {
                                DisplayVersion = rr.GetValue("DisplayVersion") == null ? "" : rr.GetValue("DisplayVersion").ToString();
                                InstallDate = rr.GetValue("InstallDate") == null ? "" : rr.GetValue("InstallDate").ToString();
                                ModDate = InstallDate.Length < 8 ? "" : (DateTime.ParseExact(InstallDate, "yyyyMMdd", null)).ToShortDateString();

                                yield return new SoftwareProductInstalled { DisplayName = rr.GetValue("DisplayName").ToString(), DisplayVersion = DisplayVersion, InstallDate = ModDate, RemoteServer = computer };
                            }

                        }
                    }

                }
            }
        }
        

    }

    public class SoftwareProductInstalled
    {
        public string DisplayName { get; set; }
       public string DisplayVersion { get; set; }
       public string InstallDate { get; set; }
        public string RemoteServer { get; set; }

        /*public override string ToString()
         {
             return this.DisplayName + " " + this.DisplayVersion + " " ;        }*/
    }
}
