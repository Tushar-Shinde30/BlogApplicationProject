using BMS.Data;
using BMS.Models;
using Microsoft.EntityFrameworkCore;

namespace BMS.Services
{
    public class UserService
    {
        private readonly UserDBContext _context;

        public UserService(UserDBContext context)
        {
            _context = context;
        }

        public class OperationResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public OperationResult Create(User userData)
        {
            try
            {
                //Check whether same user present in database or not
                var credentials = _context.BMSUsers.Where(model => model.UserName == userData.UserName && model.Password == userData.Password).FirstOrDefault();
                if (credentials == null)
                {
                    _context.BMSUsers.Add(userData);
                    _context.SaveChanges();
                    return new OperationResult { Success = true };
                }
                else
                {
                    // User with the provided username already exists
                    return new OperationResult { Success = false, ErrorMessage = "User already exists..!" };
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return new OperationResult { Success = false, ErrorMessage = ex.Message };
            }

        }

        //Login service
        public bool Login(string username, string password)
        {
            var credentials = _context.BMSUsers.FirstOrDefault(model => model.UserName == username && model.Password == password);
            return credentials != null;
        }

        public User GetUserForDelete(int userId)
        {
            try
            {
                var user = _context.BMSUsers.SingleOrDefault(x => x.UserId == userId);

                if (user != null)
                {
                    return new User()
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Email = user.Email
                    };
                }
                else
                {
                    throw new Exception($"User details not available with the id: {userId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user details for deletion", ex);
            }
        }

        public void DeleteUser(int userId)
        {
            try
            {
                var user = _context.BMSUsers.Find(userId);
                if (user != null)
                {
                    _context.BMSUsers.Remove(user);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"User with ID {userId} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Failed to delete user", ex);
            }

        }


        public void DeletingUser(int userId)
        {
            try
            {
                var user = _context.BMSUsers.SingleOrDefault(x => x.UserId == userId);

                if (user != null)
                {
                    _context.BMSUsers.Remove(user);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"Item details not available with the id: {userId}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting item", ex);
            }
        }
    }
}
