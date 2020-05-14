using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using Invertex.RadminVPNMetricDisablerPatch.Extensions;

namespace Invertex.RadminVPNMetricDisablerPatch
{
    class RadminVPNMetricDisablerPatch
    {
        static int Main(string[] args)
        {
            int exitCode = (int)ExitCodes.Success;

            string path = "C:\\Program Files (x86)\\Radmin VPN\\RvControlSvc.exe";
            string matchString = "6E 00 65 00 74 00 73 00 68 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 20 00 69 00 70 00 76 00 34 00 20 00 73 00 65 00 74 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 3D 00 22 00 25 00 6C 00 73 00 22 00 20 00 6D 00 65 00 74 00 72 00 69 00 63 00 3D 00 25 00 75";
            string replaceString = "6E 00 65 00 74 00 73 00 68 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 20 00 69 00 70 00 76 00 34 00 20 00 73 00 65 00 74 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 20 00 69 00 6E 00 74 00 65 00 72 00 66 00 61 00 63 00 65 00 3D 00 22 00 25 00 6C 00 73 00 22 00 20 00 6D 00 65 00 74 00 72 00 69 00 63 00 3D 00 00 00 00";
            bool replaceAllInstances = false;

            Console.WriteLine("Radmin VPN Metric Disabler Patch starting...");

            if(File.Exists(path))
            {
                Console.WriteLine("Target file '" + path + "' exists...");
                var permissionSet = new PermissionSet(PermissionState.None);
                var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);
                permissionSet.AddPermission(writePermission);

                if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                {
                    byte[] fileBytes = File.ReadAllBytes(path);
                    byte[] matchBytes = matchString.HexStringToBytes();
                    byte[] replaceBytes = replaceString.HexStringToBytes();

                    if (matchBytes != null && matchBytes.Length > 0
                        && replaceBytes != null && replaceBytes.Length == matchBytes.Length
                        && fileBytes != null && fileBytes.Length >= matchBytes.Length)
                    {
                        exitCode = ReplaceBytes(ref fileBytes, matchBytes, replaceBytes, replaceAllInstances);
                        if (exitCode > 0)
                        {
                            File.WriteAllBytes(path, fileBytes);
                            Console.WriteLine("Patched '" + path + "' Successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Patching '" + path + "' Failed! (Already patched?)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Patcher failed. Likely a mismatch of length between Match and Replace bytes.");
                        exitCode = (int)ExitCodes.MatchAndReplaceLengthMismatch;
                    }
                }
                else
                {
                    Console.WriteLine("Patcher lacks rights to access file '" + path + "'");
                    Console.WriteLine("'Radmin VPN Control Service' (RvControlSvc) must be stopped in order for RvControlSvc.exe to be modifiable.");
                    Console.WriteLine("Must also run patcher as Administrator, or copy target file to a folder that doesn't require Administrator rights to modify.");
                    exitCode = (int)ExitCodes.AdministrativeRightsRequired;
                }
            }
            else
            {
                Console.WriteLine("Couldn't find file '" + path + "'");
                exitCode = (int)ExitCodes.TargetFileNotFound;
            }
            Console.ReadLine();
            return exitCode;
        }

        [Flags]
        enum ExitCodes : int
        {
            Success = 0, //0 and greater is success, with higher numbers being how many replacements
            NotEnoughArguments = -1,
            TargetFileNotFound = -2,
            AdministrativeRightsRequired = -4,
            MatchAndReplaceLengthMismatch = -8
        }

        public static int ReplaceBytes(ref byte[] inBytes, byte[] matchBytes, byte[] replaceBytes, bool replaceAllInstances = false)
        {
            int matchLength = matchBytes.Length;
            int curMatch = 0;
            int instancesReplaced = 0;

            for(int i = 0; i < inBytes.Length; i++)
            {
                if(inBytes[i] == matchBytes[curMatch])
                {
                    curMatch++;
                    if (curMatch == matchLength)
                    {
                        ReplaceByteRange(ref inBytes, replaceBytes, i - (curMatch - 1));
                        instancesReplaced++;
                        if (replaceAllInstances)
                        {
                            curMatch = 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else if(curMatch > 0)
                { //If we failed after already matching some, we should make sure current isn't the start of a new match series before moving on
                    curMatch = (inBytes[i] == matchBytes[0]) ? 1 : 0;
                }
            }

            return instancesReplaced;
        }

        public static void ReplaceByteRange(ref byte[] bytes, byte[] replaceBytes, int start)
        {
            for (int i = 0; i < replaceBytes.Length; i++)
            {
                bytes[start + i] = replaceBytes[i];
            }
        }
    }
}
