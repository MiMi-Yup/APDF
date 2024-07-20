using System.Management;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace APDF.Helpers
{
    [SupportedOSPlatform("windows")]
    internal class HardwareHelper
    {
        /// <summary>
        /// Get volume serial number of drive C
        /// </summary>
        /// <returns></returns>
        private static string? GetDiskVolumeSerialNumber()
        {
            try
            {
                ManagementObject _disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                _disk.Get();
                return _disk["VolumeSerialNumber"]?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get CPU ID
        /// </summary>
        /// <returns></returns>
        private static string? GetProcessorId()
        {
            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string? _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["ProcessorId"]?.ToString();
                    break;
                }

                return _id;

            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Get motherboard serial number
        /// </summary>
        /// <returns></returns>
        private static string? GetMotherboardID()
        {

            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string? _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["SerialNumber"].ToString();
                    break;
                }

                return _id;
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Combine CPU ID, Disk C Volume Serial Number and Motherboard Serial Number as device Id
        /// </summary>
        /// <returns></returns>
        public static string GenerateUID(string appName)
        {
            //Combine the IDs and get bytes
            string _id = string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());
            byte[] _byteIds = Encoding.UTF8.GetBytes(_id);

            //Use MD5 to get the fixed length checksum of the ID string
            var _md5 = MD5.Create();
            byte[] _checksum = _md5.ComputeHash(_byteIds);

            StringBuilder result = new StringBuilder(_checksum.Length * 2);

            for (int i = 0; i < _checksum.Length; i++)
                result.Append(_checksum[i].ToString("X2"));

            return result.ToString();
        }
    }
}
