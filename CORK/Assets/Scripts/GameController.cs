using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CORK.Data.Rooms;
using CORK.Data.Characters;
using CORK.Data.Props;
using CORK.Data.Inventory;

public class GameController : MonoBehaviour
{
    public Text displayText;
    public InputAction[] inputActions;
    [SerializeField] Image roomImageDisplay;
    public PlayerInventoryData playerInventory;

    [HideInInspector] public RoomNavigation roomNavigation;
    [HideInInspector] public List<string> interactionDescriptionsInRoom = new List<string>();

    PropPickupSystem propPickupSystem;
    List<string> actionLog = new List<string>();

    void Awake()
    {
        roomNavigation = GetComponent<RoomNavigation>();
        propPickupSystem = GetComponent<PropPickupSystem>();
    }

    void Start()
    {
        LogStringWithReturn(
            "Welcome to CORK.\n" +
            "Commands:\n" +
            "  look              - describe your surroundings\n" +
            "  move [direction]  - travel through an exit (e.g. 'move north', 'move glass door')\n" +
            "  inspect [thing]   - examine something nearby (e.g. 'inspect poster')\n" +
            "  talk [person]     - talk to someone nearby (e.g. 'talk bartender')\n" +
            "  inventory         - check what you're carrying\n"
        );
        DisplayRoomText();
        DisplayLoggedText();
    }

    public void DisplayLoggedText()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());
        displayText.text = logAsText;
    }

    public void DisplayRoomText()
    {
        ClearCollectionsForNewRoom();
        UpdateRoomImage();
        UnpackRoom();

        string roomText = roomNavigation.currentRoom.roomName + "\n" + roomNavigation.currentRoom.description;

        foreach (PropData prop in roomNavigation.currentRoom.props)
        {
            if (prop == null) continue;
            if (prop.hasBeenDiscovered)
                roomText += "\n" + prop.propName + " - " + prop.description;
            else if (!string.IsNullOrEmpty(prop.description))
                roomText += "\n" + prop.description;
        }

        foreach (CharacterData character in roomNavigation.currentRoom.characters)
        {
            if (character == null) continue;
            if (character.hasBeenMet)
                roomText += "\n" + character.characterName + " - " + character.description;
            else if (!string.IsNullOrEmpty(character.description))
                roomText += "\n" + character.description;
        }

        if (interactionDescriptionsInRoom.Count > 0)
            roomText += "\n" + string.Join("\n", interactionDescriptionsInRoom.ToArray());

        LogStringWithReturn(roomText);
    }

    void UpdateRoomImage()
    {
        if (roomImageDisplay == null) return;

        if (roomNavigation.currentRoom.roomImage != null)
        {
            roomImageDisplay.sprite = roomNavigation.currentRoom.roomImage;
            roomImageDisplay.color = Color.white;
        }
        else
        {
            roomImageDisplay.sprite = null;
            roomImageDisplay.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        }
    }

    void UnpackRoom()
    {
        roomNavigation.UnpackExitsInRoom();
    }

    void ClearCollectionsForNewRoom()
    {
        interactionDescriptionsInRoom.Clear();
        roomNavigation.ClearExits();
    }

    public void LogStringWithReturn(string stringToAdd)
    {
        actionLog.Add(stringToAdd + "\n");
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        foreach (RoomData room in Resources.FindObjectsOfTypeAll<RoomData>())
            foreach (RoomConnection connection in room.connections)
                connection.hasBeenVisited = false;

        foreach (PropData prop in Resources.FindObjectsOfTypeAll<PropData>())
            prop.hasBeenDiscovered = false;

        foreach (CharacterData character in Resources.FindObjectsOfTypeAll<CharacterData>())
            character.hasBeenMet = false;

        if (playerInventory != null)
            playerInventory.items.Clear();
    }
}
