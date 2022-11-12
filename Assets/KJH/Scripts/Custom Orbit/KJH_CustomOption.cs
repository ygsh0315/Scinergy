using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// inputField�� ���� �Է¹޾� �¾�� ������ ������ �����ϰ� �ʹ�.
// - ����� ������, 
// play ��ư�� ������ ����� �������� �����δ�.
// - ����, ���� ����
// reset ��ư�� ������ �ʱ� ��ġ�� �̵��Ѵ�.
// - �ʱ� ��ġ��, 

public class KJH_CustomOption : MonoBehaviour
{
    public GameObject customUI;
    public GameObject sunLight;

    public Transform celestials;
    public Transform tr_sun;
    public Transform tr_earth;
    Transform pivot;

    Rigidbody rb_sun;
    Rigidbody rb_earth;

    public InputField input_sun;
    public InputField input_earth;

    float sunMassValue = 0f;
    float earthMassValue = 0f;

    Vector3 pivotPos;
    Vector3 originSunPos;
    Vector3 originEarthPos;

    public KJH_Orbit orbit;
    public bool isOrbitMove = false;

    List<TrailRenderer> trails = new List<TrailRenderer>();

    //public Scrollbar scroll;
    //public Text text_distance;

    public KJH_OrbitCamera camara;


    // Start is called before the first frame update
    void Start()
    {

        
        btn_custom.image.sprite = btn_sprites[1];


        // õü���� �ʱ� ��ġ
        originSunPos = tr_sun.position;
        originEarthPos = tr_earth.position;

        // �Է� ����Ǹ� ȣ��Ǵ� �Լ�
        // ���� ����
        input_sun.onEndEdit.AddListener(ChangeSunMass);
        input_earth.onEndEdit.AddListener(ChangeEarthMass);

        for (int i = 0; i < celestials.childCount; i++)
        {
            TrailRenderer trail = celestials.GetChild(i).GetComponent<TrailRenderer>();

            if (trail != null)
            {
                trails.Add(trail);
                trail.time = 0f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        sunLight.transform.LookAt(tr_earth.position);

        //text_distance.text = Vector3.Distance(tr_sun.position, tr_earth.position).ToString();

        if (isOrbitMove)
        {
            orbit.Gravity();
            //pivot.position = pivotPos;
            camara.pivot.position = pivotPos;

            // �� �༺ �߽��� �������� ī�޶� ȸ��
            //camara.pivot.position = (tr_sun.position + tr_earth.position) / 2;
        }
    }

    void ChangeSunMass(string text)
    {
        if (text == "") return;
        sunMassValue = float.Parse(text);

    }

    void ChangeEarthMass(string text)
    {
        if (text == "") return;
        earthMassValue = float.Parse(text);
    }

    public void OnClickPlay()
    {
        // �� õü�� ������ ��� �Է��� �� ����
        if (input_sun.text.Length <= 0 || input_earth.text.Length <= 0) return;

        isOrbitMove = true;

        // rigidbody �߰�
        rb_sun = tr_sun.gameObject.AddComponent<Rigidbody>();
        rb_earth = tr_earth.gameObject.AddComponent<Rigidbody>();

        rb_sun.useGravity = false;
        rb_earth.useGravity = false;


        // ���� ū �༺ �������� ī�޶� ȸ��
        if (sunMassValue > earthMassValue)
        {
            pivot = tr_sun;
            pivotPos = tr_sun.position;
        }
        else
        {
            pivot = tr_earth;
            pivotPos = tr_earth.position;
        }

        // ���� ����
        rb_sun.mass = sunMassValue;
        rb_earth.mass = earthMassValue;

        // �ʱ� ��ġ ����
        originSunPos = tr_sun.position;
        originEarthPos = tr_earth.position;

        // �˵� �׸��� Ȱ��ȭ
        ChangeTrailTime(100f);

        orbit.InitialVelocity();
    }

    public void OnClickStop()
    {
        isOrbitMove = false;
    }

    // �ʱ� ��ġ�� �̵�
    public void OnClickReset()
    {
        isOrbitMove = false;
        
        // rigidbody ������Ʈ ����
        Destroy(rb_sun);
        Destroy(rb_earth);

        // ��ġ �ʱ�ȭ
        tr_sun.position = originSunPos;
        tr_earth.position = originEarthPos;

        // inputfield �� �ʱ�ȭ
        input_sun.text = "";
        input_earth.text = "";


        // �˵� �׸��� ��Ȱ��ȭ
        ChangeTrailTime(0f);
    }

    void ChangeTrailTime(float time)
    {
        for (int i = 0; i < trails.Count; i++)
        {
            trails[i].time = time;
        }
    }

    public Button btn_custom;
    public List<Sprite> btn_sprites;

    public void OnClick_Custom()
    {
        customUI.SetActive(!customUI.activeSelf);

        if (customUI.activeSelf)
        {
            btn_custom.image.sprite = btn_sprites[1];
        }
        else
        {
            btn_custom.image.sprite = btn_sprites[0];
        }

    }

    public void OnClickClose()
    {
        customUI.SetActive(false);
        btn_custom.image.sprite = btn_sprites[0];
    }

}