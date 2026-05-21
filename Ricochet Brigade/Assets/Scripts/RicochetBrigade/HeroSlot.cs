using UnityEngine;

namespace RicochetBrigade
{
    public sealed class HeroSlot : MonoBehaviour
    {
        public int slotId;
        public string platformId;
        public int platformLocalIndex;

        public V2HeroUnit Occupant { get; private set; }

        public bool IsEmpty
        {
            get { return Occupant == null; }
        }

        public Vector2 WorldPosition
        {
            get { return transform.position; }
        }

        public void SetOccupant(V2HeroUnit hero)
        {
            Occupant = hero;
            UpdateVisual();
        }

        public void ClearOccupant(V2HeroUnit hero)
        {
            if (Occupant == hero)
            {
                Occupant = null;
                UpdateVisual();
            }
        }

        public bool IsAdjacentTo(HeroSlot other)
        {
            if (other == null || other == this || platformId != other.platformId)
            {
                return false;
            }

            return Vector2.Distance(WorldPosition, other.WorldPosition) <= 1.25f;
        }

        public void UpdateVisual()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                return;
            }

            renderer.color = IsEmpty ? new Color(0.9f, 0.94f, 1f, 0.28f) : new Color(0.05f, 0.06f, 0.08f, 0.2f);
        }
    }
}
