using System;
using System.Linq;
using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;

namespace FaceGenerator.MachineLearning.Math
{
    public class Vector
    {
        private readonly double[] _elements;

        public int Size { get; }

        public double Length => System.Math.Sqrt(_elements.Select(e => e * e).Sum());

        public double this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public Vector(double[] elements)
        {
            _elements = elements;
            Size = _elements.Length;
        }

        public static Vector operator *(Vector v, double multiplier)
        {
            ThrowIfNull(v);

            var newCoords = new double[v.Size];
            for (int i = 0; i < v.Size; i++)
            {
                newCoords[i] = v[i] * multiplier;
            }

            return new Vector(newCoords);
        }

        public static double operator *(Vector v1, Vector v2)
        {
            ThrowIfAllNull(v1, v2);
            ThrowIfNull(v1);
            ThrowIfNull(v2);

            var v1Size = v1.Size;
            var v2Size = v2.Size;

            if (v1Size != v2Size)
            {
                throw new InvalidOperationException(
                    $"Dot product takes vectors with same sizes ({v1Size} and {v2Size} were given)");
            }

            var result = 0d;

            for (int i = 0; i < v1Size; i++)
            {
                result += v1[i] * v2[i];
            }

            return result;
        }

        public static Vector operator *(Vector v, Matrix m)
        {
            ThrowIfAllNull(v, m);
            ThrowIfNull(v);
            ThrowIfNull(m);

            if (v.Size != m.Rows)
            {
                throw new InvalidOperationException("vector size and matrix rows amount must be equal when multiplying them");
            }

            var resultCoords = new double[m.Columns];
            for (int i = 0; i < m.Columns; i++)
            {
                resultCoords[i] = v * m.ColumnAt(i);
            }

            return new Vector(resultCoords);
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            ThrowIfAllNull(v, m);
            ThrowIfNull(v);
            ThrowIfNull(m);

            if (v.Size != m.Columns)
            {
                throw new InvalidOperationException("matrix columns and vector size amount must be equal when multiplying them");
            }

            var resultCoords = new double[m.Rows];
            for (int i = 0; i < m.Rows; i++)
            {
                resultCoords[i] = v * m.RowAt(i);
            }

            return new Vector(resultCoords);
        }

        public override string ToString()
        {
            return string.Join(", ", _elements);
        }
    }
}
