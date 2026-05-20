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

        string roomText = roomNavigation.currentRoom.description;

        if (roomNavigation.currentRoom.props != null && roomNavigation.currentRoom.props.Count > 0)
        {
            List<string> propNames = new List<string>();
            foreach (PropData prop in roomNavigation.currentRoom.props)
            {
                if (prop != null) propNames.Add(prop.propName);
            }
            if (propNames.Count > 0)
                roomText += "\nYou see: " + string.Join(", ", propNames) + ".";
        }

        if (roomNavigation.currentRoom.characters != null && roomNavigation.currentRoom.characters.Count > 0)
        {
            List<string> charNames = new List<string>();
            foreach (CharacterData character in roomNavigation.currentRoom.characters)
            {
                if (character != null) charNames.Add(character.characterName);
            }
            if (charNames.Count > 0)
                roomText += "\nPeople here: " + string.Join(", ", charNames) + ".";
        }

        string joinedInteractionDescriptions = string.Join("\n", interactionDescriptionsInRoom.ToArray());
        if (!string.IsNullOrEmpty(joinedInteractionDescriptions))
            roomText += "\n" + joinedInteractionDescriptions;

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
}
