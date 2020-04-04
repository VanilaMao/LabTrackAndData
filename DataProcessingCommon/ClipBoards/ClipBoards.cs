using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lab.Common.ClipBoards
{
    public class ClipBoards: IClipBoards
    {
        public void CopyToBoard(IDictionary<object, object> data)
        {
            Clipboard.SetText(string.Join("\r\n",
                data.Select(x=>x.Key + ":"+x.Value)));
        }
    }
}
