using System.IO;
using UnityEditor;

namespace TechCosmos.Metrology.Editor
{
    static class MeasureFolders
    {
        public static void Ensure(string assetFolder)
        {
            assetFolder = assetFolder.Replace('\\', '/');
            if (AssetDatabase.IsValidFolder(assetFolder))
                return;

            var parent = Path.GetDirectoryName(assetFolder)?.Replace('\\', '/');
            var name = Path.GetFileName(assetFolder);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
                return;
            Ensure(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
