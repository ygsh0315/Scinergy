using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SCINERGY_BrushNet_YS : MonoBehaviourPun
{
    // ���� ��ȣ
    public int toolNum = 1;

    public GameObject drawPrefab;
    public GameObject drawCanvas, drawCanvas_parent;
    GameObject theTrail;
    Vector3 startPos;
    Vector3 nextPos;

    // ������
    public float size = 0.05f;
    // �� ����
    public GameObject colorObject;

    // ���찳
    public bool b_eraser;
    public GameObject drawPrefab_temp;

    // ������Ʈ
    public Color spuit;

    // ���� ĵ���� ������ �׸��� �ִ��� �Ǵ�(Ư�� �׸��ٰ� �������� ���)
    bool b_onCanvas;

    // ����
    AudioSource sound;

    // ��Ʈ��ũ
    public int myCanvasIdx;
    int sortingOrder;
    public string drawPrefabName; // ��Ʈ��ũ�� �Ѱ��� ���� drawPrefab�� �̸�
    public List<List<GameObject>> lines = new List<List<GameObject>>(); //��Ʈ��ũ���� ����� 2���� ����Ʈ (��� �� ����)

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            if(CollaborateModeManager_BH.instance)
            {
                drawCanvas = CompeteModeManager_BH.instance.playerCanvas[0].GetComponentsInChildren<Transform>()[1].gameObject;
                drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[0].GetComponent<Transform>().gameObject;
                colorObject = GameObject.Find("ColorPicker");
                colorObject.SetActive(false);
                myCanvasIdx = 0;
            }
            else if(CompeteModeManager_BH.instance)
            {
                myCanvasIdx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
                drawCanvas = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponentsInChildren<Transform>()[1].gameObject;
                drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponent<Transform>().gameObject;
                colorObject = GameObject.Find("ColorPicker");
                colorObject.SetActive(false);
            }
        }

        drawPrefab = Resources.Load<GameObject>("YS/Brush");
        drawPrefabName = "YS/Brush";
        drawPrefab.GetComponent<LineRenderer>().useWorldSpace = false;

        // 2���� ����Ʈ ũ�� ����
        for(int i = 0; i < 8; i++)
        {
            lines.Add(new List<GameObject>());
        }

        // ����
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // ���� (1���� �귯��, 2���� ������, 3���� ��Ŀ, 4���� ����, 5���� Ķ���׶���, 6���� ũ����, 7���� ��������, 8���� ��ȭ, 9���� ��äȭ)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                toolNum = 1;

                // �귯�� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Brush");
                drawPrefabName = "YS/Brush";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                toolNum = 3;

                // ��Ŀ ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Marker");
                drawPrefabName = "YS/Marker";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                toolNum = 4;

                // ���� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Pencil");
                drawPrefabName = "YS/Pencil";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                toolNum = 5;

                // Ķ���׶��� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
                drawPrefabName = "YS/Calligraphy";
            }

            // �׸� �׸���
            Draw();

            // ���찳
            Eraser();

            // �������(�ڷ�)
            CtrZ();

            // �ǵ�����(������)
            CtrY();

            // ���� �����
            AllClear();
        }
    }
    void Draw()
    {
        Color eraser = Color.white;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // ����
                    sound.Play();

                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯���� ��, ��Ŀ�� ��, ������ ��, Ķ���׶����� ��, ũ������ ��, ���������� ��, ��ȭ�� ��, ��äȭ�� ��
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        // ���� �׸��� ��, ������ ����
                        drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                        if (toolNum == 4) // ������ ����� �ٸ� �����鿡 ���� ����!
                        {
                            // �� ���� ����
                            if (size <= 0.04)
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = 0.003f;
                            }
                            else
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size - 0.04f;
                            }
                        }
                        // ���� �׸��� ��, �� ����
                        drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    }

                    Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                    // ���찳
                    if (b_eraser == true)
                    {
                        drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                        color = eraser;
                    }

                    // ���߿� ���� ���� ���� �ö���Բ�
                    sortingOrder++;
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder = sortingOrder;

                    // �� ����
                    theTrail = Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                    theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                    // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                    for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                    {
                        if (lines[myCanvasIdx][i].activeSelf == false)
                        {
                            Destroy(lines[myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                            lines[myCanvasIdx].RemoveAt(i);
                            i--;
                        }
                    }
                    // ���� ��, ����Ʈ�� �־��ֱ�
                    lines[myCanvasIdx].Add(theTrail);

                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        startPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                        theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
                    }

                    // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                    photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                }
                else
                {
                    // ĵ���� ������ �׸��� ���� ����!
                    b_onCanvas = false;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // ���� ĵ������ �����ٰ� ������ ��, ������ �� �ڸ����� �ٽ� ����
                    if (b_onCanvas == false)
                    {
                        // ����
                        sound.Play();

                        // �� ����
                        theTrail =Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                        theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                        Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                        // ���찳
                        if (b_eraser == true)
                        {
                            drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                            drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                            color = eraser;
                        }

                        // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                        for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                        {
                            if (lines[myCanvasIdx][i].activeSelf == false)
                            {
                                Destroy(lines[myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                                lines[myCanvasIdx].RemoveAt(i);
                                i--;
                            }
                        }
                        // ���� ��, ����Ʈ�� �־��ֱ�
                        lines[myCanvasIdx].Add(theTrail);

                        if (Physics.Raycast(mouseRay, out hit))
                        {
                            startPos = hit.point - theTrail.transform.parent.position;

                            theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                            theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);

                            // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                            photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                        }
                    }

                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯�� �� ��, ��Ŀ�� ��, ������ ��, Ķ���׶����� ��, ũ������ ��, ���������� ��, ��ȭ�� ��, ��äȭ�� ��
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        nextPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                        photonView.RPC("RpcDrawing", RpcTarget.OthersBuffered, theTrail.GetComponent<LineRenderer>().positionCount, positionIndex, nextPos);
                    }
                }
                else
                {
                    // ĵ���� ������ �׸��� ���� ����!
                    b_onCanvas = false;

                    // ����
                    sound.Stop();
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            // ����
            sound.Stop();
        }
    }

    [PunRPC]
    void RpcDrawStart(float _size, float r, float g, float b, int _sortingOrder, int _myCanvasIdx, Vector3 _startPos, string _drawPrefabName)
    {
        drawPrefab = Resources.Load<GameObject>(_drawPrefabName);

        drawCanvas = CompeteModeManager_BH.instance.playerCanvas[_myCanvasIdx].GetComponentsInChildren<Transform>()[1].gameObject;
        drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[_myCanvasIdx].GetComponent<Transform>().gameObject;

        Color color = new Color(r, g, b);

        theTrail = Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
        theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

        LineRenderer lr = theTrail.GetComponent<LineRenderer>();
        ColorPickerTest cp = theTrail.GetComponent<ColorPickerTest>();
        // ���� �׸��� ��, ������ ����
        theTrail.GetComponent<LineRenderer>().widthMultiplier = _size;
        // ���� �׸��� ��, �� ����
        theTrail.GetComponent<LineRenderer>().startColor = color;
        theTrail.GetComponent<LineRenderer>().endColor = color;
       
        // ���߿� ���� ���� ���� �ö���Բ�
        theTrail.GetComponent<LineRenderer>().sortingOrder = _sortingOrder;

        // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            if (lines[_myCanvasIdx][i].activeSelf == false)
            {
                Destroy(lines[_myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                lines[_myCanvasIdx].RemoveAt(i);
                i--;
            }
        }

        // ���� ��, ����Ʈ�� �־��ֱ�
        lines[_myCanvasIdx].Add(theTrail);
        theTrail.GetComponent<LineRenderer>().SetPosition(0, _startPos);
        theTrail.GetComponent<LineRenderer>().SetPosition(1, _startPos);      
    }

    [PunRPC]
    void RpcDrawing(int _positionCount, int _positionIndex, Vector3 _nextPos)
    {
        theTrail.GetComponent<LineRenderer>().positionCount = _positionCount;
        theTrail.GetComponent<LineRenderer>().SetPosition(_positionIndex, _nextPos);
    }

    void Eraser()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (b_eraser == false)
            {
                b_eraser = true;
                // ���찳 ���� �Ҵ�
                drawPrefab_temp = drawPrefab;
                drawPrefab = Resources.Load<GameObject>("YS/Eraser");
                drawPrefabName = "YS/Eraser";
            }
            else if (b_eraser == true)
            {
                b_eraser = false;
                // ���찳�� ����ϱ� �� ������
                drawPrefab = drawPrefab_temp;
            }
        }
    }

    void CtrZ()
    {
        if(Input.GetKeyDown(KeyCode.Home))
        {
            for(int i = lines[myCanvasIdx].Count - 1; i >= 0; i--)
            {
                if(lines[myCanvasIdx][i].activeSelf == true)
                {
                    lines[myCanvasIdx][i].SetActive(false);
                    break;
                }
            }

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
            photonView.RPC("RpcCtrZ", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RpcCtrZ(int _myCanvasIdx)
    {
        for (int i = lines[_myCanvasIdx].Count - 1; i >= 0; i--)
        {
            if (lines[_myCanvasIdx][i].activeSelf == true)
            {
                lines[_myCanvasIdx][i].SetActive(false);
                break;
            }
        }
    }

    void CtrY()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            for (int i = 0; i < lines[myCanvasIdx].Count; i++)
            {
                if (lines[myCanvasIdx][i].activeSelf == false)
                {
                    lines[myCanvasIdx][i].SetActive(true);
                    break;
                }
            }

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
            photonView.RPC("RpcCtrY", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RpcCtrY(int _myCanvasIdx)
    {
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            if (lines[_myCanvasIdx][i].activeSelf == false)
            {
                lines[_myCanvasIdx][i].SetActive(true);
                break;
            }
        }
    }

    void AllClear()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            for(int i = 0; i < lines[myCanvasIdx].Count; i++)
            {
                Destroy(lines[myCanvasIdx][i].gameObject);
            }
            lines[myCanvasIdx].Clear();

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
            photonView.RPC("RcpAllClear", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RcpAllClear(int _myCanvasIdx)
    {
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            Destroy(lines[_myCanvasIdx][i].gameObject);
        }
        lines[_myCanvasIdx].Clear();
    }

    IEnumerator Spuit()
    {
        // ReadPixels�Լ��� ���� ������ ���ۿ��� �ȼ����� �ҷ����� ������, �ش� �������� �������� ������ ���� �� ����Ǿ�� �Ѵ�.
        // (��, ReadPixels�Լ� ���� ���� WaitForEndOfFrame�� ����Ǿ�� �Ѵ�.)
        yield return new WaitForEndOfFrame();

        // ��ũ�� �� ���
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        spuit = screenShot.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);

        // �޸� ����(�����ָ� ������)
        Destroy(screenShot);
    }
}