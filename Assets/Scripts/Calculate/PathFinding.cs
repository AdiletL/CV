using System.Collections.Generic;
using Unit.Cell;
using UnityEngine;

namespace Calculate
{
    public class PathFinding
    {
        private class CellData
        {
            public int Weight;
        }
        
        public Vector3 StartPosition, EndPosition;

        private CellController startCell;
        private CellController currentCell;
        private CellController lastCorrectCell;
        private CellController endCell;
        private CellController correctCell;
        private CellController bestCell;
        
        private RaycastHit hitResult;

        private readonly Vector3 startRayOnCell = Vector3.up * .5f;
        private readonly Vector3 startRayPositionCell = new(0, -.2f, 0);
        private readonly Vector3[] rayDirectionsCell = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        private Vector3 correctCellPosition;
        private Vector3 currentCellScale;
        private int weightCell;
        private int maxCheck = 50;
        private float rayLenght;

        private List<CellController> nearCells = new();
        private Queue<CellController> pathToPoint =  new();
        private Stack<CellController> platformStack = new();
        private Stack<CellController> unverifiedCells = new();
        private Dictionary<CellController, CellData> cellData = new(); // Временные данные платформ

        private bool isCompareDistance;

        public void SetStartPosition(Vector3 position)
        {
            StartPosition = position;
        }
        public void SetTargetPosition(Vector3 target)
        {
            EndPosition = target;
        }

        private void SetCurrentAndEndCells()
        {
            startCell = FindCell.GetCell(StartPosition + startRayOnCell, Vector3.down);
            endCell = FindCell.GetCell(EndPosition + startRayOnCell, Vector3.down);
            currentCell = startCell;
        }

        private void ClearData()
        {
            platformStack.Clear();
            pathToPoint.Clear();
            
            unverifiedCells.Clear();
            nearCells.Clear();
            cellData.Clear(); // Очищаем временные данные после завершения
        }

        public Queue<CellController> GetPath(bool isCompareDistance = false)
        {
            weightCell = 0;
            SetCurrentAndEndCells();
            ClearData();
            
            if (!currentCell || !endCell) return pathToPoint;

            this.isCompareDistance = isCompareDistance;
            for (int i = 0; i < maxCheck; i++)
            {
                if (currentCell.CurrentCoordinates != endCell.CurrentCoordinates)
                {
                    nearCells = GetNearPlatforms();
                    if (CheckAndAddPlatforms(ref nearCells).Count == 0)
                    {
                        if (unverifiedCells.TryPop(out var newPlatform))
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

                        currentCell = GetNextPlatform(nearCells, endCell.CurrentCoordinates);
                        AddPlatformToVerified(2);
                    }
                }
            }

            correctCellPosition = EndPosition;
            BuildPath(pathToPoint);
            
            return pathToPoint;
        }

        private void AddPlatformToVerified(int weight)
        {
            if (!cellData.ContainsKey(currentCell))
                cellData[currentCell] = new CellData();

            var data = cellData[currentCell];
            if (data.Weight > 0) return;

            data.Weight = weightCell += weight;

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

            for (int i = 0; i < maxCheck; i++)
            {
                if (lastCorrectCell.CurrentCoordinates == startCell.CurrentCoordinates) break;
                
                var correctPlatform = GetCorrectPlatform();
                if (!correctPlatform) break;

                lastCorrectCell = correctPlatform;
                platformStack.Push(correctPlatform);
            }

            while (platformStack.Count > 0)
            {
                var platform = platformStack.Pop();
                path.Enqueue(platform);
            }
            
            if(path.Count == 0) return;
            
            path.Peek().ResetColor();
            path.Dequeue();
        }

        private CellController GetCorrectPlatform()
        {
            correctCell = null;
            var lastWeight = weightCell;
            correctCellPosition += startRayPositionCell;
            currentCellScale = currentCell.transform.localScale;
            
            foreach (var direction in rayDirectionsCell)
            {
                rayLenght = (Mathf.Abs(Vector3.Dot(direction, currentCellScale)) / 2f) + 5f;
                
                if (!Physics.Raycast(correctCellPosition, direction, out hitResult, rayLenght, Layers.CELL_LAYER) ||
                    !hitResult.transform.TryGetComponent(out CellController cell) ||
                    cell == lastCorrectCell) continue;

                if (cell.CurrentCoordinates == startCell.CurrentCoordinates)
                {
                    correctCell = cell;
                    break;
                }
                
                if (cellData.TryGetValue(cell, out var data) &&
                         data.Weight < lastWeight)
                {
                    correctCell = cell;
                    lastWeight = data.Weight;
                }
            }

            if (correctCell)
                correctCellPosition = correctCell.transform.position;

            return correctCell;
        }

        private List<CellController> GetNearPlatforms()
        {
            var cells = new List<CellController>(4);
            var origin = currentCell.transform.position + startRayPositionCell;
            currentCellScale = currentCell.transform.localScale;
            
            foreach (var direction in rayDirectionsCell)
            {
                rayLenght = (Mathf.Abs(Vector3.Dot(direction, currentCellScale)) / 2f)  + 5f;
                Debug.DrawRay(origin, direction * rayLenght, Color.red, 2);
                if (Physics.Raycast(origin, direction, out hitResult, rayLenght, Layers.CELL_LAYER) &&
                    hitResult.transform.TryGetComponent(out CellController cell))
                {
                    cells.Add(cell);
                }
            }

            return cells;
        }

        private List<CellController> CheckAndAddPlatforms(ref List<CellController> platforms)
        {
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if ((cellData.TryGetValue(platforms[i], out var data) && data.Weight > 0) || platforms[i].IsBlocked())
                {
                    if (platforms[i].CurrentCoordinates == endCell.CurrentCoordinates)
                    {
                        unverifiedCells.Push(platforms[i]);
                        return platforms;
                    }
                    
                    platforms.RemoveAt(i);
                }
                else
                {
                    unverifiedCells.Push(platforms[i]);
                    //platforms[i].SetColor(Color.blue);
                }
            }

            return platforms;
        }

        private CellController GetNextPlatform(List<CellController> nearAccessiblePlatforms, Vector2Int endCoordinates)
        {
            if (nearAccessiblePlatforms.Count == 0) return currentCell;

            bestCell = nearAccessiblePlatforms[0];
            int bestDistance = CalculateDistance(bestCell, endCoordinates);

            if(isCompareDistance)
                bestDistance = NormalDistance(bestCell.transform.position, endCell.transform.position);

            foreach (var cell in nearAccessiblePlatforms)
            {
                int distance = CalculateDistance(cell, endCoordinates);
                if (!isCompareDistance && distance < bestDistance)
                {
                    bestCell = cell;
                    bestDistance = distance;
                }
                else if (distance == bestDistance)
                {
                    var firstDistance = NormalDistance(bestCell.transform.position, endCell.transform.position);
                    var secondDistance = NormalDistance(cell.transform.position, endCell.transform.position);
                    if (secondDistance < firstDistance)
                    {
                        bestCell = cell;
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
            pathFinding.EndPosition = end;
            return this;
        }

        public PathFinding Build()
        {
            return pathFinding;
        }
    }
}
