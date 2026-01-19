/*
 * ii's Stupid Menu  Classes/Menu/RopePhysics.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GorillaLocomotion;
using UnityEngine;

namespace iiMenu.Classes.Menu
{
    public class RopePhysics : MonoBehaviour
    {
        public float segmentLength = 0.5f;
        private readonly int solverIterations = 50;
        private readonly float gravity = 9.81f;
        private readonly float damping = 0.98f;
        private readonly float mass = 1f;

        private readonly bool pinStart = true;
        private readonly bool pinEnd = true;
        private Vector3 startPosition;
        private Vector3 endPosition;

        private readonly float airResistance = 0.02f;
        private readonly float stiffness = 1f;
        private readonly bool enableCollisions = false;
        private readonly float collisionRadius = 0.1f;

        private LineRenderer lr;
        private Vector3[] points;
        private Vector3[] prevPoints;
        private Vector3[] accelerations;
        private int pointCount;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            pointCount = lr.positionCount;

            if (pointCount < 2)
            {
                Destroy(this);
                return;
            }

            InitializeRope();
        }

        void InitializeRope()
        {
            points = new Vector3[pointCount];
            prevPoints = new Vector3[pointCount];
            accelerations = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                points[i] = lr.GetPosition(i);
                prevPoints[i] = points[i];
                accelerations[i] = Vector3.zero;
            }

            if (pinStart)
                startPosition = points[0];
            if (pinEnd)
                endPosition = points[pointCount - 1];
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            Simulate(dt);
            UpdateLineRenderer();
        }

        void Simulate(float dt)
        {
            for (int i = 0; i < pointCount; i++)
            {
                if ((i == 0 && pinStart) || (i == pointCount - 1 && pinEnd))
                    continue;

                Vector3 velocity = (points[i] - prevPoints[i]) / dt;
                Vector3 dragForce = -velocity * airResistance;
                Vector3 gravityForce = Vector3.down * gravity * mass;

                accelerations[i] = (gravityForce + dragForce) / mass;

                Vector3 newPos = points[i] + velocity * dt * damping + accelerations[i] * dt * dt;

                if (enableCollisions)
                    newPos = HandleCollision(points[i], newPos);

                prevPoints[i] = points[i];
                points[i] = newPos;
            }

            for (int iter = 0; iter < solverIterations; iter++)
                ApplyConstraints();

            if (pinStart)
            {
                points[0] = startPosition;
                prevPoints[0] = points[0];
            }

            if (!pinEnd) return;
            points[pointCount - 1] = endPosition;
            prevPoints[pointCount - 1] = points[pointCount - 1];
        }

        void ApplyConstraints()
        {
            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector3 delta = points[i + 1] - points[i];
                float currentDist = delta.magnitude;

                if (currentDist == 0) continue;

                float diff = (currentDist - segmentLength) / currentDist;
                Vector3 offset = delta * (diff * 0.5f * stiffness);

                bool startPinned = (i == 0 && pinStart);
                bool endPinned = (i == pointCount - 2 && pinEnd);

                switch (startPinned)
                {
                    case false when !endPinned:
                        points[i] += offset;
                        points[i + 1] -= offset;
                        break;
                    case true when !endPinned:
                        points[i + 1] -= offset * 2f;
                        break;
                    case false:
                        points[i] += offset * 2f;
                        break;
                }
            }
        }

        Vector3 HandleCollision(Vector3 oldPos, Vector3 newPos)
        {
            Vector3 direction = newPos - oldPos;
            float distance = direction.magnitude;

            if (distance < 0.001f || !Physics.SphereCast(oldPos, collisionRadius, direction.normalized, out RaycastHit hit, distance,
                    GTPlayer.Instance.locomotionEnabledLayers)) return newPos;

            Vector3 collisionPoint = hit.point + hit.normal * collisionRadius;
            return collisionPoint;

        }

        void UpdateLineRenderer() =>
            lr.SetPositions(points);

        public void ApplyForceToPoint(int pointIndex, Vector3 force)
        {
            if (pointIndex < 0 || pointIndex >= pointCount) return;
            Vector3 acceleration = force / mass;
            points[pointIndex] += acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;
        }

        public void ApplyImpulseToPoint(int pointIndex, Vector3 impulse)
        {
            if (pointIndex < 0 || pointIndex >= pointCount) return;
            Vector3 velocity = impulse / mass;
            points[pointIndex] += velocity * Time.fixedDeltaTime;
        }

        public void SetStartPosition(Vector3 pos)
        {
            startPosition = pos;
            if (!pinStart) return;
            points[0] = pos;
            prevPoints[0] = pos;
        }

        public void SetEndPosition(Vector3 pos)
        {
            endPosition = pos;
            if (!pinEnd) return;
            points[pointCount - 1] = pos;
            prevPoints[pointCount - 1] = pos;
        }

        public void GrabPoint(int pointIndex, Vector3 position)
        {
            if (pointIndex < 0 || pointIndex >= pointCount) return;
            points[pointIndex] = position;
            prevPoints[pointIndex] = position;
        }
    }
}
