using ConsoleApp1;

string src = "aaabbcccdde";
string compressed = Rle.Compress(src);
string decompressed = Rle.Decompress(compressed);

Console.WriteLine($"Исходная: {src}");
Console.WriteLine($"Сжатая: {compressed}");
Console.WriteLine($"Декодированная: {decompressed}");