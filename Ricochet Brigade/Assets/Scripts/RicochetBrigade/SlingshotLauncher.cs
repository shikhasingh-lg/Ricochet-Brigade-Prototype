using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RicochetBrigade
{
    public sealed class SlingshotLauncher : MonoBehaviour
    {
        public ArenaGrid grid;
        public int previewRicochets = 2;
        public float maxPullDistance = 2.6f;

        private HeroPuck loadedHero;
        private HeroPuck activeFlyingHero;
        private BrigadeColor loadedColor;
        private LineRenderer previewLine;
        private Camera mainCamera;
        private bool isDragging;
        private Vector2 dragWorld;

        private void Start()
        {
            if (grid == null)
            {
                grid = ArenaGrid.Instance != null ? ArenaGrid.Instance : FindFirstObjectByType<ArenaGrid>();
            }

            mainCamera = Camera.main;
            CreatePreviewLine();
            LoadNextHero();
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (activeFlyingHero != null && !activeFlyingHero.IsFlying)
            {
                activeFlyingHero = null;
                LoadNextHero();
            }

            if (loadedHero == null && activeFlyingHero == null)
            {
                LoadNextHero();
            }

            if (grid == null || loadedHero == null || activeFlyingHero != null || Pointer.current == null)
            {
                HidePreview();
                return;
            }

            HandlePointer();
        }

        public string StatusText()
        {
            if (loadedHero == null && activeFlyingHero != null)
            {
                return "Hero flying...";
            }

            return "Loaded: " + BrigadeTuning.ColorName(loadedColor);
        }

        private void HandlePointer()
        {
            if (Pointer.current.press.wasPressedThisFrame)
            {
                isDragging = true;
                UpdateDragPosition();
            }

            if (isDragging && Pointer.current.press.isPressed)
            {
                UpdateDragPosition();
                PositionLoadedHeroForPull();
                DrawPreview();
            }

            if (!isDragging || !Pointer.current.press.wasReleasedThisFrame)
            {
                return;
            }

            Vector2 pull = dragWorld - grid.BaseCenter();
            isDragging = false;

            if (pull.magnitude <= 0.05f)
            {
                loadedHero.transform.position = grid.BaseCenter();
                HidePreview();
                return;
            }

            Vector2 fireDirection = -pull.normalized;
            float powerPercent = Mathf.Clamp01(pull.magnitude / maxPullDistance);
            float speed = Mathf.Lerp(BrigadeTuning.MinLaunchSpeed, BrigadeTuning.MaxLaunchSpeed, powerPercent);
            HeroPuck firedHero = loadedHero;
            loadedHero = null;
            activeFlyingHero = firedHero;
            HidePreview();
            firedHero.transform.position = grid.BaseCenter();
            firedHero.Launch(fireDirection * speed);
        }

        private void UpdateDragPosition()
        {
            Vector2 rawWorld = ScreenToWorld(Pointer.current.position.ReadValue());
            Vector2 pull = rawWorld - grid.BaseCenter();
            if (pull.magnitude > maxPullDistance)
            {
                pull = pull.normalized * maxPullDistance;
            }

            dragWorld = grid.BaseCenter() + pull;
        }

        private void PositionLoadedHeroForPull()
        {
            loadedHero.transform.position = dragWorld;
        }

        private Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            if (mainCamera == null)
            {
                return Vector2.zero;
            }

            Vector3 screen = new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z);
            return mainCamera.ScreenToWorldPoint(screen);
        }

        private void LoadNextHero()
        {
            if (grid == null || loadedHero != null || activeFlyingHero != null)
            {
                return;
            }

            loadedColor = BrigadeTuning.RandomHeroColor();
            GameObject heroObject = new GameObject("Loaded " + loadedColor + " Hero");
            heroObject.transform.position = grid.BaseCenter();
            loadedHero = heroObject.AddComponent<HeroPuck>();
            loadedHero.PrepareLoaded(loadedColor);
        }

        private void CreatePreviewLine()
        {
            GameObject previewObject = new GameObject("Aim Preview");
            previewObject.transform.SetParent(transform, false);
            previewLine = previewObject.AddComponent<LineRenderer>();
            previewLine.useWorldSpace = true;
            previewLine.widthMultiplier = 0.045f;
            previewLine.positionCount = 0;
            previewLine.sortingOrder = 30;
            previewLine.material = new Material(Shader.Find("Sprites/Default"));
            previewLine.startColor = new Color(1f, 1f, 1f, 0.85f);
            previewLine.endColor = new Color(1f, 1f, 1f, 0.18f);
        }

        private void DrawPreview()
        {
            Vector2 pull = dragWorld - grid.BaseCenter();
            if (pull.magnitude <= 0.05f)
            {
                HidePreview();
                return;
            }

            List<Vector3> points = BuildPreviewPoints(grid.BaseCenter(), -pull.normalized);
            previewLine.positionCount = points.Count;
            previewLine.SetPositions(points.ToArray());
        }

        private List<Vector3> BuildPreviewPoints(Vector2 start, Vector2 direction)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(start.x, start.y, -0.2f));
            Rect bounds = grid.WorldBounds;
            Vector2 current = start;
            Vector2 currentDirection = direction.normalized;

            for (int bounceIndex = 0; bounceIndex <= previewRicochets; bounceIndex++)
            {
                float distanceToVertical = float.MaxValue;
                float distanceToHorizontal = float.MaxValue;

                if (currentDirection.x > 0.001f)
                {
                    distanceToVertical = (bounds.xMax - current.x) / currentDirection.x;
                }
                else if (currentDirection.x < -0.001f)
                {
                    distanceToVertical = (bounds.xMin - current.x) / currentDirection.x;
                }

                if (currentDirection.y > 0.001f)
                {
                    distanceToHorizontal = (bounds.yMax - current.y) / currentDirection.y;
                }
                else if (currentDirection.y < -0.001f)
                {
                    distanceToHorizontal = (bounds.yMin - current.y) / currentDirection.y;
                }

                bool hitVertical = distanceToVertical < distanceToHorizontal;
                float distance = Mathf.Max(0.1f, Mathf.Min(distanceToVertical, distanceToHorizontal));
                current += currentDirection * distance;
                points.Add(new Vector3(current.x, current.y, -0.2f));

                if (hitVertical)
                {
                    currentDirection.x *= -1f;
                }
                else
                {
                    currentDirection.y *= -1f;
                }
            }

            return points;
        }

        private void HidePreview()
        {
            if (previewLine != null)
            {
                previewLine.positionCount = 0;
            }
        }
    }
}
