using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
public class RoomBotton : MonoBehaviour
{
    [SerializeField] private TMP_Text bottonNameRoom;
    public RoomInfo roomInfo;

    #region Custom Functions

    public void SetButtonDetails(RoomInfo inputInfo)
    {
        roomInfo = inputInfo;
        bottonNameRoom.text = roomInfo.Name;
    }
    public void JoinRoomButton()
    {
        Launcher.Instance.JoinRoom(roomInfo);
    }
    #endregion

    #region Photon

    #endregion
}