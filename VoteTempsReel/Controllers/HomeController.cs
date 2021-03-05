using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VoteTempsReel.Models;

namespace VoteTempsReel.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            using (var con = new UsersContext())
            {
                string a = Environment.UserName;
                UserProfile t = con.UserProfiles.First(x => x.UserName == User.Identity.Name);
                int id = t.UserId;
                string m = t.Voter;

                if (m == "0")
                {
                    if (Session["Rem_Time"] == null)
                    {
                        Session["Rem_Time"] = DateTime.Now.AddMinutes(2).ToString("dd-MM-yyyy h:mm:ss tt");
                    }
                    ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
                    ViewBag.Rem_Time = Session["Rem_Time"];
                    
                    Supp(id);
                    return View();
                }
                else
                {
                    //ModelState.AddModelError("", "Vous avez déja voter . A la prochaine fois :)");
                    //Response.Write("<script language='javascript' type='text/javascript'>alert('Vous avez déja voter . A la prochaine fois :)');</script>");
                    return Content("<script language='javascript' type='text/javascript'>alert('Vous avez déja voté . A la prochaine fois :)');window.location='Dashboard';</script>");
                   // return RedirectToAction("Dashboard");
                    

                }
               
                
                
            }
        }

        public JsonResult SurveyQuiz()
        {
            var poll = new
            {
                question = "Votez pour votre prochain représentant ",
                choices = VotingHub.poll.Select(x => new { name = x.Key, count = x.Value }).ToList()
            };

            return Json(poll, JsonRequestBehavior.AllowGet);
        }
        UsersContext context = new UsersContext();
      
        public ActionResult Dashboard()
        {
            ViewBag.Rem_Time = Session["Rem_Time"];
            return View(context.UserProfiles.ToList());
        }
        public ActionResult Supp(int orderId)
        {
            using (var con = new UsersContext())
            {
                UserProfile payment = con.UserProfiles.First(x => x.UserId == orderId);
                payment.Voter = "1";

                con.UserProfiles.Attach(payment);
                var entry = con.Entry(payment);
                entry.Property(e => e.Voter).IsModified = true;
                con.SaveChanges();
                return RedirectToAction("List");
            }
        }
        public ActionResult Apropos()
        {
            return View();
        }

        
       
        
        
    }
}
