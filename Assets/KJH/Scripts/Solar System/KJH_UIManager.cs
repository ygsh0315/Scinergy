using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.IO;


// (v) 관측 시간을 Text로 나타내고 싶다. 
// - 년, 월, 일, 시간, 분, 오전/오후


// (v) 정지 버튼
// 스크롤바의 값을 0.5로 지정하고 싶다.

// (v) 재생 버튼
// - 정지 직전의 단위 시간으로 지정하고 싶다.

// 현재 시각 버튼 (보류)
// - (v) 관측 시각을 현재 시각으로 지정하고 싶다.
// - 행성들의 위치도 현재 시각 기준으로 되돌리고 싶다.
// -- 현재 위치를 계속 저장해 둘 List가 필요하다.
// 현재 위치를 저장해두는 기준 시간-> 분..단위..?

// 줌 스크롤 바
// 바를 위로 올리면 줌인
// 바를 아래로 내리면 줌아웃


public class KJH_UIManager : MonoBehaviour
{
    public KJH_CameraTest2 cam;
    public static KJH_UIManager instance; 
    // 현재 시간 : 계속 Update
    DateTime curDate;
    // 관측 시간 : 스크롤바 값에 따라 변경됨
    DateTime obsDate;

    public Text yearText;
    public Text dateText;
    public Text timeText;
    public Text obsDateText;

    float curTime = 0f;
    float updateTime = 1f;
    float originScrollValue = 0.75f;

    // UI
    public GameObject controlTimeUI;
    public GameObject defalutUI;

    public bool isActiveInfo = false;
    bool isActiveControl = false;

    Dictionary<string, GameObject> dict_UI = new Dictionary<string, GameObject>();

    public KJH_SelectPlanet selectPlanet;

    // 행성 목록 버튼
    public RectTransform trContent_CBList;
    List<Button> buttons = new List<Button>();

    //[Header("Defalut")]


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        curDate = DateTime.Now;
        obsDate = curDate;
        SetObsDateText();

        for (int i = 0; i < transform.childCount; i++)
        {
            // <UI_name, UI_gameObject>
            dict_UI.Add(transform.GetChild(i).name, transform.GetChild(i).gameObject);
        }

        MoveDefalutUI(1f);

