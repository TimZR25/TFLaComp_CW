using TFLaComp_1.DTO;

namespace TFLaComp_1.CardParser
{
    public interface ICardParser
    {
        List<CardDTO> Parse(string input);
    }
}