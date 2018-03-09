// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    static class Physics {//PHYSICS METHODS
        public static bool Intersects(Ray ray, Collider collider, out float dist) {
            float? t = collider.Intersect(ray);//ray.Intersects(collider);
            if (t != null) {
                dist = (float)t;
                return true;
            }
            dist = -1f;
            return false;
        }/*
        public static bool Intersects(Ray ray, BoundingBox bbox, out float dist) {
            float? t = ray.Intersects(bbox);
            if (t != null) { dist = (float)t; return true; }
            dist = -1f;
            return false;
        }
        public static bool Intersects(Ray ray, BoundingSphere bsphere, out float dist) {
            float? t = ray.Intersects(bsphere);
            if (t != null) { dist = (float)t; return true; }
            dist = -1f;
            return false;
        }*/
        internal static Collider[] OverlapSphere(Vector3 position, float radius, string collisionmask) {
            List<Collider> result = new List<Collider>();
            BoundingSphere sphere = new BoundingSphere(position, radius);
            //find all the colliders that intersect this sphere
            foreach (Collider c in Collider.allcolliders) { // TODO implement more efficient method (prune eg Octree)
                if (c.gameObject.tag != collisionmask || !c.gameObject.active) continue;
                if (c.Contains(sphere)) result.Add(c);
            }
            return result.ToArray();
        }

        public static bool Raycast(Ray ray, out RaycastHit hit, float tmax, string collisionmask) {
            float tmin = tmax;
            bool collided = false;
            Collider bestsofar = null;
            foreach (Collider c in Collider.allcolliders) { // TODO implement more efficient method (prune eg Octree)
                if (c.gameObject.tag != collisionmask || !c.gameObject.active) continue;

                float? p = c.Intersect(ray);
                if (p!=null && (float)p<=tmin) {
                    //Debug.Log("inters " + c.gameObject.name);
                    collided = true;
                    tmin = (float)p;
                    bestsofar = c;
                }
            }

            if (collided){ 
                hit.collider = bestsofar;
                hit.distance = tmin;
                hit.point = ray.GetPoint(tmin);
                return true;
            }
            hit.collider = null;
            hit.distance = -1;
            hit.point = Vector3.Zero;
            return false;
        }
    }

    public struct RaycastHit {
        public Collider collider;
        public float distance;
        public Vector3 point;
    }
}
