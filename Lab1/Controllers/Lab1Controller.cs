using Lab1.Data;
using Lab1.DTO;
using Lab1.Models;
using Lab1.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Lab1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        protected ResponseApi _response;
        private readonly UserManager<ApplicationUser> _userManager;
        public Lab1Controller(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _response = new();
            _userManager = userManager;
        }
        [HttpGet("GetAllGameLevel")]
        public async Task<IActionResult> GetAllGameLevel()
        {
            try
            {
                var gamelevel = await _db.GameLevels.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lay du lieu thanh cong";
                _response.data = gamelevel;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllQuestionGame")]
        public async Task<IActionResult> GetAllQuestionGame()
        {
            try
            {
                var question = await _db.QuestionLevels.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lay du lieu thanh cong";
                _response.data = question;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllRegion")]
        public async Task<IActionResult> GetAllRegion()
        {
            try
            {
                var region = await _db.Regions.ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lay du lieu thanh cong";
                _response.data = region;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    Name = registerDTO.Name,
                    Avatar = registerDTO.LinkAvatar,
                    RegionId = registerDTO.RegionId,
                };
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Dang ky thanh cong";
                    _response.data = user;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Dang ky that bai";
                    _response.data = result.Errors;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var email = loginRequest.Email;
                var password = loginRequest.Password;
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    _response.IsSuccess = true;
                    _response.Notification = "Dang nhap thanh cong";
                    _response.data = user;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Dang nhap that bai";
                    _response.data = null;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetAllQuestionGameByLevel/{levelId}")]
        public async Task<IActionResult> GetAllQuestionGameByLevel(int levelId)
        {
            try
            {
                var questionGame = await _db.QuestionLevels.Where(x => x.levelId == levelId).ToListAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lay du lieu thanh cong";
                _response.data = questionGame;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpPost("SaveResult")]
        public async Task<IActionResult> SaveResult(LevelResultDTO levelResult)
        {
            try
            {
                var levelResultSave = new LevelResult
                {
                    UserId = levelResult.UserId,
                    LevelId = levelResult.LevelId,
                    Score = levelResult.Score,
                    CompletionDate = DateOnly.FromDateTime(DateTime.Now),
                };
                await _db.LevelResults.AddAsync(levelResultSave);
                await _db.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Notification = "Luu ket qua thanh cong";
                _response.data = levelResult;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("Rating/{idRegion}")]
        public async Task<IActionResult> Rating(int idRegion)
        {
            try
            {
                if (idRegion > 0)
                {
                    var nameRegion = await _db.Regions.Where(x => x.RegionId == idRegion).Select(x => x.Name).FirstOrDefaultAsync();
                    if (nameRegion != null)
                    {
                        _response.IsSuccess = false;
                        _response.Notification = "Khong tim thay khu vuc";
                        _response.data = null;
                        return BadRequest(_response);
                    }
                    var userByRegion = await _db.Users.Where(x => x.RegionId == idRegion).ToListAsync();
                    var resultLevelByRegion = await _db.LevelResults.Where(x => userByRegion.Select(x => x.Id).Contains(x.UserId)).ToListAsync();
                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();
                    foreach (var item in userByRegion)
                    {
                        var sumScore = resultLevelByRegion.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevelByRegion.Where(x => x.UserId == item.Id).Count();
                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }
                    _response.IsSuccess = true;
                    _response.Notification = "Lay du lieu thanh cong";
                    _response.data = ratingVM;
                    return Ok(_response);
                }
                else
                {
                    var user = await _db.Users.ToListAsync();
                    var resultLevel = await _db.LevelResults.ToListAsync();
                    string nameRegion = "Tat ca";
                    RatingVM ratingVM = new();
                    ratingVM.NameRegion = nameRegion;
                    ratingVM.userResultSums = new();
                    foreach (var item in user)
                    {
                        var sumScore = resultLevel.Where(x => x.UserId == item.Id).Sum(x => x.Score);
                        var sumLevel = resultLevel.Where(x => x.UserId == item.Id).Count();
                        UserResultSum userResultSum = new();
                        userResultSum.NameUser = item.Name;
                        userResultSum.SumScore = sumScore;
                        userResultSum.SumLevel = sumLevel;
                        ratingVM.userResultSums.Add(userResultSum);
                    }
                    _response.IsSuccess = true;
                    _response.Notification = "Lay du lieu thanh cong";
                    _response.data = ratingVM;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }
        }
        [HttpGet("GetUserInformation/{userId}")]
        public async Task<IActionResult> GetUserInformation(string userId)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Notification = "Khong tim thay nguoi dung";
                    _response.data = null;
                    return BadRequest(_response);
                }
                UserInformationVM userInformationVM = new();
                userInformationVM.Name = user.Name;
                userInformationVM.Email = user.Email;
                userInformationVM.avatar = user.Avatar;
                userInformationVM.Region = await _db.Regions.Where(x => x.RegionId == user.RegionId).Select(x => x.Name).FirstOrDefaultAsync();
                _response.IsSuccess = true;
                _response.Notification = "Lay du lieu thanh cong";
                _response.data = userInformationVM;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Notification = "Loi";
                _response.data = ex.Message;
                return BadRequest(_response);
            }


            //public IActionResult Get()
            //{
            //    Bai3Model model = new Bai3Model
            //    {
            //        CourseName = "Back-End Game Programming",
            //        CourseCode = "Gam106",
            //        Name = "Nguyen Minh Hai",
            //        StudentCode = "PD10963",
            //        Class = "GA19301",
            //    };
            //    int status = 1;
            //    string message = "Get data sucess";
            //    var data = new {model, status, message};
            //    return new JsonResult(data);
            //}
        }
    }
}
