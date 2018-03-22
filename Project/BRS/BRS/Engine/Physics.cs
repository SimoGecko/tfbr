// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    static class Physics {
        ////////// class that provides physics methods as well as checks for OnCollisionEnter //////////


        public static void Update() {
            CheckOnCollisionEnter();
        }

        //QUERIES
        public static Collider[] OverlapSphere(Vector3 position, float radius, string collisionmask = "") {
            List<Collider> result = new List<Collider>();
            SphereCollider sphere = new SphereCollider(position, radius);

            //find all the colliders that intersect this sphere
            foreach (Collider c in Collider.allcolliders) { // TODO implement more efficient method (prune eg Octree)
                if ( (!collisionmask.Equals("") && !c.gameObject.myTag.Equals(collisionmask)) || !c.gameObject.Active) continue;

                if (c.Intersects(sphere)) result.Add(c);
            }
            return result.ToArray();
        }

        public static bool Raycast(Ray ray, out RaycastHit hit, float tmax, string collisionmask="") {
            float tmin = tmax;
            bool collided = false;
            Collider bestsofar = null;
            foreach (Collider c in Collider.allcolliders) { // TODO implement more efficient method (prune eg Octree)
                if ( (!collisionmask.Equals("") && !c.gameObject.myTag.Equals(collisionmask)) || !c.gameObject.Active) continue;

                bool intersects = c.Intersects(ray, out float p);
                if (intersects && p <= tmin) {
                    //Debug.Log("intersected " + c.gameObject.name);
                    collided = true;
                    tmin = p;
                    bestsofar = c;
                }
            }

            if (collided){ 
                hit.collider = bestsofar;
                hit.distance = tmin;
                hit.point = ray.GetPoint(tmin);
                return true;
            } else {
                hit.collider = null;
                hit.distance = -1;
                hit.point = Vector3.Zero;
                return false;
            }
        }

        //called each frame to correctly callback gameObject.OnCollisionEnter()
        static List<string> previousCollisions = new List<string>();

        static void CheckOnCollisionEnter() {
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
                    float rad2 = (float)Math.Pow(colD.Radius +colS.Radius, 2);
                    if (dist2 < rad2) {
                        //call on collision enter on both objects only if not previously colliding
                        string collisionstring = colD.gameObject.Name + colS.gameObject.Name;
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
                    float rad2 = (float)Math.Pow(colD0.Radius + colD1.Radius, 2);
                    if (dist2 < rad2) {
                        //call on collision enter on both objects only if not previously colliding
                        string collisionstring = colD0.gameObject.Name + colD1.gameObject.Name;
                        bool previouslyCollided = previousCollisions.Contains(collisionstring);

                        if (!previouslyCollided) {
                            dynamicCollider[d0].gameObject.OnCollisionEnter(dynamicCollider[d1]);
                            dynamicCollider[d1].gameObject.OnCollisionEnter(dynamicCollider[d0]);
                        }
                        currentCollisions.Add(collisionstring);
                    }
                }
            }

            //DO concollisionExit check

            previousCollisions = new List<string>(currentCollisions);
        }//end oncollisionenter check
    }

    public struct RaycastHit {
        public Collider collider;
        public float distance;
        public Vector3 point;
    }
}
