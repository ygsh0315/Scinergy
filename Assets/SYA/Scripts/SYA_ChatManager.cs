using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using SYA_UserInfoManagerSaveLoad;

public class SYA_ChatManager : MonoBehaviourPun, IChatClientListener
{
    public static SYA_ChatManager Instance;
    public ChatClient chatClient;

    //ChatItem공장
    public GameObject chatFactory;
    //스크롤뷰의 컨텐트 담을 변수
    public RectTransform trContent;
    public InputField inputField;
    public Text outputText;
    public string currentChannel;

    Dictionary<string, string> cannel = new Dictionary<string, string>();
    public string Allchannel = "Allchannel";
    public string Lobbychannel = "Lobbychannel";
    public string Constchannel = "Constchannel";
    public string Solarchannel = "Solarchannel";

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        cannel["Allchannel"] = "전체";
        cannel["Lobbychannel"] = "로비";
        cannel["Constchannel"] = "별자리";
        cannel["Solarchannel"] = "행성";
        Application.runInBackground = true;

        //채널 서버 연결
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "ASIA";
        chatClient.UseBackgroundWorkerForSending = true;
        //chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);
        string appId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        string appVersion = PhotonNetwork.AppVersion;
        //앱아이디 , 앱버전 , 채팅 기본 설정 
        chatClient.Connect(appId, appVersion, new AuthenticationValues(SYA_UserInfoManager.Instance.NicName));

        currentChannel = Allchannel;
/*        chatClient.Subscribe(new string[] { Allchannel });*/

        //인풋필드에서 엔터쳣을 때 호출되는 함수 등록
        inputField.onSubmit.AddListener(OnSubmit);
    }

    //인풋필드에서 엔터쳣을 때 호출되는 함수
    public void OnSubmit(string s)
    {
        //< color =#FFFFF>닉네임</color>
        string chatText = $"[{cannel[currentChannel]}] {PhotonNetwork.NickName} : {s}";
        //photonView.RPC("RpcAddChat", RpcTarget.All, chatText);

        PushMessage(currentChannel, chatText);

        //4 인풋챗의 내용 초기화
        inputField.text = "";
        //5 인풋챗에 포커싱 유지
        inputField.ActivateInputField();
    }
    PhotonView pv;

    private void Update()
    {
/*        if(pv==null)
        {
            pv = SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName];
            if (PhotonNetwork.MasterClient.UserId == pv.Owner.UserId)//방장이라면
            {
                chatClient.Subscribe(new string[] { Allchannel, Lobbychannel, Constchannel, Solarchannel });
            }
            else
            {
                chatClient.Subscribe(new string[] { Allchannel, Lobbychannel });
            }
        }*/
        chatClient.Service();
        if (inputField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            OnClickTab();
        }
    }

    //탭을 누른다
    public void OnClickTab()
    {
        string scene = SceneManager.GetActiveScene().name;
        //전체 → 지역 / 지역 → 전체
        if (PhotonNetwork.MasterClient.UserId != pv.Owner.UserId)
        {
            if (currentChannel == Allchannel)
            {

                if (scene == "SymposiumScene")
                {
                    currentChannel = Lobbychannel;
                }
                else
                {
                    currentChannel = scene == "KJH_RevolutionScene" ? Solarchannel : Constchannel;
                }
            }
            else
            {
                currentChannel = Allchannel;
            }
        }
        else
        {
            if (currentChannel == Lobbychannel)
            {
                currentChannel = Solarchannel;
            }
            else if (currentChannel == Solarchannel)
            {
                currentChannel = Constchannel;
            }
            else if (currentChannel == Constchannel)
            {
                currentChannel = Allchannel;
            }
            else if (currentChannel==Allchannel)
            {
                currentChannel = Lobbychannel;
            }
        }
    }

    //전송버튼을 누른다
    public void OnClickChat()
    {
        OnSubmit(inputField.text);
    }

    public void PushMessage(string channelName, string message)
    {
        chatClient.PublishMessage(channelName, message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    // 서버에 연결을 성공함
    public void OnConnected()
    {
        print("서버에 연결되었습니다.");

        // 지정한 채널명으로 접속
        if (pv == null)
        {
            pv = SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName];
            if (PhotonNetwork.MasterClient.UserId == pv.Owner.UserId)//방장이라면
            {
                chatClient.Subscribe(new string[] { Allchannel,Lobbychannel, Constchannel, Solarchannel });
            }
            else
            {
                chatClient.Subscribe(new string[] { Allchannel,Lobbychannel });
            }
        }
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }
    // 서버와의 연결이 끊어짐
    public void OnDisconnected()
    {
        print("서버에 연결이 끊어졌습니다.");
    }


    //이전 컨텐트의 높이
    float prevContentH;
    //스크롤뷰 높이
    public RectTransform trScrollView;

    //채널에 메세지가 새로 올라오면 불리는 함수
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        /*if (channelName.Equals(currentChannel))
        {*/
            //바뀌기 전 h값 넣기
            prevContentH = trContent.sizeDelta.y;

            //1 챗아이템을 만든다 (부모를 스크롤뷰의 컨텐츠)
            GameObject item = Instantiate(chatFactory, trContent);
            //2 만든 챗아이템에서 챗아이템 컴포넌트 가져오기
            SYA_ChatItem chat = item.GetComponent<SYA_ChatItem>();
            
            /*Vector2 y = trContent.;
            y.y += item.transform.localScale.y;
            trContent.localScale = y;*/
            //3 가져온 컴포넌트에 s셋팅
            string mess = $" {messages[0].ToString()}";
            chat.SetText(mess);
            StartCoroutine(AutoScrollBottom());
        //}

    }

    IEnumerator AutoScrollBottom()
    {
        yield return null;
        if (trContent.sizeDelta.y > trScrollView.sizeDelta.y)
        {
            //컨텐트가 바닥에 닿아 있었다면
            if (trContent.anchoredPosition.y >= prevContentH - trScrollView.sizeDelta.y)
            {
                //컨텐트의 y값을 다시 설정해주자
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - trScrollView.sizeDelta.y);
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
     
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
       
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
       
    }
}

