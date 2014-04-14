using System;
using System.Text;
using FileSystemIDandChk;

namespace FileSystemIDandChk.Plugins
{
    class PCEnginePlugin : Plugin
    {
        public PCEnginePlugin(PluginBase Core)
        {
            Name = "PC Engine CD Plugin";
            PluginUUID = new Guid("e5ee6d7c-90fa-49bd-ac89-14ef750b8af3");
        }

        public override bool Identify(ImagePlugins.ImagePlugin imagePlugin, ulong partitionOffset)
        {
            byte[] system_descriptor = new byte[23];
            byte[] sector = imagePlugin.ReadSector(1 + partitionOffset);

            Array.Copy(sector, 0x20, system_descriptor, 0, 23);

            return Encoding.ASCII.GetString(system_descriptor) == "PC Engine CD-ROM SYSTEM";
        }

        public override void GetInformation(ImagePlugins.ImagePlugin imagePlugin, ulong partitionOffset, out string information)
        {
            information = "";
        }
    }
}