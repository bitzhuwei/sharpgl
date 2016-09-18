namespace SharpGL.SceneGraph
{
    /// <summary>
    /// IDeepCloneable objects can create a deep clone of themselves.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepCloneable<T>
    {
        /// <summary>
        /// Creates a deep clones of this instance.
        /// </summary>
        /// <returns>A deep clone of this instance.</returns>
        T DeepClone();
    }
}