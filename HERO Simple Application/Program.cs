using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.Controller;

namespace ExampleRobotCode
{
    public enum IntakeState
    {
        IN,
        IN_SLOW,

        IDLE,
        REVERSE,
        INIT,
        UNKNOWN
    }

    public class Program
    {

        private static TalonSRX talon = new TalonSRX(0);
        private static TalonSRX talon2 = new TalonSRX(1);


        private static IntakeState currentIntakeState = IntakeState.INIT;
        private static IntakeState lastIntakeState = IntakeState.UNKNOWN;

        private static GameController gamepad = new GameController(new CTRE.Phoenix.UsbHostDevice(0));


        public static void Main()
        {


            talon.ConfigFactoryDefault();
            talon2.ConfigFactoryDefault();

            /* simple counter to print and watch using the debugger */
            int counter = 0;
            bool on = false; // boolean to control on or off
            /* loop forever */
            while (true)
            {

                if (counter % 50 == 0) // runs code every 50 iterations
                {
                    if (on) 
                    {
                        on = false; // set to off
                        talon.Set(ControlMode.PercentOutput, 0.0); // set talon to 0%
                    } else {
                        on = true; // set to on
                        talon.Set(ControlMode.PercentOutput, 1);  // set talon 100%
                    }
                }

                if (gamepad.GetButton(0)) // is button with id 0 pressed
                {
                    currentIntakeState = IntakeState.IDLE;
                }
                if (gamepad.GetButton(1))
                {
                    currentIntakeState = IntakeState.IN;
                }
                if (gamepad.GetButton(2))
                {
                    currentIntakeState = IntakeState.IN_SLOW;
                }
                if (gamepad.GetButton(3))
                {
                    currentIntakeState = IntakeState.REVERSE;
                }

                if (gamepad.GetButton(4))
                {
                    talon2.Set(ControlMode.PercentOutput, 1.0); // set talon to 100%
                } else
                {
                    talon2.Set(ControlMode.PercentOutput, 0.0); // set talon to 0%
                }



                if (currentIntakeState != lastIntakeState) // make code not run every update to have faster run time
                {
                    lastIntakeState = currentIntakeState; // update the last state for intake
                    switch (currentIntakeState) // switch on current state for intake
                    {
                        case IntakeState.INIT: // is it in INIT?
                            talon.Set(ControlMode.PercentOutput, 0.0); // set talon to 0%
                            currentIntakeState = IntakeState.IDLE; // set current state to IDLE
                            break; // leave the switch statement
                        case IntakeState.IDLE: // is is in IDLE?
                            talon.Set(ControlMode.PercentOutput, 0.0); // set talon to 0%
                            break;
                        case IntakeState.IN: 
                            talon.Set(ControlMode.PercentOutput, 1);  // set talon 100%
                            break;
                        case IntakeState.IN_SLOW:
                            talon.Set(ControlMode.PercentOutput, 0.2);  // set talon 20%
                            break;
                        case IntakeState.REVERSE:
                            talon.Set(ControlMode.PercentOutput, -1);  // set talon 100%
                            break;
                        default:
                            talon.Set(ControlMode.PercentOutput, 0.0); // set talon to 0% to prevent broken behaviour
                            currentIntakeState = IntakeState.IDLE; // set state to idle to prevent further errors
                            break;
                    }
                }

                /*
                the ifs are the equivalent of the case statement

                
                if (currentIntakeState == IntakeState.INIT)
                {

                } else if (currentIntakeState == IntakeState.IDLE)
                {

                } else if (currentIntakeState == IntakeState.IN)
                {

                } else if (currentIntakeState == IntakeState.IN_SLOW)
                {

                }
                else if (currentIntakeState == IntakeState.REVERSE)
                {

                } else   // this is the equivalent of the default section.
                {

                }

                */
                /* print the three analog inputs as three columns */
            Debug.Print("Counter Value: " + counter);

                /* increment counter */
                ++counter; /* try to land a breakpoint here and hover over 'counter' to see it's current value.  Or add it to the Watch Tab */

                /* wait a bit */
                System.Threading.Thread.Sleep(100);

                // make sure the motors stay on
                CTRE.Phoenix.Watchdog.Feed();
            }
        }
    }
}
