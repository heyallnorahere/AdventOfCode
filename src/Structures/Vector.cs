using System;
using System.Runtime.InteropServices;

namespace AdventOfCode.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public Vector(int scalar)
        {
            X = Y = scalar;
        }
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vector((int x, int y) tuple)
        {
            X = tuple.x;
            Y = tuple.y;
        }
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override bool Equals(object? obj)
        {
            if (obj is Vector vector)
            {
                return X == vector.X && Y == vector.Y;
            }
            else
            {
                return false;
            }
        }
        public int X;
        public int Y;
        public static Vector operator+(Vector v1, Vector v2) => (v1.X + v2.X, v1.Y + v2.Y);
        public static Vector operator+(Vector vector, int scalar) => vector + new Vector(scalar);
        public static Vector operator+(int scalar, Vector vector) => vector + scalar;
        public static Vector operator-(Vector vector) => (-vector.X, -vector.Y);
        public static Vector operator-(Vector v1, Vector v2) => v1 + -v2;
        public static Vector operator-(Vector vector, int scalar) => vector - new Vector(scalar);
        public static Vector operator-(int scalar, Vector vector) => new Vector(scalar) - vector;
        public static Vector operator*(Vector v1, Vector v2) => (v1.X * v2.X, v1.Y * v2.Y);
        public static Vector operator*(Vector vector, int scalar) => vector * new Vector(scalar);
        public static Vector operator*(int scalar, Vector vector) => vector * scalar;
        public static Vector operator/(Vector v1, Vector v2) => (v1.X / v2.X, v1.Y / v2.Y);
        public static Vector operator/(Vector vector, int scalar) => vector / new Vector(scalar);
        public static Vector operator/(int scalar, Vector vector) => new Vector(scalar) / vector;
        public static bool operator ==(Vector v1, Vector v2) => v1.Equals(v2);
        public static bool operator !=(Vector v1, Vector v2) => !v1.Equals(v2);
        public static implicit operator Vector((int x, int y) tuple) => new(tuple);
    }
}