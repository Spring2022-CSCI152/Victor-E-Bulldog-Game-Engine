using Silk.NET.OpenGL;

namespace Bulldog.Renderer
{
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private uint _handle;
        private GL _gl;

        public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
        {
            _gl = gl;
            
            _handle = _gl.GenVertexArray();
            Bind();
            vbo.Bind();
            ebo.Bind();
        }

        /// <summary>
        /// Constructor that only creates the VAO, and passes the GL context.
        /// VBOS MUST BE BOUND MANUALLY!
        /// </summary>
        /// <param name="gl">OpenGL Context.</param>
        public VertexArrayObject(GL gl)
        {
            _gl = gl;
            
            _handle = _gl.GenVertexArray();
            Bind();
        }

        /// <summary>
        /// Defines an array of generic vertex attribute data.
        /// </summary>
        /// <param name="index">Specifies the index of the attribute.</param>
        /// <param name="size">Specifies the number of components per vertex. Must be 1, 2, 3, 4.</param>
        /// <param name="type">Specifies the data type of each component in the array.</param>
        /// <param name="stride">Specifies the offset between consecutive vertex attributes. If stride is 0, the generic vertex attributes are understood to be tightly packed in the array.</param>
        /// <param name="offSet">Specifies the offset to the first component of this vertex attribute in the data store of the buffer</param>
        public unsafe void VertexAttributePointer(uint index, int size, VertexAttribPointerType type, uint stride, int offSet)
        {
            _gl.VertexAttribPointer(index, size, type, false, (uint) (stride * sizeof(TVertexType)), (void*) (offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(_handle);
        }
    }
}