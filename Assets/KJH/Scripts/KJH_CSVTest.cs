using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 행성의 이름에 맞게 
public class KJH_CSVTest : MonoBehaviour
{
    //public List<string> cbNames = new List<string> { "SunModel", "MercuryModel", "VenusModel", "EarthModel", "MarsModel", "JupiterModel", "SaturnModel", "UranusModel", "NeptuneModel" };
    public List<string> cbNames;

    // 천체 정보보기 를 제외한 정보 내용 리스트
    // index 0 = 태양, 1=수성, 2=금성 ...
    public List<List<string>> infos;

    // 행성 정보보기 내용 리스트
    // {{"적도지름,값", "질량,값", ...}, {}}
    public List<List<string>> detailInfos;


    void Awake()
    {
        cbNames = new List<string>();
        infos = new List<List<string>>();
        detailInfos = new List<List<string>>();

        List<Dictionary<string, object>> data = KJH_CSVReader.Read("Data");

        for (int i = 0; i < data.Count; i++)
        {
            string[] _infoList = ((string)data[i]["정보목록"]).Split(":");
            string[] _detailInfoTitles = ((string)data[i]["정보보기상세목록"]).Split(":");
            
            // 읽어온 data에서 value값만 가져오기
            List<object> _values = new List<object>(data[i].Values);
            List<string> _infos = new List<string>();
            List<string> _detailInfos = new List<string>();

            cbNames.Add((string)data[i]["오브젝트이름"]);
            _infos.Add((string)data[i]["천체이름"]);
            _infos.Add((string)data[i]["천체종류"]);

            // 정보보기 내용
            for (int j = 0; j < _infoList.Length; j++)
            {
                string s = _infoList[j] + "," + _values[j + 5];
                _infos.Add(s);
            }

            // 정보보기상세목록 내용정보값(상세보기)
            for (int k = 0; k < _detailInfoTitles.Length; k++)
            {
                string s = _detailInfoTitles[k] + "," + _values[k + 15];
                _detailInfos.Add(s);
            }

            infos.Add(_infos);
            detailInfos.Add(_detailInfos);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}