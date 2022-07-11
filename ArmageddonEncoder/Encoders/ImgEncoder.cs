using ArmageddonMounter.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmageddonEncoder.Encoders
{
    public class ImgEncoder : IMediaEncoder
    {
        public IEnumerable<string> AcceptableExtensions { get; } = new string[] { ".png" };
        public string TargetExtension => ".img";

        public async ValueTask<byte[]> Encode(byte[] data)
        {
            return await Task.Run(() => new ImgWrapper().ToInternal(data));
        }
    }
}
