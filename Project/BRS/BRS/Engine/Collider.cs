// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    public abstract class Collider : Component  {
        ////////// generic collider that provides functions //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private

        //reference


        // --------------------- BASE METHODS ------------------
        
        public override void Start() {
            allcolliders.Add(this); //TODO implement when destroyed to be removed from here
        }

        public override void Update() {

        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        public abstract float? Intersect(Ray ray);


        public abstract bool Contains(BoundingSphere other);
        // queries
        public bool Intersects(Ray ray) { return Intersect(ray) != null; }


        // other
        //STATIC
        public static List<Collider> allcolliders = new List<Collider>();

    }

    class BoxCollider : Collider {
        BoundingBox box;

        public override bool Contains(BoundingSphere other) {
            throw new System.NotImplementedException();
        }

        public override float? Intersect(Ray ray) {
            return ray.Intersects(box);
        }
    }

    class SphereCollider : Collider {
        BoundingSphere sphere;

        //it is not called when copied

        public SphereCollider() {
            sphere = gameObject.mesh.BoundingSphere;
            //Collider.allcolliders.Add(this);
        }

        public SphereCollider(Vector3 center, float radius) {
            sphere = new BoundingSphere(center, radius);
            //Collider.allcolliders.Add(this);
        }

        public float radius {
            get { return sphere.Radius; }
            set { sphere.Radius = value; }
        }

        public override float? Intersect(Ray ray) {
            return ray.Intersects(sphere.Transform(gameObject.transform.World));
        }

        public override bool Contains(BoundingSphere other) {
            ContainmentType result = sphere.Transform(gameObject.transform.World).Contains(other);
            return  result!=ContainmentType.Disjoint;
        }
    }

    class PlaneCollider : Collider {
        Plane plane;

        public PlaneCollider(Plane _plane) {
            plane = _plane;
            //Collider.allcolliders.Add(this);
        }

        public PlaneCollider(Vector3 n, float d) {
            plane = new Plane(n, d);
            //Collider.allcolliders.Add(this);
        }

        public override bool Contains(BoundingSphere other) {
            throw new System.NotImplementedException();
        }

        public override float? Intersect(Ray ray) {
            return ray.Intersects(plane);
        }
    }

}