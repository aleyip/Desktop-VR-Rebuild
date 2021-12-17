using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseAppUI : MonoBehaviour
{
    public Canvas MainCanvas;
    Image top, bottom, left, right;
    Renderer topRenderer, bottomRenderer, leftRenderer, rightRenderer;

    public Text NameText;

    public void InitPrefabLinks()
    {
        if (MainCanvas == null)
            MainCanvas = gameObject.GetComponent<Canvas>();

        //if (topRenderer == null)
        //    topRenderer = gameObject.transform.Find("Top").gameObject.GetComponent<RawImage>().GetComponent<Renderer>();

        //if (bottomRenderer == null)
        //    bottomRenderer = gameObject.transform.Find("Bottom").gameObject.GetComponent<RawImage>().GetComponent<Renderer>();

        //if (leftRenderer == null)
        //    leftRenderer = gameObject.transform.Find("Left").gameObject.GetComponent<RawImage>().GetComponent<Renderer>();

        //if (rightRenderer == null)
        //    rightRenderer = gameObject.transform.Find("Right").gameObject.GetComponent<RawImage>().GetComponent<Renderer>();

        if (top == null)
            top = gameObject.transform.Find("Top").gameObject.GetComponent<Image>();

        if (bottom == null)
            bottom = gameObject.transform.Find("Bottom").gameObject.GetComponent<Image>();

        if (left == null)
            left = gameObject.transform.Find("Left").gameObject.GetComponent<Image>();

        if (right == null)
            right = gameObject.transform.Find("Right").gameObject.GetComponent<Image>();

        if (NameText == null)
            NameText = gameObject.transform.Find("Text").gameObject.GetComponent<Text>();

        MainCanvas.worldCamera = Camera.main;
    }

    public void setColorUI(Color32 color)
    {
        top.color = color;
        bottom.color = color;
        left.color = color;
        right.color = color;
    }

    public void setName(string name)
    {
        NameText.text = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
