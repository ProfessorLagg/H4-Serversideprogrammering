using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using API.Utils.FunctionalUtils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace API.Services {
    public class LocalHashingService : IHashingService {
        private static byte[] HashFunction_SHA256(byte[] data) {
            using (var hasher = SHA256.Create()) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_SHA384(byte[] data) {
            using (var hasher = SHA384.Create()) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_SHA512(byte[] data) {
            using (var hasher = SHA384.Create()) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_HMACSHA256(byte[] key, byte[] data) {
            using (var hasher = new HMACSHA256(key)) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_HMACSHA384(byte[] key, byte[] data) {
            using (var hasher = new HMACSHA384(key)) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_HMACSHA512(byte[] key, byte[] data) {
            using (var hasher = new HMACSHA512(key)) {
                return hasher.ComputeHash(data);
            }
        }
        private static byte[] HashFunction_BCrypt(byte[] key, byte[] data) {
            string keystr = Encoding.UTF8.GetString(key);
            string datastr = Encoding.UTF8.GetString(data);
            string hashString = BCrypt.Net.BCrypt.HashPassword(keystr, datastr);
            return Encoding.UTF8.GetBytes(hashString);
        }

        private readonly Func<byte[], byte[]> HashFunc;
        public LocalHashingService(IConfiguration config) {
            IConfigurationSection configSection = config.GetSection("Hashing");
            string? hashName = configSection.GetValue<string>("Algorithm");

            string? saltString = configSection.GetValue<string>("Salt");
            byte[] saltBytes = string.IsNullOrWhiteSpace(saltString) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(saltString);

            if (string.IsNullOrEmpty(hashName)) { throw new ArgumentException("Could not find Hashing:Algorithm in config"); }
            else if (hashName == @"SHA256") { this.HashFunc = HashFunction_SHA256; }
            else if (hashName == @"SHA384") { this.HashFunc = HashFunction_SHA384; }
            else if (hashName == @"SHA512") { this.HashFunc = HashFunction_SHA512; }
            else if (hashName == @"HMACSHA256") {
                if (saltBytes.IsNullOrEmpty()) { throw new ArgumentException("Could not find Salt in configuration"); }
                this.HashFunc = Util.Bind<byte[], byte[], byte[]>(HashFunction_HMACSHA256, saltBytes);
            }
            else if (hashName == @"HMACSHA384") {
                if (saltBytes.IsNullOrEmpty()) { throw new ArgumentException("Could not find Salt in configuration"); }
                this.HashFunc = Util.Bind<byte[], byte[], byte[]>(HashFunction_HMACSHA384, saltBytes);
            }
            else if (hashName == @"HMACSHA512") {
                if (saltBytes.IsNullOrEmpty()) { throw new ArgumentException("Could not find Salt in configuration"); }
                this.HashFunc = Util.Bind<byte[], byte[], byte[]>(HashFunction_HMACSHA512, saltBytes);
            }
            else if (hashName == "BCrypt") {
                if (saltBytes.IsNullOrEmpty()) { throw new ArgumentException("Could not find Salt in configuration"); }
                this.HashFunc = Util.Bind<byte[], byte[], byte[]>(HashFunction_BCrypt, saltBytes);
            }
            else { throw new ArgumentException($"Could not parse '{hashName}' as a valid HashAlgorithm"); }
        }

        public byte[] GetHash(string str) { return this.GetHash(str, Encoding.UTF8); }
        public byte[] GetHash(string str, Encoding encoding) { return this.GetHash(encoding.GetBytes(str)); }
        public byte[] GetHash(byte[] data) { return this.HashFunc(data); }

        public async Task<byte[]> GetHashAsync(string str) { return await this.GetHashAsync(str, Encoding.UTF8); }
        public async Task<byte[]> GetHashAsync(string str, Encoding encoding) { return await this.GetHashAsync(encoding.GetBytes(str)); }
        public async Task<byte[]> GetHashAsync(byte[] data) { return await Task.Run(Util.Bind<byte[], byte[]>(this.HashFunc, data)); }
    }
}
