﻿using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(CanvasRenderer)), ExecuteInEditMode]
    public class PolygonalImage2D : UnityEngine.UI.Image
    {
        public Vector2[] points_;

        private static Vector2 calcUV(RectTransform rectTrans, Sprite sprite, Vector2 position)
        {
            Rect rect = rectTrans.rect;
            Vector2 uv = new Vector2(position.x - rect.xMin, position.y-rect.yMin);
            if(1.0e-4f < rect.width) {
                uv.x /= rect.width;
            }
            if(1.0e-4f < rect.height) {
                uv.y /= rect.height;
            }

            return uv;
        }

        public void resetPointsSmaller()
        {
            RectTransform rectTrans = GetComponent<RectTransform>();
            Rect rect = rectTrans.rect;

            points_ = new Vector2[4];
            float w = rect.width * 0.05f;
            float h = rect.height * 0.05f;

            points_[0] = new Vector2(rect.xMin+w, rect.yMax-h);
            points_[1] = new Vector2(rect.xMax-w, rect.yMax-h);
            points_[2] = new Vector2(rect.xMax-w, rect.yMin+h);
            points_[3] = new Vector2(rect.xMin+w, rect.yMin+h);
        }

        public void resetPoints()
        {
            RectTransform rectTrans = GetComponent<RectTransform>();
            points_ = new Vector2[4];
            points_[0] = new Vector2(rectTrans.rect.xMin, rectTrans.rect.yMax);
            points_[1] = new Vector2(rectTrans.rect.xMax, rectTrans.rect.yMax);
            points_[2] = new Vector2(rectTrans.rect.xMax, rectTrans.rect.yMin);
            points_[3] = new Vector2(rectTrans.rect.xMin, rectTrans.rect.yMin);
        }

        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper toFill)
        {
            if(null == sprite) {
                return;
            }
            if(null == points_) {
                resetPoints();
            }

            if(points_.Length < 3) {
                return;
            }

            int numPoints = points_.Length;
            RectTransform rectTrans = GetComponent<RectTransform>();
            UIVertex vertex = new UIVertex();

            Vector2 isize;
            isize.x = (1.0e-4f < sprite.texture.width) ? 1.0f / sprite.texture.width : 1.0f;
            isize.y = (1.0e-4f < sprite.texture.height) ? 1.0f / sprite.texture.height : 1.0f;

            Vector2 localOffset;
            localOffset.x = rectTrans.pivot.x * rectTrans.rect.width;
            localOffset.y = rectTrans.pivot.y * rectTrans.rect.height;

            Vector2 localToSprite;
            localToSprite.x = sprite.rect.width/rectTrans.rect.width;
            localToSprite.y = sprite.rect.height/rectTrans.rect.height;
            Vector2 local;

            toFill.Clear();
            if(sprite.packed) {
                //Debug.Log("sprite.textureRectOffset " + sprite.textureRectOffset);
                //Debug.Log("sprite.textureRect " + sprite.textureRect);
                //Debug.Log("sprite.rect " + sprite.rect);
                //Debug.Log("sprite.pivot " + sprite.pivot);

                //Debug.Log("RectTransform.rect " + rectTrans.rect);
                //Debug.Log("RectTransform.pivot " + rectTrans.pivot);

                for(int i = 0; i < numPoints; ++i) {
                    vertex.position = points_[i];
                    vertex.color = this.color;

                    local = points_[i] + localOffset;
                    local.x *= localToSprite.x;
                    local.y *= localToSprite.y;

                    vertex.uv0.x = (local.x + sprite.textureRect.xMin) * isize.x;
                    vertex.uv0.y = (local.y + sprite.textureRect.yMin) * isize.y;

                    //Debug.Log(vertex.uv0);
                    toFill.AddVert(vertex);
                }

            } else {
                for(int i = 0; i < numPoints; ++i) {
                    vertex.position = points_[i];
                    vertex.color = this.color;

                    local = points_[i] + localOffset;
                    local.x *= localToSprite.x;
                    local.y *= localToSprite.y;

                    vertex.uv0.x = (local.x + sprite.textureRect.xMin) * isize.x;
                    vertex.uv0.y = (local.y + sprite.textureRect.yMin) * isize.y;

                    toFill.AddVert(vertex);
                }
            }

            for(int i = 2, tri = 0; i < numPoints; ++i, tri += 3) {
                toFill.AddTriangle(0, i-1, i);
            }
        }
    }
}
