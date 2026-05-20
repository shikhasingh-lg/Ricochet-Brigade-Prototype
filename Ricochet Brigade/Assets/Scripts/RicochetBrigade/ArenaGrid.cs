using System.Collections.Generic;
using UnityEngine;

namespace RicochetBrigade
{
    public sealed class ArenaGrid : MonoBehaviour
    {
        public static ArenaGrid Instance { get; private set; }

        [Header("Grid")]
        public int columns = BrigadeTuning.Columns;
        public int rows = BrigadeTuning.Rows;
        public float cellSize = BrigadeTuning.CellSize;

        private readonly List<Vector2Int> pathCells = new List<Vector2Int>();
        private readonly HashSet<Vector2Int> pathLookup = new HashSet<Vector2Int>();
        private readonly Dictionary<Vector2Int, HeroPuck> occupiedCells = new Dictionary<Vector2Int, HeroPuck>();
        private PhysicsMaterial2D bounceMaterial;

        public Rect WorldBounds
        {
            get
            {
                Vector2 bottomLeft = CellCenter(new Vector2Int(0, 0)) - Vector2.one * cellSize * 0.5f;
                return new Rect(bottomLeft.x, bottomLeft.y, columns * cellSize, rows * cellSize);
            }
        }

        public IReadOnlyList<Vector2Int> PathCells
        {
            get { return pathCells; }
        }

        private void Awake()
        {
            Instance = this;
            Physics2D.gravity = Vector2.zero;
            InitializeDefaultPath();
            BuildRuntimeArena();
        }

        public Vector2 BaseCenter()
        {
            return Vector2.zero;
        }

        public Vector2 CellCenter(Vector2Int cell)
        {
            float x = (cell.x - (columns - 1) * 0.5f) * cellSize;
            float y = (cell.y - (rows - 1) * 0.5f) * cellSize;
            return new Vector2(x, y);
        }

        public Vector2Int WorldToCell(Vector2 world)
        {
            int column = Mathf.RoundToInt(world.x / cellSize + (columns - 1) * 0.5f);
            int row = Mathf.RoundToInt(world.y / cellSize + (rows - 1) * 0.5f);
            return new Vector2Int(Mathf.Clamp(column, 0, columns - 1), Mathf.Clamp(row, 0, rows - 1));
        }

        public Vector2[] PathWorldPoints()
        {
            Vector2[] points = new Vector2[pathCells.Count];
            for (int i = 0; i < pathCells.Count; i++)
            {
                points[i] = CellCenter(pathCells[i]);
            }

            return points;
        }

        public bool IsInside(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < columns && cell.y >= 0 && cell.y < rows;
        }

        public bool IsPathCell(Vector2Int cell)
        {
            return pathLookup.Contains(cell);
        }

        public bool IsBaseCell(Vector2Int cell)
        {
            return (cell.x == 3 || cell.x == 4) && (cell.y == 5 || cell.y == 6);
        }

        public bool IsBaseWorld(Vector2 world)
        {
            return IsBaseCell(WorldToCell(world));
        }

        public bool IsPlatformCell(Vector2Int cell)
        {
            return IsInside(cell) && !IsPathCell(cell) && !IsBaseCell(cell);
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return occupiedCells.ContainsKey(cell);
        }

        public bool TrySettleHero(HeroPuck hero, Vector2 stopWorldPosition, out Vector2Int settledCell, out float distanceToPath)
        {
            Release(hero);

            if (!TryFindNearestEmptyPlatform(stopWorldPosition, out settledCell, out distanceToPath))
            {
                return false;
            }

            occupiedCells[settledCell] = hero;
            hero.AssignedCell = settledCell;
            hero.transform.position = CellCenter(settledCell);
            return true;
        }

        public void Release(HeroPuck hero)
        {
            if (hero == null || hero.AssignedCell.x < 0)
            {
                return;
            }

            if (occupiedCells.TryGetValue(hero.AssignedCell, out HeroPuck occupant) && occupant == hero)
            {
                occupiedCells.Remove(hero.AssignedCell);
            }

            hero.AssignedCell = new Vector2Int(-1, -1);
        }

        public float DistanceToNearestPathCell(Vector2Int cell)
        {
            float best = float.MaxValue;
            Vector2 world = CellCenter(cell);

            for (int i = 0; i < pathCells.Count; i++)
            {
                float distance = Vector2.Distance(world, CellCenter(pathCells[i])) / cellSize;
                if (distance < best)
                {
                    best = distance;
                }
            }

            return best;
        }

        private bool TryFindNearestEmptyPlatform(Vector2 world, out Vector2Int bestCell, out float bestDistanceToPath)
        {
            bestCell = new Vector2Int(-1, -1);
            bestDistanceToPath = float.MaxValue;
            float bestDistance = float.MaxValue;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (!IsPlatformCell(cell) || occupiedCells.ContainsKey(cell))
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(world, CellCenter(cell));
                    float distanceToPath = DistanceToNearestPathCell(cell);
                    bool closer = distance < bestDistance - 0.001f;
                    bool tieCloserToPath = Mathf.Abs(distance - bestDistance) <= 0.001f && distanceToPath < bestDistanceToPath;

                    if (closer || tieCloserToPath)
                    {
                        bestDistance = distance;
                        bestDistanceToPath = distanceToPath;
                        bestCell = cell;
                    }
                }
            }

