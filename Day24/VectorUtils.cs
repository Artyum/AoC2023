class VectorUtils {
    public class Vector {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public long Vx { get; set; }
        public long Vy { get; set; }
        public long Vz { get; set; }
    }

    public static (bool intersects, double x, double y, double z, double t, double s) IntersectionPoint(Vector v1, Vector v2) {
        // Checking if vectors are parallel (do not have an intersection point)
        long crossVx = v1.Vy * v2.Vz - v1.Vz * v2.Vy;
        long crossVy = v1.Vz * v2.Vx - v1.Vx * v2.Vz;
        long crossVz = v1.Vx * v2.Vy - v1.Vy * v2.Vx;
        if (crossVx == 0 && crossVy == 0 && crossVz == 0) {
            // The lines are parallel
            return (false, 0, 0, 0, 0, 0);
        }

        // Linear equations for v1 and v2, solved with respect to t and s
        double t = (double)(v2.Vy * (v2.X - v1.X) + v2.Vx * (v1.Y - v2.Y)) / (v1.Vx * v2.Vy - v1.Vy * v2.Vx);
        double s = (double)(v1.Vx * (v2.Y - v1.Y) + v1.Vy * (v1.X - v2.X)) / (v2.Vx * v1.Vy - v2.Vy * v1.Vx);

        // Calculate the intersection point
        double x = v1.X + v1.Vx * t;
        double y = v1.Y + v1.Vy * t;
        double z = v1.Z + v1.Vz * t;

        return (true, x, y, z, t, s);
    }
}
