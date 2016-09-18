namespace SharpGL.SceneGraph.Raytracing
{
    /// <summary>
    /// An intersection.
    /// </summary>
    public class Intersection
    {
        /// <summary>
        /// Is it intersected?
        /// </summary>
        public bool intersected = false;

        /// <summary>
        /// The normal.
        /// </summary>
        public Vertex normal = new Vertex();

        /// <summary>
        /// The point.
        /// </summary>
        public Vertex point = new Vertex();

        /// <summary>
        /// The closeness.
        /// </summary>
        public float closeness = -1;
    }
}