using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SCINERGY_YS_ButtonManager : MonoBehaviourPun
{
    public GameObject colorPicker, picker, back, front, eraser, clear, basicBrush, pencil, calligraphy, marker, brushSize, circle, spuit;
    public SCINERGY_BrushNet_YS brushNet;
    public Image palette;

    // ���� ���� ��ġ ��
    float circle_temp;
    // ������Ʈ ���� ��
    bool b_spuit = false;

    // Start is called before the first frame update
    void Start()
    {
        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
        palette = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // �귯�� �� �־��ֱ�
        if (brushNet == null)
        {
            if (CollaborateModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player1(Clone)").GetComponent<SCINERGY_BrushNet_YS>();
            }
            else if (CompeteModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player(Clone)").GetComponent<SCINERGY_BrushNet_YS>();
            }
        }

        // �귯�� ������ ����
        BrushSize();

        // �ȷ�Ʈ �ѱ�
        palette.enabled = true;

        // ������Ʈ
        if (b_spuit == true)
        {
            Spuiting();
        }
    }

    public void PaletteOnOff()
    {
        if (colorPicker.activeSelf == false)
        {
            colorPicker.SetActive(true);
            picker.SetActive(true);
            back.SetActive(true);
            front.SetActive(true);
            eraser.SetActive(true);
            clear.SetActive(true);
            basicBrush.SetActive(true);
            pencil.SetActive(true);
            calligraphy.SetActive(true);
            marker.SetActive(true);
            brushSize.SetActive(true);
            spuit.SetActive(true);
        }
        else
        {
            colorPicker.SetActive(false);
            picker.SetActive(false);
            back.SetActive(false);
            front.SetActive(false);
            eraser.SetActive(false);
            clear.SetActive(false);
            basicBrush.SetActive(false);
            pencil.SetActive(false);
            calligraphy.SetActive(false);
            marker.SetActive(false);
            brushSize.SetActive(false);
            spuit.SetActive(false);
        }
    }

    public void Eraser()
    {
        if (brushNet.b_eraser == false)
        {
            brushNet.b_eraser = true;

            // ���찳 ���� �Ҵ�
            brushNet.drawPrefab_temp = brushNet.drawPrefab;
            brushNet.drawPrefab = Resources.Load<GameObject>("YS/Eraser");
            brushNet.drawPrefabName = "YS/Eraser";
        }
        else
        {
            brushNet.b_eraser = false;

            // ���찳�� ����ϱ� �� ������
            brushNet.drawPrefab = brushNet.drawPrefab_temp;
        }
    }

    public void BackFunction()
    {
        for (int i = brushNet.lines[brushNet.myCanvasIdx].Count - 1; i >= 0; i--)
        {
            if (brushNet.lines[brushNet.myCanvasIdx][i].activeSelf == true)
            {
                brushNet.lines[brushNet.myCanvasIdx][i].SetActive(false);
                break;
            }
        }

        // ��Ʈ��ũ (�ٸ� ��������׵� ����)
        brushNet.photonView.RPC("RpcCtrZ", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void FrontFunction()
    {
        for (int i = 0; i < brushNet.lines[brushNet.myCanvasIdx].Count; i++)
        {
            if (brushNet.lines[brushNet.myCanvasIdx][i].activeSelf == false)
            {
                brushNet.lines[brushNet.myCanvasIdx][i].SetActive(true);
                break;
            }
        }

        // ��Ʈ��ũ (�ٸ� ��������׵� ����)
        brushNet.photonView.RPC("RpcCtrY", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void CanvasClear()
    {
        for (int i = 0; i < brushNet.lines[brushNet.myCanvasIdx].Count; i++)
        {
            Destroy(brushNet.lines[brushNet.myCanvasIdx][i].gameObject);
        }
        brushNet.lines[brushNet.myCanvasIdx].Clear();

        // ��Ʈ��ũ (�ٸ� ��������׵� ����)
        brushNet.photonView.RPC("RcpAllClear", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void BasicBrush()
    {
        if (brushNet.b_eraser == true)
        {
            // ���찳 ���¿��ٸ� ���!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 1;

        // �귯�� ���� �Ҵ�
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Brush");
        brushNet.drawPrefabName = "YS/Brush";
    }

    public void Pencil()
    {
        if (brushNet.b_eraser == true)
        {
            // ���찳 ���¿��ٸ� ���!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 4;

        // ���� ���� �Ҵ�
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Pencil");
        brushNet.drawPrefabName = "YS/Pencil";
    }

    public void Calligraphy()
    {
        if (brushNet.b_eraser == true)
        {
            // ���찳 ���¿��ٸ� ���!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 5;

        // Ķ���׶��� ���� �Ҵ�
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
        brushNet.drawPrefabName = "YS/Calligraphy";
    }

    public void Marker()
    {
        if (brushNet.b_eraser == true)
        {
            // ���찳 ���¿��ٸ� ���!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 3;

        // ��Ŀ ���� �Ҵ�
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Marker");
        brushNet.drawPrefabName = "YS/Marker";
    }

    public void BrushSize()
    {
        // circle�� �����̸�,
        if (circle_temp != circle.GetComponent<RectTransform>().localPosition.x)
        {
            // circle�� �񱳴���� ���� �귯���� ũ�⿡ �ش��ϴ� ��ġ(0.05 * 0.0013812154696133)
            if (circle_temp < circle.GetComponent<RectTransform>().localPosition.x)
            {
                // ������ ��� (circle�� ������ �� �ִ� ������ 362, �귯�� ������� 0.5�� ���)
                brushNet.size += (circle.GetComponent<RectTransform>().localPosition.x - circle_temp) * 0.0013812154696133f;
            }
            else if (circle_temp > circle.GetComponent<RectTransform>().localPosition.x)
            {
                // ������ ��� (circle�� ������ �� �ִ� ������ 362, �귯�� ������� 0.5�� ���)
                brushNet.size -= (circle_temp - circle.GetComponent<RectTransform>().localPosition.x) * 0.0013812154696133f;
            }
        }

        // circle�� ���� ��ġ ����
        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
    }

    public void SpuitOn()
    {
        b_spuit = true;
    }

    void Spuiting()
    {
        brushNet.StartCoroutine("Spuit");

        if (Input.GetMouseButtonDown(0))
        {
            brushNet.colorObject.GetComponent<ColorPickerTest>().selectedColor = brushNet.spuit;

            b_spuit = false;
        }
    }
}