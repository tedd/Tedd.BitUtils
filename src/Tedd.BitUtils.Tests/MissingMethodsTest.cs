using System;
using Xunit;
using Tedd;

namespace Tedd.BitUtils.Tests
{
    public class MissingMethodsTest
    {
        [Theory]
        [InlineData(new byte[] { 0xAA, 0x55 }, 0, 0, true)]
        [InlineData(new byte[] { 0xAA, 0x55 }, 3, 1, false)]
        [InlineData(new byte[] { 0xAA, 0x55 }, 8, 1, true)]
        [InlineData(new byte[] { 0xAA, 0x55 }, 9, 2, false)]
        public void TestBitReader_Properties(byte[] data, int bitsToRead, int expectedBytesRead, bool expectedByteAligned)
        {
            var reader = new BitReader(data);

            Assert.Equal(data.AsSpan().ToArray(), reader.Buffer.ToArray());

            if (bitsToRead > 0)
            {
                reader.Read(bitsToRead);
            }

            Assert.Equal(expectedBytesRead, reader.BytesRead);
            Assert.Equal(expectedByteAligned, reader.IsByteAligned);
        }
    }
}
