using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

/// <summary>
/// Handles the 'open' command. Finds a PropContainerData in the current room by name,
/// validates its state, opens it, and lists its contents.
///
/// Create via: Assets > Create > TextAdventure > InputActions > Open
/// Assign to GameController.inputActions[] and set keyWord to "open".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Open")]
public class Open : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Open what?");
            return;
        }

        string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        List<PropData> props = controller.roomNavigation.currentRoom.props;

        // Check if ANY prop matches the input — lets us give a better message for non-containers.
        PropData anyMatch = FindPropByName(props, input);
        PropContainerData container = anyMatch as PropContainerData;

        if (anyMatch != null && container == null)
        {
            controller.LogStringWithReturn("You can't open that.");
            return;
        }

        if (container == null)
        {
            controller.LogStringWithReturn("You don't see anything like that to open here.");
            return;
        }

        if (container.isLocked)
        {
            string msg = !string.IsNullOrEmpty(container.lockedMessage)
                ? container.lockedMessage
                : "The " + container.propName + " is locked.";
            controller.LogStringWithReturn(msg);
            return;
        }

        if (container.isOpen)
        {
            controller.LogStringWithReturn("The " + container.propName + " is already open.");
            return;
        }

        container.isOpen = true;
        container.hasBeenDiscovered = true;

        controller.LogStringWithReturn("You open the " + container.propName + ".");

        if (container.containedProps == null || container.containedProps.Count == 0)
        {
            controller.LogStringWithReturn("It's empty inside.");
            return;
        }

        string contentList = "Inside the " + container.propName + ":";
        foreach (PropData prop in container.containedProps)
        {
            if (prop != null)
                contentList += "\n  - " + prop.propName;
        }
        controller.LogStringWithReturn(contentList);
    }

    static PropData FindPropByName(List<PropData> props, string input)
    {
        if (props == null) return null;

        foreach (PropData prop in props)
        {
            if (prop != null && string.Equals(prop.propName, input, System.StringComparison.OrdinalIgnoreCase))
                return prop;
        }

        // Fall back to description keyword match — mirrors Inspect behaviour.
        List<PropData> matches = new List<PropData>();
        foreach (PropData prop in props)
        {
            if (prop == null || string.IsNullOrEmpty(prop.description)) continue;
            if (prop.description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                matches.Add(prop);
        }

        return matches.Count == 1 ? matches[0] : null;
    }
}
