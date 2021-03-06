#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
// ReSharper disable RedundantUsingDirective
// ReSharper disable CheckNamespace
#nullable disable
/*
 * IConfiguration and IOption test
 *
 * Test API to show using IOptions and IConfiguration
 *
 * OpenAPI spec version: 1.0.0-oas3
 * Contact: myemail@bogus.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;

namespace IOptionTest.Models
{
    /// <summary>
    /// Message to send
    /// </summary>
    [DataContract]
    public partial class Message : IEquatable<Message>
    {
        /// <summary>
        /// Gets or Sets _Message
        /// </summary>
        [Required]

        [DataMember(Name = "message")]
        public string _Message { get; set; }

        /// <summary>
        /// Gets or Sets Level
        /// </summary>
        [Required]

        [DataMember(Name = "level")]
        public LogLevel Level { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Message {\n");
            sb.Append("  _Message: ").Append(_Message).Append("\n");
            sb.Append("  Level: ").Append(Level).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Message)obj);
        }

        /// <summary>
        /// Returns true if Message instances are equal
        /// </summary>
        /// <param name="other">Instance of Message to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Message other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    _Message == other._Message ||
                    _Message != null &&
                    _Message.Equals(other._Message)
                ) &&
                (
                    Level == other.Level ||
                   Level.Equals(other.Level)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (_Message != null)
                    hashCode = hashCode * 59 + _Message.GetHashCode();

                hashCode = hashCode * 59 + Level.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(Message left, Message right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Message left, Message right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}

#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
