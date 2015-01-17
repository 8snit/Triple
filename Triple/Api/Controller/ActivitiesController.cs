using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Triple.Api.Model;

namespace Triple.Api.Controller
{
    public class ActivitiesController : ODataController
    {
        private readonly TripleContext db = new TripleContext();

        // GET: odata/Activities
        [EnableQuery]
        public IQueryable<Activity> GetActivities()
        {
            return this.db.Activities;
        }

        // GET: odata/Activities(5)
        [EnableQuery]
        public SingleResult<Activity> GetActivity([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Activities.Where(activity => activity.Id == key));
        }

        // PUT: odata/Activities(5)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Activity> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var activity = await this.db.Activities.FindAsync(key);
            if (activity == null)
            {
                return this.NotFound();
            }

            patch.Put(activity);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.ActivityExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(activity);
        }

        // POST: odata/Activities
        public async Task<IHttpActionResult> Post(Activity activity)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.Activities.Add(activity);
            await this.db.SaveChangesAsync();

            return this.Created(activity);
        }

        // PATCH: odata/Activities(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Activity> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var activity = await this.db.Activities.FindAsync(key);
            if (activity == null)
            {
                return this.NotFound();
            }

            patch.Patch(activity);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.ActivityExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(activity);
        }

        // DELETE: odata/Activities(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var activity = await this.db.Activities.FindAsync(key);
            if (activity == null)
            {
                return this.NotFound();
            }

            this.db.Activities.Remove(activity);
            await this.db.SaveChangesAsync();

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Activities(5)/Trip
        [EnableQuery]
        public SingleResult<Trip> GetTrip([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Activities.Where(m => m.Id == key).Select(m => m.Trip));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityExists(long key)
        {
            return this.db.Activities.Count(e => e.Id == key) > 0;
        }
    }
}
