using System;
using System.Collections.Generic;

namespace Domi.UpCore.Utilities
{
    public class ArrayPool<T>
    {
        private readonly int arraySize;
        private readonly Queue<T[]> pool;

        public ArrayPool(int arraySize, int initialElements = 2)
        {
            if (arraySize < 0)
            {
                throw new ArgumentException($"ArraySize {arraySize} is invalid.");
            }

            this.arraySize = arraySize;
            this.pool = new Queue<T[]>(initialElements);

            for (int i = 0; i < initialElements; i++)
            {
                this.pool.Enqueue(new T[arraySize]);
            }
        }

        public T[] Get()
        {
            lock (this.pool)
            {
                return this.pool.Count > 0 ? this.pool.Dequeue() : new T[this.arraySize];
            }
        }

        public void Return(T[] array)
        {
            if (array == null)
            {
                return;
            }

            if (array.Length != this.arraySize)
            {
                throw new ArgumentException($"Input array size {array.Length} differs from defined array size {this.arraySize}.");
            }

            lock (this.pool)
            {
                this.pool.Enqueue(array);
            }
        }
    }
}
