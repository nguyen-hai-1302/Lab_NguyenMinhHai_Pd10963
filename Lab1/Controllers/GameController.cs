using Lab1.Data;
using Lab1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<GameLevel> gameLevels = _context.GameLevels.ToList();
            return View(gameLevels);
        }
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var gameLevel = _context.GameLevels.Find(id);
            return View(gameLevel);
        }

        [HttpPost]
        public IActionResult Create(GameLevel gameLevel)
        {
            if (ModelState.IsValid)
            {
                _context.GameLevels.Add(gameLevel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gameLevel);
        }

        [HttpPost]

        public IActionResult Edit(GameLevel gameLevel)
        {
            if (ModelState.IsValid)
            {
                _context.GameLevels.Update(gameLevel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gameLevel);
        }

        public IActionResult Delete(int id)
        {
            var gameLevel = _context.GameLevels.Find(id);
            _context.GameLevels.Remove(gameLevel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var gameLevel = _context.GameLevels.Find(id);
            ViewBag.GameLevelTitle = gameLevel.title;
            ViewBag.GameLevelId = gameLevel.LevelId;
            var question = _context.QuestionLevels.Where(q => q.levelId == id).ToList();
            return View(question);
        }

        public IActionResult CreateQuestion(int levelid)
        {
            Question question = new()
            {
                levelId = levelid
            };
            return View(question);
        }

        public IActionResult EditQuestion(int QuestionId)
        {
            var question = _context.QuestionLevels.Find(QuestionId);
            return View(question);
        }

        public IActionResult DeleteQuestion (int QuestionId)
        {
            var question = _context.QuestionLevels.Find(QuestionId);
            _context.QuestionLevels.Remove(question);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = question.levelId });
        }

        [HttpPost]
        public IActionResult CreateQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                _context.QuestionLevels.Add(question);
                _context.SaveChanges();
                return RedirectToAction("Detail", new { id = question.levelId});
            }
            return View(question);
        }

        [HttpPost]
        public IActionResult EditQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                _context.QuestionLevels.Update(question);
                _context.SaveChanges();
                return RedirectToAction("Details", new { id = question.levelId });
            }
            return View(question);
        }
    }
}
