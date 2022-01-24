using System;
using System.Text;
using System.Collections.Generic;
using FaceGenerator.MachineLearning.Extensions;
using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;

namespace FaceGenerator.MachineLearning.Math
{
    public class Matrix
    {
        protected double[,] Elements;

        public double this[int i, int j]
        {
            get => Elements[i, j];
            set => Elements[i, j] = value;
        }

        public int Rows => Elements.Rows();

        public int Columns => Elements.Columns();

        protected Matrix() { }

        public Matrix(double[,] elements)
        {
            ThrowIfNull(elements);
            Elements = elements;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return new Matrix(ProductAsElements(m1, m2));
        }

        public Vector RowAt(int i)
        {
            var row = new double[Columns];

            for (int j = 0; j < Columns; j++)
            {
                row[j] = Elements[i, j];
            }

            return new Vector(row);
        }

        public Vector ColumnAt(int i)
        {
            var column = new double[Rows];

            for (int j = 0; j < Rows; j++)
            {
                column[j] = Elements[j, i];
            }

            return new Vector(column);
        }

        public virtual Matrix Transpose()
        {
            return new Matrix(TransposedElements());
        }

        public Vector[] AsColumns()
        {
            var cols = new Vector[Columns];
            for (int i = 0; i < Columns; i++)
            {
                cols[i] = ColumnAt(i);
            }

            return cols;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    sb.Append(Elements[i, j]);
                    sb.Append("\t");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        protected static double[,] ProductAsElements(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Columns || m1.Columns != m2.Rows)
            {
                throw new InvalidOperationException($"In order to multiply matrices A and B, their dimensions should be m x n and n x m");
            }

            var elements = new double[m1.Rows, m1.Rows];
            //ObservableThreadPool.Run(ProductCallbacks());
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Rows; j++)
                {
                    elements[i, j] = m1.RowAt(i) * m2.ColumnAt(j);
                }
            }

            return elements;

            IEnumerable<Action> ProductCallbacks()
            {
                for (int i = 0; i < m1.Rows; i++)
                {
                    for (int j = 0; j < m1.Rows; j++)
                    {
                        int iCopy = i;
                        int jCopy = j;

                        yield return () =>
                        {
                            elements[iCopy, jCopy] = m1.RowAt(iCopy) * m2.ColumnAt(jCopy);
                        };
                    }
                }
            }
        }

        protected double[,] TransposedElements()
        {
            var newElements = new double[Columns, Rows];

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    newElements[i, j] = Elements[j, i];
                }
            }

            return newElements;
        }
    }
}
