using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SimpleWebBrowser
{
    public class BrowserUI : MonoBehaviour
    {
        [SerializeField]
        public Canvas MainCanvas = null;
        [SerializeField]
        public GameObject UrlField;
        [SerializeField]
        public Image Background;
        [SerializeField]
        public GameObject Back;
        [SerializeField]
        public GameObject Forward;
        [SerializeField]
        public GameObject Enter;

        public Text UrlText;

        public string editString = "";
        public string fillString = "";

        Renderer backRend, forwardRend, enterRend, urlfieldRend;
        
        Pointer pointer;

        [HideInInspector] public bool KeepUIVisible = false;
        
        public void InitPrefabLinks()
        {
            //3D
            if (MainCanvas == null)
                MainCanvas = gameObject.GetComponent<Canvas>();

            if (UrlField == null)
            {
                UrlField = gameObject.transform.Find("UrlField").gameObject;
                urlfieldRend = UrlField.GetComponent<Renderer>();
                UrlText = UrlField.transform.Find("Text").GetComponent<Text>();
            }
            if (Background == null)
            {
                //2d
                //Background = gameObject.GetComponent<Image>();
                //3d
                if (Background == null)
                    Background = gameObject.transform.Find("Background").gameObject.GetComponent<Image>();
            }
            if (Back == null)
            { 
                Back = gameObject.transform.Find("Back").gameObject;
                backRend = Back.GetComponent<Renderer>();
            }
            if (Forward == null)
            {
                Forward = gameObject.transform.Find("Forward").gameObject;
                forwardRend = Forward.GetComponent<Renderer>();
            }
            if (Enter == null)
            {
                Enter = gameObject.transform.Find("Enter").gameObject;
                enterRend = Enter.GetComponent<Renderer>();
            }
            pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
        }

        public void Show()
        {
            UrlField.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            //UrlField.placeholder.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            //UrlField.textComponent.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            Back.gameObject.SetActive(true);
            Forward.gameObject.SetActive(true);
            Background.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }

        //public void Hide()
        //{
        //    if (!KeepUIVisible)
        //    {
        //        if (!UrlField.isFocused)
        //        {
        //            UrlField.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        //            UrlField.placeholder.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        //            UrlField.textComponent.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        //            Back.gameObject.SetActive(false);
        //            Forward.gameObject.SetActive(false);
        //            Background.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        //        }
        //        else
        //        {
        //            Show();
        //        }
        //    }
        //}

        void Update()
        {
            //if (UrlField.isFocused && !KeepUIVisible)
            //{
            //    Show();
            //}

            if (pointer.hit.collider != null)
            {
                if (pointer.activeObjectID == Back.GetInstanceID() && pointer.mouseLeftHold) backRend.material.color = ColorSettings.buttonClickColor;
                else if (pointer.hit.collider.gameObject == Back) backRend.material.color = ColorSettings.buttonHoverColor;
                else backRend.material.color = ColorSettings.buttonDefaultColor;

                if (pointer.activeObjectID == Forward.GetInstanceID() && pointer.mouseLeftHold) forwardRend.material.color = ColorSettings.buttonClickColor;
                else if (pointer.hit.collider.gameObject == Forward) forwardRend.material.color = ColorSettings.buttonHoverColor;
                else forwardRend.material.color = ColorSettings.buttonDefaultColor;

                if (pointer.activeObjectID == Enter.GetInstanceID() && pointer.mouseLeftHold) enterRend.material.color = ColorSettings.buttonClickColor;
                else if (pointer.hit.collider.gameObject == Enter) enterRend.material.color = ColorSettings.buttonHoverColor;
                else enterRend.material.color = ColorSettings.buttonDefaultColor;

                if (pointer.activeObjectID == UrlField.GetInstanceID()) urlfieldRend.material.color = ColorSettings.buttonClickColor;
                else if (pointer.hit.collider.gameObject == UrlField) urlfieldRend.material.color = ColorSettings.buttonHoverColor;
                else urlfieldRend.material.color = ColorSettings.buttonDefaultColor;

                if (pointer.activeObjectID == UrlField.GetInstanceID())
                {
                    UrlText.text = editString;
                    if (pointer.inputString != null)
                    {
                        foreach (char c in pointer.inputString)
                        {
                            if (c == '\b' && editString.Length > 0)
                                editString = editString.Remove(editString.Length - 1);
                            else editString += c;
                        }
                    }
                }
                else UrlText.text = fillString;
            }
            else
            {
                backRend.material.color = ColorSettings.buttonDefaultColor;
                enterRend.material.color = ColorSettings.buttonDefaultColor;
                forwardRend.material.color = ColorSettings.buttonDefaultColor;
                if (pointer.activeObjectID != UrlField.GetInstanceID()) urlfieldRend.material.color = ColorSettings.buttonDefaultColor;
            }

        }

    }
}
