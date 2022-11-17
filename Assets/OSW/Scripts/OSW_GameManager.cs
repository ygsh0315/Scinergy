using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SYA_UI;
using Photon.Voice.Unity.Demos.DemoVoiceUI;

// 권한 Script
public class OSW_GameManager : MonoBehaviourPun
{
    public OSW_GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 방장일 경우에 모두 음소거 버튼이 활성화 되고, 아니면 활성화 안되게?
        //if (PhotonNetwork.MasterClient.UserId == SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName].Owner.UserId)
        if (SYA_SymposiumManager.Instance.playerAuthority[PhotonNetwork.NickName] == "Owner")
        {
            Debug.LogWarning("방장 들어왔다!!");
            mute.SetActive(true);
        }
    }

    public GameObject mute; // 음소거
    public GameObject unmute; // 음소거 해제
    // 모두 음소거
    public void AllMute()
    {
        // AllMute 버튼을 누르면 mute 오브젝트는 꺼주고, unmute 오브젝트는 켜준다.
        mute.SetActive(false);
        unmute.SetActive(true);

        PhotonView mine = SYA_SymposiumManager.Instance.GetMyPlayer();
        if (mine)
        {
            for (int i = 0; i < SYA_SymposiumManager.Instance.playerName.Count; ++i)
            {
                mine.RPC("RPCAllMute", RpcTarget.All, i, SYA_UIManager.Instance.micOff.activeSelf);
            }
        }
    }

    
    // 모두 음소거 해제
    public void AllUnmute()
    {
        // AllUnmute 버튼을 누르면 unmute 오브젝트는 꺼주고, mute 오브젝트는 켜준다.
        unmute.SetActive(false);
        mute.SetActive(true);

        PhotonView mine = SYA_SymposiumManager.Instance.GetMyPlayer();
        if (mine)
        {
            for (int i = 0; i < SYA_SymposiumManager.Instance.playerName.Count; ++i)
            {
                mine.RPC("RPCAllUnmute", RpcTarget.All, i, SYA_UIManager.Instance.micOn.activeSelf);
            }
        }
    }


    // 일단 생각나는대로 적어봐

    // 방장이 청중을 발표자로 변경 버튼을 누르면
    // 화면공유, 판서기능이 활성화되게 하고 싶다.
    // 활성화 시켜줘야 할 버튼 이름 : DrawOnOff, UwcOnOff, Button_Recod
    // 즉 방장이 아니면 모든 기능들이 setActive == false !!

    // 권한을 줄꺼야!
    public void GiveAuthority()
    {

    }
}
