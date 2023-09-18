using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IBasketService
    {
        Task<Basket> GetOrCreateBasketAsync(string buyerId);

        Task<Basket> AddItemToBasketAsync(string buyerId, int productId, int quantity);   //item ekle

        Task<Basket> SetQuantitiesAsync(string buyerId, Dictionary<int, int> quantities);   // Sepeti güncelle 3nolu ürünü iki yap gibi

        Task DeleteBasketItemAsync(string buyerId, int productId);  // id ile item sil

        Task EmptyBasketAsync(string buyerId);

        Task TransferBasketAsync(string sourceBuyerId, string destinationBuyerId);
    }
}
