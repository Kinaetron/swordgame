using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SwordGame
{
    public struct LiangBarsky
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private Vector2 intersectPointEnd;

        // Physics state
        public Vector2 IntersectPointStart
        {
            get { return intersectPointStart; }
            set { intersectPointStart = value; }
        }
        Vector2 intersectPointStart;

        // Physics state
        public float IntersectDistance
        {
            get { return intersectDistance; }
            set { intersectDistance = value; }
        }
        float intersectDistance;

        public LiangBarsky(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            intersectPointStart = Vector2.Zero;
            intersectPointEnd = Vector2.Zero;
            intersectDistance = 0.0f;
        }

        public bool Intersect(Rectangle rectangle)
        {
            float t0 = 0.0f; float t1 = 1.0f;
            float xdelta = endPos.X - startPos.X;
            float ydelta = endPos.Y - startPos.Y;
            float p = 0;
            float q = 0;
            float r = 0;

            for (int edge = 0; edge < 4; edge++)
            {   // Traverse through left, right, bottom, top edges.
                if (edge == 0) { p = -xdelta; q = -(rectangle.Left - startPos.X); }
                if (edge == 1) { p = xdelta; q = (rectangle.Right - startPos.X); }
                if (edge == 2) { p = -ydelta; q = -(rectangle.Top - startPos.Y); }
                if (edge == 3) { p = ydelta; q = (rectangle.Bottom - startPos.Y); }
                r = q / p;
                if (p == 0 && q < 0) return false;   // Don't draw line at all. (parallel line outside)

                if (p < 0)
                {
                    if (r > t1) return false;         // Don't draw line at all.
                    else if (r > t0) t0 = r;            // Line is clipped!
                }
                else if (p > 0)
                {
                    if (r < t0) return false;      // Don't draw line at all.
                    else if (r < t1) t1 = r;         // Line is clipped!
                }
            }

            intersectPointStart = new Vector2(startPos.X + t0 * xdelta, startPos.Y + t0 * ydelta);
            intersectPointEnd = new Vector2(startPos.X + t1 * xdelta, startPos.Y + t1 * ydelta);

            intersectDistance = Vector2.Distance(intersectPointStart, intersectPointEnd);

            return true;        // (clipped) line is drawn
        }
    }
}
