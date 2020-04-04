using System.Collections.Generic;

namespace Lab.Common.ClipBoards
{
    public interface IClipBoards
    {
        void CopyToBoard(IDictionary<object, object> data);
    }
}
