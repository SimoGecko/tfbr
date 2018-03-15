// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    static class Physics {
        ////////// class that provides physics methods as well as checks for OnCollisionEnter //////////

        static List<string> previousCollisions = new List<string>();

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

        //called each frame to correctly callback gameObject.OnCollisionEnter()
        public static void CheckOnCollisionEnter() {
            List<string> currentCollisions = new List<string>();

            List<Collider> staticCollider = new List<Collider>();
            List<Collider> dynamicCollider = new List<Collider>();
            foreach(Collider c in Collider.allcolliders) {
                if (c.isStatic) staticCollider.Add(c);
                else dynamicCollider.Add(c);
            }
            int D = dynamicCollider.Count;
            int S = staticCollider.Count;

            //do static-dynamic check
            for(int d=0; d<D; d++) {
                for(int s=0; s<S; s++) {
                    Collider colD = dynamicCollider[d];
                    Collider colS = staticCollider[s];
                    float dist2 = Vector3.DistanceSquared(colD.transform.position, colS.transform.position);
                    float rad2 = (float)Math.Pow(colD.radius +colS.radius, 2);
                    if (dist2 < rad2) {
                        //call on collision enter on both objects only if not previously colliding
                        string collisionstring = colD.gameObject.name + colS.gameObject.name;
                        bool previouslyCollided = previousCollisions.Contains(collisionstring);

                        if (!previouslyCollided) {
                            dynamicCollider[d].gameObject.OnCollisionEnter(staticCollider[s]);
                            staticCollider[s].gameObject.OnCollisionEnter(dynamicCollider[d]);
                        }
                        currentCollisions.Add(collisionstring);
                    }
                }
            }

            //do dynamic-dynamic check
            for (int d0 = 0; d0 < D; d0++) {
                for (int d1 = d0+1; d1 < D; d1++) {
                    Collider colD0 = dynamicCollider[d0];
                    Collider colD1 = dynamicCollider[d1];
                    float dist2 = Vector3.DistanceSquared(colD0.transform.position, colD1.transform.position);
                    float rad2 = (float)Math.Pow(colD0.radius + colD1.radius, 2);
                    if (dist2 < rad2) {
                        //call on collision enter on both objects only if not previously colliding
                        string collisionstring = colD0.gameObject.name + colD1.gameObject.name;
                        bool previouslyCollided = previousCollisions.Contains(collisionstring);

                        if (!previouslyCollided) {
                            dynamicCollider[d0].gameObject.OnCollisionEnter(dynamicCollider[d1]);
                            dynamicCollider[d1].gameObject.OnCollisionEnter(dynamicCollider[d0]);
                        }
                        currentCollisions.Add(collisionstring);
                    }
                }
            }

            previousCollisions = new List<string>(currentCollisions);
        }//end oncollisionenter check
    }

    public struct RaycastHit {
        public Collider collider;
        public float distance;
        public Vector3 point;
    }
}
