using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_1123_Sya : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //�÷��̾ �����Ѵ�.
        PhotonNetwork.Instantiate(SYA_UserInfoManagerSaveLoad.SYA_UserInfoManager.Instance.Avatar, Vector3.zero, Quaternion.identity);
        SceneManager.LoadScene("KYG_Scene");
    }
}