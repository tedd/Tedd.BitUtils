using System;
using System.IO;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class EbmlVIntTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 20_000;

        private static byte[] Encode(ulong value)
        {
            var buffer = new byte[EbmlVInt.MaxLength];
            var length = EbmlVInt.Write(buffer, value);
            return buffer[..length];
        }

        [Theory]
        // RFC 8794 VINT vectors: the marker bit position gives the length, the rest is big endian payload.
        [InlineData(0UL, new byte[] { 0x80 })]
        [InlineData(1UL, new byte[] { 0x81 })]
        [InlineData(126UL, new byte[] { 0xFE })]
        [InlineData(127UL, new byte[] { 0x40, 0x7F })]                   // all-ones at one byte is reserved, so roll up
        [InlineData(128UL, new byte[] { 0x40, 0x80 })]
        [InlineData(16382UL, new byte[] { 0x7F, 0xFE })]
        [InlineData(16383UL, new byte[] { 0x20, 0x3F, 0xFF })]
        [InlineData(0x00FFFFFFFFFFFFFEUL, new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE })]
        public void MatchesKnownVectors(ulong value, byte[] expected)
        {
            Assert.Equal(expected, Encode(value));
            Assert.Equal(expected.Length, EbmlVInt.GetSize(value));
            var decoded = EbmlVInt.Read(expected);
            Assert.Equal(value, decoded.Value);
            Assert.Equal(expected.Length, decoded.Length);
            Assert.False(decoded.IsUnknown);
        }

        [Fact]
        public void RoundTripsRandomValues()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var value = unchecked((ulong)_rnd.NextInt64()) >> _rnd.Next(8, 64);
                if (value > EbmlVInt.MaxValue) continue;
                var encoded = Encode(value);
                var decoded = EbmlVInt.Read(encoded);
                Assert.Equal(value, decoded.Value);
                Assert.Equal(encoded.Length, decoded.Length);
                Assert.Equal(encoded.Length, decoded.Size);
            }
        }

        [Fact]
        public void AllOnesPayload_IsTheUnknownSizeMarker()
        {
            foreach (var (encoded, length) in new (byte[], int)[]
            {
                ([0xFF], 1),
                ([0x7F, 0xFF], 2),
                ([0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF], 8)
            })
            {
                var decoded = EbmlVInt.Read(encoded);
                Assert.True(decoded.IsUnknown);
                Assert.Equal(length, decoded.Length);
                Assert.Equal(length, decoded.Size);      // an unknown marker is not shortened
            }
        }

        [Fact]
        public void PaddedEncoding_ReportsBothLengths()
        {
            // 1 written as a four byte VINT: legal in EBML, and Size reports the shortest form.
            var padded = new byte[] { 0x10, 0x00, 0x00, 0x01 };
            var decoded = EbmlVInt.Read(padded);
            Assert.Equal(1UL, decoded.Value);
            Assert.Equal(4, decoded.Length);
            Assert.Equal(1, decoded.Size);
            Assert.False(decoded.IsUnknown);
        }

        [Fact]
        public void EncodedValueKeepsTheMarker()
        {
            var decoded = EbmlVInt.Read([0x40, 0x7F]);
            Assert.Equal(127UL, decoded.Value);
            Assert.Equal(0x407FUL, decoded.EncodedValue);
        }

        [Fact]
        public void MaxValue_IsOneBelowTheAllOnesPattern()
        {
            Assert.Equal(8, EbmlVInt.GetSize(EbmlVInt.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => EbmlVInt.GetSize(EbmlVInt.MaxValue + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => EbmlVInt.Write(new byte[8], ulong.MaxValue));
            Assert.False(EbmlVInt.TryWrite(new byte[8], EbmlVInt.MaxValue + 1, out var written));
            Assert.Equal(0, written);
        }

        [Fact]
        public void RejectsMalformedAndTruncatedInput()
        {
            Assert.Throws<InvalidDataException>(() => EbmlVInt.Read([0x00]));            // no marker in the first byte
            Assert.Throws<InvalidDataException>(() => EbmlVInt.Read([]));
            Assert.Throws<InvalidDataException>(() => EbmlVInt.Read([0x40]));            // says two bytes, only one given
            Assert.False(EbmlVInt.TryRead([0x00], out _));
            Assert.False(EbmlVInt.TryRead([0x40], out _));
        }

        [Fact]
        public void MaxLengthBoundsWhatIsAccepted()
        {
            var threeByte = Encode(16383);
            Assert.Equal(3, threeByte.Length);
            Assert.True(EbmlVInt.TryRead(threeByte, out _, maxLength: 3));
            Assert.False(EbmlVInt.TryRead(threeByte, out _, maxLength: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => EbmlVInt.Read(threeByte, maxLength: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => EbmlVInt.Read(threeByte, maxLength: 9));
        }

        [Fact]
        public void ShortDestination_ThrowsOrReports()
        {
            Assert.Throws<ArgumentException>(() => EbmlVInt.Write(new byte[1], 128));
            Assert.False(EbmlVInt.TryWrite(new byte[1], 128, out var written));
            Assert.Equal(0, written);
        }

        [Fact]
        public void ConstructorValidatesItsArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new EbmlVInt(0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new EbmlVInt(9, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new EbmlVInt(1, 0, 128));   // 128 needs more than 7 payload bits
        }

        [Fact]
        public void EqualityComparesTheWholeEncoding()
        {
            var shortForm = EbmlVInt.Read([0x81]);
            var paddedForm = EbmlVInt.Read([0x40, 0x01]);
            Assert.Equal(shortForm.Value, paddedForm.Value);
            Assert.NotEqual(shortForm, paddedForm);         // same number, different wire bytes
            Assert.True(shortForm != paddedForm);
            Assert.True(shortForm == EbmlVInt.Read([0x81]));
            Assert.Equal(shortForm.GetHashCode(), EbmlVInt.Read([0x81]).GetHashCode());
        }

        [Fact]
        public void EncodedValuesSortLikeTheNumbers()
        {
            // The length-in-the-marker layout plus big endian payload keeps byte order and numeric order aligned.
            ulong previous = 0;
            var previousBytes = Encode(0);
            for (ulong value = 1; value < 5000; value++)
            {
                var bytes = Encode(value);
                if (bytes.Length == previousBytes.Length)
                    Assert.True(bytes.AsSpan().SequenceCompareTo(previousBytes) > 0, $"{value} should sort above {previous}.");
                previous = value;
                previousBytes = bytes;
            }
        }
    }
}
