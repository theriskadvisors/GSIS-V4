using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    public class AspNetBranchesController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: AspNetBranches
        public ActionResult Index()
        {
            return View(db.AspNetBranches.ToList());
        }

        // GET: AspNetBranches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetBranch aspNetBranch = db.AspNetBranches.Find(id);
            if (aspNetBranch == null)
            {
                return HttpNotFound();
            }
            return View(aspNetBranch);
        }

        // GET: AspNetBranches/Create
        public ActionResult Create()
        {
            ViewBag.BranchPrincipalId = new SelectList(db.AspNetUsers.Where(x=>x.AspNetRoles.Select(y=>y.Name).Contains("Branch_Principal")), "Id", "Email");
            return View();
        }

        // POST: AspNetBranches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,BranchPrincipalId,IsActive,Address")] AspNetBranch aspNetBranch)
        {
            if (ModelState.IsValid)
            {
                db.AspNetBranches.Add(aspNetBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchPrincipalId = new SelectList(db.AspNetUsers, "Id", "Email", aspNetBranch.BranchPrincipalId);
            return View(aspNetBranch);
        }

        // GET: AspNetBranches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetBranch aspNetBranch = db.AspNetBranches.Find(id);
            if (aspNetBranch == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchPrincipalId = new SelectList(db.AspNetUsers.Where(x=>x.AspNetRoles.Select(y => y.Name).Contains("Branch_Principal")), "Id", "Name", aspNetBranch.BranchPrincipalId);
            return View(aspNetBranch);
        }

        // POST: AspNetBranches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,BranchPrincipalId,IsActive,Address")] AspNetBranch aspNetBranch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetBranch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchPrincipalId = new SelectList(db.AspNetUsers, "Id", "Email", aspNetBranch.BranchPrincipalId);
            return View(aspNetBranch);
        }

        // GET: AspNetBranches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetBranch aspNetBranch = db.AspNetBranches.Find(id);
            if (aspNetBranch == null)
            {
                return HttpNotFound();
            }
            return View(aspNetBranch);
        }

        // POST: AspNetBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetBranch aspNetBranch = db.AspNetBranches.Find(id);
            db.AspNetBranches.Remove(aspNetBranch);
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
