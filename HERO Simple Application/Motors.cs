using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.Controller;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HERO_Simple_Application
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

    public class Motors
    {
        public static TalonSRX leftTalon1 = new TalonSRX(0);
        public static TalonSRX leftTalon2 = new TalonSRX(1);
        public static TalonSRX rightTalon1 = new TalonSRX(2);
        public static TalonSRX rightTalon2 = new TalonSRX(3);

        public static IntakeState currentIntakeState = IntakeState.INIT;
        public static IntakeState lastIntakeState = IntakeState.UNKNOWN;

        public static GameController gamepad = new GameController(new CTRE.Phoenix.UsbHostDevice(0));

        public static void Main()
        {
            /* configure the talons to factory default to prevent other code from interfering */
            leftTalon1.ConfigFactoryDefault();
            leftTalon2.ConfigFactoryDefault();
            rightTalon1.ConfigFactoryDefault();
            rightTalon2.ConfigFactoryDefault();

            int counter = 0;
            while (true)
            {
                // take input from gamepad - configure intake state based on those
                if (gamepad.GetButton(0)) // button with id 0 pressed
                {
                    currentIntakeState = IntakeState.IDLE; // puts robot in Idle state (no movement)
                }
                if (gamepad.GetButton(1)) // button with id 1 pressed
                {
                    currentIntakeState = IntakeState.IN; // puts robot in In state (movement)
                }
                if (gamepad.GetButton(2)) // button with id 2 pressed
                {
                    currentIntakeState = IntakeState.IN_SLOW; // puts robot in In Slow state (low movement)
                }
                if (gamepad.GetButton(3)) // button with id 3 pressed
                {
                    currentIntakeState = IntakeState.REVERSE; // puts robot in Reverse state (backwards movement)
                }
                if (gamepad.GetAxis(0) <= 0)
                {

                }

                // based on intake state - set motor power
                if (currentIntakeState != lastIntakeState) // code doesnt run every update to make it go faster
                {
                    lastIntakeState = currentIntakeState; // update lastIntakeState
                    switch (currentIntakeState) // switch on current state for intake
                    {
                        case IntakeState.INIT: // if in INIT state, initializes robot
                            leftTalon1.Set(ControlMode.PercentOutput, 0.0); // set talon to 0% power
                            currentIntakeState = IntakeState.IDLE; // set state to IDLE (doing nothing)
                            break;
                        case IntakeState.IDLE:
                            leftTalon1.Set(ControlMode.PercentOutput, 0.0); // set talon to 0% power
                            leftTalon2.Follow(leftTalon1);
                            rightTalon1.Set(ControlMode.PercentOutput, 0.0);
                            rightTalon2.Follow(rightTalon1);
                            break;
                        case IntakeState.IN:
                            leftTalon1.Set(ControlMode.PercentOutput, 1); // set talon to 100% power
                            leftTalon2.Follow(leftTalon1);
                            rightTalon1.Set(ControlMode.PercentOutput, 1);
                            rightTalon2.Follow(rightTalon1);
                            break;
                        case IntakeState.IN_SLOW:
                            leftTalon1.Set(ControlMode.PercentOutput, 0.2); // set talon to 20% power
                            leftTalon2.Follow(leftTalon1);
                            rightTalon1.Set(ControlMode.PercentOutput, 0.2);
                            rightTalon2.Follow(rightTalon1);
                            break;
                        case IntakeState.REVERSE:
                            leftTalon1.Set(ControlMode.PercentOutput, -1); // set talon to 100% power in reverse
                            leftTalon2.Follow(leftTalon1);
                            rightTalon1.Set(ControlMode.PercentOutput, -1);
                            rightTalon2.Follow(rightTalon1);
                            break;
                        default:
                            leftTalon1.Set(ControlMode.PercentOutput, 0.0); // set talon to 0% power to prevent it from going insane
                            leftTalon2.Follow(leftTalon1);
                            rightTalon1.Follow(leftTalon1);
                            rightTalon2.Follow(leftTalon1);
                            currentIntakeState = IntakeState.IDLE; // make bot idle to ensure it will not go insane in the future
                            break;
                    }
                }

                /* wait a bit */
                System.Threading.Thread.Sleep(10);

                /* make sure the motors stay on */
                CTRE.Phoenix.Watchdog.Feed();
            }
        }
    }
}
