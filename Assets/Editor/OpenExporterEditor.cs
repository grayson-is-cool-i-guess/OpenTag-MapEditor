using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

[CustomEditor(typeof(OpenExporter))]
public class OpenExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var exporter = (OpenExporter)target;
        string currentKey = exporter.mapName + "|" + exporter.optimization;
        GUILayout.Space(10);
        GUIStyle bigButton = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fixedHeight = 20
        };
        if (GUILayout.Button("Export Project", bigButton))
            Export(exporter);
    }

    private void Export(OpenExporter exporter)
    {
        GameObject original = exporter.gameObject;

        var clone = Object.Instantiate(original);
        clone.name = original.name + "_export";
        EnsureMeshesAreReadable(clone);
        ApplyOptimizations(clone, exporter.optimization);

        string outputPath = "Assets/ExportedMaps";
        Directory.CreateDirectory(outputPath);

        string bundleName = exporter.mapName.ToUpper();
        string prefabPath = $"{outputPath}/{bundleName}.prefab";
        string rawBundle = $"{outputPath}/{bundleName}";
        string zipPath = $"{outputPath}/{bundleName}.zip";
        string jzPath = $"{outputPath}/{bundleName}.jzlib";

        PrefabUtility.SaveAsPrefabAsset(clone, prefabPath);
        Object.DestroyImmediate(clone);
        var imp = AssetImporter.GetAtPath(prefabPath);
        imp.assetBundleName = bundleName;
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        if (File.Exists(rawBundle))
        {
            if (File.Exists(zipPath)) File.Delete(zipPath);
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                zip.CreateEntryFromFile(rawBundle, bundleName, System.IO.Compression.CompressionLevel.Optimal);

            if (File.Exists(jzPath)) File.Delete(jzPath);
            File.Move(zipPath, jzPath);

            foreach (var f in Directory.GetFiles(outputPath))
                if (!f.EndsWith(".jzlib"))
                    File.Delete(f);

            var full = Path.GetFullPath(jzPath);
            Process.Start("explorer.exe", $"/select,\"{full}\"");

            UnityEngine.Debug.Log($"exported to {jzPath}");
        }
        else
        {
            UnityEngine.Debug.LogError("bundle build failed");
        }

        AssetDatabase.DeleteAsset(prefabPath);
    }

    private void EnsureMeshesAreReadable(GameObject root)
    {
        var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        var meshColliders = root.GetComponentsInChildren<MeshCollider>(true);
        void ProcessMesh(Mesh mesh)
        {
            if (mesh == null) return;
            string meshPath = AssetDatabase.GetAssetPath(mesh);
            var modelImporter = AssetImporter.GetAtPath(meshPath) as ModelImporter;
            if (modelImporter != null && !modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
                modelImporter.SaveAndReimport();
                UnityEngine.Debug.Log($"Enabled Read/Write on mesh at {meshPath}");
            }
        }

        foreach (var mf in meshFilters)
            ProcessMesh(mf.sharedMesh);

        foreach (var mc in meshColliders)
            ProcessMesh(mc.sharedMesh);
    }


    private void ApplyOptimizations(GameObject root, OpenExporter.OptimizationLevel lvl)
    {
        if (lvl == OpenExporter.OptimizationLevel.NONE) return;

        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in all)
        {
            var go = t.gameObject;

            if (lvl >= OpenExporter.OptimizationLevel.LOW)
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            if (lvl >= OpenExporter.OptimizationLevel.LOW &&
                go.GetComponents<Component>().Length == 1 &&
                go.transform.childCount == 0)
            {
                Object.DestroyImmediate(go);
                continue;
            }

            if (lvl >= OpenExporter.OptimizationLevel.MEDIUM)
                foreach (var c in go.GetComponents<Behaviour>())
                    if (!c.enabled) Object.DestroyImmediate(c);

            if (lvl == OpenExporter.OptimizationLevel.HIGH &&
                go.transform.childCount == 1 &&
                go != root)
            {
                var child = go.transform.GetChild(0);
                child.SetParent(go.transform.parent, true);
                Object.DestroyImmediate(go);
            }
        }
    }
}