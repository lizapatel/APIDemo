using APIDemo.BAL.Entity;
using APIDemo.BAL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMaster<User> _userRepository;
        public UserController(IMaster<User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet("/GetUserList")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                List<User> lstUser = await _userRepository.GetList();
                return StatusCode(StatusCodes.Status200OK, lstUser);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("/SaveUser")]
        public async Task<IActionResult> SaveUser([FromBody] User model)
        {
            try
            {
                Int64 result = await _userRepository.Save(model);
                if (result > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { value = result });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("/EditUser")]
        public async Task<IActionResult> EditUser([FromBody] User model)
        {
            try
            {
                Int64 result = await _userRepository.Edit(model);
                if (result > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { value = result });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("/DeleteUser")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                bool IsDeleted = await _userRepository.Delete(userId);
                return StatusCode(StatusCodes.Status200OK, new { value = IsDeleted });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("/UserDetails")]
        public async Task<IActionResult> UserDetails(int userId)
        {
            try
            {
                User user = await _userRepository.GetDetail(userId);
                return StatusCode(StatusCodes.Status200OK, user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
