using FaceGenerator.MachineLearning.Math;

namespace FaceGenerator.MachineLearning.Models
{
    public abstract class BaseMatrix<TKey, TElement>
        where TKey : struct
    {
        public abstract int Rows { get; }

        public abstract int Columns { get; }

        public abstract Vector RowAt(int i);

        public abstract Vector ColumnAt(int i);
    }
}
