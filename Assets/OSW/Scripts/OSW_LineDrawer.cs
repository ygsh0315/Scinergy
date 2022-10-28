using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSW_LineDrawer : MonoBehaviour
{
    List<Vector3> linePoints;
    float timer;
    public float timeDelay;
    GameObject newLine;
    LineRenderer drawLine;
    public float linewidth;

    void Start()
    {
        linePoints = new List<Vector3>();
        timer = timeDelay;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 누르는 순간
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if ((Physics.Raycast(ray, out hitInfo) && hitInfo.collider.name == "Board") || hitInfo.collider.name == "uWC Window Object(Clone)")
            {
                //Debug.Log(hitInfo.collider.name);

                // 라인을 생성한다.
                newLine = new GameObject("Line");

                // 그려지는 라인에 LineRenderer, Material, Color, Width를 설정해준다.
                drawLine = newLine.AddComponent<LineRenderer>();
                drawLine.material = new Material(Shader.Find("Sprites/Default"));
                drawLine.startColor = RandomColor();
                drawLine.endColor = RandomColor();
                drawLine.startWidth = linewidth;
                drawLine.endWidth = linewidth;
            }
                
        }

        // 마우스 왼쪽 버튼을 누른 상태
        if (Input.GetMouseButton(0))
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if ((Physics.Raycast(ray, out hitInfo) && hitInfo.collider.name == "Board") || hitInfo.collider.name == "uWC Window Object(Clone)")
                {
                    //Debug.Log(hitInfo.collider.name);
                    linePoints.Add(GetMousePosition());
                    drawLine.positionCount = linePoints.Count;
                    drawLine.SetPositions(linePoints.ToArray());

                    // 화면 공유된 오브젝트에 글씨를 쓰고 오브젝트를 움직이면 글씨가 그 오브젝트 자식으로 들어가서 같이 움직이게
                    drawLine.transform.parent = hitInfo.transform;
                    timer = timeDelay;
                }
            }
        }

        // 마무스 왼쪽 버튼을 뗀 상태
        if (Input.GetMouseButtonUp(0))
        {
            // List 요소 모두 제거
            linePoints.Clear();
        }
    }

    Vector3 GetMousePosition()
    {
        // 스크린의 마우스 위치로부터 Ray를 만들어냄
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if ((Physics.Raycast(ray, out hitInfo) && hitInfo.collider.name == "Board") || hitInfo.collider.name == "uWC Window Object(Clone)")
        {
            //Debug.Log(hitInfo.collider.name);
        }

        return hitInfo.point;
    }

    Color RandomColor()
    {
        return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}
