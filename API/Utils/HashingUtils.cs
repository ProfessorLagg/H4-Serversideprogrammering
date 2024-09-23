using System.Security.Cryptography;
using System.Text;
namespace API.Utils.HashingUtils {
    public static partial class Utils {
        public static byte[] ComputeHash(this HashAlgorithm hasher, string value, Encoding encoding) {
            byte[] bytes = encoding.GetBytes(value);
            return hasher.ComputeHash(bytes);
        }
        public static byte[] ComputeHash(this HashAlgorithm hasher, string value) {
            return hasher.ComputeHash(value, Encoding.UTF8);
        }
    }
}
