﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Types {
    public class Vector3 {

        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 Up    = new Vector3(0, 1, 0);
        public static readonly Vector3 Down  = new Vector3(0, -1, 0);
        public static readonly Vector3 North = new Vector3(0, 0, -1);
        public static readonly Vector3 South = new Vector3(0, 0, 1);
        public static readonly Vector3 West  = new Vector3(-1, 0, 0);
        public static readonly Vector3 East  = new Vector3(1, 0, 0);

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }



        public Vector3(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Add(Vector3 v) {
            this.X += v.X;
            this.Y += v.Y;
            this.Z += v.Z;
        }

        public Vector3 Plus(Vector3 v) {
            return new Vector3(this.X + v.X, this.Y + v.Y, this.Z + v.Z);
        }

        public Vector3 Minus(Vector3 v) {
            return new Vector3(this.X - v.X, this.Y - v.Y, this.Z - v.Z);
        }


        public static Vector3 operator *(Vector3 v, int val) {
            return new Vector3(v.X * val, v.Y * val, v.Z * val);
        }

        public static Vector3 operator /(Vector3 v, int val) {
            return new Vector3(v.X / val, v.Y / val, v.Z / val);
        }


        public double DistanceSquared(Vector3 v) {
            var diff = this.Minus(v);
            return diff.X * diff.X + diff.Y * diff.Y + diff.Z * diff.Z;
        }

        public double Distance(Vector3 v) {
            return Math.Sqrt(this.DistanceSquared(v));
        }

        public override string ToString() {
            return $"({X.ToString("0.##")} / {Y.ToString("0.##")} / {Z.ToString("0.##")})";
        }
    }
}
