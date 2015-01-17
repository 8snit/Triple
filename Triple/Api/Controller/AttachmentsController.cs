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
    public class AttachmentsController : ODataController
    {
        private readonly TripleContext db = new TripleContext();

        // GET: odata/Attachments
        [EnableQuery]
        public IQueryable<Attachment> GetAttachments()
        {
            return this.db.Attachments;
        }

        // GET: odata/Attachments(5)
        [EnableQuery]
        public SingleResult<Attachment> GetAttachment([FromODataUri] long key)
        {
            return SingleResult.Create(this.db.Attachments.Where(attachment => attachment.Id == key));
        }

        // PUT: odata/Attachments(5)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, Delta<Attachment> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var attachment = await this.db.Attachments.FindAsync(key);
            if (attachment == null)
            {
                return this.NotFound();
            }

            patch.Put(attachment);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.AttachmentExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(attachment);
        }

        // POST: odata/Attachments
        public async Task<IHttpActionResult> Post(Attachment attachment)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.db.Attachments.Add(attachment);
            await this.db.SaveChangesAsync();

            return this.Created(attachment);
        }

        // PATCH: odata/Attachments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<Attachment> patch)
        {
            this.Validate(patch.GetEntity());

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var attachment = await this.db.Attachments.FindAsync(key);
            if (attachment == null)
            {
                return this.NotFound();
            }

            patch.Patch(attachment);

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.AttachmentExists(key))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Updated(attachment);
        }

        // DELETE: odata/Attachments(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var attachment = await this.db.Attachments.FindAsync(key);
            if (attachment == null)
            {
                return this.NotFound();
            }

            this.db.Attachments.Remove(attachment);
            await this.db.SaveChangesAsync();

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AttachmentExists(long key)
        {
            return this.db.Attachments.Count(e => e.Id == key) > 0;
        }
    }
}
