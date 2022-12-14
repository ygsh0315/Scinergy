using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using SYA_UI;

public class SYA_SympoGoTo : MonoBehaviourPun
{
    public GameObject space;
    PlayerMove playerMove;
    SYA_PlayerCompo compo;

    private void Update()
    {
        if (space == null)Destroy(gameObject);
        GetComponent<Collider>().enabled = SceneManager.GetActiveScene().name.Contains("Sympo");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!SceneManager.GetActiveScene().name.Contains("Symposi")) return;
        if (other.gameObject.name.Contains("Player"))
        {
            if (!other.GetComponent<PhotonView>().IsMine) return;
            /*            if (SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName].IsMine)
                        {*/
            print("in");
            //플레이어의 이동과 화면 움직임 막기
            playerMove = other.GetComponent<PlayerMove>();
            compo = other.GetComponent<SYA_PlayerCompo>();
            playerMove.cc.enabled = false;
            if (gameObject.name.Contains("Sola"))
                other.gameObject.transform.position += new Vector3(0.5f, 0, 0.5f);
            else
                other.gameObject.transform.position += new Vector3(-0.5f, 0, -0.5f);


            //playerMove.enabled = false;
            compo.isTrigger = true;

            //공간 이동 버튼 띄우기
            space.SetActive(true);
            //SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName].RPC("onoff", RpcTarget.All, !space.activeSelf);
            //}
        }
    }
    [PunRPC]
    public void onoff(bool g)
    {
        space.SetActive(g);
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (!SceneManager.GetActiveScene().name.Contains("Symposi")) return;
        if (other.gameObject.name.Contains("Player"))
        {
            if (!other.GetComponent<PhotonView>().IsMine) return;
            *//*            if (SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName].IsMine)
                        {*//*

            print("out");
            //공간 이동 버튼 띄우기
            space.SetActive(false);
            //SYA_SymposiumManager.Instance.player[PhotonNetwork.NickName].RPC("onoff", RpcTarget.All, !space.activeSelf);
            //}
        }
    }*/

    public void OffGameObject()
    {
        space.SetActive(false);
        //playerMove.enabled = true;
        compo.isTrigger = false;
        playerMove.cc.enabled = true;
    }
}