        for(int i = 0; i<trContent_CBList.childCount; i++)
        {
            buttons.Add(trContent_CBList.GetChild(i).GetComponent<Button>());
            //buttons[i].onClick.AddListener(OnClick_ButtonCBList);
        }

    }

    private void FixedUpdate()
    {
        // 현재 시각 Update
        curDate = DateTime.Now;

        // 1초마다 관측 시간 갱신
        curTime += Time.fixedDeltaTime;
        if (curTime > updateTime)
        {
            curTime = 0f;

            if (KJH_SolarSystem.instance.unitTimeNum != 0)
            {
                SetObsDate();
            }

        }
    }

    // 관측 시간 설정 함수
    void SetObsDate()
    {
        switch (KJH_SolarSystem.instance.unitTime)
        {
            case KJH_SolarSystem.UnitTime.year:
                obsDate = obsDate.AddYears((int)KJH_SolarSystem.instance.unitTimeNum);
                break;
            case KJH_SolarSystem.UnitTime.month:
                obsDate = obsDate.AddMonths((int)KJH_SolarSystem.instance.unitTimeNum);
                break;
            case KJH_SolarSystem.UnitTime.day:
                obsDate = obsDate.AddDays(KJH_SolarSystem.instance.unitTimeNum);
                break;
            case KJH_SolarSystem.UnitTime.hour:
                obsDate = obsDate.AddHours(KJH_SolarSystem.instance.unitTimeNum);
                break;
        }

        SetObsDateText();
    }

    void SetObsDateText()
    {
        // 2022
        yearText.text = obsDate.ToString("yyyy");
        // 10월 23일
        dateText.text = obsDate.ToString("MM") + "월 " + obsDate.ToString("dd") + "일";
        // 01시 15분 오전
        timeText.text = obsDate.ToString("HH") + " : " + obsDate.ToString("mm") + " " + obsDate.ToString("tt");

        // 2022년 10월 23일 01시 15분 오전
        obsDateText.text = yearText.text + "년 " + dateText.text + " " + timeText.text;
    }

    public void StopObservation()
    {
        if (Mathf.Abs(KJH_SolarSystem.instance.unitTimeScrolbar.value - 0.5f) >= 0.01f)
        {
            originScrollValue = KJH_SolarSystem.instance.unitTimeScrolbar.value;
            KJH_SolarSystem.instance.unitTimeScrolbar.value = 0.5f;
        }
    }
    public void PlayObservation()
    {
        if (Mathf.Abs(KJH_SolarSystem.instance.unitTimeScrolbar.value - 0.5f) < 0.01f)
        {
            KJH_SolarSystem.instance.unitTimeScrolbar.value = originScrollValue;
        }
    }

    // 위치 이동 시켜야 함 (보류)
    public void ToCurTime()
    {
        obsDate = DateTime.Now;
        SetObsDateText();
    }


    /* 버튼 관련 함수 */
    // DefalutUI -> ControllTime
    public void OnClick_ControllObsTime()
    {
        MoveDefalutUI(-1f);
        MoveControllTimeUI(1f);
        isActiveControl = true;
    }

    // ControllTime -> DefalutUI
    public void OnClick_Back()
    {
        MoveControllTimeUI(-1f);
        MoveDefalutUI(1f);
        isActiveControl = false;
    }

    // 천체 메뉴 닫기
    public void OnClick_CloseInfoMenu()
    {
        GameObject go = dict_UI["UI_Info"];

        // 현재 UI를 끄고, 기본 UI를 띄우고 싶다.
        if (isActiveInfo)
        {
            MoveDefalutUI(1f);
            MoveCBInfoMenu(-1f);
        }
        Invoke("ViewCBInfoMenu", 2f);

        SetActiveModel(true);

        cam.isViewNucleus = false;
        cam.isViewUI = false;
        cam.ChangeCenter(Screen.width * 0.5f);
    }

    public void OpenInfoMenu()
    {
        if (!isActiveInfo)
        {
            //MoveDefalutUI(-1f);
            //MoveCBInfoMenu(1f);

            //cam.isViewUI = true;
            //float centerX = 600 + (Screen.width - 600) * 0.5f;
            //cam.ChangeCenter(centerX);

            Transform tr = dict_UI["UI_Info"].transform;

            tr.GetChild(0).gameObject.SetActive(true);
            tr.GetChild(1).gameObject.SetActive(false);
            tr.GetChild(2).gameObject.SetActive(false);

            tr.gameObject.SetActive(true);

        }
        
        if (isActiveControl)
        {
            MoveControllTimeUI(-1f);
        }

        StopObservation();
    }

    // 기본 UI 이동 함수
    public void MoveDefalutUI(float sign)
    {
        Transform tr = dict_UI["UI_Defalut"].transform;
        //iTween.MoveTo(tr.GetChild(0).gameObject, iTween.Hash("x", tr.GetChild(0).position.x - 120f * sign, "Time", 2f));
        iTween.MoveTo(tr.GetChild(0).gameObject, iTween.Hash("y", tr.GetChild(0).position.y + 120f * sign, "Time", 2f));
    }
    // 시간 제어 UI 이동 함수
    public void MoveControllTimeUI(float sign)
    {
        //isActiveControl = sign > 0 ? true : false;
        Transform tr = dict_UI["UI_ControlTime"].transform;
        iTween.MoveTo(tr.gameObject, iTween.Hash("y", tr.position.y + 300f * sign, "Time", 2f));
    }

    // 행성 정보 메뉴 이동
    public void MoveCBInfoMenu(float sign)
    {
        isActiveInfo = sign > 0 ? true : false;
        iTween.MoveTo(dict_UI["UI_Info"], iTween.Hash("x", 425f * sign, "time", 2f));
    }

    // 정보 UI 관련 함수
    public void ViewCBInfoMenu()
    {
        SetActiveUI("UI_InfoMenu");
       
    }

    // 행성 정보 보기
    public void ViewCBInfo()
    {
        SetActiveUI("UI_ViewCBInfo");
    }

    // 천체 내부 구조 보기
    public void ViewCBStructure()
    {
        // ui 변경
        SetActiveUI("UI_ViewCBStructure");

        // 모델 변경 : 현재 선택된 모델의 부모의 1번째 자식은 끄고, 2번째 자식은 킨다.
        SetActiveModel(false);

        // 카메라 : 행성 back 방향으로 이동
        cam.isViewNucleus = true;

    }

    // 천체 정보 UI 중에서 활성화 고르기
    void SetActiveUI(string targetName)
    {
        Transform tr = dict_UI["UI_Info"].transform;
        for (int i = 1; i < tr.childCount - 3; i++)
        {
            if (tr.GetChild(i).name == targetName)
                tr.GetChild(i).gameObject.SetActive(true);
            else
                tr.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetActiveModel(bool isActiveAppearance)
    {
        selectPlanet.camaraTarget.parent.GetChild(0).gameObject.SetActive(isActiveAppearance);
        selectPlanet.camaraTarget.parent.GetChild(1).gameObject.SetActive(!isActiveAppearance);
    }


    /// <summary>
    /// 천체 목록의 버튼을 눌렀을 때 실행되는 함수
    /// - 카메라를 이동 시킴
    /// - 카메라의 cameraTarget을 해당 transform 으로 지정
    /// </summary>
    public void OnClick_CBList(Transform target)
    {
        selectPlanet.camaraTarget = target;
        cam.pivot.position = target.position;
        cam.distance = target.localScale.x + 5;
        UI_CBList.SetActive(false);
    }

    // 행성 목록 UI 띄우기
    public GameObject UI_CBList;
    public void OnClick_ViewCB()
    {
        UI_CBList.SetActive(!UI_CBList.activeSelf);
    }


    // 정보보기 메뉴의 x 버튼 클릭
    public void OnClick_Close(GameObject go)
    {
        go.SetActive(false);

        if(go.name == "UI_ViewCBStructure")
        {
            SetActiveModel(true);
        }
    }

    // 정보보기
    public void OnClick_Info()
    {
        dict_UI["UI_Info"].transform.GetChild(1).gameObject.SetActive(true);
    }

    // 내부구조
    public void OnClick_Structure()
    {
        dict_UI["UI_Info"].transform.GetChild(2).gameObject.SetActive(true);
        SetActiveModel(false);
    }

    // 행성 목록 UI 띄우기
    public void OnClick_OpenCBList()
    {

    }

}
