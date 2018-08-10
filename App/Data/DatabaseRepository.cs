using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Data.Entities;
using App.ViewModels;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace App.Data
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var users = connection.Query<User>(
                    "SELECT Email, Name, Password, OrderTokenLocker, BankAccount, Id, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, IsDeleted FROM Users ORDER BY Id desc");

                foreach (var user in users)
                {
                    user.UserRoles = this.GetRolesForUser(user.Id);
                }

                return users;
            }
        }

        public User GetUserByEmail(string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", email, DbType.AnsiString);

                var user = connection.QueryFirst<User>(
                    $"SELECT Email, Name, Password, OrderTokenLocker, BankAccount, Id, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, IsDeleted FROM Users WHERE Email = @Email ORDER BY Id desc", parameter);

                user.UserRoles = this.GetRolesForUser(user.Id);

                return user;
            }
        }

        public User GetUserById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return GetUserByIdWithConnection(id, connection);
            }
        }

        public bool CreateUser(User userToCreate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", userToCreate.Email, DbType.AnsiString);
                parameter.Add("@Name", userToCreate.Name, DbType.AnsiString);
                parameter.Add("@Password", userToCreate.Password, DbType.AnsiString);
                parameter.Add("@BankAccount", userToCreate.BankAccount, DbType.AnsiString);
                parameter.Add("@OrderToken", userToCreate.OrderToken, DbType.Boolean);

                userToCreate.Id = connection.QueryFirst<int>(
                    $"INSERT INTO Users (Email, Name, Password, OrderTokenLocker, BankAccount) " +
                    $"VALUES (@Email, @Name, @Password, @OrderToken, @BankAccount); SELECT last_insert_rowid()", parameter);


                foreach (var role in userToCreate.UserRoles)
                {
                    DynamicParameters roleParameter = new DynamicParameters();
                    parameter.Add("@UserId", userToCreate.Id, DbType.Int32);
                    parameter.Add("@RoleId", role.Id, DbType.Int32);

                    connection.Execute($"INSERT INTO UserRoles (UserId, RoleId) " +
                                       $"VALUES (@UserId, @RoleId)", roleParameter);
                }

                return true;
            }
        }

        public bool EditUser(User userToEdit)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", userToEdit.Email, DbType.AnsiString);
                parameter.Add("@Name", userToEdit.Name, DbType.AnsiString);
                parameter.Add("@Password", userToEdit.Password, DbType.AnsiString);
                parameter.Add("@BankAccount", userToEdit.BankAccount, DbType.AnsiString);
                parameter.Add("@OrderToken", userToEdit.OrderToken, DbType.Boolean);

                connection.Execute(
                           $"UPDATE Users SET Name = @Name, Password = @Password, OrderTokenLocker = @OrderToken, BankAccount = @BankAccount " +
                           $"WHERE Email = @Email ", parameter);

                DynamicParameters deleteParameter = new DynamicParameters();
                deleteParameter.Add("@UserId", userToEdit.Id, DbType.Int32);

                connection.Execute($"DELETE FROM UserRoles WHERE UserId = @UserId", deleteParameter);

                foreach (var role in userToEdit.UserRoles)
                {
                    DynamicParameters roleParameter = new DynamicParameters();
                    parameter.Add("@UserId", userToEdit.Id, DbType.Int32);
                    parameter.Add("@RoleId", role.Id, DbType.Int32);

                    connection.Execute($"INSERT INTO UserRoles (UserId, RoleId) " +
                                       $"VALUES (@UserId, @RoleId)", roleParameter);
                }

                return true;
            }
        }

        public bool FillExtendedIdentityData(string bankAccount, bool lockoutEnabled, bool orderToken, int userId, IEnumerable<Role> roles)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", userId, DbType.Int32);
                parameter.Add("@BankAccount", bankAccount, DbType.AnsiString);
                parameter.Add("@OrderToken", orderToken, DbType.Boolean);
                parameter.Add("@LockoutEnabled", lockoutEnabled, DbType.Boolean);

                connection.Execute(
                           $"UPDATE Users SET OrderTokenLocker = @OrderToken, BankAccount = @BankAccount, LockoutEnabled = @LockoutEnabled " +
                           $"WHERE Id = @Id ", parameter);

                DynamicParameters deleteParameter = new DynamicParameters();
                deleteParameter.Add("@Id", userId, DbType.Int32);

                connection.Execute($"DELETE FROM UserRoles WHERE UserId = @Id", deleteParameter);

                foreach (var role in roles)
                {
                    DynamicParameters roleParameter = new DynamicParameters();
                    roleParameter.Add("@UserId", userId, DbType.Int32);
                    roleParameter.Add("@RoleId", role.Id, DbType.Int32);

                    connection.Execute($"INSERT INTO UserRoles (UserId, RoleId) " +
                                       $"VALUES (@UserId, @RoleId)", roleParameter);
                }

                return true;
            }
        }

        public bool DeleteUser(string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", email, DbType.AnsiString);

                return connection.Execute(
                           $"DELETE FROM Users WHERE Email = @Email ", parameter) > 0;
            }
        }

        public bool ChangePassword(string email, string oldPassword, string newPassword)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", email, DbType.AnsiString);
                parameter.Add("@NewPassword", newPassword, DbType.AnsiString);
                parameter.Add("@OldPassword", oldPassword, DbType.AnsiString);

                return connection.Execute(
                           $"UPDATE Users SET Password = @NewPassword " +
                           $"WHERE Email = @Email AND Password = @OldPassword ", parameter) > 0;
            }
        }

        public IEnumerable<Yerba> GetAllYerbas()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return connection.Query<Yerba>(
                    "SELECT Id, Name, Url, Gentian, Mark, Cost, Country, Components, Producent, Description FROM Yerba ORDER BY Id desc");
            }
        }

        public Yerba GetYerbaById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", id, DbType.Int32);

                return connection.QueryFirst<Yerba>(
                    "SELECT Id, Name, Url, Gentian, Mark, Cost, Country, Components, Producent, Description FROM Yerba WHERE Id = @Id", parameter);
            }
        }

        public bool CreateYerba(Yerba yerbaToCreate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Name", yerbaToCreate.Name, DbType.AnsiString);
                parameter.Add("@Url", yerbaToCreate.Url, DbType.AnsiString);
                parameter.Add("@Gentian", yerbaToCreate.Gentian, DbType.Int32);
                parameter.Add("@Mark", yerbaToCreate.Mark, DbType.AnsiString);
                parameter.Add("@Country", yerbaToCreate.Country, DbType.AnsiString);
                parameter.Add("@Components", yerbaToCreate.Components, DbType.AnsiString);
                parameter.Add("@Producent", yerbaToCreate.Producent, DbType.AnsiString);
                parameter.Add("@Description", yerbaToCreate.Description, DbType.AnsiString);
                parameter.Add("@Size", yerbaToCreate.Size, DbType.Int32);
                parameter.Add("@Cost", yerbaToCreate.Cost, DbType.Decimal);

                int id = connection.QueryFirst<int>(
                    $"INSERT INTO Yerba (Name, Url, Gentian, Mark, Cost, Country, Components, Producent, Description, Size) " +
                    $"VALUES(@Name, @Url, @Gentian, @Mark, @Cost, @Country, @Components, @Producent, @Description, @Size); SELECT last_insert_rowid()", parameter);

                yerbaToCreate.Id = id;
                return id > 0;
            }
        }

        public bool EditYerba(Yerba yerbaToEdit)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", yerbaToEdit.Id, DbType.Int32);
                parameter.Add("@Name", yerbaToEdit.Name, DbType.AnsiString);
                parameter.Add("@Url", yerbaToEdit.Url, DbType.AnsiString);
                parameter.Add("@Gentian", yerbaToEdit.Gentian, DbType.Int32);
                parameter.Add("@Mark", yerbaToEdit.Mark, DbType.AnsiString);
                parameter.Add("@Country", yerbaToEdit.Country, DbType.AnsiString);
                parameter.Add("@Components", yerbaToEdit.Components, DbType.AnsiString);
                parameter.Add("@Producent", yerbaToEdit.Producent, DbType.AnsiString);
                parameter.Add("@Description", yerbaToEdit.Description, DbType.AnsiString);
                parameter.Add("@Size", yerbaToEdit.Size, DbType.Int32);
                parameter.Add("@Cost", yerbaToEdit.Cost, DbType.Decimal);

                return connection.Execute(
                           $"UPDATE Yerba SET Name = @Name, Url = @Url, Gentian = @Gentian, Mark = @Mark, Cost = @Cost, Country = @Country, Components = @Components, Producent = @Producent, Description = @Description, Size = @Size " +
                           $"WHERE Id = @Id ", parameter) > 0;
            }
        }

        public bool DeleteYerba(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", id, DbType.AnsiString);

                int isOK = connection.Execute($"DELETE FROM OrderItem " +
                                              $"WHERE YerbaId = @Id", parameter);

                return connection.Execute(
                           $"DELETE FROM Yerba WHERE Id = @Id ", parameter) > 0;
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var orders = connection.Query<Order>("SELECT Id, OrderDate, Created, MadeBy, ExecutedBy, TotalQuantity, TotalCost, IsClosed, Paid FROM \"Order\" ORDER BY Id desc");

                foreach (var order in orders)
                {
                    order.UserExecutedBy = GetUserByIdWithConnection(order.ExecutedBy, connection);
                    order.UserMadeBy = GetUserByIdWithConnection(order.MadeBy, connection);
                    order.Items.AddRange(GetOrderItemsForOrder(order.Id, connection));
                }

                return orders;
            }
        }

        public IEnumerable<Order> GetNotClosedOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var orders = connection.Query<Order>("SELECT Id, OrderDate, Created, MadeBy, ExecutedBy, TotalQuantity, TotalCost, IsClosed, Paid FROM \"Order\" WHERE IsClosed = 1 ORDER BY Id desc");

                foreach (var order in orders)
                {
                    order.Items.AddRange(GetOrderItemsForOrder(order.Id, connection));
                }

                return orders;
            }
        }

        public IEnumerable<OrderItem> GetOrderItems(int orderId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                return GetOrderItemsForOrder(orderId, connection);
            }
        }

        public bool CreateOrder(List<OrderItem> items, int userCreated, int executedBy, DateTime orderDate, bool paired)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var totalQuantity = items.Sum(x => x.Quantity);

                var totalCost = items.Sum(x => x.Cost);

                var maxId = CalculateOrderId(connection);

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", maxId, DbType.Int32);
                parameter.Add("@OrderDate", orderDate, DbType.Date);
                parameter.Add("@Created", DateTime.UtcNow, DbType.DateTime);
                parameter.Add("@MadeBy", userCreated, DbType.Int32);
                parameter.Add("@ExecutedBy", executedBy, DbType.Int32);
                parameter.Add("@TotalQuantity", totalQuantity, DbType.Int32);
                parameter.Add("@TotalCost", totalCost, DbType.Decimal);
                parameter.Add("@Paid", paired, DbType.Boolean);

                int addedId = connection.ExecuteScalar<int>(
                    $"INSERT INTO \"Order\" (Id, OrderDate, Created, MadeBy, ExecutedBy, TotalQuantity, TotalCost, IsClosed, Paid) " +
                    $"VALUES(@Id, @OrderDate, @Created, @MadeBy, @ExecutedBy, @TotalQuantity, @TotalCost, 0, @Paid); SELECT last_insert_rowid()", parameter);

                foreach (var orderItem in items)
                {
                    DynamicParameters orderItemParams = new DynamicParameters();
                    orderItemParams.Add("@YerbaId", orderItem.YerbaId, DbType.Int32);
                    orderItemParams.Add("@Quantity", orderItem.Quantity, DbType.Int32);
                    orderItemParams.Add("@OrderId", addedId, DbType.Int32);
                    orderItemParams.Add("@Paid", orderItem.IsPaid, DbType.Boolean);
                    orderItemParams.Add("@Cost", orderItem.Cost, DbType.Decimal);
                    orderItemParams.Add("@UserId", orderItem.UserId, DbType.Int32);

                    connection.Execute($"INSERT INTO OrderItem (YerbaId, Quantity, OrderId, Paid, Cost, UserId)" +
                    $"VALUES(@YerbaId, @Quantity, @OrderId, @Paid, @Cost, @UserId);", orderItemParams);
                }

                return true;
            }
        }

        public Order GetOrderById(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", id, DbType.Int32);

                var order = connection.QueryFirst<Order>("SELECT Id, OrderDate, Created, MadeBy, ExecutedBy, TotalQuantity, TotalCost, IsClosed, Paid FROM \"Order\" WHERE Id = @Id", parameter);

                order.UserExecutedBy = GetUserByIdWithConnection(order.ExecutedBy, connection);
                order.UserMadeBy = GetUserByIdWithConnection(order.MadeBy, connection);
                order.Items.AddRange(GetOrderItemsForOrder(order.Id, connection));

                return order;
            }
        }

        public bool DeleteOrder(int orderId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters orderItemParams = new DynamicParameters();
                orderItemParams.Add("@OrderId", orderId, DbType.Int32);

                int isOK = connection.Execute($"DELETE FROM OrderItem " +
                                   $"WHERE OrderId = @OrderId", orderItemParams);

                int orderOK = connection.Execute($"DELETE FROM \"Order\" " +
                                   $"WHERE Id = @OrderId", orderItemParams);
                return true;
            }
        }

        private IEnumerable<OrderItem> GetOrderItemsForOrder(int orderId, SQLiteConnection connection)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@OrderId", orderId, DbType.Int32);
            var orderItems = connection.Query<OrderItem>("SELECT Id, YerbaId, Quantity, OrderId, Paid, Cost, UserId FROM OrderItem WHERE OrderId = @OrderId ORDER BY Id desc", parameter);

            foreach (var orderItem in orderItems)
            {
                orderItem.UserDetails = GetUserByIdWithConnection(orderItem.UserId, connection);
            }

            return orderItems;
        }

        private int CalculateOrderId(SQLiteConnection connection)
        {
            int today = int.Parse(DateTime.Today.ToString("yyyyMMdd0"));

            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@OrderId", today, DbType.Int32);

            int? maxId = connection.QueryFirst<int?>("SELECT MAX(Id) + 1 FROM \"Order\" WHERE Id >= @OrderId", parameter);

            return maxId.HasValue ? maxId.Value : ++today;
        }

        public bool CloseOrder(int orderId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@OrderId", orderId, DbType.Int32);

                if (connection.ExecuteScalar<int>("SELECT Count(*) FROM OrderItem WHERE Paid = 0 AND OrderId = @OrderId",
                    parameter) > 0)
                {
                    return false;
                }

                return connection.Execute(
                    "UPDATE \"Order\" SET IsClosed = 1 WHERE Id = @OrderId", parameter) > 0;
            }
        }

        public bool SetOrderIsPaid(int orderId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@OrderId", orderId, DbType.Int32);

                return connection.Execute(
                           "UPDATE SET Paid = 1 FROM \"Order\" WHERE OrderId = @OrderId", parameter) > 0;
            }
        }

        public IEnumerable<OrderItem> GetUserOrderItems(string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", email, DbType.AnsiString);

                return connection.Query<OrderItem>(
                           "SELECT oi.* FROM OrderItem as oi JOIN Users as u ON oi.UserId == u.Id WHERE u.Email LIKE @Email ORDER BY u.Id desc", parameter);
            }
        }

        public bool SetNextOrderLockerForUser(string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Email", email, DbType.AnsiString);

                connection.Execute(
                   "UPDATE Users Set OrderTokenLocker = 0; UPDATE Users Set OrderTokenLocker = 1 WHERE Email LIKE @Email; ", parameter);

                return true;
            }
        }

        private User GetUserByIdWithConnection(int id, SQLiteConnection connection)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id, DbType.Int32);

            var user = connection.QueryFirst<User>(
                $"SELECT Email, Name, Password, OrderTokenLocker, BankAccount, Id, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount FROM Users WHERE Id = @Id ORDER BY Id desc", parameter);

            user.UserRoles = this.GetRolesForUser(user.Id);

            return user;
        }

        public void UpdateOrderItems(IEnumerable<OrderItem> orderItem)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var orderItemsToUpdate = orderItem.Where(x => x.Id > 0).ToList();

                foreach (var item in orderItemsToUpdate)
                {
                    DynamicParameters orderItemParams = new DynamicParameters();
                    orderItemParams.Add("@YerbaId", item.YerbaId, DbType.Int32);
                    orderItemParams.Add("@Quantity", item.Quantity, DbType.Int32);                    
                    orderItemParams.Add("@Paid", item.IsPaid, DbType.Boolean);
                    orderItemParams.Add("@Cost", item.Cost, DbType.Decimal);
                    orderItemParams.Add("@UserId", item.UserId, DbType.Int32);
                    orderItemParams.Add("@Id", item.Id, DbType.Int32);

                    connection.Execute(
                          $"UPDATE OrderItem SET YerbaId = @YerbaId, Quantity = @Quantity, Paid = @Paid, Cost = @Cost, UserId = @UserId " +
                          $"WHERE Id = @Id ", orderItemParams);
                }

                var orderItemsToAdd = orderItem.Where(x => x.Id == 0).ToList();

                foreach (var item in orderItemsToAdd)
                {
                    DynamicParameters orderItemParams = new DynamicParameters();
                    orderItemParams.Add("@YerbaId", item.YerbaId, DbType.Int32);
                    orderItemParams.Add("@Quantity", item.Quantity, DbType.Int32);
                    orderItemParams.Add("@Paid", item.IsPaid, DbType.Boolean);
                    orderItemParams.Add("@Cost", item.Cost, DbType.Decimal);
                    orderItemParams.Add("@UserId", item.UserId, DbType.Int32);
                    orderItemParams.Add("@OrderId", item.OrderId, DbType.Int32);
                    orderItemParams.Add("@Id", item.Id, DbType.Int32);

                    item.Id = connection.QueryFirst<int>(
                          $"INSERT INTO OrderItem (YerbaId, Quantity, OrderId, Paid, Cost, UserId) " +
                      $"VALUES( @YerbaId, @Quantity, @OrderId, @Paid, @Cost, @UserId); SELECT last_insert_rowid()", orderItemParams);
                }

            }
        }

        public IEnumerable<Role> GetAllRoles()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return connection.Query<Role>(
                    $"SELECT Id, Name FROM Roles");
            }
        }

        public IEnumerable<Role> GetRolesForUser(int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters roleParamteres = new DynamicParameters();
                roleParamteres.Add("@UserId", userId, DbType.Int32);

                return connection.Query<Role>(
                    $"SELECT r.* FROM Roles r JOIN UserRoles ur ON r.Id == ur.RoleId WHERE ur.UserId = @UserId", roleParamteres);
            }
        }

        public IEnumerable<PaimentRequest> GetPaimentRequests()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                return connection.Query<PaimentRequest>(
                    $"SELECT * FROM PaimentRequests");
            }
        }

        public bool CreateNewPaimentRequest(int orderItemId, int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters createNewPaimentParameters = new DynamicParameters();
                createNewPaimentParameters.Add("@UserId", userId, DbType.Int32);
                createNewPaimentParameters.Add("@OrderItemId", orderItemId, DbType.Int32);


                var newId = connection.QueryFirst<int>(
                    "INSERT INTO PaimentRequests (UserId, OrderItemId) VALUES (@UserId, @OrderItemId); SELECT last_insert_rowid()", createNewPaimentParameters);
                
                return newId > 0;
            }
        }

        public bool ConfirmPaimentRequest(int orderItemId, int userId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                DynamicParameters confirmParamteres = new DynamicParameters();
                confirmParamteres.Add("@UserId", userId, DbType.Int32);
                confirmParamteres.Add("@OrderItemId", orderItemId, DbType.Int32);


                var executeResult = connection.Execute(
                    "DELETE FROM PaimentRequests WHERE UserId = @UserId AND OrderItemId = @OrderItemId", confirmParamteres);

                if(executeResult > 0)
                {
                    connection.Execute("UPDATE OrderItem SET Paid = 1 WHERE Id = @OrderItemId", confirmParamteres);
                }

                return true;
            }
        }
    }
}
