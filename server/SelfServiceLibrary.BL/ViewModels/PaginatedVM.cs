using System.Collections.Generic;

namespace SelfServiceLibrary.BL.ViewModels
{
    public class PaginatedVM<T>
    {
        public PaginatedVM(int count, List<T> data)
        {
            Count = count;
            Data = data;
        }

        public int Count { get; }
        public List<T> Data { get; }
    }
}
