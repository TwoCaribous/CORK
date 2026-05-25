using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Inventory")]
public class Inventory : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        bool isEmpty = controller.playerInventory == null
            || controller.playerInventory.items == null
            || controller.playerInventory.items.Count == 0;

        // inventory [itemname] — inspect a specific carried item
        if (separatedInputWords.Length > 1)
        {
            if (isEmpty)
            {
                controller.LogStringWithReturn("Your pockets are empty.");
                return;
            }

            string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

            PropData found = null;
            foreach (PropData item in controller.playerInventory.items)
            {
                if (item != null && string.Equals(item.propName, input, System.StringComparison.OrdinalIgnoreCase))
                {
                    found = item;
                    break;
                }
            }

            if (found == null)
            {
                controller.LogStringWithReturn("You're not carrying anything like that.");
                return;
            }

            controller.LogStringWithReturn(found.propName + ": " + found.description);
            return;
        }

        // inventory — list everything carried
        if (isEmpty)
        {
            controller.LogStringWithReturn("Your pockets are empty.");
            return;
        }

        List<string> itemNames = new List<string>();
        foreach (PropData item in controller.playerInventory.items)
        {
            if (item != null) itemNames.Add(item.propName);
        }
        controller.LogStringWithReturn("You are carrying: " + string.Join(", ", itemNames) + ".");
    }
}
