using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Inspect")]
public class Inspect : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        List<PropData> props = controller.roomNavigation.currentRoom.props;

        if (separatedInputWords.Length < 2)
        {
            if (props == null || props.Count == 0)
            {
                controller.LogStringWithReturn("There is nothing to inspect here.");
                return;
            }

            List<string> names = new List<string>();
            foreach (PropData prop in props)
            {
                if (prop != null) names.Add(prop.propName);
            }
            controller.LogStringWithReturn("You can inspect: " + string.Join(", ", names) + ".");
        }
        else
        {
            string targetName = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

            PropData found = null;
            if (props != null)
            {
                foreach (PropData prop in props)
                {
                    if (prop != null && string.Equals(prop.propName, targetName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        found = prop;
                        break;
                    }
                }
            }

            if (found != null)
                controller.LogStringWithReturn(found.description);
            else
                controller.LogStringWithReturn("You don't see \"" + targetName + "\" here.");
        }
    }
}
