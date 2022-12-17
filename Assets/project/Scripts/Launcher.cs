using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
public class Launcher : MonoBehaviourPunCallbacks
{
    #region Variables

    [Header("menu")]
    public static Launcher Instance;
    [SerializeField] private GameObject[] screenObjects; 
    [SerializeField] private TMP_Text info_Text; 
    [SerializeField] private TMP_Text nameRoom; 
    [SerializeField] private TMP_Text createRoomError; 
    [SerializeField] private TMP_InputField roomNameInput;

    [Header("Botton")]
    [SerializeField] private RoomBotton pre_fRoomButton;
    [SerializeField] private Transform roomButtonParent;
    [SerializeField] private List<RoomBotton> bottonList = new List<RoomBotton>();
    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        info_Text.text = "Connecting to Network...";
        SetScreenObject(0);
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion

    #region Custom Functions

    public void SetScreenObject(int index)
    {
        for (int i = 0; i < screenObjects.Length; i++)
        {
            screenObjects[i].SetActive(i == index);
        }
    }
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;
            PhotonNetwork.CreateRoom(roomNameInput.text);
            info_Text.text = "Creating Room...";
            SetScreenObject(0);
        }
    }
    public void Back()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void FindRoom()
    {
        SetScreenObject(6);
    }
    public override void OnLeftRoom()
    {
        SetScreenObject(1);
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        info_Text.text = "Joining Room....";
        SetScreenObject(0);
    }

    #endregion

    #region Photon

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SetScreenObject(1);
    }
    public override void OnJoinedRoom()
    {
        nameRoom.text = PhotonNetwork.CurrentRoom.Name;
        SetScreenObject(4);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        createRoomError.text = "failed to create room" + message;
        SetScreenObject(5);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomBotton newRoomButton = Instantiate(pre_fRoomButton, roomButtonParent);
                newRoomButton.SetButtonDetails(roomList[i]);

                bottonList.Add(newRoomButton);
            }

            if (roomList[i].RemovedFromList)
            {

                for (int j = 0; j < bottonList.Count; j++)
                {
                    if (roomList[i].Name == bottonList[j].roomInfo.Name)
                    {
                        GameObject go = bottonList[j].gameObject;
                        bottonList.Remove(bottonList[j]);
                        Destroy(go);
                    }
                }

            }
        }
    }
  
    #endregion
}
