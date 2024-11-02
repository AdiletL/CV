using System.Collections.Generic;
using UnityEngine;

namespace Calculate
{
    public class PathToPoint
    {
        public Transform StartTransform, EndTransform;

        private Platform startPlatform;
        private Platform currentPlatform;
        private Platform lastCorrectPlatform;

        private readonly Vector3 startRayOnPlatform = Vector3.up * .5f;
        private readonly Vector3 startRayPositionPlatform = new(0, -.25f, 0);
        private readonly Vector3[] rayDirectionsPlatform = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        private Vector3 correctPlatformPosition;
        private int weightPlatform;

        private readonly Stack<Platform> unverifiedPlatforms = new();
        private readonly Dictionary<Platform, PlatformData> platformData = new(); // Временные данные платформ

        public void SetTarget(Transform target)
        {
            EndTransform = target;
        }

        private void SetCurrentPlatform()
        {
            startPlatform = FindPlatform.GetPlatform(StartTransform.position + startRayOnPlatform, Vector3.down);
            currentPlatform = startPlatform;
        }

        public Queue<Platform> FindPathToPoint()
        {
            weightPlatform = 0;
            SetCurrentPlatform();
            var pathToPoint = new Queue<Platform>();
            if (currentPlatform == null) return pathToPoint;

            var endPlatform = FindPlatform.GetPlatform(EndTransform.position + startRayOnPlatform, Vector3.down);
            if (endPlatform == null) return pathToPoint;

            while (currentPlatform.CurrentCoordinates != endPlatform.CurrentCoordinates)
            {
                var nearPlatforms = GetNearPlatforms();

                if (CheckAndAddPlatforms(ref nearPlatforms).Count == 0)
                {
                    if (unverifiedPlatforms.TryPop(out var newPlatform))
                    {
                        newPlatform.SetColor(Color.red);
                        currentPlatform = newPlatform;
                    }
                    else
                    {
                        return pathToPoint;
                    }
                }   
                else
                {
                    AddPlatformToVerified(currentPlatform, 1);
                    if (currentPlatform.CurrentCoordinates == endPlatform.CurrentCoordinates) break;

                    currentPlatform = GetNextPlatform(nearPlatforms, endPlatform.CurrentCoordinates);
                    AddPlatformToVerified(currentPlatform, 2);
                }
            }

            correctPlatformPosition = EndTransform.position;
            BuildPath(pathToPoint, endPlatform);

            platformData.Clear(); // Очищаем временные данные после завершения
            return pathToPoint;
        }

        private void AddPlatformToVerified(Platform platform, int weight)
        {
            if (!platformData.ContainsKey(platform))
                platformData[platform] = new PlatformData();

            var data = platformData[platform];
            if (data.IsChecked) return;

            data.IsChecked = true;
            data.Weight = weightPlatform += weight;

            platform.SetColor(Color.yellow);
            platform.SetText(data.Weight.ToString());
        }

        private void BuildPath(Queue<Platform> pathToPoint, Platform endPlatform)
        {
            var stack = new Stack<Platform>();
            stack.Push(endPlatform);
            endPlatform.SetColor(Color.white);
            lastCorrectPlatform = endPlatform;

            while (lastCorrectPlatform != startPlatform)
            {
                var correctPlatform = GetCorrectPlatform();
                if (correctPlatform == null) break;

                correctPlatform.SetColor(Color.white);
                lastCorrectPlatform = correctPlatform;
                stack.Push(correctPlatform);
            }

            while (stack.Count > 0)
            {
                pathToPoint.Enqueue(stack.Pop());
            }
        }

        private Platform GetCorrectPlatform()
        {
            Platform correctPlatform = null;
            var lastWeight = weightPlatform;
            correctPlatformPosition += startRayPositionPlatform;

            foreach (var direction in rayDirectionsPlatform)
            {
                if (Physics.Raycast(correctPlatformPosition, direction, out var hit, 4) &&
                    hit.transform.TryGetComponent(out Platform platform) &&
                    platformData.TryGetValue(platform, out var data) &&
                    data.Weight > 0 &&
                    platform != lastCorrectPlatform &&
                    data.Weight < lastWeight)
                {
                    correctPlatform = platform;
                    lastWeight = data.Weight;
                }
            }

            if (correctPlatform != null)
                correctPlatformPosition = correctPlatform.transform.position;

            return correctPlatform;
        }

        private List<Platform> GetNearPlatforms()
        {
            var platforms = new List<Platform>(4);
            var origin = currentPlatform.transform.position + startRayPositionPlatform;

            foreach (var direction in rayDirectionsPlatform)
            {
                if (Physics.Raycast(origin, direction, out var hit, 4) &&
                    hit.transform.TryGetComponent(out Platform platform) &&
                    (!platformData.TryGetValue(platform, out var data) || (!data.IsChecked && !platform.IsBlocked)))
                {
                    platforms.Add(platform);
                }
            }

            return platforms;
        }

        private List<Platform> CheckAndAddPlatforms(ref List<Platform> platforms)
        {
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].IsBlocked || (platformData.TryGetValue(platforms[i], out var data) && data.IsChecked))
                {
                    platforms.RemoveAt(i);
                }
                else
                {
                    unverifiedPlatforms.Push(platforms[i]);
                    platforms[i].SetColor(Color.blue);
                }
            }

            return platforms;
        }

        private Platform GetNextPlatform(List<Platform> nearAccessiblePlatforms, Vector2Int endCoordinates)
        {
            if (nearAccessiblePlatforms.Count == 0) return currentPlatform;

            int targetSum = endCoordinates.x + endCoordinates.y;
            Platform bestPlatform = nearAccessiblePlatforms[0];
            int bestDistance = CalculateDistance(bestPlatform, targetSum, endCoordinates);

            foreach (var platform in nearAccessiblePlatforms)
            {
                int distance = CalculateDistance(platform, targetSum, endCoordinates);
                if (distance < bestDistance)
                {
                    bestPlatform = platform;
                    bestDistance = distance;
                }
            }

            return bestPlatform;
        }

        private static int CalculateDistance(Platform platform, int targetSum, Vector2Int endCoordinates)
        {
            var coordinates = platform.CurrentCoordinates;
            int result = Mathf.Abs(targetSum - (coordinates.x + coordinates.y));
            result += Mathf.Abs(endCoordinates.x - coordinates.x) + Mathf.Abs(endCoordinates.y - coordinates.y);
            return result;
        }

        // Временная структура для хранения данных платформы
        private class PlatformData
        {
            public bool IsChecked;
            public int Weight;
        }
    }

    public class PathToPointBuilder
    {
        private readonly PathToPoint pathToPoint = new();

        public PathToPointBuilder SetPosition(Transform start, Transform end)
        {
            pathToPoint.StartTransform = start;
            pathToPoint.EndTransform = end;
            return this;
        }

        public PathToPoint Build()
        {
            return pathToPoint;
        }
    }
}
