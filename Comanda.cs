namespace ProiectPOO;
public enum OrderStatus
{
    BeingProcessed,
    Sent,
    Delivered,
    Canceled
}
public struct ShippingAddress
{
    public string City;
    public string Country;
    public string Postcode;
    public string Address;
    // daca aveti voi o idee mai buna pentru cum sa formulam aici
    // fiindca string Address = Strada, numarul casei
}
public class Comanda
{
    private Dictionary<Produs, int> ProductsOrdered;
    public string ID { get; private set; }
    public DateOnly PlacementDate { get; private set; }
    private Client Recipient { get; set; }
    private OrderStatus Status { get; set; }
    private ShippingAddress DeliveryAddress { get; set; }
    private DateOnly DeliveryDate { get; set; }

    public Comanda(Dictionary<Produs, int> productsOrdered, string iD, Client recipient, OrderStatus status, ShippingAddress deliveryAddress)
    {
        ProductsOrdered = productsOrdered;
        ID = iD;
        PlacementDate = DateOnly.FromDateTime(DateTime.Now); // autogenerez data comenzii oricand e creata una
        Recipient = recipient;
        Status = status;
        DeliveryAddress = deliveryAddress;
    }
}