            return bestCell.x >= 0;
        }

        private void InitializeDefaultPath()
        {
            pathCells.Clear();
            pathLookup.Clear();

            AddPathCell(0, 11);
            AddPathCell(0, 10);
            AddPathCell(1, 10);
            AddPathCell(2, 10);
            AddPathCell(3, 10);
            AddPathCell(4, 10);
            AddPathCell(5, 10);
            AddPathCell(6, 10);
            AddPathCell(6, 9);
            AddPathCell(6, 8);
            AddPathCell(5, 8);
            AddPathCell(4, 8);
            AddPathCell(3, 8);
            AddPathCell(2, 8);
            AddPathCell(1, 8);
            AddPathCell(1, 7);
            AddPathCell(1, 6);
            AddPathCell(1, 5);
            AddPathCell(1, 4);
            AddPathCell(2, 4);
            AddPathCell(3, 4);
            AddPathCell(4, 4);
            AddPathCell(5, 4);
            AddPathCell(6, 4);
            AddPathCell(6, 5);
            AddPathCell(6, 6);
            AddPathCell(5, 6);
            AddPathCell(5, 5);
        }

        private void AddPathCell(int column, int row)
        {
            Vector2Int cell = new Vector2Int(column, row);
            pathCells.Add(cell);
            pathLookup.Add(cell);
        }

        private void BuildRuntimeArena()
        {
            ClearRuntimeChildren();
            bounceMaterial = new PhysicsMaterial2D("Greybox Perfect Bounce")
            {
                bounciness = 1f,
                friction = 0f
            };

            GameObject visuals = new GameObject("Arena Visuals (Runtime)");
            visuals.transform.SetParent(transform, false);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    CreateTile(visuals.transform, cell);
                }
            }

            CreatePathLine(visuals.transform);
            CreateWall("wall_left", new Vector2(WorldBounds.xMin - 0.15f, 0f), new Vector2(0.3f, WorldBounds.height + 0.6f));
            CreateWall("wall_right", new Vector2(WorldBounds.xMax + 0.15f, 0f), new Vector2(0.3f, WorldBounds.height + 0.6f));
            CreateWall("wall_bottom", new Vector2(0f, WorldBounds.yMin - 0.15f), new Vector2(WorldBounds.width + 0.6f, 0.3f));
            CreateWall("wall_top", new Vector2(0f, WorldBounds.yMax + 0.15f), new Vector2(WorldBounds.width + 0.6f, 0.3f));
        }

        private void ClearRuntimeChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (!child.name.Contains("(Runtime)"))
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void CreateTile(Transform parent, Vector2Int cell)
        {
            GameObject tile = new GameObject("Cell " + cell.x + "," + cell.y);
            tile.transform.SetParent(parent, false);
            tile.transform.position = CellCenter(cell);
            tile.transform.localScale = Vector3.one * (cellSize * 0.94f);

            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Square;
            renderer.sortingOrder = -10;

            if (IsBaseCell(cell))
            {
                renderer.color = new Color(0.12f, 0.14f, 0.17f);
            }
            else if (IsPathCell(cell))
            {
                renderer.color = new Color(0.18f, 0.13f, 0.1f);
            }
            else
            {
                float checker = (cell.x + cell.y) % 2 == 0 ? 0.33f : 0.38f;
                renderer.color = new Color(checker, checker + 0.05f, checker + 0.07f);
            }
        }

        private void CreatePathLine(Transform parent)
        {
            GameObject path = new GameObject("Path Direction (Runtime)");
            path.transform.SetParent(parent, false);

            LineRenderer line = path.AddComponent<LineRenderer>();
            line.positionCount = pathCells.Count;
            line.useWorldSpace = true;
            line.widthMultiplier = 0.08f;
            line.sortingOrder = -5;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(1f, 0.7f, 0.2f, 0.9f);
            line.endColor = new Color(1f, 0.36f, 0.14f, 0.9f);

            for (int i = 0; i < pathCells.Count; i++)
            {
                Vector2 point = CellCenter(pathCells[i]);
                line.SetPosition(i, new Vector3(point.x, point.y, -0.02f));
            }
        }

        private void CreateWall(string name, Vector2 position, Vector2 size)
        {
            GameObject wall = new GameObject(name + " (Runtime)");
            wall.transform.SetParent(transform, false);
            wall.transform.position = position;

            SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
            renderer.sprite = GreyboxSprites.Square;
            renderer.color = new Color(0.08f, 0.1f, 0.13f);
            renderer.sortingOrder = -4;
            wall.transform.localScale = new Vector3(size.x, size.y, 1f);

            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.sharedMaterial = bounceMaterial;

            RicochetWall ricochetWall = wall.AddComponent<RicochetWall>();
            ricochetWall.surfaceName = name;
        }
    }
}
