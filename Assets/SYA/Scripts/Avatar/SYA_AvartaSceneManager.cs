using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using SYA_UserInfoManagerSaveLoad;

public class SYA_AvartaSceneManager : MonoBehaviourPun
{

    public static SYA_AvartaSceneManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    //�ƹ�Ÿ ����â ȭ��ǥ Ŭ���� ������ �� ������ ����
    //ȭ��ǥ,������ �� �����ϵ�,�ƹ�Ÿ������
    public Button right_Arrow;
    public Button left_Arrow;
    public List<GameObject> pos = new List<GameObject>();
    public List<Vector3> sca = new List<Vector3>();
    public List<GameObject> avatar = new List<GameObject>();

    //pos 1�� ��ġ�� ������Ʈ �����ؼ� ���ʿ� 2.5��
    public Transform avatarPos;
    Vector3 avatarSca = new Vector3(2.5f, 2.5f, 2.5f);

    //���׶�� UI ���. �ڱⰡ �����  �ƹ�Ÿ�� ������ ���� ���׶�̷�
    //�ƹ�Ÿn���� ����� ���� n�� UI���� �������׶�� ��ȣ������
    //�ƴ϶�� �Ͼᵿ�׶�� ��ȣ ������
    public Action<int> WhiteClircle;

    // Start is called before the first frame update
    void Start()
    {
        ScaSet();
        Camera.main.cullingMask = ~(1 << 18);
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonOn)
        {
            AvatarPosSca(0);
            AvatarPosSca(1);
            AvatarPosSca(2);
            AvatarPosSca(3);
            AvatarPosSca(4);
        }
        //���̾� �ٲ��ְ�
        /*        if (LayerChange)
                {
                    for (int i = 0; i < pos.Count; ++i)
                    {
                        //AvatarLayerChange(avatar[i].transform.position.z, avatar[i].transform);
                    }
                    LayerChange = false;
                }*/
        //�ø�����ũ ���
    }
    //아바타의 순서
    public GameObject[] avatarGo = new GameObject[5];

    //bool LayerChange;
    //������Ʈ ������ ������ �Լ� ����
    bool buttonOn;
    //��ư�� ������ ȸ���������� ��Ÿ���� �Ұ�
    bool rotateIng;
    //Lerp�̿��ؼ� �ƹ�Ÿ�� ��ġ�� ũ�� ��ȭ
    public void AvatarPosSca(int num)
    {
        //print(111);
        Vector3 avartaPos = avatar[num].transform.position;
        Vector3 avartaSca = avatar[num].transform.localScale;
        iTween.ScaleTo(avatar[num], sca[num], 2f);
        iTween.MoveTo(avatar[num], pos[num].transform.position, 2f);
        avatar[num].transform.localScale = avartaSca;
        rotateIng = false;
        buttonOn = false;
        /*if (Vector3.Distance(avatar[num].transform.position, pos[num].transform.position) < 0.3f
            && Vector3.Distance(avatar[num].transform.localScale, sca[num]) < 0.3f)
        {
            //avatar[num].transform.position = pos[num].transform.position;
            avatar[num].transform.localScale = sca[num];
            //LayerChange = true;
            //가운데에 있는 캐릭터인가?
            *//*if (1.2f - avatar[num].transform.localScale.x <= 0.35f)
            {
                WhiteClircle(num);
                //print(2222222);
                CreatAvatar(int.Parse(avatarGo[2].gameObject.name));
            }*//*
        }*/
    }

    List<GameObject> avatarList = new List<GameObject>();
     int avatarP= 3;

    //��� �ִ� �ƹ�Ÿ �����ؼ� ���ʿ� ����
    public void CreatAvatar(GameObject avatar)
    {
        /*if (avatarList.Count > 0)
        {
            Destroy(GameObject.Find(avatarList[0].name).gameObject);
            avatarList.Clear();
        }
        avatarP = Instantiate(avatar);
        avatarList.Add(avatarP);
        avatarP.transform.localScale = avatarSca;
        avatarP.transform.position = avatarPos.position;*/
    }
    public GameObject[] head = new GameObject[5];
    int exnum = 3;
    public void CreatAvatar(int name, bool reft)
    {
        if (reft)
        {
            exnum = (name+1) > 4 ? 0 : (name+1);

        }
        else
        {
            
            exnum = (name - 1) < 0 ? 4 : (name - 1);

        }
        print(exnum);
        head[exnum].SetActive(false);
        exnum = name;
        head[name].SetActive(true);
            avatarP = int.Parse(head[name].gameObject.name);
    }

    //�ƹ�Ÿ ���� ����
    public void AvatarSet()
    {
        buttonOn = true;
    }

    //ó�� ũ�� ����
    public void ScaSet()
    {
        for (int i = 0; i < avatar.Count; ++i)
        {
            if (i != 2)
            {
                sca.Add(new Vector3(0.8f, 0.8f, 0.8f));
            }
            else
            {
                sca.Add(new Vector3(1.2f, 1.2f, 1.2f));
            }
        }
        AvatarSet();
    }

    //�ƹ�Ÿ ����â ȭ��ǥ Ŭ���� ������ �� ������ ����
    public void ArrowClickR()
    {
        if (rotateIng) return;
        AvatarRotate(false);
        rotateIng = true;
    }
    public void ArrowClickL()
    {
        if (rotateIng) return;
        AvatarRotate(true);
        rotateIng = true;
    }

    public void zeroOff(bool left)
    {
        for (int i = 0; i < avatarGo.Length; ++i)
        {
            if (i == 0 || i == 4)
            {
                avatarGo[i].gameObject.GetComponent<Image>().enabled = false;
                avatarGo[i].gameObject.GetComponentInChildren<SYA_ChilImage>().Image.enabled = false;
                avatarGo[i].gameObject.GetComponentInChildren<SYA_ChilImage>().fade.enabled = false;
            }
            else
            {
                avatarGo[i].gameObject.GetComponent<Image>().enabled = true;
                avatarGo[i].gameObject.GetComponentInChildren<SYA_ChilImage>().Image.enabled = true;
                if (i == 2)
                {
                    avatarGo[i].gameObject.GetComponentInChildren<SYA_ChilImage>().fade.enabled = false;
                    WhiteClircle(int.Parse(avatarGo[2].gameObject.name));
                    CreatAvatar(int.Parse(avatarGo[2].gameObject.name)-1, left);
                    print(int.Parse(avatarGo[2].gameObject.name)-1);
                }
                else
                {
                    avatarGo[i].gameObject.GetComponentInChildren<SYA_ChilImage>().fade.enabled = true;
                }
            }
        }

    }

    //�ƹ�Ÿ �̵��� ���� ������ �� ������ ����
    public void AvatarRotate(bool left)
    {
        if (left)
        {
            pos.Add(pos[0]);
            pos.RemoveAt(0);
            sca.Add(sca[0]);
            sca.RemoveAt(0);

            GameObject av = avatarGo[4];
            for (int i = avatarGo.Length - 1; i > 0; i--)
            {
                avatarGo[i] = avatarGo[i - 1];
            }
            avatarGo[0] = av;
        }
        else
        {
            pos.Insert(0, pos[avatar.Count - 1]);
            pos.RemoveAt(avatar.Count);
            sca.Insert(0, sca[avatar.Count - 1]);
            sca.RemoveAt(avatar.Count);

            GameObject av = avatarGo[0];
            for (int i = 0; i < avatarGo.Length - 1; i++)
            {
                avatarGo[i] = avatarGo[i + 1];
            }
            avatarGo[4] = av;
        }
        AvatarSet();
        zeroOff(left);
    }

    public InputField nameField;

    //�ƹ�Ÿ ���� ��ư�� ������ ��
    //���ʿ� ����� �ƹ�Ÿ ������ ������ ����

    //로딩을 준비하기 위해 동작할 수 있는 오브젝트 다 끄기
    public GameObject avatarBg;
    public GameObject Go;
    public void AvatarSellect()
    {
        if (nameField.text == "") return;
        SYA_UserInfoSave.Instance.AvatarSave($"Player {avatarP}");
        SYA_UserInfoSave.Instance.NicNameSave(nameField.text);
        avatarBg.SetActive(false);
        Go.SetActive(false);
        //SYA_SceneChange.Instance.SymposiumRoomList();
    }

    /* public void AvatarLayerChange(float z, Transform avatar)
     {
         if(z>=-2)
         {
             avatar.gameObject.layer = 18;
             foreach(Transform child in avatar.transform)
             {
                 AvatarLayerChange(avatar.transform.position.z, child);
             }
         }
         else
         {
             avatar.gameObject.layer = 17;
             foreach (Transform child in avatar.transform)
             {
                 AvatarLayerChange(avatar.transform.position.z, child);
             }
         }
     }*/
}

