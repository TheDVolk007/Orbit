using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class MathHelper
    {
        private static int CalculateSumOfArithmeticSequence(int a1, int d, int n)
        {
            return n * (a1 + CalculateNthMemberOfArithmeticSequence(a1, d, n)) / 2;
        }

        private static int CalculateNthMemberOfArithmeticSequence(int a1, int d, int n)
        {
            return a1 + (n - 1) * d;
        }

        private static int FindNForCertainSum(int sum, int a1, int d)
        {
            var discriminant = Math.Pow(2 * a1 - d, 2) + 8 * d * sum;
            var n = (d - 2 * a1 + Math.Sqrt(discriminant)) / (2 * d);
            return (int)Math.Round(n);
        }

        public static List<int> FindBreakingPointsInArithmeticSequence(int a1, int d, int n, int limitPerStep)
        {
            var sum = CalculateSumOfArithmeticSequence(a1, d, n);
            var listOfPoints = new List<int>();
            var point = 0;
            while (sum > limitPerStep)
            {
                point += FindNForCertainSum(limitPerStep, a1 - point, d);
                listOfPoints.Add(point);
                sum -= limitPerStep;
            }

            return listOfPoints;
        }

        public static Vector2 TransportPointInRespectToPlaneBorders(Vector2 position, float borderRadius)
        {
            var pointsDistanceFromCenter = position.magnitude;
            if(pointsDistanceFromCenter <= borderRadius)
            {
                return position;
            }
            var overlap = pointsDistanceFromCenter - borderRadius;
            var lengthOfTransportedPosition = pointsDistanceFromCenter - 2 * overlap;
            return -position.normalized * lengthOfTransportedPosition;
        }
    }
}