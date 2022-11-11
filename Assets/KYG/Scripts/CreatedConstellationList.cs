using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatedConstellationList : MonoBehaviour
{
    public Transform CreatedConstellationListContent;
    public GameObject CreatedConstellationItemFactory;
    public GameObject SelectedConstellation;
    public TextMeshProUGUI SelectedConstellationName;
    public GameObject SelectedConstellationItem;
    public GameObject Player;
    public GameObject CreatedConstellationListUI;
    public GameObject CreatedStarList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init(string constellationName, GameObject constellationInfo)
    {
        GameObject createdConstellationItem = Instantiate(CreatedConstellationItemFactory, CreatedConstellationListContent);
        CreatedConstellationItem item = createdConstellationItem.GetComponent<CreatedConstellationItem>();
        item.constellation = constellationInfo;
        item.constellationName.text = constellationName;
    }
    public void OnConstellationLookBtn()
    {
        Vector3 ConstellationPosition = Vector3.zero;
        float averageX = 0;
        float averageY = 0;
        float averageZ = 0;
        int childStarCount = 0;
        for (int i = 0; i < SelectedConstellation.transform.childCount; i++)
        {
            Star childStar = SelectedConstellation.transform.GetChild(i).GetComponent<Star>();
            if (childStar)
            {
                childStarCount++;
                averageX += childStar.transform.position.x;
                averageY += childStar.transform.position.y;
                averageZ += childStar.transform.position.z;
            }
            ConstellationPosition = new Vector3(averageX / childStarCount, averageY / childStarCount, averageZ / childStarCount);
            print(childStarCount);
        }
        Player.GetComponent<PlayerRot>().StarSet(ConstellationPosition);
    }

    public void OnConstellationDeleteBtn()
    {
        GameManager.instance.createdConstellationList.Remove(SelectedConstellation.name);
        for(int i = 0; i<SelectedConstellation.transform.childCount; i++)
        {
            Star childStar = SelectedConstellation.transform.GetChild(i).GetComponent<Star>();
            if (childStar)
            {
                GameManager.instance.createdStarList.Remove(childStar.starName);
                childStar.StarState = Star.State.shootingStar;
                for(int j = 0; j<CreatedStarList.transform.childCount; j++)
                {
                    if (CreatedStarList.transform.GetChild(j).GetComponent<CreatedStarItem>().star == childStar.gameObject)
                    {
                        Destroy(CreatedStarList.transform.GetChild(j).gameObject);
                    }
                }
            }
            Destroy(SelectedConstellationItem);
            SelectedConstellationItem = null;
        }
    }

    public void OnDeleteAllConstellation()
    {
        foreach (KeyValuePair<string, GameObject> constellation in GameManager.instance.createdConstellationList)
        {
            for (int i = 0; i < constellation.Value.transform.childCount; i++)
            {
                Star childStar = constellation.Value.transform.GetChild(i).GetComponent<Star>();
                if (childStar)
                {
                    GameManager.instance.createdStarList.Remove(childStar.starName);
                    childStar.StarState = Star.State.shootingStar;
                    for (int j = 0; j < CreatedStarList.transform.childCount; j++)
                    {
                        if (CreatedStarList.transform.GetChild(j).GetComponent<CreatedStarItem>().star == childStar.gameObject)
                        {
                            Destroy(CreatedStarList.transform.GetChild(j).gameObject);
                        }
                    }
                }
            }
        }
        Transform[] createdConstellationItemList = CreatedConstellationListContent.GetComponentsInChildren<Transform>();
        if (createdConstellationItemList != null)
        {
            for (int i = 1; i < createdConstellationItemList.Length; i++)
            {
                if (createdConstellationItemList[i] != transform) Destroy(createdConstellationItemList[i].gameObject);
            }
        }
        GameManager.instance.createdConstellationList.Clear();
    }

    public void OnCloseBtn()
    {
        CreatedConstellationListUI.SetActive(false);
    }
}
