using MinimalEshop.Application.Domain.Enums;

public static class PaymentMethodParser
{
    public static bool TryParse(string input, out PaymentMethod method)
    {
        method = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        string value = input.Trim().ToLower();

        return value switch
        {
            "upi" => (method = PaymentMethod.UPI) == PaymentMethod.UPI,
            "card" => (method = PaymentMethod.Card) == PaymentMethod.Card,

            // synonyms for COD
            "cod" => (method = PaymentMethod.CashOnDelivery) == PaymentMethod.CashOnDelivery,
            "cash on delivery" => (method = PaymentMethod.CashOnDelivery) == PaymentMethod.CashOnDelivery,
            "cash" => (method = PaymentMethod.CashOnDelivery) == PaymentMethod.CashOnDelivery,

            // synonyms for netbanking
            "netbanking" => (method = PaymentMethod.NetBanking) == PaymentMethod.NetBanking,
            "net banking" => (method = PaymentMethod.NetBanking) == PaymentMethod.NetBanking,

            _ => false
        };
    }
}
