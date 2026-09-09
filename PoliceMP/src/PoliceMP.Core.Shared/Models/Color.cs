namespace PoliceMP.Core.Shared.Models
{
    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }

        public Color(int r, int g, int b, int a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color Red => new Color(255, 0, 0);
        public static Color Blue => new Color(0, 255, 0);
        public static Color Green => new Color(0, 0, 255);
        public static Color White => new Color(255, 255, 255);
        public static Color Black => new Color(0, 0, 0);
        public static Color Purple => new Color(194, 162, 218);
    }
}