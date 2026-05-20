using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Rooms;

public class RoomNavigation : MonoBehaviour
{
    public RoomData currentRoom;

    Dictionary<string, RoomData> exitDictionary = new Dictionary<string, RoomData>();
    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        foreach (RoomConnection connection in currentRoom.connections)
        {
            if (connection.isHidden || connection.connectedRoom == null)
                continue;

            string directionKey = connection.direction.ToLower();
            string nameKey = connection.connectedRoom.roomName.ToLower();

            if (!exitDictionary.ContainsKey(directionKey))
                exitDictionary.Add(directionKey, connection.connectedRoom);

            if (!exitDictionary.ContainsKey(nameKey))
                exitDictionary.Add(nameKey, connection.connectedRoom);

            if (!string.IsNullOrEmpty(connection.doorDescription))
                controller.interactionDescriptionsInRoom.Add(connection.doorDescription);
        }
    }

    public void AttemptToChangeRooms(string noun)
    {
        string key = noun.ToLower();

        if (exitDictionary.ContainsKey(key))
        {
            currentRoom = exitDictionary[key];
            controller.LogStringWithReturn("You head to " + currentRoom.roomName + ".");
            controller.DisplayRoomText();
        }
        else
        {
            controller.LogStringWithReturn("There is no path to \"" + noun + "\".");
        }
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }
}
