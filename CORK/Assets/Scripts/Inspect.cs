using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Inspect")]
public class Inspect : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Inspect what? Look around to find something of interest.");
            return;
        }

        string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        List<PropData> props = GetAccessibleProps(controller.roomNavigation.currentRoom.props);

        PropData found = null;
        foreach (PropData prop in props)
        {
            if (prop != null && string.Equals(prop.propName, input, System.StringComparison.OrdinalIgnoreCase))
            { found = prop; break; }
        }

        if (found == null)
        {
            List<PropData> matches = FindPropsByDescription(props, input);
            if (matches.Count == 1)
                found = matches[0];
            else if (matches.Count > 1)
            {
                controller.LogStringWithReturn("That could describe several things here. Try being more specific.");
                return;
            }
        }

        if (found == null)
        {
            controller.LogStringWithReturn("You don't find anything like that here.");
            return;
        }

        bool newDiscovery = !found.hasBeenDiscovered;
        found.hasBeenDiscovered = true;

        controller.ShowPropImage(found.propImage);
        controller.LogStringWithReturn(found.description);

        if (found is PropContainerData container)
        {
            if (container.isLocked)
                controller.LogStringWithReturn("It is locked.");
            else if (!container.isOpen)
                controller.LogStringWithReturn("It is closed. (Try: open " + container.propName.ToLower() + ")");
            else if (container.containedProps == null || container.containedProps.Count == 0)
                controller.LogStringWithReturn("It is open and empty.");
            else
            {
                string contentList = "It is open. Inside:";
                foreach (PropData prop in container.containedProps)
                    if (prop != null) contentList += "\n  - " + prop.propName;
                controller.LogStringWithReturn(contentList);
            }
        }

        if (newDiscovery)
            controller.LogStringWithReturn("You'll remember this as: " + found.propName + ".");
    }

    static List<PropData> FindPropsByDescription(List<PropData> props, string input)
    {
        List<PropData> matches = new List<PropData>();
        if (props == null) return matches;

        foreach (PropData prop in props)
        {
            if (prop == null || string.IsNullOrEmpty(prop.description)) continue;
            if (prop.description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                matches.Add(prop);
        }

        return matches;
    }

    // Returns all props the player can currently interact with:
    // room-level props plus the contents of any open containers.
    static List<PropData> GetAccessibleProps(List<PropData> roomProps)
    {
        List<PropData> accessible = new List<PropData>(roomProps);

        foreach (PropData prop in roomProps)
        {
            if (prop is PropContainerData container && container.isOpen && container.containedProps != null)
            {
                foreach (PropData contained in container.containedProps)
                    if (contained != null) accessible.Add(contained);
            }
        }

        return accessible;
    }
}
