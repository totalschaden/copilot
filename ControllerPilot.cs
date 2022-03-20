using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using ExileCore;
using ExileCore.Shared;
using SharpDX;
using SharpDX.XInput;

namespace CoPilot
{
    public class ControllerPilot
    {
        private Coroutine updateCoroutine;
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        public int deadband = 2500;
        public Vector2 leftThumb, rightThumb = new Vector2(0,0);
        public float leftTrigger, rightTrigger;
        public Vector2 windowCenter;
        private bool mouseIsDown;

        public void Start()
        {
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;
            windowCenter = Helper.WorldToValidScreenPosition(CoPilot.instance.GameController.IngameState.Data.LocalPlayer
                    .BoundsCenterPos);
            updateCoroutine = new Coroutine(UpdateController(), CoPilot.instance, "ControllerUpdate");
            Core.ParallelRunner.Run(updateCoroutine);
        }

        public void Render()
        {
            try
            {
                if (CoPilot.instance.Settings.controllerPilotDebug)
                {
                    CoPilot.instance.Graphics.DrawText("Input: " + leftThumb.X + " " + leftThumb.Y,
                        new Vector2(350, 150));
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        private IEnumerator UpdateController()
        {
            while (true)
            {
                // Currently you need to Connect Controller before you start Hud.
                if (!connected)
                {
                    yield return null;
                    continue;
                }
                
                gamepad = controller.GetState().Gamepad;

                leftThumb.X  = (Math.Abs((float)gamepad.LeftThumbX ) < deadband) ?  0 : (float)gamepad.LeftThumbX  / short.MinValue * -100;
                leftThumb.Y  = (Math.Abs((float)gamepad.LeftThumbY ) < deadband) ?  0 : (float)gamepad.LeftThumbY  / short.MaxValue * -100;
                rightThumb.X = (Math.Abs((float)gamepad.RightThumbX) < deadband) ?  0 : (float)gamepad.RightThumbX / short.MaxValue * 100;
                rightThumb.Y = (Math.Abs((float)gamepad.RightThumbY) < deadband) ?  0 : (float)gamepad.RightThumbY / short.MaxValue * 100;
                
                // Left thumb Controller Stick -> Charakter Walks in direction of stick.
                if (leftThumb.X != 0 || leftThumb.Y != 0)
                {
                    var mousePos = new Vector2(leftThumb.X > 0
                            ? leftThumb.X * CoPilot.instance.Settings.controllerPilotMouseRange + CoPilot.instance.Settings.controllerPilotDeadZone +  windowCenter.X
                            : (int) leftThumb.X * CoPilot.instance.Settings.controllerPilotMouseRange - CoPilot.instance.Settings.controllerPilotDeadZone + windowCenter.X,
                        leftThumb.Y > 0
                            ? leftThumb.Y * CoPilot.instance.Settings.controllerPilotMouseRange + CoPilot.instance.Settings.controllerPilotDeadZone + windowCenter.Y
                            : (int) leftThumb.Y * CoPilot.instance.Settings.controllerPilotMouseRange - CoPilot.instance.Settings.controllerPilotDeadZone + windowCenter.Y);
                    yield return Mouse.SetCursorPosHuman(mousePos, false);

                    if (!Mouse.IsMouseLeftPressed())
                    {
                        Mouse.LeftMouseDown();
                        mouseIsDown = true;
                    }
                }
                else if (Mouse.IsMouseLeftPressed() && mouseIsDown)
                {
                    Mouse.LeftMouseUp();
                    mouseIsDown = false;
                }
                    
                // TODO
                // Add Right Thumb to move mouse around on screen (no walking)
                // Add Leftclick with a button
                // Add PickIt Support Button
                // Add EZVendor Support Button
                
                //leftTrigger  = gamepad.LeftTrigger;
                //rightTrigger =  gamepad.RightTrigger;
                yield return null;
            }
        }
    }
}