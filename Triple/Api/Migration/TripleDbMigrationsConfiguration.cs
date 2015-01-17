using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;
using System.Linq;
using Serilog;
using Triple.Api.Model;

namespace Triple.Api.Migration
{
    public sealed class TripleDbMigrationsConfiguration : DbMigrationsConfiguration<TripleContext>
    {
        public TripleDbMigrationsConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(TripleContext context)
        {
            try
            {
                context.Trips.AddOrUpdate(e => e.Name, new Trip
                {
                    Name = "Lets visit Austria",
                    Slug = "LETSVISITAUSTR_3HFT5FSD63",
                    Attachments = new List<Attachment>
                    {
                        new Attachment
                        {
                            Rel = Rel.Venue,
                            Uri = "http://1",
                            MetaData = MetaData.ForVenue("1", "Großglockner", "", null),
                            Category = Category.Nature,
                            Location = DbGeometry.FromText("POINT (12.751478099248283 47.07541698442347)")
                        },
                        new Attachment
                        {
                            Rel = Rel.Venue,
                            Uri = "http://2",
                            MetaData = MetaData.ForVenue("2", "Stephansdom | St. Stephen's Cathedral", "", null),
                            Category = Category.SightSeeing,
                            Location = DbGeometry.FromText("POINT (16.372750997543335 48.20863075861952)")
                        },
                        new Attachment
                        {
                            Rel = Rel.Venue,
                            Uri = "http://3",
                            MetaData = MetaData.ForVenue("3", "Café Hawelka", "", null),
                            Category = Category.FoodAndDrink,
                            Location = DbGeometry.FromText("POINT (16.369937 48.207894)")
                        },
                        new Attachment
                        {
                            Rel = Rel.Venue,
                            Uri = "http://4",
                            MetaData = MetaData.ForVenue("4", "Wiener Prater", "", null),
                            Category = Category.Entertainment,
                            Location = DbGeometry.FromText("POINT (16.398274898529053 48.21630918918729)")
                        },
                        new Attachment
                        {
                            Rel = Rel.Image,
                            Uri =
                                "https://irs0.4sqi.net/img/general/128x128/58526710_4rrBPlFGsiUU5fVrQpmv1oFUfqD_twFz51eJ4xXmqVo.jpg",
                            MetaData = MetaData.ForImage(256, 256),
                            Category = Category.SightSeeing,
                            Location = DbGeometry.FromText("POINT (16.39 48.22)")
                        }
                    },
                    Activities = new List<Activity>
                    {
                        new Activity
                        {
                            Name = "Austria",
                            StartingAt = new DateTime(2015, 8, 10, 8, 0, 0),
                            EndingAt = new DateTime(2015, 8, 22, 18, 0, 0),
                            Extent =
                                DbGeometry.FromText(
                                    "MULTIPOINT (9.5855712890625 47.00273390667881, 9.7503662109375 47.5394554474239, 11.8817138671875 47.56170075451973, 12.8155517578125 48.1147666318763, 17.0672607421875 47.64318610543658, 16.0455322265625 46.649436163350245, 12.7056884765625 46.543749602738565)")
                        },
                        new Activity
                        {
                            Name = "Vienna Sightseeing",
                            StartingAt = new DateTime(2015, 8, 10, 12, 0, 0),
                            EndingAt = new DateTime(2015, 8, 18, 10, 0, 0),
                            Extent =
                                DbGeometry.FromText(
                                    "MULTIPOINT (16.168513107299805 48.21630918918729, 16.368513107299805 48.20043630314327, 16.372750997543335 48.20863075861952)")
                        },
                        new Activity
                        {
                            Name = "Hiking",
                            StartingAt = new DateTime(2015, 8, 19, 0, 0, 0),
                            EndingAt = new DateTime(2015, 8, 21, 16, 0, 0),
                            Extent = DbGeometry.FromText("MULTIPOINT (12.751478099248283 47.07541698442347)")
                        }
                    },
                    Rides = new List<Ride>
                    {
                        new Ride
                        {
                            Name = "Flight to Vienna",
                            StartingAt = new DateTime(2015, 8, 10, 8, 0, 0),
                            EndingAt = new DateTime(2015, 8, 10, 12, 0, 0),
                            Transport = Transport.Airplane,
                            Route = DbGeometry.FromText("LINESTRING (2.337 48.846, 16.46 48.145)")
                        },
                        new Ride
                        {
                            Name = "Drive to mountains",
                            StartingAt = new DateTime(2015, 8, 19, 6, 0, 0),
                            EndingAt = new DateTime(2015, 8, 19, 12, 0, 0),
                            Transport = Transport.Car,
                            Route =
                                DbGeometry.FromText(
                                    "LINESTRING (16.372750997543335 48.20863075861952, 13.041844367980955 47.7999821893096, 12.751478099248283 47.07541698442347)")
                        }
                    }
                });
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "seed failed");
            }
        }
    }
}
