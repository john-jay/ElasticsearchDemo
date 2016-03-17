using System.Collections.Generic;
using MusicStoreES.Models;

namespace MusicStoreES.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}