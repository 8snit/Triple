using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Triple.Api.Model;

namespace Triple.Api.Controller
{
    public class TripsController : ODataController
    {
        private readonly TripleContext db = new TripleContext();

        // GET: odata/Trips
        [EnableQuery]
        public IQueryable<Trip> GetTrips()
        {
            return this.db.Trips.Include(e => e.Activities).Include(e => e.Rides).Include(e => e.Attachments);
        }

        // GET: odata/Trips(5)
        [EnableQuery]
        public SingleResult<Trip> GetTrip([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Trips.Where(trip => trip.Id == key));
        }

        // PUT: odata/Trips(5)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Trip> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var trip = await this.db.Trips.FindAsync(key);
            if (trip == null)
            {
                return this.NotFound();
            }

            patch.Put(trip);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.TripExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(trip);
        }

        // POST: odata/Trips
        public async Task<IHttpActionResult> Post(Trip trip)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.Trips.Add(trip);
            await this.db.SaveChangesAsync();

            return this.Created(trip);
        }

        // PATCH: odata/Trips(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Trip> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var trip = await this.db.Trips.FindAsync(key);
            if (trip == null)
            {
                return this.NotFound();
            }

            patch.Patch(trip);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.TripExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(trip);
        }

        // DELETE: odata/Trips(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var trip = await this.db.Trips.FindAsync(key);
            if (trip == null)
            {
                return this.NotFound();
            }

            this.db.Trips.Remove(trip);
            await this.db.SaveChangesAsync();

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Trips(5)/Activities
        [EnableQuery]
        public IQueryable<Activity> GetActivities([FromODataUri] long key)
        {
            return this.db.Trips.Where(m => m.Id == key).SelectMany(m => m.Activities);
        }

        // GET: odata/Trips(5)/Rides
        [EnableQuery]
        public IQueryable<Ride> GetRides([FromODataUri] long key)
        {
            return this.db.Trips.Where(m => m.Id == key).SelectMany(m => m.Rides);
        }

        // GET: odata/Trips(5)/Attachments
        [EnableQuery]
        public IQueryable<Attachment> GetAttachments([FromODataUri] long key)
        {
            return this.db.Trips.Where(m => m.Id == key).SelectMany(m => m.Attachments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TripExists(long key)
        {
            return this.db.Trips.Count(e => e.Id == key) > 0;
        }
    }
}
