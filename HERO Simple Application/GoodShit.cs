﻿using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HERO_Simple_Application
{
    public class GoodShit
    {
        public static void Main()
        {
            /* create a gamepad object */
            CTRE.Phoenix.Controller.GameController myGamepad = new 
            CTRE.Phoenix.Controller.GameController(new CTRE.Phoenix.UsbHostDevice(0));

            /* create a talon */
            CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new
            CTRE.Phoenix.MotorControl.CAN.TalonSRX(0);

            /* simple counter to print and watch using the debugger */
            int counter = 0;

            /* loop forever */
            while (true)
            {
                /* checks for gamepad. if gamepad exists, prints axis value and calls Feed(); */
                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected) 
                {
                    /* print the axis value */
                    Debug.Print("axis:" + myGamepad.GetAxis(1));

                    /* pass axis value to talon */
                    myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));

                    /* this is checked periodically.  Recommend every 20sm or faster*/
                    if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                    {
                        /* allow motor control */
                        CTRE.Phoenix.Watchdog.Feed(); 
                    }
                }

                /* retrieve HERO values */
                CTRE.Phoenix.Controller.GameControllerValues gv = new
                CTRE.Phoenix.Controller.GameControllerValues();
                myGamepad.GetAllValues(ref gv);

                /* pass axis value to talon */
                myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));

                /* increment counter */
                ++counter; /* try to land a breakpoint here and hover over 'counter' to see it's current value.  Or add it to the Watch Tab */



                /* wait a bit */
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
