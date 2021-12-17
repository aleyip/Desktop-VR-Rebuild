using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class NotePadApp : BaseApplication
{
    // Update is called once per frame
    new void Update()
    {
        if (pointer.activeObjectID == windowObject.GetInstanceID())
        {
            var windChild = Win32Funcs.FindWindowEx(windowsRender.windowInfo.hwnd, IntPtr.Zero, "edit", null);
            if (windChild != IntPtr.Zero)
            {
                Int32 upLeftPos;
                {
                    //Pega a posicao do canto superior esquerdo do canvas para posicionar corretamente o cursor
                    Win32Types.RECT upLeftWindow, upLeftCanvas;
                    Win32Funcs.GetWindowRect(windowsRender.windowInfo.hwnd, out upLeftWindow);
                    Win32Funcs.GetWindowRect(windChild, out upLeftCanvas);
                    Int32 xBorder = upLeftCanvas.Left - upLeftWindow.Left;
                    Int32 yBorder = upLeftCanvas.Top - upLeftWindow.Top;
                    upLeftPos = (Int32)((yBorder << 16) | xBorder);
                }

                if (mousePosChanged)
                {
                    int foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos - upLeftPos);
                    mousePosChanged = false;
                }


                if (pointer.mouseLeftDown)
                {
                    Debug.Log("Pressed primary button");
                    int foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos - upLeftPos);
                }
                else if (pointer.mouseLeftUp)
                {
                    Debug.Log("Unpressed primary button");
                    int foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos - upLeftPos);
                }
                else if (pointer.inputString != null)
                {
                    Debug.Log("Trying click " + pointer.inputString);
                    foreach (char c in pointer.inputString)
                    {
                        int foo;
                        if (Win32Types.VirtualKeyCode.ContainsKey(c)) foo = Win32Types.VirtualKeyCode[c];
                        else foo = Convert.ToInt32(c);
                        foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_KEYDOWN, foo, 0x0001);
                        Debug.Log($"{foo}  { Convert.ToInt32(c)}");
                    }
                }
            }
            else
                Debug.Log("FALSE");
        }


        base.Update();
    }
}
