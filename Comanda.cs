﻿namespace ProiectPOO;
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
    public ShippingAddress DeliveryAddress { get; private set; }
    private DateOnly DeliveryDate { get; set; }
    private double OrderPrice { get; set; }

    public Comanda(Dictionary<Produs, int> productsOrdered, string iD, Client recipient, OrderStatus status, ShippingAddress deliveryAddress)
    {
        ProductsOrdered = productsOrdered;
        ID = iD;
        PlacementDate = DateOnly.FromDateTime(DateTime.Now); // autogenerez data comenzii oricand e creata una
        Recipient = recipient;
        Status = status;
        DeliveryAddress = deliveryAddress;
        OrderPrice = CalculateOrderPrice();
    }

    public double CalculateOrderPrice()
    {
        double OrderPrice = 0, ValueToAdd;
        foreach(var ProductAmountPair in ProductsOrdered)
        {
            ValueToAdd = ProductAmountPair.Key.Price * ProductAmountPair.Value;
            // verific conditiile reducerilor
            if (ProductAmountPair.Key.ThisProductsDiscounts[DiscountTypes.Constant] == true && ProductAmountPair.Key.ConstantDiscount != null)
            {
                ValueToAdd -= (int)ProductAmountPair.Key.ConstantDiscount;
            }

            if (ProductAmountPair.Key.ThisProductsDiscounts[DiscountTypes.Percentage] == true && ProductAmountPair.Key.PercentageDiscount != null)
            {
                ValueToAdd -= (ValueToAdd * (int)ProductAmountPair.Key.PercentageDiscount) / 100;
            }

            if (ProductAmountPair.Value > 2 && ProductAmountPair.Key.ThisProductsDiscounts[DiscountTypes.TwoPlusOne] == true)
            {
                ValueToAdd = ValueToAdd * (2 / 3); 
                // fiecare al 3lea produs cumparat e gratis
            }
            OrderPrice += ValueToAdd;
        }
        return OrderPrice;
    }
}

