using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPOO;

class Magazin
{
    private List<Comanda> Comenzi = new List<Comanda>();
    private string GenerateNextID()
    {
        Random Randomizer = new Random();
        string randomID;
        do
            randomID = Randomizer.Next(100000, 999999).ToString();
        while (IdAlreadyExists(randomID));
        return randomID;
    }

    public void AddOrder(List<Produs> productsOrdered, Client recipient, OrderStatus status, ShippingAddress deliveryAddress, DateOnly deliveryDate)
    {
        Comanda NewOrder = new Comanda(productsOrdered, GenerateNextID(), recipient, OrderStatus.Sent, deliveryAddress);
        Comenzi.Add(NewOrder);
    }

    public bool IdAlreadyExists(string proposedId)
    {
        foreach(Comanda comanda in Comenzi)
        {
            if(comanda.ID == proposedId) return true;
        }
        return false;
    }

    public void UpdateStore()
    {
        foreach(Comanda comanda in Comenzi)
        {
            
        }
    }

}
