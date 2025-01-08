using System.Collections.Generic;
using Unit.Cell;
using UnityEngine;

namespace Calculate
{
    public class PathFinding
    {
        private class PlatformData
        {
            public int Weight;
        }
        
        public Vector3 StartPosition, EndPosition;

        private CellController startCell;
        private CellController currentCell;
        private CellController lastCorrectCell;
        private CellController endCell;
        
        private RaycastHit hitResult;

        private readonly Vector3 startRayOnPlatform = Vector3.up * .5f;
        private readonly Vector3 startRayPositionPlatform = new(0, -.25f, 0);
        private readonly Vector3[] rayDirectionsPlatform = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        private Vector3 correctPlatformPosition;
        private int weightPlatform;

        private List<CellController> nearPlatforms = new();
        private Stack<CellController> platformStack = new();
        private Queue<CellController> pathToPoint =  new();
        private Stack<CellController> unverifiedPlatforms = new();
        private Dictionary<CellController, PlatformData> platformData = new(); // Временные данные платформ

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
            startCell = FindPlatform.GetPlatform(StartPosition + startRayOnPlatform, Vector3.down);
            currentCell = startCell;
        }

        private void ClearData()
        {
            unverifiedPlatforms.Clear();
            nearPlatforms.Clear();
            platformData.Clear(); // Очищаем временные данные после завершения
        }

        public Queue<CellController> GetPath(bool isUseColor = false)
        {
            weightPlatform = 0;
            SetCurrentPlatform();
            
            pathToPoint.Clear();
            if (!currentCell) return pathToPoint;

            endCell = FindPlatform.GetPlatform(EndPosition + startRayOnPlatform, Vector3.down);
            if (!endCell) return pathToPoint;

            this.isUseColor = isUseColor;
            ClearData();

            while (currentCell.CurrentCoordinates != endCell.CurrentCoordinates)
            {
                nearPlatforms = GetNearPlatforms();
                if (CheckAndAddPlatforms(ref nearPlatforms).Count == 0)
                {
                    if (unverifiedPlatforms.TryPop(out var newPlatform))
                    {
                        //newPlatform.SetColor(Color.red);
                        currentCell = newPlatform;
                    }
                    else
                    {
                        return pathToPoint;
                    }
                }   
                else
                {
                    AddPlatformToVerified(1);
                    if (currentCell.CurrentCoordinates == endCell.CurrentCoordinates) break;

                    currentCell = GetNextPlatform(nearPlatforms, endCell.CurrentCoordinates);
                    AddPlatformToVerified(2);
                }
            }

            correctPlatformPosition = EndPosition;
            BuildPath(pathToPoint);
            
            return pathToPoint;
        }

        private void AddPlatformToVerified(int weight)
        {
            if (!platformData.ContainsKey(currentCell))
                platformData[currentCell] = new PlatformData();

            var data = platformData[currentCell];
            if (data.Weight > 0) return;

            data.Weight = weightPlatform += weight;

            //currentPlatform.SetColor(Color.yellow);
            //currentPlatform.SetText(data.Weight.ToString());
        }

        private void BuildPath(Queue<CellController> path)
        {
            platformStack.Clear();
            if(!endCell.IsBlocked())
                platformStack.Push(endCell);
            
            //endPlatform.SetColor(Color.white);
            lastCorrectCell = endCell;

            while (lastCorrectCell != startCell)
            {
                var correctPlatform = GetCorrectPlatform();
                if (!correctPlatform) break;
                if(this.isUseColor)
                    correctPlatform.SetColor(Color.yellow);

                lastCorrectCell = correctPlatform;
                platformStack.Push(correctPlatform);
            }

            while (platformStack.Count > 0)
            {
                var platform = platformStack.Pop();
                path.Enqueue(platform);
            }

           // path.Dequeue();
        }

