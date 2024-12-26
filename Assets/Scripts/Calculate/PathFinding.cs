using System.Collections.Generic;
using UnityEngine;

namespace Calculate
{
    public class PathFinding
    {
        private class PlatformData
        {
            public bool IsChecked;
            public int Weight;
        }
        
        public Vector3 StartPosition, EndPosition;

        private Platform startPlatform;
        private Platform currentPlatform;
        private Platform lastCorrectPlatform;
        private Platform endPlatform;
        
        private RaycastHit hitResult;

        private readonly Vector3 startRayOnPlatform = Vector3.up * .5f;
        private readonly Vector3 startRayPositionPlatform = new(0, -.25f, 0);
        private readonly Vector3[] rayDirectionsPlatform = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        private Vector3 correctPlatformPosition;
        private int weightPlatform;

        private List<Platform> nearPlatforms = new();
        private Stack<Platform> platformStack = new();
        private Queue<Platform> pathToPoint =  new();
        private Stack<Platform> unverifiedPlatforms = new();
        private Dictionary<Platform, PlatformData> platformData = new(); // Временные данные платформ

        private bool isUseColor;

        public void SetStartPosition(Vector3 position)
        {
            StartPosition = position;
        }
        public void SetTargetPosition(Vector3 target)
        {
            EndPosition = target;
        }

        private void SetCurrentPlatform()
        {
            startPlatform = FindPlatform.GetPlatform(StartPosition + startRayOnPlatform, Vector3.down);
            currentPlatform = startPlatform;
        }

        private void ClearData()
        {
            unverifiedPlatforms.Clear();
            nearPlatforms.Clear();
            platformData.Clear(); // Очищаем временные данные после завершения
        }

        public Queue<Platform> GetPath(bool isUseColor = false)
        {
            weightPlatform = 0;
            SetCurrentPlatform();
            
            pathToPoint.Clear();
            if (!currentPlatform) return pathToPoint;

            endPlatform = FindPlatform.GetPlatform(EndPosition + startRayOnPlatform, Vector3.down);
            if (!endPlatform) return pathToPoint;

            this.isUseColor = isUseColor;
            ClearData();

            while (currentPlatform.CurrentCoordinates != endPlatform.CurrentCoordinates)
            {
                nearPlatforms = GetNearPlatforms();
                if (CheckAndAddPlatforms(ref nearPlatforms).Count == 0)
                {
                    if (unverifiedPlatforms.TryPop(out var newPlatform))
                    {
                        //newPlatform.SetColor(Color.red);
                        currentPlatform = newPlatform;
                    }
                    else
                    {
                        return pathToPoint;
                    }
                }   
                else
                {
                    AddPlatformToVerified(1);
                    if (currentPlatform.CurrentCoordinates == endPlatform.CurrentCoordinates) break;

                    currentPlatform = GetNextPlatform(nearPlatforms, endPlatform.CurrentCoordinates);
                    AddPlatformToVerified(2);
                }
            }

            correctPlatformPosition = EndPosition;
            BuildPath(pathToPoint);
            
            return pathToPoint;
        }

        private void AddPlatformToVerified(int weight)
        {
            if (!platformData.ContainsKey(currentPlatform))
                platformData[currentPlatform] = new PlatformData();

            var data = platformData[currentPlatform];
            if (data.IsChecked) return;

            data.IsChecked = true;
            data.Weight = weightPlatform += weight;

            //currentPlatform.SetColor(Color.yellow);
            //currentPlatform.SetText(data.Weight.ToString());
        }

        private void BuildPath(Queue<Platform> path)
        {
            platformStack.Clear();
            if(!endPlatform.IsBlocked())
                platformStack.Push(endPlatform);
            
            //endPlatform.SetColor(Color.white);
            lastCorrectPlatform = endPlatform;

            while (lastCorrectPlatform != startPlatform)
            {
                var correctPlatform = GetCorrectPlatform();
                if (!correctPlatform) break;

                if(this.isUseColor)
                    correctPlatform.SetColor(Color.yellow);
                lastCorrectPlatform = correctPlatform;
                platformStack.Push(correctPlatform);
            }

            while (platformStack.Count > 0)
            {
                path.Enqueue(platformStack.Pop());
            }

           // path.Dequeue();
        }

