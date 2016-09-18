namespace SharpGL.SceneGraph.Quadrics
{
    /// <summary>
    /// The Quadric orientation.
    /// </summary>
    public enum Orientation : uint
    {
        /// <summary>
        /// Outside.
        /// </summary>
        Outside = OpenGL.GLU_OUTSIDE,

        /// <summary>
        /// Inside.
        /// </summary>
        Inside = OpenGL.GLU_INSIDE,
    }
}