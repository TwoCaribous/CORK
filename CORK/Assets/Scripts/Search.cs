using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

/// <summary>
/// Handles the 'search' command. Scans the current room for props and lists their
/// descriptions, marking each as discovered. Undiscovered and already-discovered props
/// are treated the same — description only, so the player learns names through 'inspect'.
///
/// Dropped items on the floor are prefixed with "On the floor:".
/// Container contents are not revealed here — use 'open' or 'inspect' for those.
///
/// Create via: Assets > Create > TextAdventure > InputActions > Search
/// Assign to GameController.inputActions[] and set keyWord to "search".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Search")]
public class Search : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        List<PropData> props = controller.roomNavigation.currentRoom.props;

        List<PropData> visible = new List<PropData>();
        foreach (PropData prop in props)
        {
            if (prop != null && !string.IsNullOrEmpty(prop.description))
                visible.Add(prop);
        }

        if (visible.Count == 0)
        {
            controller.LogStringWithReturn("You look around carefully but don't find anything of note.");
            return;
        }

        string result = "You look around carefully:";
        foreach (PropData prop in visible)
        {
            string prefix = controller.droppedProps.Contains(prop) ? "On the floor: " : "";
            result += "\n  " + prefix + prop.description;
            prop.hasBeenDiscovered = true;
        }

        controller.LogStringWithReturn(result);
    }
}
