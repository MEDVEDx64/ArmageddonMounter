using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmageddonEncoder.Encoders
{
    public interface IMediaEncoder
    {
        IEnumerable<string> AcceptableExtensions { get; }
        string TargetExtension { get; }

        ValueTask<byte[]> EncodeAsync(byte[] data);
    }
}
