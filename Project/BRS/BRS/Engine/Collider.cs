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

        // queries
        public abstract float? Intersect(Ray ray);
        public abstract bool Contains(BoundingSphere other);
        public abstract float radius { get; set; }

        public bool Intersects(Ray ray) { return Intersect(ray) != null; }
        public bool isStatic { get { return transform.isStatic; } }
        // other
        //STATIC
        public static List<Collider> allcolliders = new List<Collider>();
    }

    class BoxCollider : Collider {
        BoundingBox box;
        float radius_cashed;



        //CONSTRUCTORS
        public BoxCollider(GameObject o) {
            //TODO compute more accurately
            box = new BoundingBox(-o.transform.scale / 2, o.transform.scale / 2);
        }

        public BoxCollider(Vector3 center, Vector3 size) {
            box = new BoundingBox(center - size / 2, center + size / 2);
        }

        public override bool Contains(BoundingSphere other) {
            throw new System.NotImplementedException();
        }

        public override float? Intersect(Ray ray) {
            return ray.Intersects(box);
        }
        public override float radius {
            get { if(radius_cashed==0) radius_cashed = Vector3.Distance(box.Max, box.Min)/2; return radius_cashed; }
            set { radius_cashed = value; }
        }
    }

    class SphereCollider : Collider {
        BoundingSphere sphere;

        //it is not called when copied

        public SphereCollider(GameObject o) {
            sphere = o.mesh.BoundingSphere;
            //Collider.allcolliders.Add(this);
        }

        public SphereCollider(Vector3 center, float _radius) {
            sphere = new BoundingSphere(center, _radius);
            //Collider.allcolliders.Add(this);
        }

        public override float radius {
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

        public override float radius { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override bool Contains(BoundingSphere other) {
            throw new System.NotImplementedException();
        }

        public override float? Intersect(Ray ray) {
            return ray.Intersects(plane);
        }
    }

}