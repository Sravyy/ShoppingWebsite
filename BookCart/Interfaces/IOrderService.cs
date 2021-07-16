using BookCart.Dto;
using System.Collections.Generic;

namespace BookCart.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(int userId, OrdersDTO orderDetails);
        List<OrdersDTO> GetOrderList(int userId);
    }
}
