using System;
using System.Collections.Generic;
using System.Diagnostics;
using FaceGenerator.MachineLearning.Threading;
using FaceGenerator.MachineLearning.Math;

namespace FaceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            EigenTest();
        }

        private static void CollectionsTest()
        {
            var n = 1000000;
            var rand = new Random();
            var sw = new Stopwatch();
            sw.Start();

            var array = new Action[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = () => { var x = rand.Next().ToString()[0]; };
            }
            for (int i = 0; i < n; i++)
            {
                array[i]();
            }

            sw.Stop();
            var case1Ellapsed = sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();

            var enumerable = GetEnumerable();
            foreach (var item in enumerable)
            {
                item();
            }

            sw.Stop();
            var case2Ellapsed = sw.ElapsedMilliseconds;
            sw.Reset();

            Console.WriteLine(case1Ellapsed);
            Console.WriteLine(case2Ellapsed);

            IEnumerable<Action> GetEnumerable()
            {
                for (int i = 0; i < n; i++)
                {
                    yield return () => { var x = rand.Next().ToString()[0]; };
                }
            }
        }

        private static SymmetricMatrix GenerateMatrix(int n, int bound)
        {
            var rand = new Random();
            var elements = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    elements[i, j] = 2 * rand.NextDouble() * bound - bound;
                }
            }

            return new SymmetricMatrix(elements);
        }

        private static SymmetricMatrix ExampleFourMatrix()
        {
            return new SymmetricMatrix(new double[,]
            {
                { 1, 1, -1, 4 },
                { 1, 7, 3, 0},
                { -1, 3, 0, 3},
                { 4, 0, 3, -2}
            });
        }

        private static SymmetricMatrix ExampleThreeMatrix()
        {
            return new SymmetricMatrix(new double[,]
            {
                { 5, 1, 2 },
                { 1, 4, 1 },
                { 2, 1, 3 }
            });
        }

        private static void ConcurrencyMatrixTest()
        {
            var matrix1 = GenerateMatrix(500, 300);
            var matrix2 = GenerateMatrix(500, 300);

            var sw = new Stopwatch();
            sw.Start();
            ProdSync(matrix1, matrix2);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Reset();

            sw.Start();
            ProdAsync(matrix1, matrix2);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Reset();
        }

        private static SquareMatrix ProdSync(SquareMatrix m1, SquareMatrix m2) 
        {
            var elements = new double[m1.Rows, m1.Rows];
            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Rows; j++)
                {
                    elements[i, j] = m1.RowAt(i) * m2.ColumnAt(j);
                }
            }

            return new SquareMatrix(elements);
        }

        private static SquareMatrix ProdAsync(SquareMatrix m1, SquareMatrix m2)
        {
            var elements = new double[m1.Rows, m1.Rows];
            ObservableThreadPool.Run(ProductCallbacks());

            return new SquareMatrix(elements);

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

        private static void EigenTest()
        {
            // var matrix = GenerateMatrix(30, 10);
            var matrix = ExampleFourMatrix();

            var eigenResult = matrix.EigenDecomposition();
            var eigenValues = eigenResult.Item1;
            var eigenVectors = eigenResult.Item2.AsColumns();

            Console.WriteLine("Decomposition completed");

            for (int i = 0; i < eigenValues.Size; i++)
            {
                if (!matrix.AssertEigenVector(eigenVectors[i], eigenValues[i]))
                {
                    Console.WriteLine($"Assert failed ({i})");
                    break;
                }
            }
        }
    }
}
