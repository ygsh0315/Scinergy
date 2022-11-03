using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class SYA_SympoRoomItem : MonoBehaviour
{
    //방 이름
    public Text roomName;
    //방 생성자
    public Text ownerName;
    //참여 인원
    public Text playerCount;
    //공개 및 비공개 여부
    public GameObject unpubl;

    //text
    public GameObject password;
    public InputField one;
    public InputField two;
    public InputField three;
    public InputField four;
    public string roomPassword;


    //사진

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetInfo(RoomInfo info)
    {
        string roomName_ = info.CustomProperties["room_name"].ToString();
        name = roomName_;

        //방이름 (0/0)
        roomName.text = roomName_;
        playerCount.text = $"{info.PlayerCount}명 참여 중";
        ownerName.text = info.CustomProperties["owner"].ToString();
        roomPassword= info.CustomProperties["password"].ToString(); ;
        unpubl.SetActive(info.CustomProperties["public"].ToString() == "False");
    }

    public void JoinRoom()
    {
        if(!unpubl.activeSelf)
        {
            PhotonNetwork.JoinRoom(name);
        }
        else
        {
            password.SetActive(true);
            password.transform.parent = GameObject.Find("Canvas").transform;
            password.transform.localPosition = Vector2.zero;
        }
    }

    public void passwordJoin()
    {
        string inputPassword =$"{one.text}{two.text}{three.text}{four.text}";
        if(inputPassword==roomPassword)
        {
            PhotonNetwork.JoinRoom(name);
        }
        else
        {
            print("다시 입력하세요");
        }
    }
}
