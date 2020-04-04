using System;

namespace Lab.UICommon.Positions
{
    public struct Length : IEquatable<Length>
    {
        public static Length Auto { get; } = new Length(0, LengthUnitType.Auto);

        public static Length FromAbsolute(double value)
        {        
            return new Length(value, LengthUnitType.Absolute);
        }

        public static Length FromRelative(double value)
        {
            if (value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be greater than 1.");
            }

            return new Length(value, LengthUnitType.Relative);
        }

        private Length(double value, LengthUnitType lengthUnitType)
        {
            Value = value;
            LengthUnitType = lengthUnitType;
        }

        public double Value { get; }

        public LengthUnitType LengthUnitType { get; }

        #region IEquatable Members

        public bool Equals(Length other)
        {
            return Value.Equals(other.Value) && LengthUnitType == other.LengthUnitType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Length && Equals((Length)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (int)LengthUnitType;
            }
        }

        public static bool operator ==(Length left, Length right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Length left, Length right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
