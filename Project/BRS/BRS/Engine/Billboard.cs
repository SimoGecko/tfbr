// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB


namespace BRS.Engine {
    class Billboard : Component {
        ////////// simple billboard that looks at the camera //////////

        // --------------------- VARIABLES ---------------------

        //public


        //private


        //reference
        private readonly Transform _parent;
        public static Billboard Instance;

        // --------------------- BASE METHODS ------------------
        public Billboard(Transform t) {
            _parent = t;
            Instance = this;
        }

        public override void Start() {

        }

        public override void Update() {
            transform.position = _parent.position;

            transform.LookAt(Camera.Main.transform.position);
            //Quaternion extraRot = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(90));
            //transform.localRotation* extraRot;
            
        }



        // --------------------- CUSTOM METHODS ----------------


        // commands
        //public void Draw() {
        //    //Vector2 position = Camera.main.WorldToScreenPoint(transform.position);
        //    //Debug.Log("pos=" + position);
        //    //UserInterface.instance.DrawString(position, "OOO");
        //}


        // queries



        // other

    }

}