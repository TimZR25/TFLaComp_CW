using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TFLaComp_1.DTO;

namespace TFLaComp_1
{
    internal class AnalyzerCard
    {
        private readonly List<CardDTO> _cards;


        public static readonly Dictionary<string, string> PaymentSystems = new()
        {
            { "4", "Visa" },
            { "5", "MasterCard" },
            { "6", "UnionPay" },
            { "2", "Мир" }
        };

        private static readonly Dictionary<string, string> BankCodes = new()
        {
            { "276", "Сбербанк" },
            { "469", "Сбербанк" },
            { "213", "Тинькофф" },
            { "377", "Тинькофф" },
            { "584", "Альфа-Банк" },
            { "486", "Альфа-Банк" },
            { "242", "ВТБ" },
            { "622", "ВТБ" },
            { "547", "Газпромбанк" },
            { "209", "Газпромбанк" },
            { "627", "Райффайзенбанк" },
            { "100", "Райффайзенбанк" },
            { "179", "Открытие" },
            { "478", "Промсвязьбанк" },
            { "103", "Россельхозбанк" },
            { "222", "Совкомбанк" },
            { "038", "Хоум Кредит" },
            { "890", "ЮниКредит Банк" },
            { "154", "Росбанк" },
            { "308", "Кредит Европа Банк" },
            { "762", "Русский Стандарт" },
            { "058", "МТС Банк" },
            { "112", "Почта Банк" },
            { "314", "МКБ" },
            { "405", "Зенит" },
            { "506", "Уралсиб" },
            { "652", "Ак Барс" },
            { "724", "Сетелем Банк" },
            { "779", "Локо-Банк" },
            { "893", "Дом.РФ" },
            { "211", "Соверен Банк" },
            { "254", "Энерготрансбанк" },
            { "301", "Банк Санкт-Петербург" },
            { "321", "Пересвет" },
            { "425", "СМП Банк" },
            { "481", "Новикомбанк" },
            { "521", "Экспобанк" },
            { "536", "Тинькофф" },
            { "578", "Банк Финсервис" },
            { "590", "Банк Оренбург" },
            { "656", "Банк Казани" },
            { "011", "Авангард" }
        };


        public AnalyzerCard(List<CardDTO> cards) 
        {
            _cards = cards;
        }

        private string GetPaymentSystem(CardDTO card)
        {
            string paymentSystemId = card.NumberCard.Substring(0, 1);
            if (PaymentSystems.TryGetValue(paymentSystemId, out string? value))
            {
                return value;
            }
            else
                return "Неизвестная платежная система";
        }

        private string GetBankCode(CardDTO card)
        {
            string bin = card.NumberCard.Substring(1, 4);
            
            if(BankCodes.TryGetValue(bin, out string? value))
            {
                return value;
            }
            else
                return "Неизвестный банк";
        }

        public List<FullCardDTO> Analyze()
        {

            List<FullCardDTO> fullCards = new List<FullCardDTO>();

            foreach (CardDTO card in _cards) 
            {
                FullCardDTO fullCardDTO = new FullCardDTO(card, GetPaymentSystem(card), GetBankCode(card));
                fullCards.Add(fullCardDTO);
            }

            return fullCards;
        }

    }
}
