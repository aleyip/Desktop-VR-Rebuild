using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class LibreOfficeCalcApp : BaseApplication
{
    // Update is called once per frame
    new void Update()
    {
        if (pointer.activeObjectID == windowObject.GetInstanceID())
        {
            if (windowsRender.windowInfo.hwnd != IntPtr.Zero)
            {
                Int32 upLeftPos;
                {
                    Int32 xBorder = 8;
                    Int32 yBorder = 30;
                    upLeftPos = (Int32)((yBorder << 16) | xBorder);
                }
                if (mousePosChanged)
                {
                    Win32Funcs.PostMessage(windowsRender.windowInfo.hwnd, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos - upLeftPos);
                    mousePosChanged = false;
                }

                if (pointer.mouseLeftDown)
                {
                    Win32Funcs.PostMessage(windowsRender.windowInfo.hwnd, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos - upLeftPos);
                }
                else if (pointer.mouseLeftUp)
                {
                    Win32Funcs.PostMessage(windowsRender.windowInfo.hwnd, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos - upLeftPos);
                }
                else if (pointer.inputString != null)
                {
                    foreach (char c in pointer.inputString)
                    {
                        int foo;
                        if (Win32Types.VirtualKeyCode.ContainsKey(c)) foo = Win32Types.VirtualKeyCode[c];
                        else foo = Convert.ToInt32(c);
                        foo = Win32Funcs.PostMessage(windowsRender.windowInfo.hwnd, Win32Types.command.WM_KEYDOWN, foo, 0x0001);
                    }
                }
            }
            else
                Debug.Log("FALSE");

        }
        base.Update();
    }
}
