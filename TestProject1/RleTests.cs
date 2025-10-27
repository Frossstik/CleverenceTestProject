using ConsoleApp1;

namespace TestProject1
{
    public class RleTests
    {
        [Fact]
        public void CompressTest()
        {
            var input = "aaabbcccdde";
            var expected = "a3b2c3d2e";
            Assert.Equal(expected, Rle.Compress(input));
        }

        [Fact]
        public void DecompressTest()
        {
            var input = "a3b2c3d2e";
            var expected = "aaabbcccdde";
            Assert.Equal(expected, Rle.Decompress(input));
        }

        [Fact]
        public void CompressCharTest()
        {
            var input = "a";
            var expected = "a";
            Assert.Equal(expected, Rle.Compress(input));
        }
    }
}
