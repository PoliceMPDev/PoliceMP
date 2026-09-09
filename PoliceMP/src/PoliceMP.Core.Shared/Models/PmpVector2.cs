namespace PoliceMP.Core.Shared.Models
{
    /// <summary>
    /// Represents a position in 2D space.
    /// </summary>
    public class PmpVector2
    {
        /// <summary>
        /// Gets or sets the position on the X axis.
        /// </summary>
        /// <value>
        /// The position on the X axis.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the position on the Y axis.
        /// </summary>
        /// <value>
        /// The position on the Y axis.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PmpVector2" /> class.
        /// </summary>
        public PmpVector2() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PmpVector2" /> class.
        /// </summary>
        /// <param name="x">The position on the X axis.</param>
        /// <param name="y">The position on the Y axis.</param>
        public PmpVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this vector.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this vector.
        /// </returns>
        public override string ToString() => $"X: {this.X}, Y: {this.Y}";

        /// <summary>
        /// Determines whether the specified <see cref="PmpVector2" />, is equal to this instance.
        /// </summary>
        /// <param name="pos">The <see cref="PmpVector2" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="PmpVector2" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        protected bool Equals(PmpVector2 pos) => this.X.Equals(pos.X) && this.Y.Equals(pos.Y);

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PmpVector2)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>This method determines whether two Vectors have the same value.</summary>
        /// <seealso cref="operator!=" />
        /// <seealso cref="Equals" />
        public static bool operator ==(PmpVector2 a, PmpVector2 b)
        {
            if ((object)a == null) return (object)b == null;
            return a.Equals(b);
        }

        /// <summary>This method determines whether two Vectors do not have the same value.</summary>
        /// <seealso cref="operator==" />
        /// <seealso cref="Equals" />
        public static bool operator !=(PmpVector2 a, PmpVector2 b) => !(a == b);
    }
}