        private CellController GetCorrectPlatform()
        {
            CellController correctCell = null;
            var lastWeight = weightPlatform;
            correctPlatformPosition += startRayPositionPlatform;
            float rayLenght = 100;
            Vector3 objectScale = currentCell.transform.localScale;
            
            foreach (var direction in rayDirectionsPlatform)
            {
                rayLenght = Mathf.Abs(Vector3.Dot(direction, objectScale)) / 2f;
                
                if (!Physics.Raycast(correctPlatformPosition, direction, out hitResult, rayLenght, Layers.CELL_LAYER) ||
                    !hitResult.transform.TryGetComponent(out CellController platform) ||
                    !platformData.TryGetValue(platform, out var data) ||
                    platform == lastCorrectCell ||
                    data.Weight >= lastWeight) continue;
                
                correctCell = platform;
                lastWeight = data.Weight;
            }

            if (correctCell)
                correctPlatformPosition = correctCell.transform.position;

            return correctCell;
        }

        private List<CellController> GetNearPlatforms()
        {
            var platforms = new List<CellController>(4);
            var origin = currentCell.transform.position + startRayPositionPlatform;
            float rayLenght = 100;
            Vector3 objectScale = currentCell.transform.localScale;
            
            foreach (var direction in rayDirectionsPlatform)
            {
                rayLenght = Mathf.Abs(Vector3.Dot(direction, objectScale)) / 2f;
                Debug.DrawRay(origin, direction * rayLenght, Color.red, 2);
                if (Physics.Raycast(origin, direction, out hitResult, rayLenght, Layers.CELL_LAYER) &&
                    hitResult.transform.TryGetComponent(out CellController platform))
                {
                    platforms.Add(platform);
                }
            }

            return platforms;
        }

        private List<CellController> CheckAndAddPlatforms(ref List<CellController> platforms)
        {
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if ((platformData.TryGetValue(platforms[i], out var data) && data.Weight > 0) || platforms[i].IsBlocked())
                {
                    if (platforms[i].CurrentCoordinates == endCell.CurrentCoordinates)
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

        private CellController GetNextPlatform(List<CellController> nearAccessiblePlatforms, Vector2Int endCoordinates)
        {
            if (nearAccessiblePlatforms.Count == 0) return currentCell;

            CellController bestCell = nearAccessiblePlatforms[0];
            int bestDistance = CalculateDistance(bestCell, endCoordinates);

            foreach (var platform in nearAccessiblePlatforms)
            {
                int distance = CalculateDistance(platform, endCoordinates);
                if (distance < bestDistance)
                {
                    bestCell = platform;
                    bestDistance = distance;
                }
                else if (distance == bestDistance)
                {
                    var firstDistance = NormalDistance(bestCell.transform.position, endCell.transform.position);
                    var secondDistance = NormalDistance(platform.transform.position, endCell.transform.position);
                    if (secondDistance < firstDistance)
                    {
                        bestCell = platform;
                        bestDistance = distance;
                    }
                }
            }

            return bestCell;
        }

        private int CalculateDistance(CellController cell, Vector2Int endCoordinates)
        {
            var coordinates = cell.CurrentCoordinates;
            int result = Mathf.Abs(endCoordinates.x - coordinates.x) + Mathf.Abs(endCoordinates.y - coordinates.y);
            return result;
        }

        private int NormalDistance(Vector3 origin, Vector3 target)
        {
            Debug.DrawRay(origin, target - origin, Color.yellow, 2); // Рисуем луч в направлении от origin к target
            return (int)(origin - target).sqrMagnitude; // Квадрат расстояния
        }

    }

    public class PathFindingBuilder
    {
        private readonly PathFinding pathFinding = new();

        public PathFindingBuilder SetStartPosition(Vector3 start)
        {
            pathFinding.StartPosition = start;
            return this;
        }
        public PathFindingBuilder SetEndPosition(Vector3 end)
        {
            pathFinding.StartPosition = end;
            return this;
        }

        public PathFinding Build()
        {
            return pathFinding;
        }
    }
}
