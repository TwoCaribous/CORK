using UnityEngine;

/// <summary>
/// DEBUG ONLY — Remove before shipping.
/// Attach to any GameObject in the Main scene to instantly set all progression flags on Start.
/// </summary>
public class DebugSetFlags : MonoBehaviour
{
    void Start()
    {
        GameController controller = FindObjectOfType<GameController>();
        if (controller == null) return;

        controller.gameFlags.SetFlag("lineTask1Complete");
        controller.gameFlags.SetFlag("lineTask2Complete");
        controller.gameFlags.SetFlag("lineTask3Complete");

        Debug.Log("[DebugSetFlags] All line task flags set.");
    }
}
