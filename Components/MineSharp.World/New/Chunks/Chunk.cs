using MineSharp.World.New.Chunks.Sections;

namespace MineSharp.World.New.Chunks;

public class Chunk<TChunkSection> where TChunkSection : IChunkSection
{
    public const int SIZE         = 16;
    public const int SECTION_SIZE = 16;
    
    
}
