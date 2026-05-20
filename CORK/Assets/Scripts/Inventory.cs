using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Inventory")]
public class Inventory : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (controller.playerInventory == null || controller.playerInventory.items == null || controller.playerInventory.items.Count == 0)
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
