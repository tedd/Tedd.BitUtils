using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    /// <summary>
    /// Rol/Ror for SByte, Byte, Int16 and UInt16 are new in this version (the original library only implemented
    /// them for 32/64-bit integers). These tests exercise the full rotation cycle for every bit width.
    /// </summary>
    public class RolRorSmallTypesTest
    {
        [Fact]
        public void Rol_Byte_FullCycle()
        {
            byte v = 1;
            for (var i = 0; i < 7; i++)
            {
                v.Rol();
                Assert.Equal((byte)(1 << (i + 1)), v);
            }
            v.Rol(); // wraps back to bit 0
            Assert.Equal((byte)1, v);
        }

        [Fact]
        public void Ror_Byte_FullCycle()
        {
            byte v = 0b1000_0000;
            for (var i = 0; i < 7; i++)
            {
                v.Ror();
                Assert.Equal((byte)(0b1000_0000 >> (i + 1)), v);
            }
            v.Ror();
            Assert.Equal((byte)0b1000_0000, v);
        }

        [Fact]
        public void RolCopy_Byte_DoesNotModifyOriginal()
        {
            byte v = 0b0000_0001;
            var r = v.RolCopy();
            Assert.Equal((byte)0b0000_0001, v);
            Assert.Equal((byte)0b0000_0010, r);
        }

        [Fact]
        public void Rol_SByte_MatchesByteBitPattern()
        {
            sbyte v = unchecked((sbyte)0b1000_0001);
            byte expectedPattern = 0b1000_0001;
            v.Rol();
            expectedPattern.Rol();
            Assert.Equal(unchecked((sbyte)expectedPattern), v);
        }

        [Fact]
        public void Rol_UInt16_WithCount_MatchesRepeatedSingleRotate()
        {
            for (ushort start = 1; start != 0; start <<= 1)
            {
                for (var count = 0; count < 16; count++)
                {
                    var viaCount = start;
                    viaCount.Rol(count);

                    var viaRepeated = start;
                    for (var i = 0; i < count; i++)
                        viaRepeated.Rol();

                    Assert.Equal(viaRepeated, viaCount);
                }
            }
        }

        [Fact]
        public void Ror_Int16_WithCount_MatchesRepeatedSingleRotate()
        {
            short start = unchecked((short)0xACE1);
            for (var count = 0; count < 16; count++)
            {
                var viaCount = start;
                viaCount.Ror(count);

                var viaRepeated = start;
                for (var i = 0; i < count; i++)
                    viaRepeated.Ror();

                Assert.Equal(viaRepeated, viaCount);
            }
        }

        [Fact]
        public void Rol_Then_Ror_IsIdentity_AllSmallTypes()
        {
            byte b = 0b1101_0010;
            var bCopy = b;
            bCopy.Rol(); bCopy.Ror();
            Assert.Equal(b, bCopy);

            ushort u = 0xBEEF;
            var uCopy = u;
            uCopy.Rol(5); uCopy.Ror(5);
            Assert.Equal(u, uCopy);
        }
    }
}
