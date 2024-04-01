namespace BookBeat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        BookID = c.Int(),
                        TrackID = c.Int(),
                        Title = c.String(),
                        MediaType = c.String(),
                        Content = c.String(),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookID)
                .ForeignKey("dbo.Tracks", t => t.TrackID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.BookID)
                .Index(t => t.TrackID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "TrackID", "dbo.Tracks");
            DropForeignKey("dbo.Reviews", "BookID", "dbo.Books");
            DropIndex("dbo.Reviews", new[] { "TrackID" });
            DropIndex("dbo.Reviews", new[] { "BookID" });
            DropIndex("dbo.Reviews", new[] { "UserID" });
            DropTable("dbo.Reviews");
        }
    }
}
