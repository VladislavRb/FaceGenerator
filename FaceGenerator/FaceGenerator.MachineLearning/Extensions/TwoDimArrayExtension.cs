namespace FaceGenerator.MachineLearning.Extensions
{
    public static class TwoDimArrayExtension
    {
        public static int Rows(this double[,] array)
        {
            return array.GetLength(0);
        }

        public static int Columns(this double[,] array)
        {
            return array.GetLength(1);
        }

        public static void SwapRows(this double[,] array, int i, int j)
        {
            if (i == j)
            {
                return;
            }

            for (int k = 0; k < array.Columns(); k++)
            {
                (array[i, k], array[j, k]) = (array[j, k], array[i, k]);
            }
        }

        public static void AddRow(this double[,] array, int affectedRowIndex, int addedRowIndex, double coef)
        {
            for (int i = 0; i < array.Columns(); i++)
            {
                array[affectedRowIndex, i] += coef * array[addedRowIndex, i];
            }
        }

        public static double[,] Copy(this double[,] array)
        {
            var rows = array.Rows();
            var columns = array.Columns();

            var copy = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    copy[i, j] = array[i, j];
                }
            }

            return copy;
        }

        public static double[][] LowerTriangle(this double[,] array)
        {
            var rows = array.Rows();
            var columns = array.Columns();
            
            var n = rows < columns ? rows : columns;

            var triangle = new double[n][];
            for (int i = 0; i < n; i++)
            {
                var rowI = new double[i + 1];
                for (int j = 0; j < i + 1; j++)
                {
                    rowI[j] = array[i, j];
                }

                triangle[i] = rowI;
            }

            return triangle;
        }
    }
}
