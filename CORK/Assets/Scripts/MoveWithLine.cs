using UnityEngine;
using CORK.Data.Rooms;
using CORK.Data;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/MoveWithLine")]
public class MoveWithLine : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Move where?");
            return;
        }

        string target = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        
        RoomData currentRoom = controller.roomNavigation.currentRoom;
        RoomConnection targetConnection = null;
        
        foreach (var conn in currentRoom.connections)
        {
            if (conn.direction.ToLower() == target.ToLower() ||
                conn.displayName.ToLower() == target.ToLower() ||
                (conn.hasBeenVisited && conn.connectedRoom.roomName.ToLower() == target.ToLower()))
            {
                targetConnection = conn;
                break;
            }
        }
        
        if (targetConnection == null)
        {
            controller.LogStringWithReturn("You don't see a way to go '" + target + "'.");
            return;
        }
        
        // Check if trying to enter Dr. Cain's office (change "Cain" to your room name)
        bool isCainsOffice = targetConnection.connectedRoom != null && 
            (targetConnection.connectedRoom.roomName.Contains("Cain") || 
             targetConnection.connectedRoom.roomName == "343");
        
        if (isCainsOffice)
        {
            int tasksCompleted = CountLineTasks(controller);
            
            if (tasksCompleted < 3)
            {
                ShowLineMessage(controller, tasksCompleted);
                return; // Block movement
            }
        }
        
        // Handle locked doors normally
        if (targetConnection.isLocked)
        {
            string msg = string.IsNullOrEmpty(targetConnection.lockedMessage) 
                ? "It's locked." 
                : targetConnection.lockedMessage;
            controller.LogStringWithReturn(msg);
            return;
        }
        
        // Move to the room
        controller.roomNavigation.currentRoom = targetConnection.connectedRoom;
        controller.DisplayRoomText();
    }
    
    private int CountLineTasks(GameController controller)
    {
        int count = 0;
        if (controller.gameFlags.HasFlag("lineTask1Complete")) count++;
        if (controller.gameFlags.HasFlag("lineTask2Complete")) count++;
        if (controller.gameFlags.HasFlag("lineTask3Complete")) count++;
        return count;
    }
    
    private void ShowLineMessage(GameController controller, int tasksCompleted)
    {
        switch (tasksCompleted)
        {
            case 0:
                controller.LogStringWithReturn("There's a massive line stretching down the hallway. You can barely see Dr. Cain's door at the end. You need to find a way to clear this line.");
                break;
            case 1:
                controller.LogStringWithReturn("The line is shorter now — about 2/3 of what it was. A few students gave up and left. The rest mutter something about how they'd rather burn alive than fail compilers.");
                break;
            case 2:
                controller.LogStringWithReturn("Only a handful of students remain in line. Almost there.");
                break;
        }
    }
}