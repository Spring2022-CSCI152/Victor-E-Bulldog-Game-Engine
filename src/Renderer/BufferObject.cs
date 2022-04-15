using Silk.NET.OpenGL;

namespace Bulldog.Renderer;

    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;

        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;
            
            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint) (data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
            }
        }

        /// <summary>
        /// Creates and initializes a buffer object's data store.
        /// </summary>
        /// <param name="gl">OpenGL context.</param>
        /// <param name="bufferType">Specifies the type of buffer. Must be in the BufferTargetARB enum.</param>
        /// <param name="size">Specifies the size of the data store.</param>
        /// <param name="data">Specifies a pointer to data that will be copied into the data store.</param>
        public unsafe BufferObject(GL gl, BufferTargetARB bufferType, nuint size, Span<TDataType> data)
        {
            _gl = gl;
            _bufferType = bufferType;
            
            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, size*((nuint)sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
            }
        }

        /// <summary>
        /// Updates a subset of a buffer object's data store.
        /// </summary>
        /// <param name="offset">Specifies the offset into the buffer object's data store where data replacement will begin.</param>
        /// <param name="data">Specifies a pointer to the new data that will be copied into the data store.</param>
        public unsafe void SetSubData(nint offset, Span<TDataType> data)
        {
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferSubData(_bufferType, offset * sizeof(TDataType), (nuint) (data.Length * sizeof(TDataType)), d);
            }
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
        }
    }