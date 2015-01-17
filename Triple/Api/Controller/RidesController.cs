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
    public class RidesController : ODataController
    {
        private readonly TripleContext db = new TripleContext();

        // GET: odata/Rides
        [EnableQuery]
        public IQueryable<Ride> GetRides()
        {
            return this.db.Rides;
        }

        // GET: odata/Rides(5)
        [EnableQuery]
        public SingleResult<Ride> GetRide([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Rides.Where(ride => ride.Id == key));
        }

        // PUT: odata/Rides(5)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Ride> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var ride = await this.db.Rides.FindAsync(key);
            if (ride == null)
            {
                return this.NotFound();
            }

            patch.Put(ride);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.RideExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(ride);
        }

        // POST: odata/Rides
        public async Task<IHttpActionResult> Post(Ride ride)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.Rides.Add(ride);
            await this.db.SaveChangesAsync();

            return this.Created(ride);
        }

        // PATCH: odata/Rides(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Ride> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var ride = await this.db.Rides.FindAsync(key);
            if (ride == null)
            {
                return this.NotFound();
            }

            patch.Patch(ride);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.RideExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(ride);
        }

        // DELETE: odata/Rides(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var ride = await this.db.Rides.FindAsync(key);
            if (ride == null)
            {
                return this.NotFound();
            }

            this.db.Rides.Remove(ride);
            await this.db.SaveChangesAsync();

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Rides(5)/Trip
        [EnableQuery]
        public SingleResult<Trip> GetTrip([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Rides.Where(m => m.Id == key).Select(m => m.Trip));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RideExists(long key)
        {
            return this.db.Rides.Count(e => e.Id == key) > 0;
        }
    }
}
