using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class BitWriterReaderTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 5_000;

        [Fact]
        public void SequentialFields_RoundTrip()
        {
            Span<byte> buffer = stackalloc byte[4];
            var writer = new BitWriter(buffer);
            writer.Write(0b101, 3);
            writer.Write(0b11011, 5);
            writer.Write(0xABC, 12);
            Assert.Equal(20, writer.BitPosition);
            Assert.Equal(3, writer.BytesWritten);
            Assert.Equal(0b101_11011, buffer[0]);

            var reader = new BitReader(buffer);
            Assert.Equal(0b101UL, reader.Read(3));
            Assert.Equal(0b11011UL, reader.Read(5));
            Assert.Equal(0xABCUL, reader.Read(12));
            Assert.Equal(20, reader.BitPosition);
        }

        [Fact]
        public void RandomFieldSequences_RoundTrip()
        {
            var buffer = new byte[64];
            var widths = new int[32];
            var values = new ulong[32];
            for (var iteration = 0; iteration < Iterations; iteration++)
            {
                var count = _rnd.Next(1, widths.Length + 1);
                var writer = new BitWriter(buffer);
                for (var i = 0; i < count; i++)
                {
                    widths[i] = _rnd.Next(0, 17);
                    values[i] = (ulong)_rnd.NextInt64() & (widths[i] == 0 ? 0 : (1UL << widths[i]) - 1);
                    writer.Write(values[i], widths[i]);
                }
                var reader = new BitReader(buffer);
                for (var i = 0; i < count; i++)
                    Assert.Equal(values[i], reader.Read(widths[i]));
            }
        }

        [Fact]
        public void BitAtATime_MatchesFieldAtATime()
        {
            Span<byte> byBit = stackalloc byte[8];
            Span<byte> byField = stackalloc byte[8];
            const ulong value = 0xF0E1D2C3B4A59687UL;

            var bitWriter = new BitWriter(byBit);
            for (var i = 63; i >= 0; i--) bitWriter.WriteBit(((value >> i) & 1) != 0);
            new BitWriter(byField).Write(value, 64);

            Assert.True(byBit.SequenceEqual(byField));

            var reader = new BitReader(byBit);
            ulong rebuilt = 0;
            for (var i = 0; i < 64; i++) rebuilt = (rebuilt << 1) | (reader.ReadBit() ? 1UL : 0UL);
            Assert.Equal(value, rebuilt);
        }

        [Fact]
        public void AlignToByte_PadsWithZeros()
        {
            Span<byte> buffer = [0xFF, 0xFF];
            var writer = new BitWriter(buffer);
            writer.Write(0b111, 3);
            Assert.False(writer.IsByteAligned);
            Assert.Equal(5, writer.AlignToByte());
            Assert.True(writer.IsByteAligned);
            Assert.Equal(8, writer.BitPosition);
            Assert.Equal(0b111_00000, buffer[0]);
            Assert.Equal(0, writer.AlignToByte());     // already aligned: nothing written

            var reader = new BitReader(buffer);
            reader.Read(3);
            Assert.Equal(5, reader.AlignToByte());
            Assert.Equal(8, reader.BitPosition);
            Assert.Equal(0, reader.AlignToByte());
        }

        [Fact]
        public void WrittenSpan_CoversExactlyTheBytesTouched()
        {
            var buffer = new byte[4];
            var writer = new BitWriter(buffer);
            writer.Write(0xFF, 8);
            Assert.Equal(1, writer.WrittenSpan.Length);
            writer.Write(1, 1);
            Assert.Equal(2, writer.WrittenSpan.Length);
            Assert.Equal(4, writer.Buffer.Length);
        }

        [Fact]
        public void RunningOutOfRoom_Throws()
        {
            var buffer = new byte[1];
            Assert.Throws<InvalidOperationException>(() =>
            {
                var writer = new BitWriter(buffer);
                writer.Write(0, 8);
                writer.Write(0, 1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                var reader = new BitReader(buffer);
                reader.Read(8);
                reader.ReadBit();
            });
        }

        [Fact]
        public void TryVariants_LeavePositionUntouchedOnFailure()
        {
            var buffer = new byte[1];
            var writer = new BitWriter(buffer);
            Assert.True(writer.TryWrite(0xAB, 8));
            Assert.False(writer.TryWrite(1, 1));
            Assert.Equal(8, writer.BitPosition);
            Assert.False(writer.TryWriteBit(true));
            Assert.Equal(8, writer.BitPosition);
            Assert.Equal(0xAB, buffer[0]);

            var reader = new BitReader(buffer);
            Assert.True(reader.TryRead(8, out var value));
            Assert.Equal(0xABUL, value);
            Assert.False(reader.TryRead(1, out value));
            Assert.Equal(0UL, value);
            Assert.Equal(8, reader.BitPosition);
            Assert.False(reader.TryReadBit(out var state));
            Assert.False(state);
            Assert.Equal(8, reader.BitPosition);
        }

        [Fact]
        public void BitsRemaining_TracksPosition()
        {
            var buffer = new byte[2];
            var writer = new BitWriter(buffer);
            Assert.Equal(16, writer.BitsRemaining);
            writer.Write(0, 5);
            Assert.Equal(11, writer.BitsRemaining);
            writer.Reset();
            Assert.Equal(16, writer.BitsRemaining);
            Assert.Equal(0, writer.BitPosition);

            var reader = new BitReader(buffer);
            Assert.Equal(16, reader.BitsRemaining);
            reader.Read(5);
            Assert.Equal(11, reader.BitsRemaining);
            reader.Reset();
            Assert.Equal(16, reader.BitsRemaining);
        }

        [Fact]
        public void Skip_MovesWithoutTouchingTheBuffer()
        {
            Span<byte> buffer = [0xFF, 0xFF];
            var writer = new BitWriter(buffer);
            writer.Skip(4);
            writer.Write(0, 4);
            Assert.Equal(0xF0, buffer[0]);      // the skipped bits are still the original ones

            var reader = new BitReader(buffer);
            reader.Skip(4);
            Assert.Equal(0UL, reader.Read(4));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var w = new BitWriter(new byte[1]);
                w.Skip(-1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                var w = new BitWriter(new byte[1]);
                w.Skip(9);
            });
        }

        [Fact]
        public void InvalidWidth_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var writer = new BitWriter(new byte[16]);
                writer.Write(0, 65);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var reader = new BitReader(new byte[16]);
                reader.Read(-1);
            });
        }

        [Fact]
        public void EmptyBuffer_IsUsableButFull()
        {
            var writer = new BitWriter([]);
            Assert.Equal(0, writer.BitsRemaining);
            Assert.Equal(0, writer.BytesWritten);
            Assert.True(writer.IsByteAligned);
            Assert.True(writer.TryWrite(0, 0));         // a zero width write still fits
            Assert.False(writer.TryWriteBit(true));

            var reader = new BitReader([]);
            Assert.True(reader.TryRead(0, out var value));
            Assert.Equal(0UL, value);
            Assert.False(reader.TryReadBit(out _));
        }

        [Fact]
        public void BitReader_Buffer_ReturnsOriginalSpan()
        {
            var data = new byte[] { 10, 20, 30, 40 };
            var reader = new BitReader(data);
            Assert.True(data.AsSpan().SequenceEqual(reader.Buffer));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 1)]
        [InlineData(9, 2)]
        [InlineData(16, 2)]
        [InlineData(17, 3)]
        public void BitReader_BytesRead_ReturnsExpectedValues(int bitsToSkip, int expectedBytesRead)
        {
            var buffer = new byte[8];
            var reader = new BitReader(buffer);
            if (bitsToSkip > 0)
                reader.Skip(bitsToSkip);
            Assert.Equal(expectedBytesRead, reader.BytesRead);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(7, false)]
        [InlineData(8, true)]
        [InlineData(9, false)]
        [InlineData(16, true)]
        public void BitReader_IsByteAligned_ReturnsExpectedValues(int bitsToSkip, bool expectedIsAligned)
        {
            var buffer = new byte[8];
            var reader = new BitReader(buffer);
            if (bitsToSkip > 0)
                reader.Skip(bitsToSkip);
            Assert.Equal(expectedIsAligned, reader.IsByteAligned);
        }
    }
}
