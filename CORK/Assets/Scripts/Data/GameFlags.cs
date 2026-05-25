using System.Collections.Generic;
using UnityEngine;

namespace CORK.Data
{
    /// <summary>
    /// Tracks named boolean flags that represent persistent world state (e.g. "alarmPulled", "canvasHacked").
    /// Assign one instance in the Inspector. Flags are set/checked by commands and
    /// read by DialogueCondition to gate character responses.
    ///
    /// Create via: Assets > Create > CORK > Game Flags
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Game Flags", fileName = "GameFlags")]
    public class GameFlags : ScriptableObject
    {
        [Tooltip("Flags that are currently active. Edited at runtime; do not set values here in the Inspector.")]
        public List<string> activeFlags = new List<string>();

        /// <summary>Returns true if the named flag is currently active.</summary>
        public bool HasFlag(string flag) => activeFlags.Contains(flag);

        /// <summary>Activates a flag. Silently ignored if already set.</summary>
        public void SetFlag(string flag)
        {
            if (!activeFlags.Contains(flag))
                activeFlags.Add(flag);
        }

        /// <summary>Removes all active flags. Called by GameController.OnDestroy to reset state between Play sessions.</summary>
        public void ClearAll() => activeFlags.Clear();
    }
}
