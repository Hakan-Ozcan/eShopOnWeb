using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
//Bu, bir veritabanı tablosunu temsil ettiğini ve bu nesnelerin bir kök varlık olduğunu belirtir.
public class Basket : BaseEntity, IAggregateRoot
{
    public string BuyerId { get; private set; }

    
    private readonly List<BasketItem> _items = new List<BasketItem>();

    //Sepet öğelerini okumak için bir IReadOnlyCollection<BasketItem> özelliği. Bu, _items listesini salt okunur bir koleksiyona dönüştürür.    
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    //Sepetteki toplam öğe sayısını hesaplamak için bir özellik. Quantity özelliği üzerinden öğelerin miktarlarını toplar.
    public int TotalItems => _items.Sum(i => i.Quantity);


    public Basket(string buyerId)
    {
        BuyerId = buyerId;
    }

    //Metot, _items listesinde catalogItemId'ye sahip bir öğenin olup olmadığını kontrol eder. Bunun için LINQ Any metodu kullanılır. Eğer _items listesinde catalogItemId'ye sahip bir öğe yoksa, yeni bir BasketItem öğesi oluşturulur ve _items listesine eklenir.
    //Eğer _items listesinde catalogItemId'ye sahip bir öğe mevcutsa, bu öğe alınır ve AddQuantity metoduna miktar parametresi ile birlikte çağrılır. Bu, varolan öğenin miktarını artırmak için kullanılır.
    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        if (!Items.Any(i => i.CatalogItemId == catalogItemId))
        {
            _items.Add(new BasketItem(catalogItemId, quantity, unitPrice));
            return;
        }
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);
        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems()
    {
        _items.RemoveAll(i => i.Quantity == 0);
    }

    public void SetNewBuyerId(string buyerId)
    {
        BuyerId = buyerId;
    }
}
