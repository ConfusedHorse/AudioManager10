namespace AudioManager10.View.Resource.Enum
{
    public static class QuantityConverter
    {
        public static int GetRectCount(this int value)
        {
            switch (value)
            {
                case 0: return 20;
                case 1: return 33;
                case 2: return 50;
                default: return 33;
            }
        }

        public static int GetRectCount(this Quantity value)
        {
            switch (value)
            {
                case Quantity.Fewer: return 20;
                case Quantity.Normal: return 33;
                case Quantity.More: return 50;
                default: return 33;
            }
        }
    }
}