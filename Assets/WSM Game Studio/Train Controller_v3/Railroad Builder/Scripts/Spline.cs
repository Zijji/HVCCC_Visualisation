using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace WSMGameStudio.Splines
{
    public class Spline : MonoBehaviour
    {
        [Range(15f, 100f)]
        public float newCurveLength = 15f;

        [SerializeField]
        private Vector3[] _controlPointsPositions;

        [SerializeField]
        private Quaternion[] _controlPointsRotations;

        [SerializeField]
        private BezierControlPointMode[] _modes;

        [SerializeField]
        private bool _loop;

        #region PROPERTIES

        public bool Loop
        {
            get { return _loop; }
            set
            {
                _loop = value;
                if (value == true)
                {
                    _modes[_modes.Length - 1] = _modes[0];
                    SetControlPointPosition(0, _controlPointsPositions[0]);
                }
            }
        }

        public int ControlPointCount
        {
            get { return _controlPointsPositions == null ? 0 : _controlPointsPositions.Length; }
        }

        public int CurveCount
        {
            get { return (_controlPointsPositions == null ? 0 : _controlPointsPositions.Length - 1) / 3; }
        }

        #endregion

        /// <summary>
        /// Get control point by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetControlPointPosition(int index)
        {
            if (_controlPointsPositions == null)
                Reset();

            return _controlPointsPositions[index];
        }

        /// <summary>
        /// Get rotation by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Quaternion GetControlPointRotation(int index)
        {
            return _controlPointsRotations[index];
        }

        /// <summary>
        /// Set control point rotation
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public void SetControlPointRotation(int index, Quaternion rotation)
        {
            if (index % 3 == 0)
            {
                Quaternion deltaRotation = rotation * Quaternion.Inverse(_controlPointsRotations[index]);
                if (_loop)
                {
                    if (index == 0)
                    {
                        _controlPointsRotations[1] *= deltaRotation;
                        _controlPointsRotations[_controlPointsRotations.Length - 2] *= deltaRotation;
                        _controlPointsRotations[_controlPointsRotations.Length - 1] = rotation;
                    }
                    else if (index == _controlPointsPositions.Length - 1)
                    {
                        _controlPointsRotations[0] = rotation;
                        _controlPointsRotations[1] *= deltaRotation;
                        _controlPointsRotations[index - 1] *= deltaRotation;
                    }
                    else
                    {
                        _controlPointsRotations[index - 1] *= deltaRotation;
                        _controlPointsRotations[index + 1] *= deltaRotation;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        _controlPointsRotations[index - 1] *= deltaRotation;
                    }
                    if (index + 1 < _controlPointsRotations.Length)
                    {
                        _controlPointsRotations[index + 1] *= deltaRotation;
                    }
                }
            }

            _controlPointsRotations[index] = rotation;
        }

        /// <summary>
        /// Set control point by index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        public void SetControlPointPosition(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 deltaPosition = point - _controlPointsPositions[index];
                if (_loop)
                {
                    if (index == 0)
                    {
                        _controlPointsPositions[1] += deltaPosition;
                        _controlPointsPositions[_controlPointsPositions.Length - 2] += deltaPosition;
                        _controlPointsPositions[_controlPointsPositions.Length - 1] = point;
                    }
                    else if (index == _controlPointsPositions.Length - 1)
                    {
                        _controlPointsPositions[0] = point;
                        _controlPointsPositions[1] += deltaPosition;
                        _controlPointsPositions[index - 1] += deltaPosition;
                    }
                    else
                    {
                        _controlPointsPositions[index - 1] += deltaPosition;
                        _controlPointsPositions[index + 1] += deltaPosition;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        _controlPointsPositions[index - 1] += deltaPosition;
                    }
                    if (index + 1 < _controlPointsPositions.Length)
                    {
                        _controlPointsPositions[index + 1] += deltaPosition;
                    }
                }
            }

            _controlPointsPositions[index] = point;
            EnforceMode(index);
        }

        /// <summary>
        /// Get control point mode by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BezierControlPointMode GetControlPointMode(int index)
        {
            return _modes[(index + 1) / 3];
        }

        /// <summary>
        /// Set control point mode
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mode"></param>
        public void SetControlPointMode(int index, BezierControlPointMode mode, bool enforceMode)
        {
            int modeIndex = (index + 1) / 3;
            _modes[modeIndex] = mode;
            if (_loop)
            {
                if (modeIndex == 0)
                {
                    _modes[_modes.Length - 1] = mode;
                }
                else if (modeIndex == _modes.Length - 1)
                {
                    _modes[0] = mode;
                }
            }

            if (enforceMode)
                EnforceMode(index);
        }

        /// <summary>
        /// Make sure selected control point mode is applied
        /// </summary>
        /// <param name="index"></param>
        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = _modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !_loop && (modeIndex == 0 || modeIndex == _modes.Length - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = _controlPointsPositions.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= _controlPointsPositions.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= _controlPointsPositions.Length)
                {
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = _controlPointsPositions.Length - 2;
                }
            }

            Vector3 middle = _controlPointsPositions[middleIndex];
            Vector3 enforcedTangent = middle - _controlPointsPositions[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _controlPointsPositions[enforcedIndex]);
            }
            _controlPointsPositions[enforcedIndex] = middle + enforcedTangent;
        }

        /// <summary>
        /// Get point
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = _controlPointsPositions.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetPoint(
                _controlPointsPositions[i], _controlPointsPositions[i + 1], _controlPointsPositions[i + 2], _controlPointsPositions[i + 3], t));
        }


        /// <summary>
        /// Get point rotation at spline postion t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Quaternion GetRotation(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = _controlPointsRotations.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return Bezier.GetPointRotation(_controlPointsRotations[i], _controlPointsRotations[i + 1], _controlPointsRotations[i + 2], _controlPointsRotations[i + 3], t);
        }

        /// <summary>
        /// Get velocity
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = _controlPointsPositions.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return transform.TransformPoint(Bezier.GetFirstDerivative(
                _controlPointsPositions[i], _controlPointsPositions[i + 1], _controlPointsPositions[i + 2], _controlPointsPositions[i + 3], t)) - transform.position;
        }

        /// <summary>
        /// Get direction
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        /// <summary>
        /// Get a list of spline oriented points based on the number os steps
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public List<OrientedPoint> GetOrientedPoints(int steps)
        {
            List<OrientedPoint> ret = new List<OrientedPoint>();

            float stepPercentage = 1f / steps;
            float t = 0;

            while (t < 1f)
            {
                OrientedPoint orientedPoint = GetOrientedPoint(t);
                ret.Add(orientedPoint);
                t += stepPercentage;
            }

            return ret;
        }

        /// <summary>
        /// Get point position and rotation on spline position t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public OrientedPoint GetOrientedPoint(float t)
        {
            return new OrientedPoint(GetPoint(t), GetRotation(t) * Quaternion.LookRotation(GetDirection(t)));
        }

        /// <summary>
        /// Reset spline
        /// </summary>
        public void Reset()
        {
            _loop = false;
            _controlPointsPositions = new Vector3[4];
            _controlPointsRotations = new Quaternion[4];

            for (int i = 0; i < _controlPointsPositions.Length; i++)
            {
                _controlPointsPositions[i] = new Vector3(0f, 0f, i * (newCurveLength / 3));
                _controlPointsRotations[i] = Quaternion.identity;
            }

            _modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Aligned,
                BezierControlPointMode.Aligned
            };
        }

        /// <summary>
        /// Reset all control points rotations
        /// </summary>
        public void ResetRotations()
        {
            ResetRotations(Quaternion.identity);
        }

        /// <summary>
        /// Set all control points rotations to new rotation value
        /// </summary>
        public void ResetRotations(Quaternion newRotation)
        {
            for (int i = 0; i < _controlPointsRotations.Length; i++)
            {
                _controlPointsRotations[i] = newRotation;
            }
        }

        /// <summary>
        /// Add new curve to spline
        /// </summary>
        public void AddCurve()
        {
            //Add positions
            Vector3 lastPointPosition = _controlPointsPositions[_controlPointsPositions.Length - 1];
            Vector3 lastPointDirection = transform.InverseTransformDirection(GetDirection(1));
            Quaternion lastPointRotation = GetRotation(1);

            Array.Resize(ref _controlPointsPositions, _controlPointsPositions.Length + 3);
            Array.Resize(ref _controlPointsRotations, _controlPointsRotations.Length + 3);

            //Add the 3 new control points
            for (int i = 3; i > 0; i--)
            {
                //Calculate new position based on last point direction
                lastPointPosition += (lastPointDirection * (newCurveLength / 3));
                //Position
                _controlPointsPositions[_controlPointsPositions.Length - i] = lastPointPosition;
                //Rotation
                _controlPointsRotations[_controlPointsRotations.Length - i] = lastPointRotation;//Quaternion.identity;
            }

            //Add modes
            Array.Resize(ref _modes, _modes.Length + 1);
            _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
            EnforceMode(_controlPointsPositions.Length - 4);

            if (_loop)
            {
                _controlPointsPositions[_controlPointsPositions.Length - 1] = _controlPointsPositions[0];
                _controlPointsRotations[_controlPointsRotations.Length - 1] = _controlPointsRotations[0];
                _modes[_modes.Length - 1] = _modes[0];
                EnforceMode(0);
            }
        }

        /// <summary>
        /// Remove the last curve (Disables loop property)
        /// </summary>
        public void RemoveCurve()
        {
            if (CurveCount <= 1)
            {
                Debug.Log("Spline has only one curve. Cannot remove last curve.");
                return;
            }

            Loop = false;

            Array.Resize(ref _controlPointsPositions, _controlPointsPositions.Length - 3);
            Array.Resize(ref _controlPointsRotations, _controlPointsRotations.Length - 3);
            Array.Resize(ref _modes, _modes.Length - 1);
        }

        /// <summary>
        /// Adjust control points vertical position to follow terrain elevations
        /// </summary>
        [ExecuteInEditMode]
        public void FollowTerrain()
        {
            Vector3 pointPosition = Vector3.zero;
            Vector3 origin = Vector3.zero;
            RaycastHit hit = new RaycastHit();

            //Disable colliders
            MeshCollider[] colliders = GetComponentsInChildren<MeshCollider>();
            bool[] collidersState = new bool[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                collidersState[i] = colliders[i].enabled;
                colliders[i].enabled = false;
            }

            //Adjust main control points
            for (int i = 0; i < _controlPointsPositions.Length; i += 3)
            {
                FollowTerrrainControlPointIteration(out pointPosition, out origin, out hit, i);
            }

            //Adjust auxiliar control points
            for (int i = 0; i < _controlPointsPositions.Length; i++)
            {
                if (i % 3 == 0)
                    continue;

                FollowTerrrainControlPointIteration(out pointPosition, out origin, out hit, i);
            }

            //Renable colliders
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = collidersState[i];

            UpdateMeshRenderer();
        }

        /// <summary>
        /// Reset control points heights
        /// </summary>
        public void Flatten()
        {
            Vector3 firstPointPosition = GetControlPointPosition(0);

            for (int i = 0; i < _controlPointsPositions.Length; i += 3)
            {
                SetControlPointPosition(i, new Vector3(_controlPointsPositions[i].x, firstPointPosition.y, _controlPointsPositions[i].z));
            }

            //Adjust auxiliar control points
            for (int i = 0; i < _controlPointsPositions.Length; i++)
            {
                if (i % 3 == 0)
                    continue;

                SetControlPointPosition(i, new Vector3(_controlPointsPositions[i].x, firstPointPosition.y, _controlPointsPositions[i].z));
            }

            UpdateMeshRenderer();
        }

        /// <summary>
        /// Updates Spline Mesh renderer (if exists)
        /// </summary>
        private void UpdateMeshRenderer()
        {
            SplineMeshRenderer splineMeshRenderer = GetComponent<SplineMeshRenderer>();

            if (splineMeshRenderer != null)
                splineMeshRenderer.ExtrudeMesh();
        }

        /// <summary>
        /// Find terrain elevation and update control point height to match terrain
        /// </summary>
        /// <param name="pointPosition"></param>
        /// <param name="origin"></param>
        /// <param name="hit"></param>
        /// <param name="i"></param>
        private void FollowTerrrainControlPointIteration(out Vector3 pointPosition, out Vector3 origin, out RaycastHit hit, int i)
        {
            pointPosition = GetControlPointPosition(i);

            origin = transform.TransformPoint(pointPosition);
            //origin = pointPosition + transform.position;

            if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity))
                FollowTerrainElevation(hit, i);
            else if (Physics.Raycast(origin, Vector3.up, out hit, Mathf.Infinity))
                FollowTerrainElevation(hit, i);
        }

        /// <summary>
        /// Set control point height to match terrain elevation
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="i"></param>
        private void FollowTerrainElevation(RaycastHit hit, int i)
        {
            SetControlPointMode(i, BezierControlPointMode.Free, false);

            if (i == 0)
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

            SetControlPointPosition(i, transform.InverseTransformPoint(hit.point));
        }

        /// <summary>
        /// Return bezier curve total distance
        /// </summary>
        /// <returns></returns>
        public float GetTotalDistance()
        {
            float distance = 0f;

            for (float t = 0f; t < 1f; t += 0.1f)
            {
                distance += Vector3.Distance(GetPoint(t), GetPoint(t + 0.1f));
            }

            return distance;
        }
    }
}
