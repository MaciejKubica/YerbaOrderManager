using System;
using System.Collections.Generic;
using App.Data.Entities;

namespace App.Data
{
    public interface IDatabaseRepository
    {
        // User 
        IEnumerable<User> GetAllUsers();

        User GetUserByEmail(string email);

        User GetUserById(int id);

        bool CreateUser(User userToCreate);

        bool EditUser(User userToEdit);

        bool FillExtendedIdentityData(string bankAccount, bool lockoutEnabled, bool orderToken, int userId, IEnumerable<Role> roles);       

        bool DeleteUser(string email);

        bool ChangePassword(string email, string oldPassword, string newPassword);

        // Yerba
        IEnumerable<Yerba> GetAllYerbas();

        Yerba GetYerbaById(int id);

        bool CreateYerba(Yerba yerbaToCreate);

        bool EditYerba(Yerba yerbaToEdit);

        bool DeleteYerba(int id);

        // Order

        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetNotClosedOrders();        

        IEnumerable<OrderItem> GetOrderItems(int orderId);

        bool CreateOrder(List<OrderItem> items, int userCreated, int executedBy, DateTime orderDate, bool paired);

        Order GetOrderById(int id);

        bool DeleteOrder(int orderId);

        bool CloseOrder(int orderId);

        bool SetOrderIsPaid(int orderId);

        IEnumerable<OrderItem> GetUserOrderItems(string email);

        bool SetNextOrderLockerForUser(string email);

        void UpdateOrderItems(OrderItem[] orderItem);

        IEnumerable<Role> GetAllRoles();

        IEnumerable<Role> GetRolesForUser(int userId);
    }
}
