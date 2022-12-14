using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class StarGenerator : MonoBehaviourPun
{
    public static StarGenerator instance;
    #region Input
    public TMP_InputField starNameInput;
    public TMP_Dropdown generateTypeDropdown;
    public TMP_InputField decInput;
    public TMP_InputField raInput;
    public TMP_InputField apparentMagnitudeInput;
    public TMP_InputField starAmount;
    public TMP_Dropdown typeDropdown;
    public TMP_Dropdown brightnessDropdown;
    public Button generateBtn;
    public List<GameObject> starTypeList = new List<GameObject>();
    public List<GameObject> starBrightnessList = new List<GameObject>();
    #endregion

 
    //별 이름
    public string starName;
    //적경
    public float ra;
    //적위
    public float dec;
    //별 종류
    public GameObject starType;
    //별 밝기
    public GameObject brightness;
    //겉보기 등급
    public float apparentMagnitude;

    public GameObject player;

    public GameObject starList;

    public GameObject createdStarListUI;

    public GameObject testObject;

    public int generateTypeNumber = 0;

    public bool drawStar;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        typeDropdown.onValueChanged.AddListener(OnTypeDropDownEvent);
        brightnessDropdown.onValueChanged.AddListener(OnBrightnessDropDownEvent);
        generateTypeDropdown.onValueChanged.AddListener(OnGenerateTypeDropDownEvent);
        typeDropdown.ClearOptions();
        brightnessDropdown.ClearOptions();
    }
    // Start is called before the first frame update
    void Start()
    {
        typeDropdownSet();
        brightnessDropDownSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            starNameInput.text = "TestStar";
            generateTypeDropdown.value = 2;
            typeDropdown.value = 1;
            apparentMagnitudeInput.text = "3";
            brightnessDropdown.value = 7;
            if (GameManager.instance.createdStarList.ContainsKey(starName))
            {
                starName += (" (" + starNumber + ")");
                starNumber++;
            }
            else
            {
                starNumber = 1;
            }
        }
        if (starNameInput.GetComponent<TMP_InputField>().isFocused)
        {
            player.GetComponent<PlayerMove>().enabled = false;
        }
        else
        {
            player.GetComponent<PlayerMove>().enabled = true;
        }
        if(starNameInput.text !="") starName = starNameInput.text;
        if (decInput.text != "") dec = float.Parse(decInput.text);
        if (raInput.text != "") ra = float.Parse(raInput.text);
        if (apparentMagnitudeInput.text != "") apparentMagnitude = float.Parse(apparentMagnitudeInput.text);

        if (drawStar)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                DrawStar();
            }

        }
        GenerateType();

    }
    int starNumber = 1;
    public void DrawStar()
    {
        Vector3 lookDir = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)) - Camera.main.transform.position;
        lookDir.Normalize();
        Vector3 rayOrigin = Camera.main.transform.position + lookDir * GameManager.instance.celestialSphereRadius * 1.1f;
        //Debug.DrawRay(rayOrigin, -lookDir * 1000, Color.red);

        Ray starDrawRay = new Ray(rayOrigin, -lookDir);
        RaycastHit starDrawInfo;

        if (Input.GetButtonDown("Fire1"))
        {
            print("클릭은 됐어요");
            if (Physics.Raycast(starDrawRay, out starDrawInfo))
            {
                print("레이도 쐈구요");
                if (starDrawInfo.collider.name == "CelestialSphere")
                {
                    print("천구도 맞았어요");
                    Vector3 shoot = starDrawRay.direction;
                    shoot.y = 0;
                    photonView.RPC("RPCDrawStar", RpcTarget.All,
                        typeDropdown.value-1,
                        starName,
                        starDrawInfo.point,
                        shoot,
                        ra,
                        dec,
                        apparentMagnitude,
                        brightnessDropdown.value - 1,
                        generateTypeNumber);
                    
                }
            }
        }
    }
    [PunRPC]
    void RPCDrawStar(int starType,string starName, Vector3 starDrawInfo, Vector3 shoot, float ra, float dec,float apparentMagnitude, int brightness, int generateTypeNumber)
    {
        CreatedStarList createdStarList = starList.GetComponent<CreatedStarList>();
        GameObject star = Instantiate(starTypeList[starType]);
        if (GameManager.instance.createdStarList.ContainsKey(starName))
        {
            starName += (" (" + starNumber + ")");
            starNumber++;
        }
        else
        {
            starNumber = 1;
        }
        GameManager.instance.createdStarList[starName] = star;
        dec = Mathf.Asin(starDrawInfo.y / GameManager.instance.celestialSphereRadius);
        ra = Mathf.Acos(starDrawInfo.z / (GameManager.instance.celestialSphereRadius * Mathf.Cos(dec)));
        if ((Vector3.Cross(Vector3.forward, shoot).normalized - Vector3.up).magnitude > 0.5f)
        {
            ra *= -1;
        }

        dec *= 180 / Mathf.PI;
        ra *= 180 / Mathf.PI;
        ra /= 15f;
        star.GetComponent<Star>().InfoSet(starName, ra, dec, starTypeList[starType], starBrightnessList[brightness], apparentMagnitude, generateTypeNumber);
        //player.GetComponent<PlayerRot>().StarSet(star.transform.position);
        createdStarList.Init(starName, star);
    }
    public void typeDropdownSet()
    {
        List<TMP_Dropdown.OptionData> typeOptionList = new List<TMP_Dropdown.OptionData>();
        typeOptionList.Add(new TMP_Dropdown.OptionData("생성하실 별의 종류를 선택하세요"));
        foreach (GameObject type in starTypeList)
        {
            typeOptionList.Add(new TMP_Dropdown.OptionData(type.gameObject.name));
        }
        typeDropdown.AddOptions(typeOptionList);
        typeDropdown.value = 0;
    }

    public void brightnessDropDownSet()
    {
        List<TMP_Dropdown.OptionData> brightnessOptionList = new List<TMP_Dropdown.OptionData>();
        brightnessOptionList.Add(new TMP_Dropdown.OptionData("생성하실 별의 밝기를 선택하세요"));
        foreach (GameObject brightness in starBrightnessList)
        {
            brightnessOptionList.Add(new TMP_Dropdown.OptionData(brightness.gameObject.name));
        }
        brightnessDropdown.AddOptions(brightnessOptionList);
        brightnessDropdown.value = 0;
    }

    public void OnGenerateTypeDropDownEvent(int index)
    {
        generateTypeNumber = index;
    }
    public void GenerateType()
    {
        switch (generateTypeNumber)
        {
            case 0:
                generateBtn.interactable = false;
                return;
            case 1:
                decInput.transform.gameObject.SetActive(true);
                raInput.transform.gameObject.SetActive(true);
                apparentMagnitudeInput.transform.gameObject.SetActive(true);
                starAmount.transform.gameObject.SetActive(false);
                starNameInput.interactable = true;
                typeDropdown.interactable = true;
                brightnessDropdown.interactable = true;
                if (starNameInput.text == "" || generateTypeDropdown.value == 0 || raInput.text ==""  || decInput.text == "" || typeDropdown.value == 0 || brightnessDropdown.value == 0)
                {
                    generateBtn.interactable = false;
                }
                else
                {
                    generateBtn.interactable = true;
                }
                drawStar = false;
                return;
            case 2:
                decInput.transform.gameObject.SetActive(false);
                raInput.transform.gameObject.SetActive(false);
                apparentMagnitudeInput.transform.gameObject.SetActive(true);
                starAmount.transform.gameObject.SetActive(false);
                starNameInput.interactable = true;
                typeDropdown.interactable = true;
                brightnessDropdown.interactable = true;
                if(starNameInput.text != ""|| typeDropdown.value == 0 || brightnessDropdown.value == 0)
                {
                    generateBtn.interactable = false;
                }
                else
                {
                    generateBtn.interactable = true;
                }
                drawStar = true;
                return;
            case 3:
                decInput.transform.gameObject.SetActive(false);
                raInput.transform.gameObject.SetActive(false);
                starAmount.transform.gameObject.SetActive(true);
                apparentMagnitudeInput.transform.gameObject.SetActive(false);
                starNameInput.interactable = false;
                typeDropdown.interactable = false;
                brightnessDropdown.interactable = false;
                if(starAmount.text != "")
                {
                    generateBtn.interactable = true;
                }
                else
                {
                    generateBtn.interactable = false;
                }
                return;
        }
    }
    public void OnTypeDropDownEvent(int index)
    {
        starType = starTypeList[index - 1];
    }
    public void OnBrightnessDropDownEvent(int index)
    {
        brightness = starBrightnessList[index - 1];
    }

    public void OnGenerateStarBtn()
    {
        if (generateTypeNumber ==3)
        {
            for(int i = 0; i < int.Parse(starAmount.text); i++)
            {
                starName = "Star" + i;
                ra = Random.Range(0f, 25f);
                dec = Random.Range(-90f, 91f);
                apparentMagnitude = Random.Range(0f, 8f);
                int brightnessType = Random.Range(1, starBrightnessList.Count);
                photonView.RPC("RPCRandomGenerate", RpcTarget.All,
                    0,
                    starName,
                    ra,
                    dec,
                    apparentMagnitude,
                    brightnessType-1,
                    generateTypeNumber
                    );

            }
        }
        else
        {
            if (GameManager.instance.createdStarList.ContainsKey(starName))
            {
                starName += (" (" + starNumber + ")");
                starNumber++;
            }
            else
            {
                starNumber = 1;
            }
            photonView.RPC("RPCNormalGenerate", RpcTarget.All,
                            typeDropdown.value-1,
                            starName,
                            ra,
                            dec,
                            apparentMagnitude,
                            brightnessDropdown.value-1,
                            generateTypeNumber
                            );
            player.GetComponent<PlayerRot>().StarSet(star.transform.position);
        }
        starNameInput.text = null;
        starName = null;
        raInput.text = null;
        decInput.text = null;
        apparentMagnitudeInput.text = null; 
    }
    public GameObject star;
    [PunRPC]
    void RPCNormalGenerate(int starType, string starName, float ra, float dec, float apparentMagnitude, int brightness,  int generateTypeNumber)
    {
        CreatedStarList createdStarList = starList.GetComponent<CreatedStarList>();
        star = Instantiate(starTypeList[starType]);
        GameManager.instance.createdStarList[starName] = star;
        star.GetComponent<Star>().InfoSet(starName, ra, dec, starTypeList[starType], starBrightnessList[brightness], apparentMagnitude, generateTypeNumber);
        createdStarList.Init(starName, star);
    }
    [PunRPC]
    void RPCRandomGenerate(int starType, string starName, float ra, float dec, float apparentMagnitude, int brightnessType, int generateTypeNumber)
    {
            CreatedStarList createdStarList = starList.GetComponent<CreatedStarList>();
            //GameObject star = Instantiate(starTypeList[0]);
            GameObject star = Instantiate(starTypeList[starType]);            
            GameManager.instance.createdStarList[starName] = star;          
            brightness = starBrightnessList[brightnessType];
            star.GetComponent<Star>().InfoSet(starName, ra, dec, starTypeList[starType], brightness, apparentMagnitude, generateTypeNumber);
            createdStarList.Init(starName, star);       
    }

    public void OnStarListBtn()
    {
        if (createdStarListUI.activeSelf)
        {
            createdStarListUI.SetActive(false);
        }
        else
        {
            createdStarListUI.SetActive(true);
        }
    }
    public void OnCloseBtn()
    {
        gameObject.SetActive(false);
    }
}
