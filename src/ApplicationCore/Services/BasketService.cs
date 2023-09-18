using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRepository<Basket> _basketRepo;
        private readonly IRepository<BasketItem> _basketItemRepo;
        private readonly IRepository<Product> _productRepo;

        public BasketService(IRepository<Basket> basketRepo, IRepository<BasketItem> basketItemRepo, IRepository<Product> productRepo)
        {
            _basketRepo = basketRepo;
            _basketItemRepo = basketItemRepo;
            _productRepo = productRepo;
        }

        //Elimizde neler var 
        public async Task<Basket> AddItemToBasketAsync(string buyerId, int productId, int quantity)
        {
            var basket = await GetOrCreateBasketAsync(buyerId);

            //Önce bakacağımız ihtimal önceden bu sepette böyle bir ürün var mı? Varsa Yeni ürün ekleme var olanı 1 arttır
            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);

            if (basketItem != null)
            {
                basketItem.Quantity += quantity;
            }
            else // böyle bir ürün yoksa yeni oluştur
            {
                var product = await _productRepo.GetByIdAsync(productId);

                if (product == null) // ürün bulunamadı hatası fırlat. Öyle bir hatamız yok. Hatayı exception ile üreteceğiz
                    throw new ProductNotFoundException(productId);

                basketItem = new BasketItem()
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Product = product
                };
            }


            await _basketRepo.UpdateAsync(basket);
            return basket;
        }

        public async Task DeleteBasketItemAsync(string buyerId, int productId)  // sana buyerid sinin verdiğim sepetten product idsini verdiğim ürünü kaldır
        {
            //Önce sepeti elimize alalım
            var basket = await GetOrCreateBasketAsync(buyerId);

            //sonra silincek basket item ını alalım
            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            //Eger basketitem varsa boş değilse
            if (basketItem == null) return;
            //Basketitemrepodan o item i sil
            await _basketItemRepo.DeleteAsync(basketItem);
        }

        public async Task EmptyBasketAsync(string buyerId)
        {
            //Yine Sepeti elimize alalım 
            var basket = await GetOrCreateBasketAsync(buyerId);


            //AsReadOnly yerine toılist kullanılabilir
            foreach(var item in basket.Items.AsReadOnly())
            {
                //basketitem ın içini komple gez. ve tek tek bütün itemları sil 
                await _basketItemRepo.DeleteAsync(item);
            }
        }

        public async Task<Basket> GetOrCreateBasketAsync(string buyerId)
        {
            var specBasket = new BasketWithItemsSpecification(buyerId);
            var basket = await _basketRepo.FirstOrDefaultAsync(specBasket);

            if (basket == null)
            {
                basket = new Basket() { BuyerId = buyerId };
                basket = await _basketRepo.AddAsync(basket);
            }

            return null;
        }


        //12:24 te yaptık bu kısmı
        public async Task<Basket> SetQuantitiesAsync(string buyerId, Dictionary<int, int> quantities)  // Sepette miktarları güncelleyeceğiz. 1idli ürünü 3 adet 5 id li ürünü 8 adet yap gibi
        {
            var basket = await GetOrCreateBasketAsync(buyerId);  // sepeti eline al  

            foreach (var item in basket.Items)
            {
                if (quantities.ContainsKey(item.ProductId))
                {
                    item.Quantity = quantities[item.ProductId];
                    await _basketItemRepo.UpdateAsync(item);
                }
            }
            return basket;
        }


        //12:25 te ytaptık bu kısmı
        public async Task TransferBasketAsync(string sourceBuyerId, string destinationBuyerId)  // login olmadan önce 2 silgi eklediysek anonim sepet ile login olduktan sonraki gerçek sepeti birleştirmeliyiz. Ve anonim sepeti sonra silmeliyiz
        {
            var specSourceBasket = new BasketWithItemsSpecification(sourceBuyerId);
            var sourceBasket = await _basketRepo.FirstOrDefaultAsync(specSourceBasket);

            //böyle bir sepet yoksa taşınacak birşey de yoktur.
            if (sourceBasket == null) return;

            var destinationBasket = await GetOrCreateBasketAsync(destinationBuyerId);  // hedef sepeti eline al yoksa oluştur eline al

            foreach (var item in sourceBasket.Items)   //tek tek öğeleri eskiden yeniye aktar
            {
                var targetItem = destinationBasket.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (targetItem == null)
                {
                    destinationBasket.Items.Add(new BasketItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    targetItem.Quantity += item.Quantity;   // 
                }
            }

            await _basketRepo.UpdateAsync(destinationBasket);
            await _basketRepo.DeleteAsync(sourceBasket);



        }
    }
}
