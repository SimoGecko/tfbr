// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BRS {
    public abstract class Collider : Component  {
        ////////// generic collider that provides intersection queries //////////

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
        public abstract bool Intersects(Ray ray, out float hit);
        public abstract bool Intersects(Collider other);
        public abstract float Radius { get; set; }

        //public bool isStatic { get { return transform.isStatic; } }
        public bool isStatic = true;

        protected Transform Transf { get { if (gameObject != null) return gameObject.transform; else return Transform.Identity; } }
        // other
        //STATIC
        public static List<Collider> allcolliders = new List<Collider>();
    }

    class BoxCollider : Collider { // it is axis-aligned, cannot be rotated
        BoundingBox box;
        public BoundingBox worldBox { get { return new BoundingBox(Transf.position - Transf.scale/2, Transf.position + Transf.scale / 2); } }
        //public BoundingBox worldBox { get { return box; } }
        float radius_cashed;

        //CONSTRUCTORS
        public BoxCollider(GameObject o, bool _static = true) {
            //TODO compute more accurately (using mesh)
            box = new BoundingBox(-o.transform.scale / 2, o.transform.scale / 2);
            isStatic = _static;
        }

        public BoxCollider(Vector3 center, Vector3 size, bool _static = true) {
            box = new BoundingBox(center - size / 2, center + size / 2);
            isStatic = _static;
        }

        //queries
        public override bool Intersects(Collider other) {
            ContainmentType result = ContainmentType.Disjoint;
            if (other is BoxCollider) {
                result = worldBox.Contains(((BoxCollider)other).worldBox);
            }
            else if(other is SphereCollider) {
                result = worldBox.Contains(((SphereCollider)other).worldSphere);
            }
            return result != ContainmentType.Disjoint;
        }
        
        public override bool Intersects(Ray ray, out float t) {
            t = -1;
            float? result = ray.Intersects(worldBox); 
            if (result == null) return false;
            else {
                t = (float)result;
                return true;
            }
        }

        public override float Radius {
            get { if(radius_cashed==0) radius_cashed = Vector3.Distance(box.Max, box.Min)/2; return radius_cashed; }
            set { radius_cashed = value; }
        }
    }

    class SphereCollider : Collider {
        BoundingSphere sphere;
        public BoundingSphere worldSphere { get { return sphere.Transform(Transf.World); } }
        //public BoundingSphere worldSphere { get { return sphere; } }

        //it is not called when copied (?)

        //CONSTRUCTOR
        public SphereCollider(GameObject o, bool _static = true) {
            sphere = o.mesh.BoundingSphere;
            isStatic = _static;
        }

        public SphereCollider(Vector3 center, float _radius, bool _static = true) {
            sphere = new BoundingSphere(center, _radius);
            isStatic = _static;
        }

        //QUERIES


        public override bool Intersects(Ray ray, out float t) {
            t = -1;
            float? result = ray.Intersects(worldSphere);
            if (result == null) return false;
            else {
                t = (float)result;
                return true;
            }
        }

        public override bool Intersects(Collider other) {
            ContainmentType result = ContainmentType.Disjoint;
            if (other is BoxCollider) {
                result = worldSphere.Contains(((BoxCollider)other).worldBox);
            } else if (other is SphereCollider) {
                result = worldSphere.Contains(((SphereCollider)other).worldSphere);
            }
            return  result != ContainmentType.Disjoint;
        }


        public override float Radius {
            get { return sphere.Radius; }
            set { sphere.Radius = value; }
        }
    }

    /*
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
    }*/

}