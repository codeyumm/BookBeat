namespace BookBeat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aspnetusertable_foriegnkey : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Author = c.String(),
                        Genre = c.String(),
                        PublicationYear = c.Int(),
                        ISBN = c.String(),
                        Description = c.String(),
                        CoverImageURL = c.String(),
                    })
                .PrimaryKey(t => t.BookID);
            
            CreateTable(
                "dbo.MediaLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MediaID = c.Int(nullable: false),
                        MediaType = c.String(),
                        IsAddedLater = c.Boolean(nullable: false),
                        IsAlreadyHeardOrRead = c.Boolean(nullable: false),
                        UserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Tracks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Artist = c.String(),
                        Album = c.String(),
                        AlbumArt = c.String(),
                        ReleaseDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MediaLists", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.MediaLists", new[] { "UserID" });
            DropTable("dbo.Tracks");
            DropTable("dbo.MediaLists");
            DropTable("dbo.Books");
        }
    }
}
