using System.Text;
using System.Text.RegularExpressions;
using TFLaComp_1.DTO;

namespace TFLaComp_1.CardParser
{
    public class RegExCardParser : ICardParser
    {
        public List<CardDTO> Parse(string input)
        {
            List<CardDTO> cards = new List<CardDTO>();

            input = input.Trim();

            StringBuilder digits = new StringBuilder();

            foreach (var item in AnalyzerCard.PaymentSystems)
            {
                digits = digits.Append(item.Key);
            }

            string pattern = $@"(?<!\d)([{digits}]\d{{15}}|([{digits}]\d{{3}}(?:\s\d{{4}}){{3}})|([{digits}]\d{{3}}(?:-\d{{4}}){{3}}))(?!\d)";

            Match match = Regex.Match(input, pattern);
            while (match.Success)
            {
                string value = match.Value;
                value = value.Trim();
                if (value.Contains(' ') == false)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        value = value.Insert(4 + i * 4 + i, " ");
                    }
                }

                CardDTO card = new CardDTO(value, match.Index, match.Index + match.Value.Length - 1);
                cards.Add(card);
                match = match.NextMatch();
            }

            return cards;
        }
    }
}
