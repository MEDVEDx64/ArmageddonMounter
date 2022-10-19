using ArmageddonMounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmageddonEncoder.Encoders
{
    public class PngEncoder : IMediaEncoder
    {
        public IEnumerable<string> AcceptableExtensions { get; } = new string[] { ".img" };
        public string TargetExtension => ".png";

        public async ValueTask<byte[]> EncodeAsync(byte[] data)
        {
            return await Task.Run(() => new ImgWrapper().ToExternal(data));
        }
    }
}
