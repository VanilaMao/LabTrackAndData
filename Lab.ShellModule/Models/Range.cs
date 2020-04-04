

namespace Lab.ShellModule.Models
{
    public class Range
    {
        public int Start { get; set; }

        public int End { get; set; }

        public bool IsInRange(int pos)
        {
            return pos >= Start && pos <= End;
        }
        public bool IsInRange(int? pos)
        {
            if (pos.HasValue)
            {
                return IsInRange(pos.Value);
            }
            return false;
        }
    }
}
