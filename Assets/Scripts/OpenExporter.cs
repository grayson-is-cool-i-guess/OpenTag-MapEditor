using UnityEngine;

public class OpenExporter : MonoBehaviour
{
    public enum OptimizationLevel { NONE, LOW, MEDIUM, HIGH }

    [Header("Map Export Settings")]
    public string mapName = "";
    public OptimizationLevel optimization = OptimizationLevel.NONE;
}
