using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KJH_CameraTest2 : MonoBehaviour
{
    // 카메라 대상
    Transform target;
    // 회전 기준 -> cameraTarget의 position
    public Transform pivot;
    // 메인 카메라
    public Transform cam;

    // 카메라 회전
    Vector2 preMouse;
    public float rotSpeed;
    float mx;
    float my;
    float rotY;
    float rotX;

    // 카메라 줌인/줌아웃
    public float distance = 20f;
    public float minDistance = 3f;
    public float maxDistance = 100f;
    public float zoomSpeed = 1;

    // 카메라 이동
    public KJH_UIManager uiManager;
    public KJH_SelectPlanet selectPlanet;

    // 카메라 움직임 제어
    bool isStop = false;
    bool isRot = false;
    public bool isViewUI = false;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        selectPlanet = cam.GetComponent<KJH_SelectPlanet>();
    }


    // Update is called once per frame
    void Update()
    {
        // B키 누르면 카메라 회전 및 줌인 비활성화
        if (Input.GetKeyDown(KeyCode.B))
            isStop = !isStop;

        if (isStop) return;

        // 카메라 포커스 대상
        if (selectPlanet.camaraTarget != null)
        {
            target = selectPlanet.camaraTarget;
            minDistance = target.localScale.x + 3f;
            //pivot.position = target.position;
        }

        // 마우스 버튼 입력에 따른 카메라 회전 제어 변수
        if (Input.GetMouseButtonDown(0))
            isRot = true;

        if (Input.GetMouseButtonUp(0))
            isRot = false;

        // ui를 클릭하지 않을 때 카메라 회전 실행
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            CameraZoom();

            // 마우스 왼쪽 버튼을 누르고 있을 때
            if (isRot)
            {
                // 이전 마우스 입력값
                preMouse.x = mx;
                preMouse.y = my;

                // 현재 마우스 입력값
                mx = Input.GetAxis("Mouse X");
                my = Input.GetAxis("Mouse Y");

                if(isViewNucleus == false)
                {
                    Rotate(mx, my);
                }
            }
            // 마우스 왼쪽 버튼을 누르고 있지 않을 때
            else
            {
                Rotate(preMouse.x, preMouse.y);
                preMouse = Vector2.Lerp(preMouse, Vector2.zero, Time.deltaTime);
            }
        }

        if(isMovingToCB == false)
        {
            pivot.position = target.position;
        }

        if (target && isMovingToCB)
        {
            MoveToCB(target);
        }

        if (isViewNucleus)
        {
            ViewNucleus();
        }

    }


    // 마우스 왼쪽 버튼 입력에 따른 카메라 회전
    void Rotate(float x, float y)
    {
        rotX += x * rotSpeed * Time.deltaTime;
        rotY += y * rotSpeed * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, -90, 90);

        pivot.localEulerAngles = new Vector3(-rotY, rotX, 0);
    }

    void CameraZoom()
    {
        Vector3 dir = pivot.position - cam.position;
        dir.Normalize();
        float step = GetAxisRawScrollUniversal() * zoomSpeed;
        distance = Mathf.Clamp(distance - step, minDistance, maxDistance);

        Vector3 zoomDir;
        if (isViewUI)
        {
            zoomDir = dir;
        }
        else
        {
            zoomDir = cam.rotation * Vector3.forward;
        }

        Vector3 goalPos = pivot.position - (zoomDir * distance);

        cam.position = Vector3.Lerp(cam.position, goalPos, Time.deltaTime * 3);

        if (Vector3.Distance(cam.position, goalPos) < 0.1f)
        {
            cam.position = goalPos;
        }
    }

    float GetAxisRawScrollUniversal()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll < 0) return -1;
        if (scroll > 0) return 1;
        return 0;
    }

    // 카메라 이동
    // 1. 천체로 이동
    public bool isMovingToCB = false;
    public void MoveToCB(Transform moveTarget)
    {
        distance = minDistance;

        if (Vector3.Distance(pivot.position, target.position) < target.localScale.x)
        {
            // 정보 ui 띄우기
            pivot.position = target.position;
            KJH_UIManager.instance.OpenInfoMenu();
            isMovingToCB = false;
        }
        else
        {
            // 카메라 위치를 행성으로 이동
            pivot.position = Vector3.Lerp(pivot.position, moveTarget.position, Time.deltaTime * 3f);
        }
    }

    public void ChangeCenter(float x)
    {
        Vector3 viewCenter = new Vector3(x, Screen.height * 0.5f, -cam.localPosition.z);
        Vector3 pos = Camera.main.ScreenToWorldPoint(viewCenter);
        Vector3 gap = pivot.position - pos;
        cam.position += gap;
    }

    public bool isViewNucleus = false;
    // 내부구조 카메라
    void ViewNucleus()
    {
        Vector3 nucleusDir = target.forward;
        pivot.forward = Vector3.Slerp(pivot.forward, nucleusDir, Time.deltaTime * 5f);

        if (Vector3.Angle(pivot.forward, nucleusDir) < 0.1f)
        {
            pivot.forward = nucleusDir;
            //isViewNucleus = false;
        }
    }
}