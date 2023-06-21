using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CarInsurance.Models;
using Microsoft.Ajax.Utilities;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Define base quote
                double baseQuote = 50;
                double quote = CalculateQuote(baseQuote, GetAge(insuree), insuree);

                insuree.Quote = Convert.ToDecimal(quote);
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // Private function to calculate age.
        private int GetAge(Insuree birthDay)
        {
            DateTime currentDate = DateTime.Now;
            DateTime birthDate = new DateTime(birthDay.DateOfBirth.Year, birthDay.DateOfBirth.Month, birthDay.DateOfBirth.Day);
            int age = currentDate.Year - birthDate.Year;
            // Check if the birth date hasn't occurred yet this year
            if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

        // Calculate quote
        private double CalculateQuote(double baseQuote, int age, Insuree insuree)
        {
            double quote = baseQuote;
            
            // Calculate quote based on age
            if (age < 18)
            {
                quote += 100;
            } else if (age >= 19 && age <= 25) // ages 19  - 25
            {
                quote += 50;
            } else // 26 and older
            {
                quote += 25;
            }

            if (insuree.CarYear  < 2000 || insuree.CarYear > 2015)
            {
                quote += 25;
            }

            //  Check if Make and Model is Porsche 911 Carrera.
            if (insuree.CarMake.ToLower().Trim() == "porsche")
            {
                quote += 25;
                if (insuree.CarModel.ToLower().Trim() ==  "911 carrera")
                {
                    quote += 25;
                }
            }

            // Add $10 for every speeding ticket.
            var speedingTicketQuote = insuree.SpeedingTickets * 10;
            quote += speedingTicketQuote;
            
            //  25%  to total if there is DUI citation.
            if (insuree.DUI)
            {
                quote *= 1.25;
            }

            // add  50% to  total if full coverage
            if  (insuree.CoverageType)
            {
                quote *= 1.5;
            }

            return quote;
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
