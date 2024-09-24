using System.Text;

namespace API.Services {
    public interface IHashingService {
        public byte[] GetHash(string str);
        public byte[] GetHash(string str, Encoding encoding);
        public byte[] GetHash(byte[] data);
        public Task<byte[]> GetHashAsync(string str);
        public Task<byte[]> GetHashAsync(string str, Encoding encoding);
        public Task<byte[]> GetHashAsync(byte[] data);
    }
}
