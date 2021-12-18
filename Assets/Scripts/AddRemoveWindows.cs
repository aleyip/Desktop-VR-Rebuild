using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;
using SimpleWebBrowser;

public class AddRemoveWindows : MonoBehaviour
{
    WindowCaptureManager captureManager;
    public Dictionary<IntPtr, BaseApplication> listObjects;

    float lastUpdateTime;
    float lastPollWindowsTime;

    public float captureRateFps = 30;
    public float windowPollPerSecond = 2;

    public bool showNotepad, showPaint, showBrowser, showSheets, showOthers;

    public Vector3 createPos;
    public Vector3 createRot;

    private Pointer pointer;

    // Start is called before the first frame update
    void Start()
    {
        listObjects = new Dictionary<IntPtr, BaseApplication>();

        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();

        captureManager = new WindowCaptureManager();
        captureManager.OnAddWindow += OnAddWindow;
        captureManager.OnRemoveWindow += OnRemoveWindow;
        lastUpdateTime = Time.time;
        lastPollWindowsTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Poll for new windows
        if (Time.time - lastPollWindowsTime < 1.0f / windowPollPerSecond)
        {
            return;
        }
        else
        {
            lastPollWindowsTime = Time.time;
            // calls OnAddWindow or OnRemoveWindow above if any windows have been added or removed
            captureManager.Poll();
        }
    }

    bool IsGoodWindow(WindowCapture window)
    {
        Debug.Log("Saw window: " + window.windowInfo.title);
        // You can stick whatever logic or names you want here for windows you want to keep to render

        string windowLowerTitle = window.windowInfo.title.ToLower();
        if (windowLowerTitle.Contains("sem"))
        {
            return true;
        }
        return false;
    }

    #region Add Remove Events
    void OnAddWindow(WindowCapture window)
    {
        Debug.Log("Trying");
        Debug.Log($"{window.windowInfo.title} {window.windowInfo.hwnd.ToInt64():X}");
        if (!listObjects.ContainsKey(window.hwnd))
        {
            if (window.windowInfo.className.Equals("Notepad") && showNotepad)
            {
                Debug.Log("Creating notepad");
                NotePadApp newObj = gameObject.AddComponent<NotePadApp>();
                newObj.passWindow(window);
                newObj.Rotate(createRot);
                newObj.Move(createPos);
                listObjects[window.hwnd] = newObj;
            }
            else if(window.windowInfo.className.Equals("MSPaintApp") && showPaint)
            {
                Debug.Log("Creating paint");
                PaintApp newObj = gameObject.AddComponent<PaintApp>();
                newObj.passWindow(window);
                newObj.Rotate(createRot);
                newObj.Move(createPos);
                listObjects[window.hwnd] = newObj;
            }
            else if (window.windowInfo.className.Equals("SALFRAME") && showSheets)
            {
                Debug.Log("Creating sheets");
                BaseApplication newObj = gameObject.AddComponent<LibreOfficeCalcApp>();
                newObj.passWindow(window);
                newObj.Rotate(createRot);
                newObj.Move(createPos);
                listObjects[window.hwnd] = newObj;
            }
            Debug.Log("Add");
        }
    }

    void OnRemoveWindow(WindowCapture window)
    {
        Debug.Log("removed " + window.windowInfo.title);
        if (listObjects.ContainsKey(window.hwnd))
        {
            BaseApplication windowObjectRemoving = listObjects[window.hwnd];
            Destroy(windowObjectRemoving);
            listObjects.Remove(window.hwnd);
        }
    }
    #endregion
}
