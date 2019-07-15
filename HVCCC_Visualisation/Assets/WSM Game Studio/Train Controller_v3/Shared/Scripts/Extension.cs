using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.RailroadSystem
{
    /// <summary>
    /// Units of speed
    /// </summary>
    public enum SpeedUnits
    {
        kph,
        mph
    }

    /// <summary>
    /// Rail switchin modes
    /// </summary>
    public enum SwitchMode
    {
        Always,
        Once,
        Random,
        IfActivated,
        IfDeactivated
    }
    /// <summary>
    /// Station stop mode
    /// </summary>
    public enum StopMode
    {
        Always,
        Once,
        Random
    }

    /// <summary>
    /// Train behaviour after stoping at station
    /// </summary>
    public enum StationBehaviour
    {
        LeaveAfterTime,
        StopForever
    }

    /// <summary>
    /// Which doors must open on station
    /// </summary>
    public enum StationDoorDirection
    {
        BothSides,
        Left, 
        Right
    }

    public static class GeneralSettings
    {
        public const float AccelerationRate = 5.0f;
        public const float BrakeDrag = 5.0f;
        public const float IdleDrag = 0.1f;
        public const float WheelsMaxAngularVelocity = 50f;
        public const float DownForceFactor = 8f;
    }

    public static class AnimationParameters
    {
        public const string Open = "Open";
    }

    public static class SharedMethods
    {
        /// <summary>
        /// Control speed base on max speed, acceleration and brakes
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="isGrounded"></param>
        /// <param name="maxSpeedKph"></param>
        /// <param name="speed_KPH"></param>
        /// <param name="acceleration"></param>
        /// <param name="brake"></param>
        /// <param name="targetVelocityIn"></param>
        /// <param name="targetVelocityOut"></param>
        /// <param name="currentSpeedIn"></param>
        /// <param name="currentSpeedOut"></param>
        /// <param name="targetSpeedIn"></param>
        /// <param name="targetSpeedOut"></param>
        public static void SpeedControl(Rigidbody rigidbody, bool isGrounded, float maxSpeedKph, float speed_KPH, float acceleration, float brake, Vector3 targetVelocityIn, out Vector3 targetVelocityOut, float currentSpeedIn, out float currentSpeedOut, float targetSpeedIn, out float targetSpeedOut)
        {
            currentSpeedOut = currentSpeedIn;
            targetSpeedOut = targetSpeedIn;
            targetVelocityOut = targetVelocityIn;

            if (isGrounded)
            {
                targetSpeedOut = SharedMethods.GetTargetSpeed(acceleration, maxSpeedKph);
                targetSpeedOut = SharedMethods.ApplyBrakes(rigidbody, brake, targetSpeedOut);
                currentSpeedOut = SharedMethods.SoftAcceleration(currentSpeedOut, targetSpeedOut);

                //Apply velocity
                if (speed_KPH < maxSpeedKph)
                {
                    targetVelocityOut = currentSpeedOut == 0f? Vector3.zero : rigidbody.velocity + (rigidbody.transform.forward * currentSpeedOut);
                    rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, targetVelocityOut, Time.deltaTime * GeneralSettings.AccelerationRate);
                }
            }
        }

        /// <summary>
        /// Calculates target speed
        /// </summary>
        /// <param name="acceleration">Range between -1 and 1</param>
        /// <param name="maxSpeed"></param>
        /// <returns></returns>
        private static float GetTargetSpeed(float acceleration, float maxSpeed)
        {
            float targetSpeed;

            if (acceleration > 0.0f) //Moving forwards
                targetSpeed = Mathf.Lerp(0.0f, maxSpeed, acceleration);
            else if (acceleration < 0.0f) //Moving backwards
                targetSpeed = Mathf.Lerp(0.0f, maxSpeed * (-1), acceleration * (-1));
            else //if (_acceleration == 0.0f) //Stopping
                targetSpeed = 0.0f;

            return targetSpeed;
        }

        /// <summary>
        /// Applies brakes
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="brake"></param>
        /// <param name="brakeDrag"></param>
        /// <param name="idleDrag"></param>
        /// <param name="targetSpeed"></param>
        /// <returns></returns>
        public static float ApplyBrakes(Rigidbody rigidbody, float brake, float targetSpeed)
        {
            if (brake > 0.0f)
            {
                targetSpeed = 0.0f;
                rigidbody.angularDrag = GeneralSettings.BrakeDrag;
            }
            else
                rigidbody.angularDrag = GeneralSettings.IdleDrag;

            return targetSpeed;
        }

        /// <summary>
        /// Slowly goes from current speed to target speed
        /// </summary>
        /// <param name="accelerationRate"></param>
        /// <param name="currentSpeed"></param>
        /// <param name="targetSpeed"></param>
        /// <returns></returns>
        private static float SoftAcceleration(float currentSpeed, float targetSpeed)
        {
            if (Mathf.Abs(currentSpeed) < Mathf.Abs(targetSpeed))
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, GeneralSettings.AccelerationRate * Time.deltaTime);
            else if (Mathf.Abs(currentSpeed) >= Mathf.Abs(targetSpeed))
                currentSpeed = targetSpeed;

            return currentSpeed;
        }

        /// <summary>
        /// Update wheels properties
        /// </summary>
        /// <param name="wheelsScripts"></param>
        /// <param name="accelerationMode"></param>
        /// <param name="isGrounded"></param>
        /// <param name="acceleration"></param>
        /// <param name="brake"></param>
        /// <param name="maxTorque"></param>
        public static void UpdateWheels(List<TrainWheel_v3> wheelsScripts, float brake)
        {
            foreach (var wheel in wheelsScripts)
            {
                wheel.Brake = brake;
            }
        }

        /// <summary>
        /// Trains SFX, motor, wheels on trails, etc
        /// </summary>
        public static void PlaySFX(SFX sfx, float speedKMH, float brake, bool enginesOn, bool isGrounded)
        {
            if (sfx.engineSFX != null)
            {
                if (enginesOn && !sfx.engineSFX.isPlaying)
                    sfx.engineSFX.Play();
                else if (!enginesOn && sfx.engineSFX.isPlaying)
                    sfx.engineSFX.Stop();
            }

            if (isGrounded)
            {
                if (sfx.wheelsSFX != null)
                {
                    if (speedKMH >= 1f && !sfx.wheelsSFX.isPlaying)
                        sfx.wheelsSFX.Play();
                    else if (speedKMH <= 1f && sfx.wheelsSFX.isPlaying)
                        sfx.wheelsSFX.Stop();
                }

                if (sfx.brakesSFX != null)
                {
                    if (speedKMH >= 0.5f && brake > 0.5f && !sfx.brakesSFX.isPlaying)
                        sfx.brakesSFX.Play();
                    else if (sfx.brakesSFX.isPlaying && speedKMH <= 0.5f || brake < 0.5f)
                        sfx.brakesSFX.Stop(); 
                }
            }
            else
            {
                if (sfx.wheelsSFX != null) sfx.wheelsSFX.Stop();
                if (sfx.brakesSFX != null) sfx.brakesSFX.Stop();
            }
        }
    }

    public static class Extension
    {
        /// <summary>
        /// Random event lottery
        /// </summary>
        /// <param name="randomSwitchProbability"></param>
        /// <returns></returns>
        public static bool RandomEvent(int randomSwitchProbability)
        {
            return Random.Range(1, 101) <= randomSwitchProbability;
        }

        /// <summary>
        /// Converts meters per second to kilometers per hour
        /// </summary>
        /// <param name="metersPerSecond"></param>
        /// <returns></returns>
        public static float Convert_MPS_To_KPH(float metersPerSecond)
        {
            return metersPerSecond * 3.6f;
        }

        /// <summary>
        /// Converts kilometers per hour to meters per second
        /// </summary>
        /// <param name="kilometersPerHour"></param>
        /// <returns></returns>
        public static float Convert_KPH_To_MPS(float kilometersPerHour)
        {
            return kilometersPerHour / 3.6f;
        }

        /// <summary>
        /// Converts meters per second to miles per hour
        /// </summary>
        /// <param name="metersPerSecond"></param>
        /// <returns></returns>
        public static float Convert_MPS_To_MPH(float metersPerSecond)
        {
            return metersPerSecond * 2.23694f;
        }

        /// <summary>
        /// Converts miles per hour to meters per second
        /// </summary>
        /// <param name="milesPerHour"></param>
        /// <returns></returns>
        public static float Convert_MPH_To_MPS(float milesPerHour)
        {
            return milesPerHour / 2.23694f;
        }
    }
}
