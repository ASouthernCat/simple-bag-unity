using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BagPanel : MonoBehaviour
{
    private readonly int maxCnt = 25;
    private Button addButton;
    private Button removeButton;
    private Button orderByIdButton;
    private Button orderByCntButton;
    private Image draw;
    private Transform itemContanier;
    private GameObject itemObj;
    private GameObject alertObj;

    public enum Item
    {
        blue,
        red,
        weapon
    }

    Hashtable htName = new Hashtable();
    Hashtable htColor = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        addButton = transform.Find("Add").GetComponent<Button>();
        addButton.onClick.AddListener(OnAddClick);
        removeButton = transform.Find("Remove").GetComponent<Button>();
        removeButton.onClick.AddListener(OnRemoveClick);
        orderByIdButton = transform.Find("OrderById").GetComponent<Button>();
        orderByIdButton.onClick.AddListener(OnOrderByIdClick);
        orderByCntButton = transform.Find("OrderByCnt").GetComponent<Button>();
        orderByCntButton.onClick.AddListener(OnOrderByCntClick);
        itemContanier = transform.Find("ItemContanier");
        itemObj = transform.Find("Item").gameObject;
        draw = transform.Find("Draw").GetComponent<Image>();

        alertObj =  Instantiate(transform.Find("Alert").gameObject);
        alertObj.transform.SetParent(this.transform);
        alertObj.transform.position = this.transform.position;

        htName.Add(Item.blue,"蓝药");
        htName.Add(Item.red, "红药");
        htName.Add(Item.weapon, "武器");

        htColor.Add(Item.blue, Color.blue);
        htColor.Add(Item.red, Color.red);
        htColor.Add(Item.weapon, Color.white);
    }

    private void OnOrderByCntClick()
    {
        Debug.Log("OnOrderByCntClick");
        var transformsList = GetItemContanierList();
        transformsList.Sort((Transform a, Transform b) =>
        {
            return int.Parse(a.Find("count").GetComponent<Text>().text).CompareTo(int.Parse(b.Find("count").GetComponent<Text>().text));
        });

        foreach (Transform t in transformsList)
        {
            t.SetAsFirstSibling();
        }
    }

    private void OnOrderByIdClick()
    {
        Debug.Log("OnOrderByIdClick");

        foreach(Transform t in GetItemContanierList())
        {
            t.SetAsFirstSibling();
        }
    }

    private List<Transform> GetItemContanierList()
    {
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < itemContanier.childCount; i++)
        {
            transforms.Add(itemContanier.GetChild(i));
        }

        //foreach (var t in transforms)
        //{
        //    Debug.Log(t.name);
        //}

        transforms.Sort((Transform a, Transform b) =>
        {
            return a.name.CompareTo(b.name);
        });

        return transforms;
    }

    private void OnRemoveClick()
    {
        Debug.Log("OnRemoveClick");
        for(int i=0; i<itemContanier.childCount; i++)
        {
            var childItem = itemContanier.GetChild(i);
            var toggle = childItem.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                int count = int.Parse(childItem.Find("count").GetComponent<Text>().text);
                if(count > 1)
                {
                    childItem.Find("count").GetComponent<Text>().text = (count - 1).ToString();
                }
                else
                {
                    Destroy(childItem.gameObject);
                }

            }
        }
    }

    private void OnAddClick()
    {
        Debug.Log("OnAddClick");

        if (itemContanier.childCount < maxCnt)
        {
            //随机抽取
            Random rd = new Random(System.DateTime.Now.GetHashCode());
            var id = (Item)rd.Next(0, 3);
            Debug.Log("随机收取到道具：" + id);

            draw.color = (Color)htColor[id];

            Transform item =  itemContanier.Find(id.ToString());
            //道具已存在
            if (item) {
                Debug.Log("道具已存在");                
                String count = item.Find("count").GetComponent<Text>().text;
                item.Find("count").GetComponent<Text>().text = (int.Parse(count) + 1).ToString();
            }
            else
            {
                //不存在，则添加道具

                GameObject o = Instantiate(itemObj);
                o.transform.SetParent(itemContanier);
                o.SetActive(true);

                //获取组件
                Transform trans = o.transform;
                Text nameText = trans.Find("name").GetComponent<Text>();
                Text countText = trans.Find("count").GetComponent<Text>();

                //填充信息
                o.GetComponent<Image>().color = (Color)htColor[id];
                nameText.text = htName[id].ToString();
                countText.text = "01";
                o.name = id.ToString();

                //选中状态
                var toggle = o.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(isOn =>
                {
                    ColorBlock cb = toggle.colors;
                    if (isOn)
                    {
                        cb.normalColor = Color.green;
                        cb.selectedColor = Color.green;
                    }
                    else
                    {
                        cb.normalColor = new Color(0, 0, 0, 0);
                        cb.selectedColor = new Color(0, 0, 0, 0);
                    }
                    toggle.colors = cb;
                });

            }
        }
        else
        {
            Debug.Log("背包已满！！！");
            alertObj.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
