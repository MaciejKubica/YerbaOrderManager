using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Data.Entities;
using App.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    [Route("api/[controller]")]
    public class OrderManagerController : Controller
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ILogger<OrderManagerController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUserExtended> _userManager;
        private readonly SignInManager<StoreUserExtended> _signInManager;

        public OrderManagerController(IDatabaseRepository databaseRepository, ILogger<OrderManagerController> logger, IMapper mapper,
            UserManager<StoreUserExtended> userManager, SignInManager<StoreUserExtended> signInManager)
        {
            _databaseRepository = databaseRepository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUsers()
        {
            try
            {
                return Ok(_databaseRepository.GetAllUsers());
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserByEmail(string email = "")
        {
            try
            {                
                if (string.IsNullOrEmpty(email))
                {
                    return NotFound(email);
                }

                return Ok(_mapper.Map<User, UserViewModel>(_databaseRepository.GetUserByEmail(email)));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to receive user by email {0} {1}", email, e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetLoggedUserData()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Ok(_mapper.Map<User, UserViewModel>(_databaseRepository.GetUserByEmail(User.Identity.Name)));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to receive logged details", e);
                return BadRequest(e);
            }
        }        

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetRoles()
        {
            try
            {
                return Ok(_databaseRepository.GetAllRoles());
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get admin {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userManager.CreateAsync(_mapper.Map<UserViewModel, StoreUserExtended>(userViewModel), userViewModel.Password);

                    if (result.Succeeded)
                    {
                        var foundUser = await _userManager.FindByEmailAsync(userViewModel.Email);
                        _databaseRepository.FillExtendedIdentityData(userViewModel.BankAccount,
                            userViewModel.LockoutEnabled, userViewModel.OrderTokenLocker, foundUser.Id, _mapper.Map<IEnumerable<RolesViewModel>, IEnumerable<Role>>(userViewModel.Roles));
                        return Created("", userViewModel);
                    }
                    else
                    {
                        return BadRequest("Cannot create new user");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create user {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckUserPassword([FromBody] CheckPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        return Ok("Password matches");
                    }
                    else
                    {
                        return NotFound($"Cannot found user with id: {model.UserId}");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to check user password {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordViewModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userResult = await _userManager.FindByEmailAsync(userViewModel.Email);

                    if (userResult != null)
                    {                        
                        if (await _userManager.CheckPasswordAsync(userResult, userViewModel.OldPassword))
                        {

                            var result = await _userManager.ChangePasswordAsync(userResult, userViewModel.OldPassword, userViewModel.NewPassword);

                            return Ok("Password changed");
                        }                        
                    }

                    return NotFound(userViewModel.Email);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create user {0}", e);
                return BadRequest(e);
            }
        }
        
        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> EditUser([FromBody] UserViewModel userToEdit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _userManager.UpdateAsync(_mapper.Map<UserViewModel, StoreUserExtended>(userToEdit));
                    _databaseRepository.FillExtendedIdentityData(userToEdit.BankAccount,
                        userToEdit.LockoutEnabled, userToEdit.OrderTokenLocker, userToEdit.Id, _mapper.Map<IEnumerable<RolesViewModel>, IEnumerable<Role>>(userToEdit.Roles));
                    return Ok(userToEdit);
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser([FromBody] string email)
        {
            try
            {
                var foundUser = await _userManager.FindByEmailAsync(email);

                if(foundUser != null)
                {
                    var result = await _userManager.DeleteAsync(foundUser);
                    return Ok(result.Succeeded);
                }
                else
                {
                    return BadRequest("Cannot find user to delete");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to delete user {0}", e);
                return BadRequest(e);
            }
        }

        #region Yerba API

        [HttpGet("[action]")]
        public IActionResult GetYerbas()
        {
            try
            {
                return Ok(_databaseRepository.GetAllYerbas());
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receive yerbas {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetYerbaById(int id)
        {
            try
            {                
                return Ok(_mapper.Map<Yerba, YerbaViewModel>(_databaseRepository.GetYerbaById(id)));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to receive yerba by id: {0} {1}", id, e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult CreateYerba([FromBody] YerbaViewModel yerba)
        {
            try
            {
                if (_databaseRepository.CreateYerba(_mapper.Map<YerbaViewModel, Yerba>(yerba)))
                {
                    return Ok(yerba);
                }
                else
                {
                    return BadRequest("Unable to create new yerba");
                }                
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create yerba {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult EditYerba([FromBody] YerbaViewModel yerba)
        {
            try
            {
                if (_databaseRepository.EditYerba(_mapper.Map<YerbaViewModel, Yerba>(yerba)))
                {
                    return Ok(yerba);
                }
                else
                {
                    return BadRequest("Unable to edit yerba");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to edit yerba {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult DeleteYerba([FromBody] int id)
        {
            try
            {
                return Ok(_databaseRepository.DeleteYerba(id));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to delete yerba {0}", e);
                return BadRequest(e);
            }
        }

        #endregion

        [HttpGet("[action]")]
        public IActionResult GetAllOrders()
        {
            try
            {
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(_databaseRepository.GetAllOrders()));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receive all orders {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreateOrder([FromBody] CreateOrderViewModel orderToCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var mappedOrder = _mapper.Map<CreateOrderViewModel, Order>(orderToCreate); 
                    if (_databaseRepository.CreateOrder(mappedOrder.Items, mappedOrder.MadeBy, mappedOrder.ExecutedBy,
                        mappedOrder.OrderDate, mappedOrder.IsPaid))
                    {
                        return Ok(orderToCreate);
                    }
                    else
                    {
                        return BadRequest("There was error during create order");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetOrderById([FromHeader] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(_mapper.Map<Order, OrderViewModel>(_databaseRepository.GetOrderById(id)));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get order with Id {1} {0}", ex, id);
                return BadRequest(ex);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ChangeUserData([FromBody] UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databaseRepository.EditUser(_mapper.Map<UserViewModel, User>(model));

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update user details {0}", ex);
                return BadRequest(ex);
            }
        }


        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult DeleteOrder([FromBody] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_databaseRepository.DeleteOrder(id))
                    {
                        return Ok(id);
                    }
                    else
                    {
                        return BadRequest("There was error during create order");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CloseOrder([FromBody] int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_databaseRepository.CloseOrder(id))
                    {
                        return Ok(id);
                    }
                    else
                    {
                        return BadRequest("Cannot close order because it contain unpaid items or error occurred");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetPaimentsRequests()
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    return Ok(_mapper.Map<IEnumerable<PaimentRequest>, IEnumerable<PaimentRequestsViewModel>>(_databaseRepository.GetPaimentRequests()));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to receive all paiments from database or mapping fails: ", ex);
                return BadRequest(ex);
            }
        }


        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ConfirmPaimentRequest([FromBody] ConfirmPaimentRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_databaseRepository.ConfirmPaimentRequest(model.OrderItemId, model.UserId))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound($"There is no paiment for that parameters: {model.OrderItemId}, {model.UserId}");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }


        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreatePaimentRequest([FromBody] ConfirmPaimentRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_databaseRepository.CreateNewPaimentRequest(model.OrderItemId, model.UserId))
                    {
                        return Created("", model);
                    }
                    else
                    {
                        return BadRequest($"There was a problem with new paiment creation: {model.OrderItemId}, {model.UserId}");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to get users {0}", e);
                return BadRequest(e);
            }
        }


        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateOrderItems([FromBody] OrderItemViewModel[] model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _databaseRepository.UpdateOrderItems(_mapper.Map<IEnumerable<OrderItemViewModel>, IEnumerable<OrderItem>>(model.ToArray()));

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update user details {0}", ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult GetUserOrderItems([FromHeader] string email)
        {
            try
            {

                return Ok(_databaseRepository.GetUserOrderItems(email));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receive all orders {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetLoggedUserOrderItems()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {                  
                    return Ok(_databaseRepository.GetUserOrderItems(User.Identity.Name));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receive all orders {0}", e);
                return BadRequest(e);
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult MakeNextOrderLocker()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Ok(_databaseRepository.SetNextOrderLockerForUser(User.Identity.Name));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receive all orders {0}", e);
                return BadRequest(e);
            }
        }

    }
}