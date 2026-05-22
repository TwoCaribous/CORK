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
        List<PropData> props = controller.roomNavigation.currentRoom.props;

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

        controller.LogStringWithReturn(found.description);

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
}