        private Platform GetCorrectPlatform()
        {
            Platform correctPlatform = null;
            var lastWeight = weightPlatform;
            correctPlatformPosition += startRayPositionPlatform;

            foreach (var direction in rayDirectionsPlatform)
            {
                if (!Physics.Raycast(correctPlatformPosition, direction, out hitResult, 100, Layers.PLATFORM_LAYER) ||
                    !hitResult.transform.TryGetComponent(out Platform platform) ||
                    !platformData.TryGetValue(platform, out var data) ||
                    platform == lastCorrectPlatform ||
                    data.Weight >= lastWeight) continue;
                
                correctPlatform = platform;
                lastWeight = data.Weight;
            }

            if (correctPlatform)
                correctPlatformPosition = correctPlatform.transform.position;

            return correctPlatform;
        }

        private List<Platform> GetNearPlatforms()
        {
            var platforms = new List<Platform>(4);
            var origin = currentPlatform.transform.position + startRayPositionPlatform;

            foreach (var direction in rayDirectionsPlatform)
            {
                Debug.DrawRay(origin, direction, Color.red, 2);
                if (Physics.Raycast(origin, direction, out hitResult, 100, Layers.PLATFORM_LAYER) &&
                    hitResult.transform.TryGetComponent(out Platform platform))
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
                if ((platformData.TryGetValue(platforms[i], out var data) && data.IsChecked) || platforms[i].IsBlocked())
                {
                    if (platforms[i].CurrentCoordinates == endPlatform.CurrentCoordinates)
                    {
                        unverifiedPlatforms.Push(platforms[i]);
                        return platforms;
                    }
                    
                    platforms.RemoveAt(i);
                }
                else
                {
                    unverifiedPlatforms.Push(platforms[i]);
                    //platforms[i].SetColor(Color.blue);
                }
            }

            return platforms;
        }

        private Platform GetNextPlatform(List<Platform> nearAccessiblePlatforms, Vector2Int endCoordinates)
        {
            if (nearAccessiblePlatforms.Count == 0) return currentPlatform;

            Platform bestPlatform = nearAccessiblePlatforms[0];
            int bestDistance = CalculateDistance(bestPlatform, endCoordinates);

            foreach (var platform in nearAccessiblePlatforms)
            {
                int distance = CalculateDistance(platform, endCoordinates);
                if (distance < bestDistance)
                {
                    bestPlatform = platform;
                    bestDistance = distance;
                }
                else if (distance == bestDistance)
                {
                    var firstDistance = NormalDistance(bestPlatform.transform.position, endPlatform.transform.position);
                    var secondDistance = NormalDistance(platform.transform.position, endPlatform.transform.position);
                    if (secondDistance < firstDistance)
                    {
                        bestPlatform = platform;
                        bestDistance = distance;
                    }
                }
            }

            return bestPlatform;
        }

        private int CalculateDistance(Platform platform, Vector2Int endCoordinates)
        {
            var coordinates = platform.CurrentCoordinates;
            int result = Mathf.Abs(endCoordinates.x - coordinates.x) + Mathf.Abs(endCoordinates.y - coordinates.y);
            return result;
        }

        private int NormalDistance(Vector3 origin, Vector3 target)
        {
            Debug.DrawRay(origin, target - origin, Color.yellow, 2); // Рисуем луч в направлении от origin к target
            return (int)(origin - target).sqrMagnitude; // Квадрат расстояния
        }

    }

    public class PathToPointBuilder
    {
        private readonly PathFinding pathFinding = new();

        public PathToPointBuilder SetPosition(Vector3 start, Vector3 end)
        {
            pathFinding.StartPosition = start;
            pathFinding.EndPosition = end;
            return this;
        }

        public PathFinding Build()
        {
            return pathFinding;
        }
    }
}
